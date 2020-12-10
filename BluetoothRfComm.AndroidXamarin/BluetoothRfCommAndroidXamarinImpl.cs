using Android.Bluetooth;
using Android.Content;
using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using BluetoothCommonAndroidXamarin;
using BluetoothCommonAndroidXamarin.Data_models;
using BluetoothCommonAndroidXamarin.Messaging;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Linq;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace BluetoothRfComm.AndroidXamarin {

    public partial class BluetoothRfCommAndroidXamarinImpl : IBTInterface {

        #region Data

        private ClassLog log = new ClassLog("BluetoothRfCommAndroidXamarinImpl");
        private IMsgPump<BTAndroidMsgPumpConnectData> msgPump = new BTAndroidMsgPump();
        private bool connected = false;
        private BluetoothDevice device = null;

        private BluetoothCommonFunctionality common = new BluetoothCommonFunctionality(BluetoothDeviceType.Classic);

        #endregion

        #region IBTInterface events

        // TODO Implement other events
        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<BTDeviceInfo> BT_DeviceInfoGathered;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<BT_PairInfoRequest> BT_PairInfoRequested;
        public event EventHandler<BTPairOperationStatus> BT_PairStatus;
        public event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

        #endregion

        #region ICommStackChannel Events

        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        #region Constructors

        public BluetoothRfCommAndroidXamarinImpl() {
            this.msgPump.MsgPumpConnectResultEvent += this.MsgPumpConnectResultEventHandler;
            this.msgPump.MsgReceivedEvent += this.MsgReceivedEventHandler;
            this.common.DiscoveredBTDevice += this.Common_DiscoveredBTDevice;
            this.common.DiscoveryComplete += this.Common_DiscoveryComplete;
            this.common.BT_PairStatus += this.Common_BT_PairStatus;
            this.common.BT_UnPairStatus += this.Common_BT_UnPairStatus;
        }

        #endregion


        #region IBTInterface methods

        public void ConnectAsync(BTDeviceInfo device) {
            Task.Run(() => {
                try {
                    this.Disconnect();
                    this.device = BluetoothAdapter.DefaultAdapter.BondedDevices.FirstOrDefault(d => d.Name == device.Name);
                    this.msgPump.ConnectAsync2(new BTAndroidMsgPumpConnectData(this.device));
                }
                catch (Exception e) {
                    this.log.Exception(9999, "ConnectAsync", "", e);
                }
            });
        }


        public void Disconnect() {
            try {
                if (this.connected) {
                    this.msgPump.Disconnect();
                    // Never dispose the device since it came from the Adaptor Bonded list
                    if (this.device != null) {
                        this.device = null;
                    }
                    this.connected = false;
                }
            }
            catch (Exception e) {
                this.log.Exception(9898, "Disconnect", "", e);
            }
            this.connected = false;
        }


        public void DiscoverDevicesAsync(bool paired) {
            this.common.DiscoverDevicesAsync(paired);
        }


        public void GetDeviceInfoAsync(BTDeviceInfo deviceDataModel) {
            throw new NotImplementedException();
        }


        public void PairgAsync(BTDeviceInfo info) {
            this.common.Pair(info.Name);
        }


        public void UnPairAsync(BTDeviceInfo info) {
            this.common.UnPairAsync(info.Name);
        }

        #endregion

        #region ICommStackChannel methods

        public bool SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
            return true;
        }

        #endregion

        #region Event handlers

        private void MsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("MsgReceivedEventHandler", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }


        private void MsgPumpConnectResultEventHandler(object sender, CommunicationStack.Net.DataModels.MsgPumpResults results) {
            this.connected = results.Code == MsgPumpResultCode.Connected;

            this.log.Info("", () => string.Format(
                "Connect result:{0}  Socket:{1} Msg:{2}",
                results.Code, results.SocketErr, results.ErrorString));
            this.ConnectionCompleted?.Invoke(this, this.connected);
        }


        private void Common_DiscoveryComplete(object sender, bool e) {
            this.DiscoveryComplete?.Invoke(sender, e);
        }


        private void Common_DiscoveredBTDevice(object sender, BTDeviceInfo e) {
            this.DiscoveredBTDevice?.Invoke(sender, e);
        }


        private void Common_BT_UnPairStatus(object sender, BTUnPairOperationStatus e) {
            this.BT_UnPairStatus?.Invoke(sender, e);
        }


        private void Common_BT_PairStatus(object sender, BTPairOperationStatus e) {
            this.BT_PairStatus?.Invoke(sender, e);
        }


        private void ToEliminateCompilerWarnings() {
            // Potential future implementation
            this.BT_DeviceInfoGathered?.Invoke(this, new BTDeviceInfo());

            // Android raises its own PIN dialog
            this.BT_PairInfoRequested?.Invoke(this, new BT_PairInfoRequest() {
                DeviceName = "NOT IMPLEMENTED"
            });
        }

        #endregion

        #region Private

        private Context GetContext() {
            return Android.App.Application.Context;
        }

        #endregion

    }

}