using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using CommunicationStack.Net.MsgPumps;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace BluetoothRfComm.AndroidXamarin {

    public partial class BluetoothRfCommAndroidXamarinImpl : IBTInterface {

        #region Data

        private ClassLog log = new ClassLog("BluetoothRfCommAndroidXamarinImpl");
        private IMsgPump<NetSocketConnectData> msgPump = new NetSocketMsgPump();
        private bool connected = false;

        #endregion

        #region IBTInterface events

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
        }

        #endregion

        #region IBTInterface methods

        public void ConnectAsync(BTDeviceInfo device) {
            throw new NotImplementedException();
        }

        public void Disconnect() {
            this.msgPump.Disconnect();
            this.connected = false;
        }


        public void DiscoverDevicesAsync(bool paired) {
            try {
                Task.Run(() => {
                    try {
                        this.DoDiscovery(paired);
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "", e);
                    }
                });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        public void GetDeviceInfoAsync(BTDeviceInfo deviceDataModel) {
            throw new NotImplementedException();
        }

        public void PairgAsync(BTDeviceInfo info) {
            throw new NotImplementedException();
        }

        public void UnPairAsync(BTDeviceInfo info) {
            throw new NotImplementedException();
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
            this.ConnectionCompleted?.Invoke(this, this.connected);
        }


        #endregion


        private Context GetContext() {
            return Android.App.Application.Context;
        }


    }

}