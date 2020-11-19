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
using Java.Util;
using VariousUtils.Net;

namespace BluetoothRfComm.AndroidXamarin {

    public partial class BluetoothRfCommAndroidXamarinImpl : IBTInterface {

        #region Data

        BT_DeviceUnpairedDiscoveryReceiver discoverReceiver = null;

        #endregion

        // The GUID for serial connection. i.e. the remote service
        //protected const string serialGuid = "00001101-0000-1000-8000-00805f9b34fb";


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
                this.log.InfoEntry("DoDiscoveryPaired");
                if (BluetoothAdapter.DefaultAdapter != null &&
                    BluetoothAdapter.DefaultAdapter.IsEnabled) {
                    this.log.Info("DoDiscoveryPaired", () => string.Format("Number of paired devices"));
                    foreach (BluetoothDevice device in BluetoothAdapter.DefaultAdapter.BondedDevices) {
                        if (device.Type == BluetoothDeviceType.Classic) {
                            this.log.Info("DoDiscoveryPaired", () => string.Format("Found paired device:{0}", device.Name));
                            this.RaiseDeviceDiscovered(device);
                        }
                    }
                    this.DiscoveryComplete?.Invoke(this, true);
                }
                else {
                    this.log.Error(9, "Default adapter null or not enabled");

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



        BluetoothSocket socket = null;
        BluetoothDevice dev = null;

        private void RaiseDeviceDiscovered(BluetoothDevice device) {
            BTDeviceInfo info = new BTDeviceInfo() {
                IsPaired = true,
                Name = device.Name,
                DeviceClassName = device.Class.Name,
                Address = device.Address,
                // TODO - any others as needed

            };


            this.log.Info("RaiseDeviceDiscovered", () => string.Format(
                "{0} - {1} - {2}", info.Name, info.DeviceClassName, device.Address));


            //if (this.socket != null) {
            //    this.socket.Dispose();
            //    this.socket = null;
            //}

            //this.socket =  device.CreateRfcommSocketToServiceRecord(UUID.FromString(BT_Ids.SerialServiceGuid));

            //this.log.Info("RaiseDeviceDiscovered", () => string.Format(
            //    "{0} - {1} - {2}", socket.RemoteDevice.Name, info.DeviceClassName, device.Address));



            //this.socket.Connect();
            //byte[] d = "OpenDoor\r\n".ToAsciiByteArray();
            //socket.OutputStream.Write(d, 0, d.Length);



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