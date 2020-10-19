using CommunicationStack.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;
using WifiCommon.Net.DataModels;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<List<WifiNetworkInfo>> DiscoveredNetworks;
        public event EventHandler<WifiError> OnWifiError;
        public event EventHandler<MsgPumpConnectResults> OnWifiConnectionAttemptCompleted;
        public event EventHandler<string> Wifi_BytesReceived;

        public void WifiDiscoverAsync() {
            this.wifi.DiscoverWifiAdaptersAsync();
        }


        public void WifiConnectAsync(WifiNetworkInfo dataModel) {
            this.wifi.ConnectAsync(dataModel);
        }

        public void WifiDisconect() {
            this.wifi.Disconnect();
        }


        public void WifiSend(string msg) {
            //this.wifi.SendOutMsg()
            this.wifiStack.SendToComm(msg);
        }


        private void WifiInit() {
            this.wifiStack.SetCommChannel(this.wifi);
            this.wifiStack.InTerminators = "\n\r".ToAsciiByteArray();
            this.wifiStack.OutTerminators = "\n\r".ToAsciiByteArray();
            this.wifiStack.MsgReceived += this.WifiStack_BytesReceivedHander;

            this.wifi.DiscoveredNetworks += this.Wifi_DiscoveredNetworksHandler;
            this.wifi.OnError += this.Wifi_OnErrorHandler;
            this.wifi.OnWifiConnectionAttemptCompleted += this.Wifi_OnWifiConnectionAttemptCompletedHandler;
        }

        private void WifiStack_BytesReceivedHander(object sender, byte[] e) {
            string msg = Encoding.ASCII.GetString(e, 0, e.Length);
            this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
            this.Wifi_BytesReceived?.Invoke(sender, msg);
        }

        private void WifiTeardown() {
            this.wifi.DiscoveredNetworks -= this.Wifi_DiscoveredNetworksHandler;
            this.wifi.OnError -= this.Wifi_OnErrorHandler;
            this.wifi.OnWifiConnectionAttemptCompleted -= this.Wifi_OnWifiConnectionAttemptCompletedHandler;
            this.wifiStack.MsgReceived -= this.WifiStack_BytesReceivedHander;
        }


        private void Wifi_OnWifiConnectionAttemptCompletedHandler(object sender, MsgPumpConnectResults e) {
            this.log.Info("Wifi_OnWifiConnectionAttemptCompletedHandler", () => string.Format(
                "Is OnWifiConnectionAttemptCompleted null={0}", 
                this.OnWifiConnectionAttemptCompleted == null));
            this.OnWifiConnectionAttemptCompleted?.Invoke(sender, e);
        }


        private void Wifi_OnErrorHandler(object sender, WifiError e) {
            this.log.Info("Wifi_OnErrorHandler", () => string.Format("Is OnWifiError null={0}", this.OnWifiError == null));
            this.OnWifiError?.Invoke(sender, e);
        }


        private void Wifi_DiscoveredNetworksHandler(object sender, List<WifiNetworkInfo> e) {
            this.log.Info("Wifi_DiscoveredNetworksHandler", () => string.Format("Is DiscoveredNetworks null={0}", this.DiscoveredNetworks == null));
            this.DiscoveredNetworks?.Invoke(sender, e);
        }


    }
}
