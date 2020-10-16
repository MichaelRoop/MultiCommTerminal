using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
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
                else {
                    if (adapters.Count > 1) {
                        this.log.Warning(9999, () => string.Format("{0} WIFI adapters. Selecting first", adapters.Count));
                    }
                    this.wifiAdapter = adapters[0];
                    this.DisplayAdapterValues(this.wifiAdapter);
                }
            }



            await this.wifiAdapter.ScanAsync();
            List<WifiNetworkInfo> networks = new List<WifiNetworkInfo>();
            // We will not return those with blank SSID
            foreach (var net in wifiAdapter.NetworkReport.AvailableNetworks) {
                if (net.Ssid.Length > 0) {
                    this.log.Info("", () => string.Format("Found {0}", net.Ssid));
                    networks.Add(new WifiNetworkInfo() {
                        SSID = net.Ssid,
                        Kind = net.NetworkKind.Convert(),
                    });
                }
            }

            this.log.Info("", () => string.Format("Found {0} networks", networks.Count));
            this.log.Info("", () => string.Format("DiscoveredNetworks={0}", this.DiscoveredNetworks == null));

            this.DiscoveredNetworks?.Invoke(this, networks);

            //this.log.Info("DoDiscovery", () => string.Format("Count {0}", result.Count));
            //foreach(var adapter in result) {
            //    this.log.Info("DoDiscovery", () => string.Format("Available networks {0}", adapter.NetworkReport.AvailableNetworks.Count));

                //    //// This gets all the available networks
                //    //await adapter.ScanAsync();
                //    //this.DisplayNetworkReport(adapter.NetworkReport);

                //    // Yes, this successfuly connects with passed in credentials
                //    //await this.ConnectToNetwork(adapter, "MikieArduinoWifi", "1234567890");
                //}
        }

        private void DisplayAdapterValues(WiFiAdapter adapter) {
            this.log.InfoEntry("ListAdapterValues");
            // TODO fill in object for use in display
            this.log.Info("ListAdapterValues", () => string.Format("  IATA Type: {0}", adapter.NetworkAdapter.IanaInterfaceType.ToEnum()));
            this.log.Info("ListAdapterValues", () => string.Format(" Max BPS In: {0}", adapter.NetworkAdapter.InboundMaxBitsPerSecond));
            this.log.Info("ListAdapterValues", () => string.Format("Max BPS Out: {0}", adapter.NetworkAdapter.OutboundMaxBitsPerSecond));
            this.log.Info("ListAdapterValues", () => string.Format(" Adapter Id: {0}", adapter.NetworkAdapter.NetworkAdapterId.ToString()));
            this.log.Info("ListAdapterValues", () => string.Format(" Network Id: {0}", adapter.NetworkAdapter.NetworkItem.NetworkId.ToString()));
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




    }
}
