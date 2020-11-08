using Communications.UWP.Core.MsgPumps;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VariousUtils.Net;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        #region Data

        ClassLog log = new ClassLog("WifiImpleUwp");
        private static IMsgPump<SocketMsgPumpConnectData> msgPump = new SocketMsgPumpWifi();

        #endregion

        #region Properties

        public bool Connected { get; private set; } = false;

        #endregion

        #region Constructors

        public WifiImpleUwp() {
            msgPump.MsgPumpConnectResultEvent += MsgPump_ConnectResultEventHandler;
            msgPump.MsgReceivedEvent += MsgPump_MsgReceivedEventHandler;
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
            msgPump.WriteAsync(msg);
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
            Task t = this.DisconnectAsync();
            if (!t.Wait(1000)) {
                this.log.Error(9999, "Disconnect", "Timeout on disconnecting");
            }
        }


        private Task DisconnectAsync() {
            return Task.Run(() => {
                try {
                    this.log.InfoEntry("DisconnectAsync");
                    if (msgPump.Connected) {

                        this.log.Info("DisconnectAsync", "Disconnecting pump");
                        msgPump.Disconnect();
                        this.log.Info("DisconnectAsync", "Finish disconnecting pump");
                    }

                    // TODO Arduino has problems if we close the adapter
                    // Just close and reopen the socket
                    //if (wifiAdapter != null) {
                    //    this.log.Info("Disconnect", "Disconnecting adapter");
                    //    // This just kills the Arduino. No possibility of future connections
                    //    wifiAdapter.Disconnect();
                    //}
                }
                catch(Exception e) {
                    this.log.Exception(8888, "", e);
                }
            });
        }


        #endregion

    }
}
