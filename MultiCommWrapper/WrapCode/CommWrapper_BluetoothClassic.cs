using BluetoothCommon.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Text;
using VariousUtils;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<BTDeviceInfo> BTClassicDeviceDiscovered;
        public event EventHandler<bool> BTClassicDiscoveryComplete;
        public event EventHandler<bool> BTClassicConnectionCompleted;
        public event EventHandler<string> BTClassicBytesReceived;

        #region Event handlers

        private void BTClassic_BytesReceivedHandler(object sender, byte[] data) {
            // TODO Eventually assemble a message according to terminators and just send up completed message

            string msg = Encoding.ASCII.GetString(data, 0, data.Length);
            this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
            if (this.BTClassicBytesReceived != null) {
                this.BTClassicBytesReceived(this, msg);
            }
        }


        private void BTClassic_ConnectionCompletedHander(object sender, bool e) {
            if (this.BTClassicConnectionCompleted != null) {
                this.BTClassicConnectionCompleted(this, e);
            }
        }


        private void BTClassic_DiscoveryCompleteHandler(object sender, bool e) {
            if (this.BTClassicDiscoveryComplete != null) {
                this.BTClassicDiscoveryComplete(this, e);
            }
        }


        private void BTClassic_DiscoveredDeviceHandler(object sender, BTDeviceInfo e) {
            if (this.BTClassicDeviceDiscovered != null) {
                this.BTClassicDeviceDiscovered(this, e);
            }
        }

        #endregion

        public void BTClassicDiscoverAsync() {
            this.classicBluetooth.DiscoverDevicesAsync();
        }


        public void BTClassicConnectAsync(BTDeviceInfo device) {
            this.classicBluetooth.ConnectAsync(device);
        }


        public void BTClassicDisconnect() {
            this.classicBluetooth.Disconnect();
        }

        public void BTClassicSend(string msg) {
            this.BtClassicStack.SendToComm(msg);
        }


        #region Init and teardown

        private void InitBluetoothClassic() {
            // Connect comm channel and its stack
            this.BtClassicStack.SetCommChannel(this.classicBluetooth);
            this.BtClassicStack.InTerminators = "\n\r".ToAsciiByteArray();
            this.BtClassicStack.OutTerminators = "\n\r".ToAsciiByteArray();

            this.classicBluetooth.DiscoveredBTDevice += this.BTClassic_DiscoveredDeviceHandler;
            this.classicBluetooth.DiscoveryComplete += this.BTClassic_DiscoveryCompleteHandler;
            this.classicBluetooth.ConnectionCompleted += this.BTClassic_ConnectionCompletedHander;
            this.BtClassicStack.MsgReceived += this.BTClassic_BytesReceivedHandler;
        }


        private void TeardownBluetoothClassic() {
            this.classicBluetooth.Disconnect();
            this.classicBluetooth.DiscoveredBTDevice -= this.BTClassic_DiscoveredDeviceHandler;
            this.classicBluetooth.DiscoveryComplete -= this.BTClassic_DiscoveryCompleteHandler;
            this.classicBluetooth.ConnectionCompleted -= this.BTClassic_ConnectionCompletedHander;
            this.BtClassicStack.MsgReceived -= this.BTClassic_BytesReceivedHandler;
        }

        #endregion


    }
}
