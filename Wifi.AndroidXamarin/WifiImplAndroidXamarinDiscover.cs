using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;

namespace Wifi.AndroidXamarin {

    public partial class WifiImplAndroidXamarin : IWifiInterface {

        protected class WifiListReceiver : BroadcastReceiver {

            Action<IList<ScanResult>> onSuccess = null;

            public WifiListReceiver(Action<IList<ScanResult>> onSuccess) {
                this.onSuccess = onSuccess;
            }

            public override void OnReceive(Context context, Intent intent) {

                LogUtils.Net.Log.Info("WifiListReceiver",  "OnReceive", "*******************");


                IList<ScanResult> scanResults = new List<ScanResult>();
                WifiManager manager = (WifiManager)context.GetSystemService(Context.WifiService);
                foreach (var result in  manager.ScanResults) {

                    LogUtils.Net.Log.Info("WifiListReceiver", "OnReceive", "********* GOT ONE  **********");


                    scanResults.Add(result);
                }
                this.onSuccess.Invoke(scanResults);                
            }
        }

        WifiListReceiver listReceiver = null;



        private void DoDiscovery() {
            this.log.Info("DoDiscovery", "Starting");
            /*
            List<WifiNetworkInfo> networks = new List<WifiNetworkInfo>();
            foreach ( WifiConfiguration net in this.manager.ConfiguredNetworks) {
                WifiNetworkInfo info = new WifiNetworkInfo() {
                    SSID = net.Ssid,
                    MacAddress_BSSID = net.Bssid,
                    RemoteHostName = net.ProviderFriendlyName, // bogus
                    //RemoteServiceName = net.ser
                };

                this.log.Info("ListDiscoveryCallback", () => string.Format("                SSID:{0}", net.Ssid));
                this.log.Info("ListDiscoveryCallback", () => string.Format("               BSSID:{0}", net.Bssid));
                this.log.Info("ListDiscoveryCallback", () => string.Format("       Friendly name:{0}", net.ProviderFriendlyName));
                this.log.Info("ListDiscoveryCallback", () => string.Format("               Class:{0}", net.Class.ToString()));

                //this.log.Info("ListDiscoveryCallback", () => string.Format("        Capabilities:{0}", net.Capabilities));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("   Center Frequency0:{0}", net.CenterFreq0));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("   Center Frequency1:{0}", net.CenterFreq1));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("       Channel Width:{0}", net.ChannelWidth));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("           Frequency:{0}", renetsult.Frequency));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("  Is 80211 responder:{0}", net.Is80211mcResponder()));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("Is Passpoint Network:{0}", net.IsPasspointNetwork));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("               Level:{0}", net.Level));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("       Friendly name:{0}", net.OperatorFriendlyName.ToString()));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("           Timestamp:{0}", net.Timestamp.ToString()));
                //this.log.Info("ListDiscoveryCallback", () => string.Format("          Venue name:{0}", net.VenueName.ToString()));
                networks.Add(info);
            }
            this.DiscoveredNetworks?.Invoke(this, networks);
            */

            // Previous receiver
            this.listReceiver = new WifiListReceiver(this.ListDiscoveryCallback);
            this.GetContext().RegisterReceiver(
                this.listReceiver, new IntentFilter(WifiManager.ScanResultsAvailableAction));
            this.manager.StartScan();


        }


        private void ListDiscoveryCallback(IList<ScanResult> results) {
            this.log.Info("ListDiscoveryCallback", "Starting");

            List<WifiNetworkInfo> networks = new List<WifiNetworkInfo>();
            foreach (var result in results) {
                WifiNetworkInfo info = new WifiNetworkInfo() {
                    SSID =  result.Ssid,
                    MacAddress_BSSID = result.Bssid,
                    ChanneCenterFrequencyKlhz = result.Frequency,
                };
                this.log.Info("ListDiscoveryCallback", () => string.Format("                SSID:{0}", result.Ssid));
                this.log.Info("ListDiscoveryCallback", () => string.Format("               BSSID:{0}", result.Bssid));
                this.log.Info("ListDiscoveryCallback", () => string.Format("        Capabilities:{0}", result.Capabilities));
                this.log.Info("ListDiscoveryCallback", () => string.Format("   Center Frequency0:{0}", result.CenterFreq0));
                this.log.Info("ListDiscoveryCallback", () => string.Format("   Center Frequency1:{0}", result.CenterFreq1));
                this.log.Info("ListDiscoveryCallback", () => string.Format("       Channel Width:{0}", result.ChannelWidth));
                this.log.Info("ListDiscoveryCallback", () => string.Format("               Class:{0}", result.Class.ToString()));
                this.log.Info("ListDiscoveryCallback", () => string.Format("           Frequency:{0}", result.Frequency));
                this.log.Info("ListDiscoveryCallback", () => string.Format("  Is 80211 responder:{0}", result.Is80211mcResponder()));
                this.log.Info("ListDiscoveryCallback", () => string.Format("Is Passpoint Network:{0}", result.IsPasspointNetwork));
                this.log.Info("ListDiscoveryCallback", () => string.Format("               Level:{0}", result.Level));
                this.log.Info("ListDiscoveryCallback", () => string.Format("       Friendly name:{0}", result.OperatorFriendlyName.ToString()));
                this.log.Info("ListDiscoveryCallback", () => string.Format("           Timestamp:{0}", result.Timestamp.ToString()));
                this.log.Info("ListDiscoveryCallback", () => string.Format("          Venue name:{0}", result.VenueName.ToString()));


                networks.Add(info);
            }
            this.DiscoveredNetworks?.Invoke(this, networks);
            //this.GetContext().UnregisterReceiver(this.listReceiver);
            //this.listReceiver.Dispose();
            //this.listReceiver = null;
        }

        //.GetValueOrDefault()


    }

}