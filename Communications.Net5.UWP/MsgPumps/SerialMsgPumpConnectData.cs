using System;
using System.Collections.Generic;
using System.Text;
using Windows.Storage.Streams;

namespace Communications.UWP.Core.MsgPumps {

    public class SerialMsgPumpConnectData {
        public IInputStream InStream { get; set; } = null;
        public IOutputStream OutStream { get; set; } = null;
        public uint MaxReadBufferSize { get; set; } = 250;
    }
}
