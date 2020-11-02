using Communications.UWP.Core.MsgPumps;
using CommunicationStack.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.Enumerations;
using SerialCommon.Net.interfaces;
using System;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;

namespace Serial.UWP.Core {

    public partial class SerialImplUwp : ISerialInterface {

        SerialDevice device = null;
        IMsgPump<SerialMsgPumpConnectData> msgPump = new SerialMsgPump();

        private void DoDisconnect() {
            // Always disconnect pump first
            this.msgPump.Disconnect();
            if (this.device != null) {
                this.device.Dispose();
                this.device = null;                
            }
            this.Connected = false;
        }


        public void ConnectAsync(SerialDeviceInfo info) {
            Task.Run(async () => {
                try {
                    this.Disconnect();

                    string aqs = SerialDevice.GetDeviceSelector(info.PortName);
                    DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(aqs);
                    this.log.Info("ConnectAsync", () => string.Format("aqs:{0}", aqs));
                    if (devices.Count > 0) {
                        this.log.Info("ConnectAsync", () => string.Format("Devices found:{0}", devices.Count));
                        //this.log.Info("ConnectAsync", () => string.Format(""));

                        this.device = await SerialDevice.FromIdAsync(devices[0].Id);
                        if (this.device != null) {
                            // Set values from the device info
                            this.device.BaudRate = info.Baud;
                            this.device.DataBits = info.DataBits;
                            this.device.StopBits = info.StopBits.Convert();
                            this.device.Parity = info.Parity.Convert();
                            this.device.ReadTimeout = TimeSpan.FromMilliseconds(5);
                            this.device.WriteTimeout = TimeSpan.FromMilliseconds(5);


                            this.msgPump.ConnectAsync(new SerialMsgPumpConnectData() {
                                InStream = this.device.InputStream,
                                OutStream = this.device.OutputStream,
                                MaxReadBufferSize = 255,
                            });
                            this.Connected = true;
                        }
                        else {
                            this.log.Error(9999, "The device was not retrieved");
                            this.OnError?.Invoke(this, new SerialUsbError(info.PortName, SerialErrorCode.RetrieveFailed));
                        }
                    }
                    else {
                        this.log.Error(9999, () => string.Format("{0} Not Found", info.PortName));
                        this.OnError?.Invoke(this, new SerialUsbError(info.PortName, SerialErrorCode.NotFound));
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                    this.OnError?.Invoke(this, new SerialUsbError(info.PortName, SerialErrorCode.Unknown));
                }
            });


        }

    
    }


}
