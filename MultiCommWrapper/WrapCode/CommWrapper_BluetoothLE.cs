using BluetoothCommon.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {
    
    public partial class CommWrapper : ICommWrapper {

        /// <summary>Event raised when a device is discovered</summary>
        public event EventHandler<BluetoothLEDeviceInfo> BluetoothLE_DeviceDiscovered;


        private void BleBluetooth_DeviceDiscovered(object sender, BluetoothLEDeviceInfo e) {
            if (this.BluetoothLE_DeviceDiscovered != null) {
                this.BluetoothLE_DeviceDiscovered(this, e);
            }
        }

        private void InitBluetoothLE() {
            this.bleBluetooth.DeviceDiscovered += BleBluetooth_DeviceDiscovered;
        }

        private void TearDownBluetoothLE() {
            this.bleBluetooth.DeviceDiscovered -= BleBluetooth_DeviceDiscovered;
        }


        public void BluetoothLEDiscoverAsync() {
            this.bleBluetooth.DiscoverDevices();
        }

        public void BluetoothLEConnectAsync(BluetoothLEDeviceInfo device) {
            this.bleBluetooth.Connect(device);
        }


    }
}
