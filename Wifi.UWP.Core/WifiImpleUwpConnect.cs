using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using Communications.UWP.Core.MsgPumps;
using System;
using System.Threading.Tasks;
using Wifi.UWP.Core.Helpers;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;
using WifiCommon.Net.interfaces;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        /// <summary>Run asynchronous connection where ConnectionCompleted is raised on completion</summary>
        /// <param name="deviceDataModel">The data model with information on the device</param>
        public void ConnectAsync(WifiNetworkInfo dataModel) {
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    this.msgPump.Disconnect();
                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", dataModel.RemoteHostName, dataModel.RemoteServiceName));

                    WiFiAvailableNetwork net = this.GetNetwork(dataModel.SSID);
                    if (net != null) {
                        // Connect WIFI level
                        // TODO Need to establish WIFI connection first with credentials
                        // TODO How to establish kind of authentication

                        switch (dataModel.AuthenticationType) {
                            // Arduino authentication - requires password but no user name
                            case NetAuthenticationType.RSNA_PSK:
                                break;
                        }

                        PasswordCredential cred = new PasswordCredential() {
                            Password = dataModel.Password,
                            //UserName = dataModel.UserName, // this blows up
                        };
                        WiFiConnectionResult result = await this.wifiAdapter.ConnectAsync(net, WiFiReconnectionKind.Automatic, cred);
                        if (result.ConnectionStatus == WiFiConnectionStatus.Success) {
                            //ConnectionProfile profile = await this.wifiAdapter.NetworkAdapter.GetConnectedProfileAsync();
                            //this.log.Info("ConnectAsync", () => string.Format("Connected to:{0}", profile.ProfileName));
                            //if (profile.IsWlanConnectionProfile) {

                            this.log.Info("ConnectAsync", () => string.Format("Connecting to {0}:{1}", dataModel.RemoteHostName, dataModel.RemoteServiceName));
                            this.msgPump.ConnectAsync(new SocketMsgPumpConnectData() {
                                    MaxReadBufferSize = 255,
                                    RemoteHostName = dataModel.RemoteHostName,
                                    ServiceName = dataModel.RemoteServiceName,
                                    // TODO - determine protection level according to connection
                                    ProtectionLevel = SocketProtectionLevel.PlainSocket,
                                });
                            //}
                            //else {
                            //    // TODO Add error. Not WIFI
                            //    this.OnError?.Invoke(this, new WifiError(WifiErrorCode.NetworkNotAvailable));
                            //}
                        }
                        else {
                            this.OnError?.Invoke(this,
                                new WifiError(EnumerationHelpers.Convert(result.ConnectionStatus)));
                        }
                    }
                }
                catch (ErrReportException erE) {
                    this.OnError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = erE.Report.Msg });
                }
                catch (Exception e) {
                    this.log.Exception(9999, "Connect Asyn Error", e);
                    this.OnError?.Invoke(this, new WifiError(WifiErrorCode.Unknown));
                }
            });
        }



        WiFiAvailableNetwork GetNetwork(string ssid) {
            foreach (WiFiAvailableNetwork net in this.wifiAdapter.NetworkReport.AvailableNetworks) {
                if (net.Ssid == ssid) {
                    return net;
                }
            }
            this.OnError(this, new WifiError(WifiErrorCode.NetworkNotAvailable));
            return null;
        }











    }
}
