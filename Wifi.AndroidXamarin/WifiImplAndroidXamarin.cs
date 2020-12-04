using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CommunicationStack.Net.DataModels;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VariousUtils.Net;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;
using Common.Net;
using System.Threading.Tasks;
using Android.Net;

namespace Wifi.AndroidXamarin {

    public partial class WifiImplAndroidXamarin : IWifiInterface {

        #region Data

        ClassLog log = new ClassLog("WifiImplAndroidXamarin");
        WifiAndroidMsgPump msgPump = new WifiAndroidMsgPump();
        WifiManager manager = null;
        ConnectivityManager connectivityManager;
        WifiAndroidListReceiver listReceiver = null;
        private bool isListReceiverRunning = false;

        #endregion

        #region IWifiInterface Properties

        public bool Connected { get; private set; }

        #endregion

        #region IWifiInterface Events

        public event EventHandler<List<WifiAdapterInfo>> DiscoveredAdapters;
        public event EventHandler<List<WifiNetworkInfo>> DiscoveredNetworks;
        public event EventHandler<WifiError> OnError;
        public event EventHandler<MsgPumpResults> OnWifiConnectionAttemptCompleted;
        public event EventHandler<WifiCredentials> CredentialsRequestedEvent;

        #endregion

        #region ICommStackChannel Events

        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        #region Constructors

        public WifiImplAndroidXamarin() {
            this.isListReceiverRunning = false;
            this.msgPump.MsgPumpConnectResultEvent += this.MsgPumpConnectResultEventHandler;
            this.msgPump.MsgReceivedEvent += this.MsgPumpMsgReceivedEventHandler;
            this.manager = this.GetWifiManager();
            this.connectivityManager = this.GetConnectivityManager();
            this.listReceiver = new WifiAndroidListReceiver(this.ListDiscoveryCallback);
        }

        #endregion

        #region IWifiInterface Methods

        public void ConnectAsync(WifiNetworkInfo dataModel) {
            Task.Run(() => {
                this.DoConnection(dataModel);
            });
        }

        public void Disconnect() {
            this.msgPump.Disconnect();
            if (this.connectCallback != null) {
                this.connectivityManager.UnregisterNetworkCallback(this.connectCallback);
                this.connectCallback.Dispose();
                this.connectCallback = null;
            }


            //this.connectivityManager.UnregisterNetworkCallback()
            //ConnectivityManager.UnregisterNetworkCallback(); //on the callback.

            //this.msgPump.Disconnect();
            //this.manager.Disconnect();
        }

        public void DiscoverWifiAdaptersAsync() {
            this.DoDiscovery();
        }


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

        #region Event handlers

        private void MsgPumpMsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("MsgPumpMsgReceivedEventHandler", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }


        // TODO - from the message pump but need a new pump with the socket from the wifi
        private void MsgPumpConnectResultEventHandler(object sender, MsgPumpResults e) {
            this.log.Info("MsgPumpConnectResultEventHandler", () => string.Format(
                "Code:{0} SocketErr:{1} Msg:{2}", 
                e.Code, e.SocketErr, e.ErrorString));
            this.OnWifiConnectionAttemptCompleted?.Invoke(sender, e);
        }

        #endregion

        #region Private

        private WifiManager GetWifiManager() {
            return (WifiManager)Android.App.Application.Context.GetSystemService(Context.WifiService);
        }


        private ConnectivityManager GetConnectivityManager() {
            return (ConnectivityManager)Application.Context.GetSystemService(Context.ConnectivityService);
        }


        private Context GetContext() {
            return Android.App.Application.Context;
        }


        /// <summary>Returns formated string "{arg}"</summary>
        /// <param name="arg">The arg to format</param>
        /// <returns>Formatted arg</returns>
        private string GetJavaArg(string arg) {
            //return string.Format("""{{{0}}}""", arg);
            //return "\"{" + arg + "}\"";
            //return "{" + arg + "}";

            return '"' + arg + '"';

        }


        private void ToEliminateCompilerWarnings() {
            this.DiscoveredAdapters?.Invoke(this, null);
            this.CredentialsRequestedEvent?.Invoke(this, null);
        }


        #endregion

    }
}