using BluetoothCommon.Net;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Text;
using VariousUtils;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<BTDeviceInfo> BT_DeviceDiscovered;
        public event EventHandler<bool> BT_DiscoveryComplete;
        public event EventHandler<bool> BT_ConnectionCompleted;
        public event EventHandler<string> BT_BytesReceived;

        #region Event handlers

        private void BTClassic_BytesReceivedHandler(object sender, byte[] data) {
            // TODO Eventually assemble a message according to terminators and just send up completed message

            string msg = Encoding.ASCII.GetString(data, 0, data.Length);
            this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
            if (this.BT_BytesReceived != null) {
                this.BT_BytesReceived(this, msg);
            }
        }


        private void BTClassic_ConnectionCompletedHander(object sender, bool e) {
            if (this.BT_ConnectionCompleted != null) {
                this.BT_ConnectionCompleted(this, e);
            }
        }


        private void BTClassic_DiscoveryCompleteHandler(object sender, bool e) {
            if (this.BT_DiscoveryComplete != null) {
                this.BT_DiscoveryComplete(this, e);
            }
        }


        private void BTClassic_DiscoveredDeviceHandler(object sender, BTDeviceInfo e) {
            if (this.BT_DeviceDiscovered != null) {
                this.BT_DeviceDiscovered(this, e);
            }
        }

        #endregion

        public void BTClassicDiscoverAsync() {
            this.DisconnectAll();
            this.classicBluetooth.DiscoverDevicesAsync();
        }


        public void BTClassicConnectAsync(BTDeviceInfo device) {
            this.log.InfoEntry("BTClassicConnectAsync");
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "Failure on BTClassicConnectAsync", () => {
                this.classicBluetooth.ConnectAsync(device);
            });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => BT_DiscoveryComplete?.Invoke(this, false));
            }
        }


        public void BTClassicDisconnect() {
            this.classicBluetooth.Disconnect();
        }


        public void BTClassicSend(string msg) {
            this.btClassicStack.SendToComm(msg);
        }

        #region Init and teardown

        private void InitBluetoothClassic() {
            // Connect comm channel and its stack
            this.btClassicStack.SetCommChannel(this.classicBluetooth);
            this.btClassicStack.InTerminators = "\n\r".ToAsciiByteArray();
            this.btClassicStack.OutTerminators = "\n\r".ToAsciiByteArray();

            this.classicBluetooth.DiscoveredBTDevice += this.BTClassic_DiscoveredDeviceHandler;
            this.classicBluetooth.DiscoveryComplete += this.BTClassic_DiscoveryCompleteHandler;
            this.classicBluetooth.ConnectionCompleted += this.BTClassic_ConnectionCompletedHander;
            this.btClassicStack.MsgReceived += this.BTClassic_BytesReceivedHandler;
        }


        private void TeardownBluetoothClassic() {
            this.classicBluetooth.Disconnect();
            this.classicBluetooth.DiscoveredBTDevice -= this.BTClassic_DiscoveredDeviceHandler;
            this.classicBluetooth.DiscoveryComplete -= this.BTClassic_DiscoveryCompleteHandler;
            this.classicBluetooth.ConnectionCompleted -= this.BTClassic_ConnectionCompletedHander;
            this.btClassicStack.MsgReceived -= this.BTClassic_BytesReceivedHandler;
        }

        #endregion


    }
}
