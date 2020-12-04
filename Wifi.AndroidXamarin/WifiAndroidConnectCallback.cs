using Android.Net;
using System;

namespace Wifi.AndroidXamarin {

    /// <summary>Callback raised on connection attempt</summary>
    /// <remarks>
    /// https://stackoverflow.com/questions/59992041/connect-to-wifi-programmatically-in-xamarin-forms-android-10
    /// </remarks>
    public class WifiAndroidConnectCallback : ConnectivityManager.NetworkCallback {

        #region Data

        ConnectivityManager connectivityManager;
        private string host = string.Empty;
        private int port = 0;

        #endregion

        #region Properties

        /// <summary>Assign an action here that will be fired on connection</summary>
        public Action<WifiAndroidMsgPumpConnectData> NetworkAvailable { get; set; }

        /// <summary>Assign an action here that will be fired on failure</summary>
        public Action NetworkUnavailable { get; set; }

        #endregion

        #region Constructor and overrides

        public WifiAndroidConnectCallback(ConnectivityManager manager, string host, int port) {
            this.connectivityManager = manager;
            this.host = host;
            this.port = port;
        }


        /// <summary>Called when the connection is successful</summary>
        /// <param name="network">The connected network object</param>
        public override void OnAvailable(Network network) {
            this.connectivityManager.BindProcessToNetwork(network);
            base.OnAvailable(network);
            this.NetworkAvailable?.Invoke(
                new WifiAndroidMsgPumpConnectData(network, this.host, this.port));
        }


        /// <summary>Called when the connection failed</summary>
        public override void OnUnavailable() {
            this.NetworkUnavailable?.Invoke();
            base.OnUnavailable();
        }

        #endregion

    }

}