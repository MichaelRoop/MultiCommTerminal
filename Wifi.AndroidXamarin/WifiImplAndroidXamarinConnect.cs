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
using Xamarin.Essentials;
using static Android.Net.Wifi.WifiConfiguration;

namespace Wifi.AndroidXamarin {

    public partial class WifiImplAndroidXamarin : IWifiInterface {


        const int NETID = 1223344;

        private void DoConnection(WifiNetworkInfo dataModel) {

            this.log.Info("DoConnection", () => string.Format("    NetworkAccess:{0}", Connectivity.NetworkAccess));
            this.log.Info("DoConnection", () => string.Format("     Wifi Enabled:{0}", this.manager.IsWifiEnabled));
            this.log.Info("DoConnection", () => string.Format(" Has Wifi Profile:{0}", Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi)));

            if (Connectivity.ConnectionProfiles.Contains(ConnectionProfile.WiFi)) {
                var profile = Connectivity.ConnectionProfiles.FirstOrDefault(p => p == ConnectionProfile.WiFi);

               // int x = 5;
            }



            //return;


            //https://stackoverflow.com/questions/35729617/is-possible-to-programmatically-connect-to-wi-fi-network-using-xamarin
            if (!this.manager.IsWifiEnabled) {
                this.manager.SetWifiEnabled(true);
            }

            // move to disconnect method. With the pump disconnection
            this.manager.Disconnect();

            this.log.Info("DoConnection", () => string.Format("Number configured networks:{0}", this.manager.ConfiguredNetworks.Count));


            //string ssid = this.GetJavaArg(dataModel.SSID);
            //string pwd = this.GetJavaArg(dataModel.Password);


            //string ssid = '"' + dataModel.SSID + '"';
            //string pwd = "\"" + dataModel.Password + "\"";
            //string ssid = $"\"{{dataModel.SSID}}\"";
            //string pwd = $"\"{{dataModel.Password}}\"";

            string ssid = "\"{" + dataModel.SSID + "}\"";
            string pwd = "\"{" + dataModel.Password + "}\"";




            //https://spin.atomicobject.com/2018/02/15/connecting-wifi-xamarin-forms/
            //https://forums.xamarin.com/discussion/172062/connect-to-a-specified-wifi-wap-not-working
            // Add if not in configured networks
            var net = this.manager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == ssid);
            if (net == null) {
                this.log.Info("DoConnection", "NOT in Configured networks - ADDING");
                WifiConfiguration config = new WifiConfiguration() {
                    Ssid = ssid,
                    PreSharedKey = pwd,
                    //Priority = 40, // Deprecated
                    NetworkId = NETID,
                };
                // WPA or WPA2
                config.AllowedProtocols.Set((int)ProtocolType.Rsn);
                config.AllowedProtocols.Set((int)ProtocolType.Wpa);
                config.AllowedKeyManagement.Set((int)KeyManagementType.WpaPsk);
                config.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Ccmp);
                config.AllowedPairwiseCiphers.Set((int)PairwiseCipherType.Tkip);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Wep40);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Wep104);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Ccmp);
                config.AllowedGroupCiphers.Set((int)GroupCipherType.Tkip);
                config.AllowedAuthAlgorithms.Set((int)AuthAlgorithmType.Open);
                config.HiddenSSID = false;


                int result = this.manager.AddNetwork(config);
                //this.manager.SaveConfiguration();

                this.log.Info("DoConnection",
                    () => string.Format("Added network:{0} - Pwd:{1} Id:{2} RESULT:{3}", 
                        config.Ssid, config.PreSharedKey, config.NetworkId, result));


            }


            this.log.Info("DoConnection", () => string.Format("Number configured networks:{0}", this.manager.ConfiguredNetworks.Count));
            foreach (var x in this.manager.ConfiguredNetworks) {
                this.log.Info("DoConnection", 
                    () => string.Format("Networks:{0} - {1}", x.Ssid, x.NetworkId));
            }


            net = this.manager.ConfiguredNetworks.FirstOrDefault(n => n.Ssid == ssid);
            //if (net == null) {
            //    this.log.Error(9999, "Failed to add to ConfiguredNetworks");
            //    return;
            //}
            //else {
            //    this.log.Info("DoConnection", "In Configured networks");
            //}

            // Disconnect from any existing network
            this.manager.Disconnect();
            bool enabled = this.manager.EnableNetwork(net.NetworkId, true);
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