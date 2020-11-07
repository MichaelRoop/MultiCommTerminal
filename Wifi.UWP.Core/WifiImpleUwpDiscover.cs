using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using VariousUtils.Net;
using Wifi.UWP.Core.Helpers;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;
using WifiCommon.Net.interfaces;
using Windows.Devices.WiFi;
using Windows.Networking;
using Windows.Networking.Connectivity;
using Windows.Networking.Sockets;
using Windows.Security.Credentials;
using Windows.Storage.Streams;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        #region Data

        private static WiFiAdapter wifiAdapter = null;
        private static WifiAdapterInfo adapterInfo = new WifiAdapterInfo();
        private static IReadOnlyList<WiFiAvailableNetwork> networks = new List<WiFiAvailableNetwork>();

        #endregion


        public void DiscoverWifiAdaptersAsync() {
            try {
                Task.Run(() => {
                    try {
                        this.DoDiscovery();
                    }
                    catch(Exception e) {
                        this.log.Exception(9999, "", e);
                        this.OnError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = e.Message });
                    }
                });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                this.OnError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = e.Message });
            }
        }


        private async void DoDiscovery() {
            if (wifiAdapter == null) {
                IReadOnlyList<WiFiAdapter> adapters = await WiFiAdapter.FindAllAdaptersAsync();
                if (adapters.Count == 0) {
                    this.OnError?.Invoke(this, new WifiError(WifiErrorCode.NoAdapters));
                    return;
                }

                if (adapters.Count > 1) {
                    this.log.Warning(9999, () => string.Format("{0} WIFI adapters. Selecting first", adapters.Count));
                }
                wifiAdapter = adapters[0];
            }

            this.InitAdapterInfo(wifiAdapter, adapterInfo);
            await this.ScanForNetworks(wifiAdapter, adapterInfo);

            // TODO - change to send up the whole adapter
            this.DiscoveredNetworks?.Invoke(this, adapterInfo.Networks);

            // Hack test to connect and send
            //await this.ConnectToNetworkAndSendTest(this.wifiAdapter, "MikieArduinoWifi", "1234567890");
        }


        private async Task ScanForNetworks(WiFiAdapter adapter, WifiAdapterInfo info) {
            // TODO - will need a list of available UWP network objects to be able to find fof connection - or use the UWP adapter
            await adapter.ScanAsync();
            networks = wifiAdapter.NetworkReport.AvailableNetworks;
            info.Networks = new List<WifiNetworkInfo>();
            // We will not return those with blank SSID
            foreach (var net in networks) {
                if (net.Ssid.Length > 0) {
                    WifiNetworkInfo netInfo = new WifiNetworkInfo() {
                        SSID = net.Ssid,
                        Kind = net.NetworkKind.Convert(),
                        ChanneCenterFrequencyKlhz = net.ChannelCenterFrequencyInKilohertz,
                        BeaconInterval = net.BeaconInterval,
                        MacAddress_BSSID = net.Bssid,
                        RssiInDecibleMilliwatts = net.NetworkRssiInDecibelMilliwatts,
                        PhysicalLayerKind = net.PhyKind.Convert(),
                        AuthenticationType = net.SecuritySettings.NetworkAuthenticationType.Convert(),
                        EncryptionType = net.SecuritySettings.NetworkEncryptionType.Convert(),
                        IsWifiDirect = net.IsWiFiDirect,
                        SignalStrengthInBars = net.SignalBars,
                        UpTime = net.Uptime,
                    };
                    this.DumpNetworkInfo(netInfo);
                    info.Networks.Add(netInfo);
                }
            }
            this.log.Info("ScanForNetworks", () => string.Format("Found {0} networks", info.Networks.Count));
        }


        private void InitAdapterInfo(WiFiAdapter adapter, WifiAdapterInfo info) {
            this.log.InfoEntry("InitAdapterInfo");
            info.AdapterId = adapter.NetworkAdapter.NetworkAdapterId;
            info.IanaType = adapter.NetworkAdapter.IanaInterfaceType.ToEnum();
            info.MaxBitsPerSecondIn = adapter.NetworkAdapter.InboundMaxBitsPerSecond;
            info.MaxBitsPerSecondOut = adapter.NetworkAdapter.OutboundMaxBitsPerSecond;
            this.DumpAdapterInfo(info);
        }


        private  void DisplayNetworkReport(WiFiNetworkReport report) {
            this.log.Info("DisplayNetworkReport", () => 
                string.Format("Timestamp {0} Count {1}", report.Timestamp, report.AvailableNetworks.Count));
            foreach (var net in report.AvailableNetworks) {
                this.log.Info("DisplayNetworkReport", () =>
                    string.Format(
                        "SSID:{0}",
                        net.Ssid));
            }
        }



        private async Task<WifiErrorCode> ConnectToNetwork(WiFiAdapter adapter, string ssid, string password) {
            // Should already be scanned
            WiFiNetworkReport report = adapter.NetworkReport;
            WifiErrorCode returnValue = WifiErrorCode.NetworkNotAvailable;
            foreach (var net in report.AvailableNetworks) {
                if (net.Ssid == ssid) {
                    // TODO Will need to have multiple types of authentication
                    PasswordCredential cred = new PasswordCredential() {
                        Password = password
                    };
                    returnValue = (await adapter.ConnectAsync(net, WiFiReconnectionKind.Automatic, cred)).ConnectionStatus.Convert();
                    break;
                }
            }
            if (returnValue != WifiErrorCode.Success) {
                this.OnError?.Invoke(this, new WifiError(returnValue));
            }
            return returnValue;
        }



        /// <summary>Working hack function to connect to device, open socket and send data string
        /// 
        /// </summary>
        /// <param name="adapter"></param>
        /// <param name="ssid"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        //private async Task ConnectToNetworkAndSendTest(WiFiAdapter adapter, string ssid, string password) {
        //    //// Should already be scanned
        //    //await adapter.ScanAsync();
        //    WiFiNetworkReport report = adapter.NetworkReport;
        //    this.log.Info("ConnectToNetworkAndSendTest", () =>
        //        string.Format("Timestamp {0} Count {1}", report.Timestamp, report.AvailableNetworks.Count));
        //    foreach (var net in report.AvailableNetworks) {
        //        if (net.Ssid == ssid) {
        //            PasswordCredential cred = new PasswordCredential() {
        //                Password = password
        //            };

        //            var result = await adapter.ConnectAsync(net, WiFiReconnectionKind.Automatic, cred);
        //            this.log.Info("ConnectToNetworkAndSendTest", () =>
        //                string.Format("Connection result {0}", result.ConnectionStatus));
        //            if (result.ConnectionStatus == WiFiConnectionStatus.Success) {
        //                // Setup socket and send dummy string
        //                //https://stackoverflow.com/questions/36475752/network-connection-with-uwp-apps
        //                try {
        //                    //StreamSocket ss = new StreamSocket();

        //                    ConnectionProfile profile = await this.wifiAdapter.NetworkAdapter.GetConnectedProfileAsync();
        //                    //connectProfile.ProfileName
        //                    this.log.Info("ConnectToNetworkAndSendTest", () => 
        //                        string.Format("Connected to:{0}", profile.ProfileName));
        //                    if (profile.IsWlanConnectionProfile) {
        //                        // Only get the SSID - no socket information
        //                        //profile.WlanConnectionProfileDetails.GetConnectedSsid();
        //                    }


        //                    //foreach (var ep in NetworkInformation.GetInternetConnectionProfile) {

        //                    //}


        //                    //foreach (var ids in NetworkInformation.GetLanIdentifiers()) {
        //                    //foreach (var prf in NetworkInformation.GetConnectionProfiles()) {


        //                    using (StreamSocket ss = new StreamSocket()) {
        //                        HostName host = new HostName("192.168.4.1"); // IP of the Arduino WIFI

        //                        this.log.Info("ConnectToNetworkAndSendTest", () =>
        //                            string.Format("Connecting socket to  {0}", host.DisplayName, host.CanonicalName));

        //                        await ss.ConnectAsync(host, "80");
        //                        StreamSocketInformation i = ss.Information;
        //                        this.log.Info("ConnectToNetworkAndSendTest", () => string.Format("Connected to socket Local {0}:{1} Remote {2}:{3} - {4}",
        //                            i.LocalAddress, i.LocalPort,
        //                            i.RemoteHostName, i.RemotePort, i.RemoteServiceName));
        //                        /*
        //                        // This works
        //                        Stream outStream = ss.OutputStream.AsStreamForWrite();
        //                        StreamWriter writer = new StreamWriter(outStream);
        //                        string data = "This is test from UWP";
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Sending");
        //                        //await writer.WriteLineAsync(data);
        //                        var arr = data.ToAsciiByteArray();
        //                        //await writer.WriteAsync(arr, 0, arr.Length);
        //                        await outStream.WriteAsync(arr, 0, arr.Length);
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Flushing");
        //                        await outStream.FlushAsync();
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Sending");
        //                        await outStream.WriteAsync(arr, 0, arr.Length);
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Flushing");
        //                        await outStream.FlushAsync();
        //                        // Hmm do not seem to need the writer
        //                        */


        //                        /*
        //                        Stream outStream = ss.OutputStream.AsStreamForWrite();
        //                        StreamWriter writer = new StreamWriter(outStream);
        //                        string data = "This is test from UWP";
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Sending");
        //                        await writer.WriteLineAsync(data);
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Flushing");
        //                        await writer.FlushAsync();
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Finished flush");
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Sending");
        //                        await writer.WriteLineAsync(data);
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Flushing");
        //                        await writer.FlushAsync();
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Finished flush");
        //                        */

        //                        // This also works - 10 seconds for first, second instant
        //                        // The time is due to establishing the socket first time. Will go to the connection routine
        //                        this.writer = new DataWriter(ss.OutputStream);
        //                        string data = "This is test from UWP";
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Sending");
        //                        this.writer.UnicodeEncoding = UnicodeEncoding.Utf8;
        //                        this.writer.WriteBytes(data.ToAsciiByteArray());
        //                        this.writer.WriteByte(0x0A);
        //                        //this.writer.WriteString(data);
        //                        await ss.OutputStream.WriteAsync(this.writer.DetachBuffer());
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Finished sending");
        //                        //this.writer.WriteString(data);
        //                        this.writer.WriteBytes(data.ToAsciiByteArray());
        //                        this.writer.WriteByte(0x0A);
        //                        await ss.OutputStream.WriteAsync(this.writer.DetachBuffer());
        //                        this.log.Info("ConnectToNetworkAndSendTest", "Finished sending");


        //                    }
        //                }
        //                catch (Exception e) {
        //                    this.log.Exception(9999, "", e);
        //                }

        //            }


        //            break;
        //        }
        //    }
        //}

        #region Debug dump methods

        private void DumpAdapterInfo(WifiAdapterInfo info) {
            this.log.Info("DumpAdapterInfo", () => string.Format("  IANA Type: {0}", info.IanaType));
            this.log.Info("DumpAdapterInfo", () => string.Format(" Max BPS In: {0}", info.MaxBitsPerSecondIn));
            this.log.Info("DumpAdapterInfo", () => string.Format("Max BPS Out: {0}", info.MaxBitsPerSecondOut));
            this.log.Info("DumpAdapterInfo", () => string.Format(" Adapter Id: {0}", info.AdapterId.ToString()));
        }


        private void DumpNetworkInfo(WifiNetworkInfo info) {
            this.log.Info("DumpNetworkInfo", () => string.Format("-----------------------------------------------------"));
            this.log.Info("DumpNetworkInfo", () => string.Format("                    SSID: {0}", info.SSID));
            this.log.Info("DumpNetworkInfo", () => string.Format("             Wifi Direct: {0}", info.IsWifiDirect));
            this.log.Info("DumpNetworkInfo", () => string.Format("                    Kind: {0}", info.Kind));
            this.log.Info("DumpNetworkInfo", () => string.Format("                     MAC: {0}", info.MacAddress_BSSID));
            this.log.Info("DumpNetworkInfo", () => string.Format("     Authentication Type: {0}", info.AuthenticationType.ToString().UnderlineToSpaces()));
            this.log.Info("DumpNetworkInfo", () => string.Format("         Encryption Type: {0}", info.EncryptionType.ToString().UnderlineToSpaces()));
            this.log.Info("DumpNetworkInfo", () => string.Format("         Beacon Interval: {0}", info.BeaconInterval));
            this.log.Info("DumpNetworkInfo", () => string.Format("Center Channel Frequency: {0}", info.ChanneCenterFrequencyKlhz));
            this.log.Info("DumpNetworkInfo", () => string.Format("          Physical Layer: {0}", info.PhysicalLayerKind));
            this.log.Info("DumpNetworkInfo", () => string.Format("           RSSI Decibles: {0}", info.RssiInDecibleMilliwatts));
            this.log.Info("DumpNetworkInfo", () => string.Format("    Signal Strength Bars: {0}", info.SignalStrengthInBars));
            this.log.Info("DumpNetworkInfo", () => string.Format("                 Up Time: {0}", info.UpTime));
        }

        #endregion


        //private async Task ConnectToNetwork(WiFiAdapter adapter, string ssid, string password) {
        //    await adapter.ScanAsync();
        //    WiFiNetworkReport report = adapter.NetworkReport;

        //    this.log.Info("DisplayNetworkReport", () =>
        //        string.Format("Timestamp {0} Count {1}", report.Timestamp, report.AvailableNetworks.Count));
        //    foreach (var net in report.AvailableNetworks) {
        //        this.log.Info("DisplayNetworkReport", () =>
        //            string.Format(
        //                "SSID:{0}",
        //                net.Ssid));
        //        if (net.Ssid == ssid) {
        //            PasswordCredential cred = new PasswordCredential() {
        //                Password = password
        //            };

        //            var result = await adapter.ConnectAsync(net, WiFiReconnectionKind.Automatic, cred);
        //            this.log.Info("DisplayNetworkReport", () =>
        //                string.Format("Connection result {0}", result.ConnectionStatus));
        //        }
        //    }
        //}



    }
}
