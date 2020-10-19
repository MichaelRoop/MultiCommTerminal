using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<List<WifiNetworkInfo>> DiscoveredNetworks;
        public event EventHandler<WifiError> OnWifiError;
        public event EventHandler<MsgPumpConnectResults> OnWifiConnectionAttemptCompleted;
        public event EventHandler<string> Wifi_BytesReceived;
        public event EventHandler<WifiCredentials> CredentialsRequestedEvent;

        public void WifiDiscoverAsync() {
            this.wifi.DiscoverWifiAdaptersAsync();
        }


        public void WifiConnectAsync(WifiNetworkInfo dataModel) {
            // We could check for cred here
            // Could put a simple request event from Wifi code if it determines it needs password

            // TODO use a try catch and change signature to have an error delegate return?
            try {
                if (this.WifiGetConnectCredentials(dataModel)) {
                    this.wifi.ConnectAsync(dataModel);
                }
            }
            catch (ErrReportException erE) {
                this.OnWifiError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = erE.Report.Msg });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                this.OnWifiError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = e.Message });
            }



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


        /// <summary>Retrieve credentials if not already stored with data model</summary>
        /// <param name="dataModel">The data on the connection</param>
        private bool WifiGetConnectCredentials(WifiNetworkInfo dataModel) {
            if (dataModel.RemoteHostName.Trim().Length == 0 ||
                dataModel.RemoteServiceName.Trim().Length == 0 ||
                dataModel.Password.Trim().Length == 0) {

                if (this.WifiGetStoredCredentials(dataModel)) {
                    return true;
                }

                if (this.GetUserCredentials(dataModel) == WifiErrorCode.Success) {
                    this.WifiStoreCredentials(dataModel);
                    return true;
                }
                return false;
            }
            return true;
        }



        private bool WifiGetStoredCredentials(WifiNetworkInfo dataModel) {
            // TODO - implement. Could wifi ssid or GUID to retrieve a stored WifiCredentials object
            //return false;


            dataModel.RemoteHostName = "192.168.4.1"; // IP of Arduino socket
            dataModel.RemoteServiceName = "80"; // Arduino HTTP port 80
            dataModel.Password = "1234567890";
            dataModel.UserName = ""; // Not using User name for now
            return true;
        }


        private void WifiStoreCredentials(WifiNetworkInfo dataModel) {
            // TODO implement - put data in credentials object and store

        }


        /// <summary>Raise an event to request credentials from user
        /// 
        /// </summary>
        /// <param name="ssid"></param>
        /// <returns></returns>
        private WifiErrorCode GetUserCredentials(WifiNetworkInfo dataModel) {
            // TODO - implement. Could also use the wifi GUID
            WifiCredentials cred = new WifiCredentials();
            WrapErr.ChkVar(this.CredentialsRequestedEvent, 9999, "No subscribers to CredentialsRequestedEvent");
            this.CredentialsRequestedEvent?.Invoke(this, cred);

            // TODO - check on validity of entries
            dataModel.RemoteHostName = cred.RemoteHostName;
            dataModel.RemoteServiceName = cred.RemoteServiceName;
            dataModel.Password = cred.WifiPassword;
            return WifiErrorCode.Success;
        }


        #region Wifi event handlers

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

        #endregion




    }
}
