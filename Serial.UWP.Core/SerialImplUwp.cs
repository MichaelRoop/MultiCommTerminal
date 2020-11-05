using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using LogUtils.Net;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.Enumerations;
using SerialCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using VariousUtils.Net;

namespace Serial.UWP.Core {

    public partial class SerialImplUwp : ISerialInterface {

        public event EventHandler<List<SerialDeviceInfo>> DiscoveredDevices;
        public event EventHandler<SerialUsbError> OnError;
        public event EventHandler<MsgPumpResults> OnSerialConnectionAttemptCompleted;


        #region ICommStack Implementations

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<byte[]> MsgReceivedEvent;


        public bool SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
            return true;
        }

        #endregion



        ClassLog log = new ClassLog("SerialImplUwp");


        public bool Connected { get; private set; } = false;

        public SerialImplUwp() {
            this.msgPump.MsgPumpConnectResultEvent += this.MsgPump_ConnectResultEventHandler;
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


        private void MsgPump_ConnectResultEventHandler(object sender, MsgPumpResults e) {
            switch (e.Code) {
                case MsgPumpResultCode.Connected:
                case MsgPumpResultCode.ConnectionFailure:
                    this.OnSerialConnectionAttemptCompleted?.Invoke(this, e);
                    break;
                case MsgPumpResultCode.NotConnected:
                    this.OnError?.Invoke(this, new SerialUsbError(SerialErrorCode.NotConnected));
                    break;
                case MsgPumpResultCode.ReadFailure:
                    this.OnError?.Invoke(this, new SerialUsbError(SerialErrorCode.ReadFailure));
                    break;
                case MsgPumpResultCode.WriteFailure:
                    this.OnError?.Invoke(this, new SerialUsbError(SerialErrorCode.WriteFailure));
                    break;
                default:
                    this.log.Error(9999, () => string.Format("Unhandled MsgPumpResult:{0}", e.Code));
                    break;
            }
        }

        #endregion

    }
}
