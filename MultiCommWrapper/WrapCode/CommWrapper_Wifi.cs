using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using WifiCommon.Net.DataModels;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<List<WifiNetworkInfo>> DiscoveredNetworks;
        public event EventHandler<WifiError> OnWifiError;

        public void WifiDiscoverAsync() {
            this.wifi.DiscoverWifiAdaptersAsync();
        }


        private void WifiInit() {
            this.wifi.DiscoveredNetworks += Wifi_DiscoveredNetworksHandler;
            this.wifi.OnError += this.Wifi_OnErrorHandler;
        }

        private void Wifi_OnErrorHandler(object sender, WifiError e) {
            this.log.Info("Wifi_OnErrorHandler", () => string.Format("Is OnWifiError null={0}", this.OnWifiError == null));
            this.OnWifiError?.Invoke(sender, e);
        }

        private void Wifi_DiscoveredNetworksHandler(object sender, List<WifiNetworkInfo> e) {
            this.log.Info("Wifi_DiscoveredNetworksHandler", () => string.Format("Is DiscoveredNetworks null={0}", this.DiscoveredNetworks == null));
            this.DiscoveredNetworks?.Invoke(sender, e);
        }


        private void WifiTeardown() {
            this.wifi.DiscoveredNetworks -= Wifi_DiscoveredNetworksHandler;
            this.wifi.OnError -= this.Wifi_OnErrorHandler;
        }


    }
}
