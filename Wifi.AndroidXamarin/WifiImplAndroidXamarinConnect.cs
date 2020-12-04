using Android.Net;
using Android.Net.Wifi;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;
using WifiCommon.Net.interfaces;
using static Android.Net.ConnectivityManager;

namespace Wifi.AndroidXamarin {

    public partial class WifiImplAndroidXamarin : IWifiInterface {


        private NetworkCallback connectCallback = null;

        private void DoConnection(WifiNetworkInfo dataModel) {

            if (!this.manager.IsWifiEnabled) {
                this.RaiseError(WifiErrorCode.NoAdapters);
                return;
            }

            this.Disconnect();

            int port;
            if (!int.TryParse(dataModel.RemoteServiceName, out port)) {
                this.RaiseError(WifiErrorCode.NonNumericPort);
                return;
            }
            // TODO - need check the host name length

            WifiNetworkSpecifier specifier = new WifiNetworkSpecifier.Builder().
                SetSsid(dataModel.SSID).
                SetWpa2Passphrase(dataModel.Password).
                Build();

            NetworkRequest request = new NetworkRequest.Builder()
               .AddTransportType(TransportType.Wifi)
               .RemoveCapability(NetCapability.Internet) // Internet not required
               .SetNetworkSpecifier(specifier) // we want _our_ network
               .Build();


            this.connectCallback = new WifiAndroidConnectCallback(this.connectivityManager, dataModel.RemoteHostName, port) {
                NetworkAvailable = this.OnNetworkAvailable,
                NetworkUnavailable = this.OnNetworkUnavailable
            };
            this.connectivityManager.RequestNetwork(request, this.connectCallback);
        }


        private void OnNetworkAvailable(WifiAndroidMsgPumpConnectData data) {

            //Java.Net.Socket s = network.SocketFactory.CreateSocket("", 88);
            //s.ConnectAsync()

            //TODO will need to dispose network when done

            //var cData = new NetSocketConnectData() {
            //    RemoteHostName = dataModel.RemoteHostName,
            //    RemotePort = int.Parse(dataModel.RemoteServiceName),
            //};
            //this.msgPump.ConnectAsync(cData);


            //network.SocketFactory.
            // TODO - get the socket and make a pump for it


            this.log.Info("OnNetworkAvailable", () => string.Format(
                "YAY - I AM CONNECTED"));


            this.msgPump.ConnectAsync(data);


            // wait till the msg pump connects to raise the event
            //this.OnWifiConnectionAttemptCompleted?.Invoke(this,
            //    new MsgPumpResults(MsgPumpResultCode.Connected));
        }

        private void OnNetworkUnavailable() {
            this.log.Info("OnNetworkUnavailable", () => string.Format("BOOOOO - FAILED CONNECTION"));

            this.OnWifiConnectionAttemptCompleted?.Invoke(this,
                new MsgPumpResults(MsgPumpResultCode.NotConnected));
        }

        private void RaiseError(WifiErrorCode code, string details = "") {
            WifiError err = new WifiError(code);
            if (details.Length > 0) {
                err.ExtraInfo = details;
            }
            this.OnError?.Invoke(this, err);
        }




    }
}