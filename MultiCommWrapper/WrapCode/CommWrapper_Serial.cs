using LanguageFactory.Net.data;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.Enumerations;
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

        public List<KeyValuePropertyDisplay> Serial_GetDeviceInfoForDisplay(SerialDeviceInfo info) {
            List<KeyValuePropertyDisplay> list = new List<KeyValuePropertyDisplay>();
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Name), info.Name));
            // Primary values
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Port), info.PortName));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.BaudRate), info.Baud));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.DataBits), info.DataBits));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.StopBits), info.StopBits.Display()));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Parity), info.Parity.ToString())); // Converter
            list.Add(new KeyValuePropertyDisplay("Flow Handshake", info.FlowHandshake.ToString().CamelCaseToSpaces()));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Vendor), info.USB_VendorIdDisplay));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Product), info.USB_ProductIdDisplay));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Default), info.IsDefault));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Enabled), info.IsEnabled));

            // TODO Add others

            //list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Name), info.Name));


            return list;
        }


        public List<StopBitDisplayClass> Serial_GetStopBitsForDisplay() {
            List<StopBitDisplayClass> stopBits = new List<StopBitDisplayClass>();
            stopBits.Add(new StopBitDisplayClass(SerialStopBits.One));
            stopBits.Add(new StopBitDisplayClass(SerialStopBits.OnePointFive));
            stopBits.Add(new StopBitDisplayClass(SerialStopBits.Two));
            return stopBits;
        }


        public List<SerialParityDisplayClass> Serial_GetParitysForDisplay() {
            List<SerialParityDisplayClass> paritys = new List<SerialParityDisplayClass>();
            paritys.Add(new SerialParityDisplayClass(SerialParityType.None));
            paritys.Add(new SerialParityDisplayClass(SerialParityType.Even));
            paritys.Add(new SerialParityDisplayClass(SerialParityType.Odd));
            paritys.Add(new SerialParityDisplayClass(SerialParityType.Mark));
            paritys.Add(new SerialParityDisplayClass(SerialParityType.Space));
            return paritys;
        }


        public List<uint> Serial_GetBaudsForDisplay() {
            List<uint> bauds = new List<uint>();
            bauds.Add(300);
            bauds.Add(600);
            bauds.Add(1200);
            bauds.Add(2400);
            bauds.Add(4800);
            bauds.Add(9600);
            bauds.Add(14400);
            bauds.Add(19200);
            bauds.Add(28800);
            bauds.Add(31250);
            bauds.Add(38400);
            bauds.Add(57600);
            bauds.Add(115200);
            return bauds;
        }


        public List<ushort> Serial_GetDataBits() {
            List<ushort> dataBits = new List<ushort>();
            dataBits.Add(5);
            dataBits.Add(6);
            dataBits.Add(7);
            dataBits.Add(8);
            return dataBits;
        }



        #endregion

        #region Init Teardown

        //SerialImplUwp serial
        private void SerialInit() {
            this.serialStack.SetCommChannel(this.serial);
            this.serialStack.InTerminators = "\r\n".ToAsciiByteArray();
            this.serialStack.OutTerminators = "\r\n".ToAsciiByteArray();
            this.serialStack.MsgReceived += this.SerialStack_MsgReceivedHandler;

            this.serial.DiscoveredDevices += this.Serial_DiscoveredDevicesHandler;
            this.serial.OnError += this.Serial_OnErrorHandler;
            // TODO connection complete
        }

        private void SerialTeardown() {
            this.serialStack.MsgReceived -= this.SerialStack_MsgReceivedHandler;

            this.serial.DiscoveredDevices -= this.Serial_DiscoveredDevicesHandler;
            this.serial.OnError -= this.Serial_OnErrorHandler;
        }

        #endregion

        #region Event handlers

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

        #endregion


    }
}
