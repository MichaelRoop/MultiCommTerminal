using BluetoothCommon.Net;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using Common.Net.Network;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
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
            ErrReport report;
            WrapErr.ToErrReport(out report, 20010, "Failure on BTClassic_BytesReceivedHandler", () => {
                string msg = Encoding.ASCII.GetString(data, 0, data.Length);
                this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
                this.BT_BytesReceived?.Invoke(this, msg);
            });
            this.RaiseIfException(report);
        }


        private void BTClassic_ConnectionCompletedHander(object sender, bool e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20011, "Failure on BTClassic_ConnectionCompletedHander", () => {
                this.BT_ConnectionCompleted?.Invoke(this, e);
            });
            this.RaiseIfException(report);
        }


        private void BTClassic_DiscoveryCompleteHandler(object sender, bool e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20012, "Failure on BTClassic_DiscoveryCompleteHandler", () => {
                this.BT_DiscoveryComplete?.Invoke(this, e);
            });
            this.RaiseIfException(report);

        }


        private void BTClassic_DiscoveredDeviceHandler(object sender, BTDeviceInfo e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20013, "Failure on BTClassic_DiscoveredDeviceHandler", () => {
                this.BT_DeviceDiscovered?.Invoke(this, e);
            });
            this.RaiseIfException(report);
        }


        private void BTClassic_DeviceInfoGathered(object sender, BTDeviceInfo e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20014, "Failure on BTClassic_DeviceInfoGathered", () => {
                this.BT_DeviceInfoGathered?.Invoke(this, e);
            });
            this.RaiseIfException(report);
        }


        private void BTClassic_PairInfoRequested(object sender, BT_PairInfoRequest e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20017, "Failure on BTClassic_PairInfoRequested", () => {
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
            });
            this.RaiseIfException(report);
        }


        private void BTClassic_PairStatus(object sender, BTPairOperationStatus e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20015, "Failure on BTClassic_PairStatus", () => {
                this.BT_PairStatus?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }


        private void BTClassic_UnPairStatus(object sender, BTUnPairOperationStatus e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20016, "Failure on BTClassic_UnPairStatus", () => {
                this.BT_UnPairStatus?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }

        #endregion

        public void BTClassicDiscoverAsync(bool paired) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20001, "Failure on BTClassicDiscoverAsync", () => {
                this.classicBluetooth.DiscoverDevicesAsync(paired);
            });
            this.RaiseIfException(report);
        }


        public void BTClassicGetExtraInfoAsync(BTDeviceInfo device) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20002, "Failure on BTClassicGetExtraInfoAsync", () => {
                this.classicBluetooth.GetDeviceInfoAsync(device);                
            });
            this.RaiseIfException(report);
        }


        public void BTClassicConnectAsync(BTDeviceInfo device) {
            this.log.InfoEntry("BTClassicConnectAsync");
            ErrReport report;
            WrapErr.ToErrReport(out report, 20003, "Failure on BTClassicConnectAsync", () => {
                this.classicBluetooth.ConnectAsync(device);
            });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => BT_DiscoveryComplete?.Invoke(this, false));
            }
        }


        public void BTClassicDisconnect() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20004, "Failure on BTClassicDisconnect", () => {
                this.classicBluetooth.Disconnect();
            });
            this.RaiseIfException(report);
        }


        public void BTClassicPairAsync(BTDeviceInfo device) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20005, "Failure on BTClassicPairAsync", () => {
                this.classicBluetooth.PairgAsync(device);
            });
            this.RaiseIfException(report);
        }


        public void BTClassicUnPairAsync(BTDeviceInfo device) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20006, "Failure on BTClassicUnPairAsync", () => {
                this.classicBluetooth.UnPairAsync(device);
            });
            this.RaiseIfException(report);
        }


        public void BTClassicSend(string msg) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 20007, "Failure on BTClassicSend", () => {
                this.GetCurrentTerminator(
                    CommMedium.Bluetooth,
                    (data) => {
                        this.btClassicStack.InTerminators = data.TerminatorBlock;
                        this.btClassicStack.OutTerminators = data.TerminatorBlock;

                    }, (err) => { });
                this.btClassicStack.SendToComm(msg);
            });
            this.RaiseIfException(report);
        }


        public List<KeyValuePropertyDisplay> BT_GetDeviceInfoForDisplay(BTDeviceInfo info) {
            List<KeyValuePropertyDisplay> list = new List<KeyValuePropertyDisplay>();
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Name), info.Name));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Address), info.Address));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.DeviceClass), string.Format("{0} ({1})", info.DeviceClassName, info.DeviceClassInt)));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.ServiceClass), string.Format("{0} (0x{1:X})", info.ServiceClassName, info.ServiceClassInt)));
            
            // Apparently not used
            //list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.LastSeen), info.LastSeen.ToString()));
            //list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.LastUsed), info.LastUsed.ToString()));
            
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Authenticated), info.Authenticated));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.RemoteHost), info.RemoteHostName));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.RemoteService), info.RemoteServiceName));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Connected), info.Connected));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.PairingAllowed), info.CanPair));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Paired), info.IsPaired));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.SignalStrength), info.Strength)); 
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
