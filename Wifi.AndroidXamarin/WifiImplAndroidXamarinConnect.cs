using Android.App;
using Android.Content;
using Android.Net;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.MsgPumps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;
using WifiCommon.Net.interfaces;
using Xamarin.Essentials;
using static Android.Net.ConnectivityManager;
//using static Android.Net.Wifi.WifiConfiguration;

namespace Wifi.AndroidXamarin {

    //// https://stackoverflow.com/questions/59992041/connect-to-wifi-programmatically-in-xamarin-forms-android-10
    //public class WifiConnectCallback : ConnectivityManager.NetworkCallback {
        
    //    ConnectivityManager connectivityManager;
    //    private string host = string.Empty;
    //    private int port = 0;

    //    public Action<WifiAndroidMsgPumpConnectData> NetworkAvailable { get; set; }
    //    public Action NetworkUnavailable { get; set; }

    //    public WifiConnectCallback(ConnectivityManager manager, string host, int port) {
    //        this.connectivityManager = manager;
    //        this.host = host;
    //        this.port = port;
    //    }

    //    public override void OnAvailable(Network network) {
    //        base.OnAvailable(network);
    //        this.NetworkAvailable?.Invoke(
    //            new WifiAndroidMsgPumpConnectData(network, this.host, this.port));
    //        this.connectivityManager.BindProcessToNetwork(network);
    //    }

    //    public override void OnUnavailable() {
    //        this.NetworkUnavailable?.Invoke();
    //        base.OnUnavailable();
    //    }

    //}



    public partial class WifiImplAndroidXamarin : IWifiInterface {


        const int NETID = 1223344;

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