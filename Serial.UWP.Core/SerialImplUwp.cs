using CommunicationStack.Net.DataModels;
using LogUtils.Net;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using VariousUtils.Net;

namespace Serial.UWP.Core {

    public partial class SerialImplUwp : ISerialInterface {

        public event EventHandler<List<SerialDeviceInfo>> DiscoveredDevices;
        public event EventHandler<SerialUsbError> OnError;
        public event EventHandler<MsgPumpConnectResults> OnSerialConnectionAttemptCompleted;


        #region ICommStack Implementations

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<byte[]> MsgReceivedEvent;


        public bool SendOutMsg(byte[] msg) {
            throw new NotImplementedException();
        }

        #endregion



        ClassLog log = new ClassLog("SerialImplUwp");


        public bool Connected => throw new NotImplementedException();

        public SerialImplUwp() {
            this.msgPump.ConnectResultEvent += this.MsgPump_ConnectResultEventHandler;
            this.msgPump.MsgReceivedEvent += this.MsgPump_MsgReceivedEventHandler;
        }

        public void Disconnect() {
            this.DoDisconnect();
        }


        #region Event handlers

        private void MsgPump_MsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("MsgPump_MsgReceivedEventHandler", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }


        private void MsgPump_ConnectResultEventHandler(object sender, MsgPumpConnectResults e) {
            this.OnSerialConnectionAttemptCompleted?.Invoke(this, e);
        }

        #endregion

    }
}
