using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BluetoothCommon.Net.interfaces;
using BluetoothCommon.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogUtils.Net;
using BluetoothRfComm.AndroidXamarin.Receivers;

namespace BluetoothRfComm.AndroidXamarin {

    public partial class BluetoothRfCommAndroidXamarinImpl : IBTInterface {

        #region Data

        BT_DeviceUnpairedDiscoveryReceiver discoverReceiver = null;

        #endregion


        private void DoDiscovery(bool paired) {
            if (paired) {
                this.DoDiscoveryPaired();
            }
            else {
                this.DoDiscoveryUnpaired();
            }
        }


        private void DoDiscoveryPaired() {
            try {
                if (BluetoothAdapter.DefaultAdapter != null &&
                    BluetoothAdapter.DefaultAdapter.IsEnabled) {
                    foreach (BluetoothDevice device in BluetoothAdapter.DefaultAdapter.BondedDevices) {
                        if (device.Type == BluetoothDeviceType.Classic) {
                            this.RaiseDeviceDiscovered(device);
                        }
                    }
                    this.DiscoveryComplete?.Invoke(this, true);
                }
                else {
                    // TODO - need error event
                    this.DiscoveryComplete?.Invoke(this, false);
                }
            }
            catch(Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        private void DoDiscoveryUnpaired() {
            try {
                this.KillDiscoverReceiver();
                this.discoverReceiver = new BT_DeviceUnpairedDiscoveryReceiver(
                    this.RaiseDeviceDiscovered);
                this.GetContext().RegisterReceiver(
                    this.discoverReceiver, new IntentFilter(BluetoothDevice.ActionFound));
                BluetoothAdapter.DefaultAdapter.StartDiscovery();
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        private void RaiseDeviceDiscovered(BluetoothDevice device) {
            BTDeviceInfo info = new BTDeviceInfo() {
                IsPaired = true,
                Name = device.Name,
                DeviceClassName = device.Class.Name,
                Address = device.Address,
                // TODO - any others as needed
            };
            this.DiscoveredBTDevice?.Invoke(this, info);
        }


        private void KillDiscoverReceiver() {
            if (this.discoverReceiver != null) {
                this.GetContext().UnregisterReceiver(this.discoverReceiver);
                this.discoverReceiver.Dispose();
                this.discoverReceiver = null;
            }
        }


    }

}