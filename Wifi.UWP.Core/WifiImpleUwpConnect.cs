using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;
using WifiCommon.Net.interfaces;
using Windows.Devices.WiFi;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        // TODO - this goes in the interface

        /// <summary>Run asynchronous connection where ConnectionCompleted is raised on completion</summary>
        /// <param name="deviceDataModel">The data model with information on the device</param>
        public void ConnectAsync(WifiNetworkInfo dataModel) {
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    // TODO Implement the tear down
                    //this.TearDown(true);
                    //await this.GetExtraInfo(deviceDataModel, false, false);

                    // TODO - can we get the host name and port (service) on scan?
                    string hostName = "192.168.4.1"; // IP of Arduino
                    string serviceName = "80"; // Current exposed port (service)
                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", hostName, serviceName));

                    WiFiAvailableNetwork net = this.GetNetwork(dataModel.SSID);
                    if (net != null) {
                        // Connect WIFI level
                        // TODO Need to establish WIFI connection first with credentials
                        


                        this.socket = new StreamSocket();
                        await this.socket.ConnectAsync(new HostName(hostName), serviceName);

                        // TODO establish credential requirements

                        this.writer = new DataWriter(this.socket.OutputStream);
                        this.writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                        this.reader = new DataReader(this.socket.InputStream);
                        this.reader.InputStreamOptions = InputStreamOptions.Partial;
                        this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                        this.reader.ByteOrder = ByteOrder.LittleEndian;

                        this.readCancelationToken = new CancellationTokenSource();
                        this.readCancelationToken.Token.ThrowIfCancellationRequested();
                        this.continueReading = true;

                        this.Connected = true;
                        this.LaunchReadTask();

                        // TODO event on connection complete
                        //this.ConnectionCompleted?.Invoke(this, true);
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "Connect Asyn Error", e);
                    // TODO event on connection complete
                    //this.ConnectionCompleted?.Invoke(this, false);
                }
            });
        }



        WiFiAvailableNetwork GetNetwork(string ssid) {
            foreach (WiFiAvailableNetwork net in this.wifiAdapter.NetworkReport.AvailableNetworks) {
                if (net.Ssid == ssid) {
                    return net;
                }
            }
            this.OnError(this, new WifiError(WifiErrorCode.NetworkNotAvailable));
            return null;
        }


    }
}
