using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CommunicationStack.Net.interfaces;
using CommunicationStack.Net.DataModels;
using System.Threading.Tasks;

namespace Communications.AndroidXamarin.MsgPumps {

    public class SocketConnData {


    }

    public class SerialMsgPump : IMsgPump<SocketConnData> {
        public bool Connected => throw new NotImplementedException();

        public event EventHandler<MsgPumpResults> MsgPumpConnectResultEvent;
        public event EventHandler<byte[]> MsgReceivedEvent;

        public void ConnectAsync(SocketConnData paramsObj) {
            throw new NotImplementedException();
        }

        public Task ConnectAsync2(SocketConnData paramsObj) {
            throw new NotImplementedException();
        }

        public void Disconnect() {
            throw new NotImplementedException();
        }

        public void WriteAsync(byte[] msg) {
            throw new NotImplementedException();
        }
    }
}