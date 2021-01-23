using Android.Bluetooth;
using Android.Content;
using BluetoothCommon.Net;
using BluetoothCommonAndroidXamarin.Receivers;
using BluetoothLE.Net.DataModels;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace BluetoothCommonAndroidXamarin {

    public partial class BluetoothCommonFunctionality {

        #region Data

        private UnboundDeviceDiscoveryReceiver discoverReceiver = null;
        private const int DISCOVER_TIMEOUT = 15000;

        #endregion

        #region Public

        public void DiscoverDevicesAsync(bool paired) {
            try {
                Task.Run(() => {
                    try {
                        if (paired) {
                            this.DoDiscoveryPaired();
                        }
                        else {
                            this.DoDiscoveryUnpaired();
                        }
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "", e);
                    }
                });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }

        #endregion

        #region Private

        private void DoDiscoveryPaired() {
            try {
                this.log.InfoEntry("DoDiscoveryPaired");
                if (BluetoothAdapter.DefaultAdapter != null) {
                    if (!BluetoothAdapter.DefaultAdapter.IsEnabled) {
                        BluetoothAdapter.DefaultAdapter.Enable();
                    }

                    if (BluetoothAdapter.DefaultAdapter.IsEnabled) {
                        this.log.Info("DoDiscoveryPaired", () => string.Format("Number of paired devices"));
                        foreach (BluetoothDevice device in BluetoothAdapter.DefaultAdapter.BondedDevices) {
                            if (device.Type == this.deviceType) {
                                this.log.Info("DoDiscoveryPaired", () => string.Format("Found paired device:{0}", device.Name));
                                this.RaiseDeviceDiscovered(device);
                            }
                        }
                        this.DiscoveryComplete?.Invoke(this, true);
                    }
                    else {
                        this.log.Error(9, "Default adapter failed to enabled");
                        this.DiscoveryComplete?.Invoke(this, false);
                    }
                }
                else {
                    this.log.Error(9, "Default adapter null");
                    // TODO - need error event
                    this.DiscoveryComplete?.Invoke(this, false);
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        private void DoDiscoveryUnpaired() {
            try {
                this.KillDiscoverReceiver();
                this.unBondedDevices.Clear();
                if (BluetoothAdapter.DefaultAdapter == null) {
                    this.DiscoveryComplete?.Invoke(this, false);
                    return;
                }

                this.discoverReceiver = new
                    UnboundDeviceDiscoveryReceiver(
                        this.deviceType,
                        this.RaiseDeviceDiscovered,
                        this.unBondedDevices);
                this.GetContext().RegisterReceiver(
                    this.discoverReceiver, new IntentFilter(BluetoothDevice.ActionFound));
                BluetoothAdapter.DefaultAdapter.StartDiscovery();
                this.StartAutoEnd();
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        private void KillDiscoverReceiver() {
            if (this.discoverReceiver != null) {
                this.GetContext().UnregisterReceiver(this.discoverReceiver);
                this.discoverReceiver.Dispose();
                this.discoverReceiver = null;
            }
        }


        private void StartAutoEnd() {
            Task.Run(() => {
                try {
                    AutoResetEvent timeout = new AutoResetEvent(false);
                    timeout.WaitOne(DISCOVER_TIMEOUT);
                    this.KillDiscoverReceiver();
                    this.DiscoveryComplete?.Invoke(this, this.UnBondedDevices.Count > 0);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "StartAutoEnd", "", e);
                }
            });
        }


        private void RaiseDeviceDiscovered(BluetoothDevice device) {
            if (this.deviceType == BluetoothDeviceType.Classic) {
                BTDeviceInfo info = new BTDeviceInfo() {
                    IsPaired = device.BondState == Bond.Bonded,
                    Name = device.Name,
                    DeviceClassName = device.Class.Name,
                    Address = device.Address,
                    // TODO - any others as needed
                };

                this.log.Info("RaiseDeviceDiscovered", () => string.Format(
                    "{0} - {1} - {2}", info.Name, info.DeviceClassName, device.Address));
                this.DiscoveredBTDevice?.Invoke(this, info);
            }
            else if (this.deviceType == BluetoothDeviceType.Le) {
                BluetoothLEDeviceInfo info = new BluetoothLEDeviceInfo() {
                    // TODO - initialize
                };
                this.DiscoveredBLEDevice?.Invoke(this, info);
            }
            else {
                this.log.Error(9999, "", () => string.Format("Unhandled device type:{0}", this.deviceType));
            }
        }


        private Context GetContext() {
            return Android.App.Application.Context;
        }

        #endregion

    }

}