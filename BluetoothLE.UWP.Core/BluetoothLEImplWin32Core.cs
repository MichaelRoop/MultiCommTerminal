using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BluetoothLE.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        #region data

        private ManualResetEvent stopped = new ManualResetEvent(false);

        /// <summary>Allows discovery of device list</summary>
        private DeviceWatcher devWatcher = null;

        private BluetoothLEDevice currentDevice = null;
        private ClassLog log = new ClassLog("BluetoothLEImplWin32");

        #endregion

        #region IBLETInterface:ICommStackChannel methods


        public bool SendOutMsg(byte[] msg) {
            // TODO - send out by some kind of stream to BLE device - see classic
            return false;
        }

        #endregion

        #region Interface methods

        /// <summary>Interface call to discover devices</summary>
        /// <returns>A list of discovered devices.  However I am now using events so it is empty</returns>
        public void DiscoverDevices() {
            this.log.Info("DiscoverDevices", "Doing stuff...");
            this.DoLEWatcherSearch();
        }


        public void Disconnect() {
            if (this.currentDevice != null) {
                // Apparently do not need this. Dispose will do it
                //if (this.currentDevice.ConnectionStatus == BluetoothConnectionStatus.Connected) {
                //    // Disconnect & wait for disconnection
                //    //BluetoothLEDevice.
                //}

                this.DisconnectBTLEDeviceEvents();
                this.currentDevice.Dispose();
                this.currentDevice = null;
            }
        }


        public void Connect(BluetoothLEDeviceInfo deviceInfo) {
            // TODO - need to have a copy of the BluetoothLEDeviceInfo saved also which subscribes to the BLE OS Device
            //        info and passes those events up to the UI
            this.Disconnect();
            Task.Run(async () => {
                try {
                    await this.ConnectToDevice(deviceInfo);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "On Task Run ConnectToDevice", e);
                }
            });
        }

        public void GetInfo(BluetoothLEDeviceInfo deviceDataModel) {
            Task.Run(async () => {
                try {
                    await this.HarvestDeviceInfo(deviceDataModel);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "On Task Run GetInfo", e);
                }
            });
        }

        #endregion

        #region Connection to device code

        private void ConnectBTLEDeviceEvents() {
            if (this.currentDevice != null) {
                this.currentDevice.ConnectionStatusChanged += this.CurrentDevice_ConnectionStatusChanged;
                this.currentDevice.GattServicesChanged += this.CurrentDevice_GattServicesChanged;
                this.currentDevice.NameChanged += this.CurrentDevice_NameChanged;
            }
        }


        private void DisconnectBTLEDeviceEvents() {
            if (this.currentDevice != null) {
                this.currentDevice.ConnectionStatusChanged -= this.CurrentDevice_ConnectionStatusChanged;
                this.currentDevice.GattServicesChanged -= this.CurrentDevice_GattServicesChanged;
                this.currentDevice.NameChanged -= this.CurrentDevice_NameChanged;
            }
        }



        private void CurrentDevice_NameChanged(BluetoothLEDevice sender, object args) {
            this.log.Info("CurrentDevice_NameChanged", () =>
                string.Format("Device '{0}' name has changed", sender.Name));
        }


        private void CurrentDevice_GattServicesChanged(BluetoothLEDevice sender, object args) {
            this.log.Info("CurrentDevice_NameChanged", () =>
                string.Format("Device '{0}' services have changed", sender.Name));
        }


        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args) {
            this.log.Info("CurrentDevice_NameChanged", () =>
                string.Format("Device '{0}' Connection status changed to {1}", sender.Name, sender.ConnectionStatus.ToString()));
        }

        #endregion


    }
}
