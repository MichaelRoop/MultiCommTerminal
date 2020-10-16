using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using WifiCommon.Net.interfaces;
using Windows.Devices.WiFi;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        public void DiscoverWifiAdaptersAsync() {
            try {
                Task.Run(() => {
                    try {
                        this.DoDiscovery();
                    }
                    catch(Exception e) {
                        this.log.Exception(9999, "", e);
                    }

                });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }



        private async void DoDiscovery() {

            var result = await WiFiAdapter.FindAllAdaptersAsync();
            this.log.Info("DoDiscovery", () => string.Format("Count {0}", result.Count));
            foreach(var adapter in result) {
                foreach( var net in adapter.NetworkReport.AvailableNetworks) {
                    this.log.Info("DoDiscovery", () =>
                        string.Format(
                            "SSID:{0}", 
                            net.Ssid));
                }
            }
        }

    }
}
