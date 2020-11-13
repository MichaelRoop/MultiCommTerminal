using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace Communications.Net5.Win.MsgPumps {

    public class SocketMsgPump /*: IMsgPump<SocketMsgPumpConnectData>*/ {

        ClassLog log = new ClassLog("");

        public bool Connected => throw new NotImplementedException();

        public event EventHandler<MsgPumpResults> MsgPumpConnectResultEvent;
        public event EventHandler<byte[]> MsgReceivedEvent;

        public void ConnectAsync(/*SocketMsgPumpConnectData paramsObj*/) {


            //SocketInformationOptions sio = new SocketInformationOptions() {
            //};

            //SocketInformation si = new SocketInformation() {
               
            //};

            //var s = new Socket();

        }

        //public Task ConnectAsync2(/*SocketMsgPumpConnectData paramsObj*/) {
        //}

        public void Disconnect() {
            
        }

        public void WriteAsync(byte[] msg) {
            
        }


        public void DoDiscovery() {

            //System.Windows.Devices.

            IPHostEntry hostEntry = Dns.GetHostEntry("");
            foreach (IPAddress add in hostEntry.AddressList) {

            }


        }


    }
}
