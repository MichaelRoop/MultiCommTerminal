using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region ICommWrapper events

        public event EventHandler<List<SerialDeviceInfo>> SerialDiscoveredDevices;
        public event EventHandler<SerialUsbError> SerialOnError;
        public event EventHandler<string> Serial_BytesReceived;

        #endregion

        #region ICommWrapper methods

        public void SerialUsbDisconnect() {
            this.serial.Disconnect();
        }


        public void SerialUsbDiscoverAsync() {
            this.serial.DiscoverSerialDevicesAsync();
        }


        public void SerialUsbConnect(SerialDeviceInfo dataModel) {
            this.serial.ConnectAsync(dataModel);
        }


        public void SerialUsbSend(string msg) {
            this.serialStack.SendToComm(msg);
        }

        #endregion


        //SerialImplUwp serial
        private void SerialInit() {
            this.serialStack.SetCommChannel(this.serial);
            this.serialStack.InTerminators = "\n\r".ToAsciiByteArray();
            this.serialStack.OutTerminators = "\n\r".ToAsciiByteArray();
            this.serialStack.MsgReceived += this.SerialStack_MsgReceivedHandler;

            this.serial.DiscoveredDevices += this.Serial_DiscoveredDevicesHandler;
            this.serial.OnError += this.Serial_OnErrorHandler;
            // TODO connection complete
        }


        private void Serial_OnErrorHandler(object sender, SerialUsbError err) {
            // TODO - determine if you have to clean up the serial USB
            this.SerialOnError(sender, err);
        }


        private void Serial_DiscoveredDevicesHandler(object sender, List<SerialDeviceInfo> e) {
            this.log.Info("Serial_DiscoveredDevicesHandler", () => string.Format("Is Serial_DiscoveredDevicesHandler null={0}", this.SerialDiscoveredDevices == null));
            this.SerialDiscoveredDevices?.Invoke(sender, e);
        }


        private void SerialStack_MsgReceivedHandler(object sender, byte[] data) {
            string msg = Encoding.ASCII.GetString(data, 0, data.Length);
            this.log.Info("SerialStack_MsgReceivedHandler", () => string.Format("Msg In: '{0}'", msg));
            this.Serial_BytesReceived?.Invoke(sender, msg);
        }


        private void SerialTeardown() {
            this.serialStack.MsgReceived -= this.SerialStack_MsgReceivedHandler;

            this.serial.DiscoveredDevices -= this.Serial_DiscoveredDevicesHandler;
            this.serial.OnError -= this.Serial_OnErrorHandler;
        }

    }
}
