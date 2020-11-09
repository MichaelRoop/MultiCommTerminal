using Communications.UWP.Core.MsgPumps;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.interfaces;
using Ethernet.Common.Net.DataModels;
using Ethernet.Common.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Devices.Enumeration;
using Windows.Networking.Connectivity;

namespace Ethernet.UWP.Core {



    public partial class EthernetImplUwp : IEthernetInterface {

        #region Data

        private ClassLog log = new ClassLog("EthernetImplUwp");
        private IMsgPump<SocketMsgPumpConnectData> msgPump = new SocketMsgPumpEthernet();

        #endregion

        #region Properties

        public bool Connected { get; private set; }

        #endregion

        #region IEthernetInterface Events

        public event EventHandler<EthernetParams> ParamsRequestedEvent;
        public event EventHandler<MsgPumpResults> OnEthernetConnectionAttemptCompleted;
        public event EventHandler<MsgPumpResults> OnError;

        #endregion

        #region ICommStackChannel Events

        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion


        public EthernetImplUwp() {
            this.msgPump.MsgReceivedEvent += MsgPump_MsgReceivedEventHandler;
            this.msgPump.MsgPumpConnectResultEvent += MsgPump_MsgPumpConnectResultEventHandler;
        }

        public void Disconnect() {
            this.log.InfoEntry("Disconnect");
            this.msgPump.Disconnect();
        }


        #region ICommStackChannel Methods

        public bool SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
            return true;
        }

        #endregion

        private void MsgPump_MsgPumpConnectResultEventHandler(object sender, CommunicationStack.Net.DataModels.MsgPumpResults results) {
            this.log.Info("MsgPump_MsgPumpConnectResultEvent", () =>
                string.Format("Error:{0} - '{1}'", results.Code, results.ErrorString));
            this.OnEthernetConnectionAttemptCompleted?.Invoke(sender, results);


            //if (results.Code == CommunicationStack.Net.Enumerations.MsgPumpResultCode.Connected) {
            //    this.log.Info("MsgPump_MsgPumpConnectResultEvent", "************************ WE ARE CONNECTED ************************ ");
            //    //this.msgPump.WriteAsync("OpenDoor\r\n".ToAsciiByteArray());
            //    //this.msgPump.WriteAsync("CloseDoor\r\n".ToAsciiByteArray());
            //}
            //else {
            //    this.log.Info("MsgPump_MsgPumpConnectResultEvent", 
            //        "************************ FAILED CONNECTION ************************ ");
            //    this.log.Info("MsgPump_MsgPumpConnectResultEvent", () => 
            //        string.Format("Error:{0} - '{1}'", results.Code, results.ErrorString));
            //}
        }


        private void MsgPump_MsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("MsgPump_MsgReceivedEventHandler", () => string.Format("Received:{0}",e.ToAsciiString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }


    }
}
