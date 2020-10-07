using BluetoothCommon.Net;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Text;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<BTDeviceInfo> BT_DeviceDiscovered;
        public event EventHandler<bool> BT_DiscoveryComplete;
        public event EventHandler<bool> BT_ConnectionCompleted;
        public event EventHandler<string> BT_BytesReceived;
        public event EventHandler<BT_PairingInfoDataModel> BT_PairInfoRequested;
        public event EventHandler<BTPairOperationStatus> BT_PairStatus;
        public event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

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


        private void BTClassic_PairInfoRequested(object sender, BT_PairInfoRequest e) {
            if (this.BT_PairInfoRequested != null) {
                // Build title
                BT_PairingInfoDataModel dataModel = new BT_PairingInfoDataModel() {
                    RequestTitle = string.Format("{0} ({1})",
                        this.GetText(MsgCode.PairBluetooth), e.DeviceName)
                };
                dataModel.IsPinRequested = e.PinRequested;

                // Build message
                dataModel.RequestMsg = e.PinRequested ?
                     this.GetText(MsgCode.EnterPin) :
                     this.GetText(MsgCode.Continue);

                // push up to user
                this.BT_PairInfoRequested(sender, dataModel);

                // copy data from user so BT calling up can determine what to do
                e.Response = dataModel.HasUserConfirmed;
                e.Pin = dataModel.PIN;
            }
            else {
                this.log.Error(9999, "No subscribers to the wrapper pair info");
            }
        }


        private void BTClassic_PairStatus(object sender, BTPairOperationStatus e) {
            this.BT_PairStatus?.Invoke(sender, e);
        }


        private void BTClassic_UnPairStatus(object sender, BTUnPairOperationStatus e) {
            this.BT_UnPairStatus?.Invoke(sender, e);
        }

        #endregion

        public void BTClassicDiscoverAsync(bool paired) {
            this.DisconnectAll();
            this.classicBluetooth.DiscoverDevicesAsync(paired);
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


        public void BTClassicPairAsync(BTDeviceInfo device) {
            this.classicBluetooth.PairgAsync(device);
        }


        public void BTClassicUnPairAsync(BTDeviceInfo device) {
            this.classicBluetooth.UnPairAsync(device);
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
            this.classicBluetooth.BT_PairInfoRequested += BTClassic_PairInfoRequested;
            this.classicBluetooth.BT_PairStatus += this.BTClassic_PairStatus;
            this.classicBluetooth.BT_UnPairStatus += this.BTClassic_UnPairStatus;
        }


        private void TeardownBluetoothClassic() {
            this.classicBluetooth.Disconnect();
            this.classicBluetooth.DiscoveredBTDevice -= this.BTClassic_DiscoveredDeviceHandler;
            this.classicBluetooth.DiscoveryComplete -= this.BTClassic_DiscoveryCompleteHandler;
            this.classicBluetooth.ConnectionCompleted -= this.BTClassic_ConnectionCompletedHander;
            this.btClassicStack.MsgReceived -= this.BTClassic_BytesReceivedHandler;
            this.classicBluetooth.BT_PairInfoRequested += BTClassic_PairInfoRequested;
        }

        #endregion


    }
}
