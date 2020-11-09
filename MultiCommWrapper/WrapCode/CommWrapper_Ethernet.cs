using CommunicationStack.Net.DataModels;
using Ethernet.Common.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Ethernet events

        public event EventHandler<string> Ethernet_BytesReceived;
        public event EventHandler<EthernetParams> EthernetParamsRequestedEvent;
        public event EventHandler<MsgPumpResults> OnEthernetConnectionAttemptCompleted;
        public event EventHandler<MsgPumpResults> OnEthernetError;

        #endregion

        #region ICommWrapper methods

        public void EthernetConnect(EthernetParams dataModel) {
            // TODO have an OnError and wrap this in try catch
            this.log.Info("EthernetConnect", "Connecting to ethernet");
            this.ethernet.ConnectAsync(dataModel);
        }


        public void EthernetDisconnect() {
            this.ethernet.Disconnect();
        }


        public void EthernetSend(string msg) {
            this.ethernetStack.SendToComm(msg);
        }

        #endregion


        private void EthernetTeardown() {
            this.ethernetStack.MsgReceived -= this.EthernetStack_MsgReceivedHandler;
            this.ethernet.ParamsRequestedEvent -= Ethernet_ParamsRequestedEventHandler;
            this.ethernet.OnError -= this.Ethernet_OnErrorHandler;
            this.ethernet.OnEthernetConnectionAttemptCompleted -= this.Ethernet_OnEthernetConnectionAttemptCompletedHandler;
        }


        private void EthernetInit() {
            this.ethernetStack.SetCommChannel(this.ethernet);
            this.ethernetStack.InTerminators = "\r\n".ToAsciiByteArray();
            this.ethernetStack.OutTerminators = "\r\n".ToAsciiByteArray();
            this.ethernetStack.MsgReceived += this.EthernetStack_MsgReceivedHandler;  

            this.ethernet.ParamsRequestedEvent += Ethernet_ParamsRequestedEventHandler;
            this.ethernet.OnError += this.Ethernet_OnErrorHandler;
            this.ethernet.OnEthernetConnectionAttemptCompleted += this.Ethernet_OnEthernetConnectionAttemptCompletedHandler;
        }

        private void Ethernet_OnEthernetConnectionAttemptCompletedHandler(object sender, MsgPumpResults e) {

            // Can do something here to save any pending IP entered
            this.log.Error(9999, "Got connect attempt returned");

            this.OnEthernetConnectionAttemptCompleted?.Invoke(sender, e);
        }

        private void Ethernet_OnErrorHandler(object sender, MsgPumpResults e) {
            this.OnEthernetError?.Invoke(sender, e);
        }

        private void EthernetStack_MsgReceivedHandler(object sender, byte[] e) {
            string msg = Encoding.ASCII.GetString(e, 0, e.Length);
            this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
            this.Ethernet_BytesReceived?.Invoke(sender, msg);
        }


        private void Ethernet_ParamsRequestedEventHandler(object sender, EthernetParams e) {
            this.EthernetParamsRequestedEvent?.Invoke(sender, e);
        }



    }

}
