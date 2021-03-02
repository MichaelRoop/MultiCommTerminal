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
            //Task t =this.DisconnectAsync();
            //if (!t.Wait(1000)) {
            //    this.log.Error(9999, "ConnectAsync", "Timeout on wait for disconnect Async in user thread");
            //}

            this.DisconnectSynchronous(true);
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");

                    //await this.DisconnectAsync();

                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", dataModel.RemoteHostName, dataModel.RemoteServiceName));

                    WiFiAvailableNetwork net = this.GetNetwork(dataModel.SSID);
                    if (net != null) {
                        // Connect WIFI level
                        // TODO How to establish kind of authentication

                        switch (dataModel.AuthenticationType) {
                            // Arduino authentication - requires password but no user name
                            case NetAuthenticationType.RSNA_PSK:
                                break;
                        }

                        WiFiConnectionResult result = null;
                        PasswordCredential cred = this.GetCredentials(dataModel);
                        if (cred == null) {
                            result = await wifiAdapter.ConnectAsync(net, WiFiReconnectionKind.Automatic);
                        }
                        else {
                            result = await wifiAdapter.ConnectAsync(net, WiFiReconnectionKind.Automatic, cred);
                        }
    
                        // If the password is bad you get a timeout rather than failed credentials
                        switch (result.ConnectionStatus) {
                            case WiFiConnectionStatus.Success:
                                //ConnectionProfile profile = await this.wifiAdapter.NetworkAdapter.GetConnectedProfileAsync();
                                //this.log.Info("ConnectAsync", () => string.Format("Connected to:{0}", profile.ProfileName));
                                //if (profile.IsWlanConnectionProfile) {

                                //await this.DumpWifiAdapterInfo(wifiAdapter);
                                this.log.Info("ConnectAsync", () => string.Format("Connecting to {0}:{1}", dataModel.RemoteHostName, dataModel.RemoteServiceName));
                                //this.log.Info("ConnectAsync", () => string.Format(
                                //    "Connecting to {0}:{1}:{2}", dataModel.RemoteHostName, dataModel.RemoteServiceName, dataModel.Password));
                                // Connect socket
                                await msgPump.ConnectAsync2(new SocketMsgPumpConnectData() {
                                    MaxReadBufferSize = 255,
                                    RemoteHostName = dataModel.RemoteHostName,
                                    ServiceName = dataModel.RemoteServiceName,
                                    // TODO - determine protection level according to connection
                                    ProtectionLevel = SocketProtectionLevel.PlainSocket,
                                });
                                break;
                            case WiFiConnectionStatus.UnspecifiedFailure:
                            case WiFiConnectionStatus.AccessRevoked:
                            case WiFiConnectionStatus.InvalidCredential:
                            case WiFiConnectionStatus.NetworkNotAvailable:
                            case WiFiConnectionStatus.Timeout:
                            case WiFiConnectionStatus.UnsupportedAuthenticationProtocol:
                                this.OnError?.Invoke(this, new WifiError(result.ConnectionStatus.Convert()));
                                break;
                            default:
                                this.OnError?.Invoke(this, new WifiError( WifiErrorCode.Unknown));
                                break;
                        }
                    }
                    else {
                        this.OnError?.Invoke(this, new WifiError(WifiErrorCode.NetworkNotAvailable) { ExtraInfo = dataModel.SSID });
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
            foreach (WiFiAvailableNetwork net in wifiAdapter.NetworkReport.AvailableNetworks) {
                if (net.Ssid == ssid) {
                    return net;
                }
            }
            this.OnError(this, new WifiError(WifiErrorCode.NetworkNotAvailable));
            return null;
        }

        private async Task DumpWifiAdapterInfo(WiFiAdapter adapter) {
            try {
                if (adapter == null) {
                    this.log.Info("DumpWifiAdapterInfo", () => string.Format("Entry"));
                }
                else {
                    if (adapter.NetworkAdapter == null) {
                        this.log.Info("DumpWifiAdapterInfo", () => string.Format("NULL Wifi Adapter.NetworkAdapter"));
                    }
                    else {
                        try {
                            ConnectionProfile profile = await adapter.NetworkAdapter.GetConnectedProfileAsync();
                            if (profile == null) {
                                this.log.Info("DumpWifiAdapterInfo", () => string.Format("NULL Wifi Adapter.NetworkAdapter Profile"));
                            }
                            else {
                                this.log.Info("DumpWifiAdapterInfo", () => string.Format("Connected to:{0}", profile.ProfileName));
                                if (profile.IsWlanConnectionProfile) {
                                    this.log.Info("DumpWifiAdapterInfo", () => string.Format("WLan connection"));
                                    this.log.Info("DumpWifiAdapterInfo", () => string.Format("          SSID:{0}", profile.WlanConnectionProfileDetails.GetConnectedSsid()));
                                    this.log.Info("DumpWifiAdapterInfo", () => string.Format("Authentication:{0}", profile.NetworkSecuritySettings.NetworkAuthenticationType));
                                    this.log.Info("DumpWifiAdapterInfo", () => string.Format("    Encryption:{0}", profile.NetworkSecuritySettings.NetworkEncryptionType));
                                }
                                else if (profile.IsWwanConnectionProfile) {
                                    this.log.Info("DumpWifiAdapterInfo", () => string.Format("WWan connection"));
                                }
                                else {
                                    this.log.Info("DumpWifiAdapterInfo", () => string.Format("UNKNOWN connection type"));
                                }

                                //this.log.Info("DumpWifiAdapterInfo", () => string.Format("Connected to:{0}", profile.ProfileName));
                                //this.log.Info("DumpWifiAdapterInfo", () => string.Format("Connected to:{0}", profile.ProfileName));
                                //this.log.Info("DumpWifiAdapterInfo", () => string.Format("Connected to:{0}", profile.ProfileName));
                                //this.log.Info("DumpWifiAdapterInfo", () => string.Format("Connected to:{0}", profile.ProfileName));
                            }
                        }
                        catch (Exception e) {
                            this.log.Exception(9999, "", e);
                        }
                    }

                }


                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));
                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));
                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));
                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));
                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));
                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));
                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));
                //this.log.Info("DumpWifiAdapterInfo", () => string.Format(""));

            }
            catch (Exception e) {
                this.log.Exception(9999, "DumpWifiAdapterInfo", "", e);
            }
        }


        private PasswordCredential GetCredentials(WifiNetworkInfo info) {
            if (info.AuthenticationType == NetAuthenticationType.Open_802_11 &&
                info.EncryptionType == NetEncryptionType.None) {
                return null;
            }
            else {
                // We only ever use the password. Never user name
                return new PasswordCredential() {
                    Password = info.Password,
                };
            }
        }






    }
}
