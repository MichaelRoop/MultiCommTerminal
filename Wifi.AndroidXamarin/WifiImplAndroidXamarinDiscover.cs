using Android.Content;
using Android.Net.Wifi;
using System.Collections.Generic;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;

namespace Wifi.AndroidXamarin {

    public partial class WifiImplAndroidXamarin : IWifiInterface {

        /// <summary>Register the receiver and start scan for WIFI networks</summary>
        private void DoDiscovery() {
            this.log.Info("DoDiscovery", "Starting");
            if (!this.isListReceiverRunning) {
                this.isListReceiverRunning = true;
                this.GetContext().RegisterReceiver(
                    this.listReceiver,
                    new IntentFilter(WifiManager.ScanResultsAvailableAction));
                // Marked obsolete but will continue to use until something new offered
                this.manager.StartScan();
            }
        }


        /// <summary>List receiver callback</summary>
        /// <param name="results">The results of the scan</param>
        private void ListDiscoveryCallback(IList<ScanResult> results) {
            this.log.Info("ListDiscoveryCallback", "Starting");

            List<WifiNetworkInfo> networks = new List<WifiNetworkInfo>();
            foreach (var result in results) {
                // Ignore those with hidden SSID
                if (result.Ssid.Length > 0) {
                    WifiNetworkInfo info = new WifiNetworkInfo() {
                        SSID = result.Ssid,
                        MacAddress_BSSID = result.Bssid,
                        ChanneCenterFrequencyKlhz = result.Frequency,
                    };
                    this.log.Info("ListDiscoveryCallback", () => string.Format("                SSID:{0}", result.Ssid));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("               BSSID:{0}", result.Bssid));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("        Capabilities:{0}", result.Capabilities));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("   Center Frequency0:{0}", result.CenterFreq0));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("   Center Frequency1:{0}", result.CenterFreq1));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("       Channel Width:{0}", result.ChannelWidth));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("               Class:{0}", result.Class.ToString()));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("           Frequency:{0}", result.Frequency));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("  Is 80211 responder:{0}", result.Is80211mcResponder()));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("Is Passpoint Network:{0}", result.IsPasspointNetwork));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("               Level:{0}", result.Level));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("       Friendly name:{0}", result.OperatorFriendlyName.ToString()));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("           Timestamp:{0}", result.Timestamp.ToString()));
                    //this.log.Info("ListDiscoveryCallback", () => string.Format("          Venue name:{0}", result.VenueName.ToString()));
                    networks.Add(info);
                }
            }
            // unregister the receiver on completion
            this.GetContext().UnregisterReceiver(this.listReceiver);
            this.isListReceiverRunning = false;
            this.DiscoveredNetworks?.Invoke(this, networks);
        }

    }

}