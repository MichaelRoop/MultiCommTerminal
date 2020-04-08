using BluetoothCommon.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<BTDeviceInfo> BluetoothClassicDeviceDiscovered;
        public event EventHandler<bool> BluetoothClassicDiscoveryComplete;
        public event EventHandler<bool> BluetoothClassicConnectionCompleted;
        public event EventHandler<string> BluetoothClassicBytesReceived;

        #region Event handlers

        private void ClassicBT_BytesReceived(object sender, byte[] data) {
            // TODO Eventually assemble a message according to terminators and just send up completed message

            string msg = Encoding.ASCII.GetString(data, 0, data.Length);
            this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
            if (this.BluetoothClassicBytesReceived != null) {
                this.BluetoothClassicBytesReceived(this, msg);
            }
        }


        private void ClassicBT_ConnectionCompleted(object sender, bool e) {
            if (this.BluetoothClassicConnectionCompleted != null) {
                this.BluetoothClassicConnectionCompleted(this, e);
            }
        }


        private void ClassicBT_DiscoveryComplete(object sender, bool e) {
            if (this.BluetoothClassicDiscoveryComplete != null) {
                this.BluetoothClassicDiscoveryComplete(this, e);
            }
        }


        private void ClassicBT_DiscoveredDevice(object sender, BTDeviceInfo e) {
            if (this.BluetoothClassicDeviceDiscovered != null) {
                this.BluetoothClassicDeviceDiscovered(this, e);
            }
        }

        #endregion

        public void BluetoothClassicDiscoverAsync() {
            this.classicBluetooth.DiscoverDevices();
        }


        public void BluetoothClassicConnectAsync(BTDeviceInfo device) {
            this.classicBluetooth.Connect(device);
        }


        public void BluetoothClassicDisconnect() {
            this.classicBluetooth.Disconnect();
        }

        public void BluetoothClassicSend(string msg) {
            this.classicBluetooth.Send(msg);
        }


        #region Init and teardown

        private void InitBluetoothClassic() {
            this.classicBluetooth.DiscoveredBTDevice += this.ClassicBT_DiscoveredDevice;
            this.classicBluetooth.DiscoveryComplete += this.ClassicBT_DiscoveryComplete;
            this.classicBluetooth.ConnectionCompleted += this.ClassicBT_ConnectionCompleted;
            this.classicBluetooth.BytesReceived += this.ClassicBT_BytesReceived;
        }


        private void TeardownBluetoothClassic() {
            this.classicBluetooth.Disconnect();
            this.classicBluetooth.DiscoveredBTDevice -= this.ClassicBT_DiscoveredDevice;
            this.classicBluetooth.DiscoveryComplete -= this.ClassicBT_DiscoveryComplete;
            this.classicBluetooth.ConnectionCompleted -= this.ClassicBT_ConnectionCompleted;
            this.classicBluetooth.BytesReceived -= this.ClassicBT_BytesReceived;
        }

        #endregion


    }
}
