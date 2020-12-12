using BluetoothCommon.Net;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using Common.Net.Network;
using LanguageFactory.Net.data;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<BTDeviceInfo> BT_DeviceDiscovered;
        public event EventHandler<bool> BT_DiscoveryComplete;
        public event EventHandler<BTDeviceInfo> BT_DeviceInfoGathered;
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


        private void BTClassic_DeviceInfoGathered(object sender, BTDeviceInfo e) {
            this.BT_DeviceInfoGathered?.Invoke(this, e);
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

        public void BTClassicGetExtraInfoAsync(BTDeviceInfo device) {
            this.classicBluetooth.GetDeviceInfoAsync(device);                
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
            this.GetCurrentTerminator(
                (data)=> {
                    this.btClassicStack.InTerminators = data.TerminatorBlock;
                    this.btClassicStack.OutTerminators = data.TerminatorBlock;

                }, (err) => { });
            this.btClassicStack.SendToComm(msg);
        }


        public List<KeyValuePropertyDisplay> BT_GetDeviceInfoForDisplay(BTDeviceInfo info) {
            List<KeyValuePropertyDisplay> list = new List<KeyValuePropertyDisplay>();
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Name), info.Name));
            list.Add(new KeyValuePropertyDisplay("Address", info.Address));
            list.Add(new KeyValuePropertyDisplay("Device Class", string.Format("{0} ({1})", info.DeviceClassName, info.DeviceClassInt)));
            list.Add(new KeyValuePropertyDisplay("Service Class", string.Format("{0} (0x{1:X})", info.ServiceClassName, info.ServiceClassInt)));
            list.Add(new KeyValuePropertyDisplay("Last Seen", info.LastSeen.ToString()));
            list.Add(new KeyValuePropertyDisplay("Last Used", info.LastUsed.ToString()));
            list.Add(new KeyValuePropertyDisplay("Authenticated", info.Authenticated));
            list.Add(new KeyValuePropertyDisplay("Remote Host Name", info.RemoteHostName));
            list.Add(new KeyValuePropertyDisplay("Remote Service Name", info.RemoteServiceName));
            list.Add(new KeyValuePropertyDisplay("Connected", info.Connected));
            list.Add(new KeyValuePropertyDisplay("Can Pair", info.CanPair));
            list.Add(new KeyValuePropertyDisplay("Is Paired", info.IsPaired));
            list.Add(new KeyValuePropertyDisplay("Strength", info.Strength));
            return list;
        }


        // TODO - change names. Properties for both BT and BLE
        public List<NetPropertyDataModelDisplay> BT_GetProperties(BTDeviceInfo info) {
            try {
                List<NetPropertyDataModelDisplay> list = new List<NetPropertyDataModelDisplay>();
                foreach (var sp in info.Properties) {
                    list.Add(new NetPropertyDataModelDisplay(sp.Value));
                }
                return list;
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                return new List<NetPropertyDataModelDisplay>();
            }
        }


        #region Init and teardown

        private void TeardownBluetoothClassic() {
            this.classicBluetooth.Disconnect();
            this.classicBluetooth.DiscoveredBTDevice -= this.BTClassic_DiscoveredDeviceHandler;
            this.classicBluetooth.BT_DeviceInfoGathered -= BTClassic_DeviceInfoGathered;
            this.classicBluetooth.DiscoveryComplete -= this.BTClassic_DiscoveryCompleteHandler;
            this.classicBluetooth.ConnectionCompleted -= this.BTClassic_ConnectionCompletedHander;
            this.btClassicStack.MsgReceived -= this.BTClassic_BytesReceivedHandler;
            this.classicBluetooth.BT_PairInfoRequested += BTClassic_PairInfoRequested;
        }

        #endregion


    }
}
