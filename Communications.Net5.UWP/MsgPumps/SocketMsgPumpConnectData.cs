using CommunicationStack.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Networking.Sockets;

namespace Communications.UWP.Core.MsgPumps {

    public class SocketMsgPumpConnectData {
        public string RemoteHostName { get; set; }
        public string ServiceName { get; set; }
        public SocketProtectionLevel ProtectionLevel { get; set; } = SocketProtectionLevel.PlainSocket;
        public uint MaxReadBufferSize { get; set; } = 256;


    }
}
