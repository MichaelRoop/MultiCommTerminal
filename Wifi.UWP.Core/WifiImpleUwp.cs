using Communications.UWP.Core.MsgPumps;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using VariousUtils.Net;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        #region Data

        ClassLog log = new ClassLog("WifiImpleUwp");
        IMsgPump<SocketMsgPumpConnectData> msgPump = new SocketMsgPump();

        #endregion

        #region Properties

        public bool Connected { get; private set; } = false;

        #endregion

        #region Constructors

        public WifiImpleUwp() {
            this.msgPump.MsgPumpConnectResultEvent += MsgPump_ConnectResultEventHandler;
            this.msgPump.MsgReceivedEvent += MsgPump_MsgReceivedEventHandler;
        }

        private void MsgPump_MsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("MsgPump_MsgReceivedEventHandler", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }

        private void MsgPump_ConnectResultEventHandler(object sender, MsgPumpResults e) {
            //throw new NotImplementedException();
            this.OnWifiConnectionAttemptCompleted?.Invoke(sender, e);
            
        }

        #endregion

        #region ICommStackChannel Events

        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        #region ICommStackChannel Methods

        /// <summary>Write message from ICommStackChannel interface</summary>
        /// <param name="msg">The message to write out</param>
        /// <returns>always true</returns>
        public bool SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
            return true;
        }

        #endregion

        #region IWifiInterface events

        public event EventHandler<List<WifiAdapterInfo>> DiscoveredAdapters;


        public event EventHandler<List<WifiNetworkInfo>> DiscoveredNetworks;


        public event EventHandler<WifiError> OnError;


        public event EventHandler<MsgPumpResults> OnWifiConnectionAttemptCompleted;


        public event EventHandler<WifiCredentials> CredentialsRequestedEvent;


        #endregion

        #region IWifiInterface methods

        public void Disconnect() {
            this.log.InfoEntry("Disconnect");
            this.msgPump.Disconnect();

            if (this.wifiAdapter != null) {
                this.log.Info("Disconnect", "Disconnecting adapter");
                // This just kills the Arduino. No possibility of future connections
                //this.wifiAdapter.Disconnect();
            }

        }

        #endregion

    }
}
