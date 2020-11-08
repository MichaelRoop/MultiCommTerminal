using Communications.UWP.Core.MsgPumps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using VariousUtils.Net;
using WifiCommon.Net.DataModels;
using Windows.Networking.Sockets;

namespace Ethernet.UWP.Core {
    public partial class EthernetImplUwp {

        public void ConnectAsync() {
            this.log.InfoEntry("ConnectAsync");
            this.Disconnect();
            Thread.Sleep(500);

            // TODO - this will be passed in 
            WifiNetworkInfo dataModel = new WifiNetworkInfo() {
                RemoteHostName = "192.168.1.88",
                RemoteServiceName = "9999"
            };

            this.msgPump.ConnectAsync(new SocketMsgPumpConnectData() {
                MaxReadBufferSize = 100,
                RemoteHostName = dataModel.RemoteHostName,
                ServiceName = dataModel.RemoteServiceName,
                // TODO - determine protection level according to connection
                ProtectionLevel = SocketProtectionLevel.PlainSocket,
            });


        }


    }
}
