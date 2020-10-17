using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VariousUtils.Net;
using Wifi.UWP.Core.Helpers;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;
using WifiCommon.Net.interfaces;
using Windows.Devices.WiFi;
using Windows.Security.Credentials;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        #region Data

        private WiFiAdapter wifiAdapter = null;
        private WifiAdapterInfo adapterInfo = new WifiAdapterInfo();
        IReadOnlyList<WiFiAvailableNetwork> networks = new List<WiFiAvailableNetwork>();

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
            if (this.wifiAdapter == null) {
                IReadOnlyList<WiFiAdapter> adapters = await WiFiAdapter.FindAllAdaptersAsync();
                if (adapters.Count == 0) {
                    this.OnError?.Invoke(this, new WifiError(WifiErrorCode.NoAdapters));
                    return;
                }

                if (adapters.Count > 1) {
                    this.log.Warning(9999, () => string.Format("{0} WIFI adapters. Selecting first", adapters.Count));
                }
                this.wifiAdapter = adapters[0];
            }

            this.InitAdapterInfo(this.wifiAdapter, this.adapterInfo);
            await this.ScanForNetworks(this.wifiAdapter, this.adapterInfo);

            // TODO - change to send up the whole adapter
            this.DiscoveredNetworks?.Invoke(this, this.adapterInfo.Networks);

            #region junk
            //this.log.Info("DoDiscovery", () => string.Format("Count {0}", result.Count));
            //foreach(var adapter in result) {
            //    this.log.Info("DoDiscovery", () => string.Format("Available networks {0}", adapter.NetworkReport.AvailableNetworks.Count));
            //    //// This gets all the available networks
            //    //await adapter.ScanAsync();
            //    //this.DisplayNetworkReport(adapter.NetworkReport);
            //    // Yes, this successfuly connects with passed in credentials
            //    //await this.ConnectToNetwork(adapter, "MikieArduinoWifi", "1234567890");
            //}
            #endregion
        }


        private async Task ScanForNetworks(WiFiAdapter adapter, WifiAdapterInfo info) {
            // TODO - will need a list of available UWP network objects to be able to find fof connection - or use the UWP adapter
            await adapter.ScanAsync();
            this.networks = this.wifiAdapter.NetworkReport.AvailableNetworks;
            info.Networks = new List<WifiNetworkInfo>();
            // We will not return those with blank SSID
            foreach (var net in this.networks) {
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



        private async Task ConnectToNetwork(WiFiAdapter adapter, string ssid, string password) {
            await adapter.ScanAsync();
            WiFiNetworkReport report = adapter.NetworkReport;

            this.log.Info("DisplayNetworkReport", () =>
                string.Format("Timestamp {0} Count {1}", report.Timestamp, report.AvailableNetworks.Count));
            foreach (var net in report.AvailableNetworks) {
                this.log.Info("DisplayNetworkReport", () =>
                    string.Format(
                        "SSID:{0}",
                        net.Ssid));

                if (net.Ssid == ssid) {
                    PasswordCredential cred = new PasswordCredential() {
                        Password = password
                    };

                   var result = await adapter.ConnectAsync(net, WiFiReconnectionKind.Automatic, cred);
                    this.log.Info("DisplayNetworkReport", () =>
                        string.Format("Connection result {0}", result.ConnectionStatus));



                }

            }
        }


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

    }
}
