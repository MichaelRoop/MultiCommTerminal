using Android.Net;

namespace Wifi.AndroidXamarin {

    public class WifiAndroidMsgPumpConnectData {
        public Network DiscoveredNetwork { get; set; } = null;
        public string HostName { get; set; } = string.Empty;
        public int Port { get; set; } = 0;


        public WifiAndroidMsgPumpConnectData(Network network, string host, int port) {
            this.DiscoveredNetwork = network;
            this.HostName = host;
            this.Port = port;
        }

    }
}