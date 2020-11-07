using Communications.UWP.Core.MsgPumps;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Devices.Enumeration;
using Windows.Networking.Connectivity;

namespace Ethernet.UWP.Core {



    public partial class EthernetImplUwp {

        private ClassLog log = new ClassLog("+=+=+=+ EthernetImplUwp");
        private IMsgPump<SocketMsgPumpConnectData> msgPump = new SocketMsgPumpEthernet();


        public EthernetImplUwp() {
            this.msgPump.MsgReceivedEvent += MsgPump_MsgReceivedEvent;
            this.msgPump.MsgPumpConnectResultEvent += MsgPump_MsgPumpConnectResultEvent;
        }

        private void MsgPump_MsgPumpConnectResultEvent(object sender, CommunicationStack.Net.DataModels.MsgPumpResults results) {
            if (results.Code == CommunicationStack.Net.Enumerations.MsgPumpResultCode.Connected) {
                this.log.Info("MsgPump_MsgPumpConnectResultEvent", "************************ WE ARE CONNECTED ************************ ");
                this.msgPump.WriteAsync("OpenDoor\r\n".ToAsciiByteArray());
                this.msgPump.WriteAsync("CloseDoor\r\n".ToAsciiByteArray());
            }
            else {
                this.log.Info("MsgPump_MsgPumpConnectResultEvent", 
                    "************************ FAILED CONNECTION ************************ ");
                this.log.Info("MsgPump_MsgPumpConnectResultEvent", () => 
                    string.Format("Error:{0} - '{1}'", results.Code, results.ErrorString));


            }
        }

        private void MsgPump_MsgReceivedEvent(object sender, byte[] e) {
            this.log.Info("MsgPump_MsgReceivedEvent", e.ToAsciiString());
        }

        public void Disconnect() {
            this.log.InfoEntry("Disconnect");
            this.msgPump.Disconnect();
        }

    }
}
