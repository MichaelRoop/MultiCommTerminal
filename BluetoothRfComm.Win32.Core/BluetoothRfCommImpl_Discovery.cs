using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BluetoothRfComm.UWP.Core {

    /// <summary>Discovery portion of implementation class : BluetoothRfCommImpl_Discovery </summary>
    public partial class BluetoothRfCommUwpCore : IBTInterface {

        public void DiscoverDevicesAsync(bool paired) {
            try {
                Task.Run(() => {
                    try {
                        this.DoDiscovery(paired);
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


        /// <summary>Discover devices</summary>
        /// <param name="paired">If discovery limited to paired or non paired devices</param>
        private async void DoDiscovery(bool paired) {
            try {
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(
                    BluetoothDevice.GetDeviceSelectorFromPairingState(paired));
                foreach (DeviceInformation info in devices) {
                    try {
                        this.log.Info("DoDiscovery", () => string.Format("Found device {0}", info.Name));

                        BTDeviceInfo deviceInfo = new BTDeviceInfo() {
                            Name = info.Name,
                            Connected = false,
                            Address = info.Id,
                        };

                        using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(info.Id)) {
                            deviceInfo.Connected = device.ConnectionStatus == BluetoothConnectionStatus.Connected;
                            deviceInfo.CanPair = this.GetBoolProperty(device.DeviceInformation.Properties, KEY_CAN_PAIR, false);
                            deviceInfo.IsPaired = this.GetBoolProperty(device.DeviceInformation.Properties, KEY_IS_PAIRED, false);
                            // Container Id also
                            //device.DeviceAccessInformation.CurrentStatus == DeviceAccessStatus. // Allowed, denied by user, by system, unspecified
                            //device.DeviceInformation.EnclosureLocation.; // Dock, lid, panel, etc
                            //device.DeviceInformation.IsDefault;
                            //device.DeviceInformation.IsEnabled;
                            //device.DeviceInformation.Kind == //AssociationEndpoint, Device, etc
                            //device.DeviceInformation.Properties

                            if (device.ClassOfDevice != null) {
                                deviceInfo.DeviceClassInt = (uint)device.ClassOfDevice.MajorClass;
                                deviceInfo.DeviceClassName = string.Format("{0}:{1}",
                                    device.ClassOfDevice.MajorClass.ToString(),
                                    device.ClassOfDevice.MinorClass.ToString());
                                //device.ClassOfDevice.ServiceCapabilities == BluetoothServiceCapabilities.ObjectTransferService, etc
                            }

                            // Serial port service name
                            // Bluetooth#Bluetooth10:08:b1:8a:b0:02-20:16:04:07:61:01#RFCOMM:00000000:{00001101-0000-1000-8000-00805f9b34fb}
                            // TODO - determine if all the info in device is disposed by the device.Dispose
                        } // end of using (device)

                        this.DiscoveredBTDevice?.Invoke(this, deviceInfo);
                    }
                    catch (Exception ex2) {
                        this.log.Exception(9999, "", ex2);
                    }
                }
                this.DiscoveryComplete?.Invoke(this, true);
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                this.DiscoveryComplete?.Invoke(this, false);
            }
        }



    }



}
