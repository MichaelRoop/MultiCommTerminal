using Android.App;
using Android.Content;
using Android.Net.Wifi;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using CommunicationStack.Net.MsgPumps;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;

namespace Wifi.AndroidXamarin {

    public partial class WifiImplAndroidXamarin : IWifiInterface {

        private void DoConnection(WifiNetworkInfo dataModel) {

            //https://spin.atomicobject.com/2018/02/15/connecting-wifi-xamarin-forms/

            WifiConfiguration config = new WifiConfiguration() {
                Ssid = this.GetJavaArg(dataModel.RemoteHostName),
                PreSharedKey = this.GetJavaArg(dataModel.Password),
            };


            // TEMP - just add and go
            this.manager.AddNetwork(config);

            var network = this.manager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == config.Ssid);
            if (network == null) {
                this.log.Error(9999, "Could not get network");
                return;
            }
            else {
                this.log.Info("DoConnection", "CONNECTED");
            }

            this.manager.Disconnect();
            bool enabled = this.manager.EnableNetwork(network.NetworkId, true);
            this.log.Info("DoConnection", () => string.Format("ENABLED_CONNECTED:{0}", enabled));
            if (enabled) {
                if (manager.Reconnect()) {
                    this.log.Info("DoConnection", "Able to reconnect");

                    // Until I can debug the WIFI - In emulator cannot find network. Says it connects
                    // to WIFI but does not
                    //return;

                    var cData = new NetSocketConnectData() {
                        RemoteHostName = dataModel.RemoteHostName,
                        RemotePort = int.Parse(dataModel.RemoteServiceName),
                    };
                    this.msgPump.ConnectAsync(cData);
                }
                else {
                    this.log.Info("DoConnection", "UNABLE to reconnect");
                }
            }


        }



    }
}