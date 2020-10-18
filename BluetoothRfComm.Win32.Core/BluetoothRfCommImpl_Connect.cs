using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using Communications.UWP.Core.MsgPumps;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothRfComm.UWP.Core {

    public partial class BluetoothRfCommUwpCore : IBTInterface {

        /// <summary>Synchronous connection</summary>
        /// <param name="deviceDataModel">Data model with device information</param>
        /// <returns>true on connection, false on failure</returns>
        public bool Connect(BTDeviceInfo deviceDataModel) {
            AutoResetEvent done = new AutoResetEvent(false);
            bool result = false;
            this.ConnectionCompleted += (sender, isOk) => {
                result = isOk;
                done.Set();
            };
            this.ConnectAsync(deviceDataModel);
            if (!done.WaitOne(5000)) {
                result = false;
            }
            return result;
        }



        /// <summary>Run asynchronous connection where ConnectionCompleted is raised on completion</summary>
        /// <param name="deviceDataModel">The data model with information on the device</param>
        public void ConnectAsync(BTDeviceInfo deviceDataModel) {
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    //this.TearDown(true);
                    this.msgPump.Disconnect();

                    await this.GetExtraInfo(deviceDataModel, false, false);

                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", deviceDataModel.RemoteHostName, deviceDataModel.RemoteServiceName));

                    this.msgPump.ConnectAsync(new SocketMsgPumpConnectData() {
                        MaxReadBufferSize = READ_BUFF_MAX_SIZE,
                        ProtectionLevel = SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication,
                        RemoteHostName = deviceDataModel.RemoteHostName,
                        ServiceName = deviceDataModel.RemoteServiceName,
                    });

                    //this.socket = new StreamSocket();
                    //await this.socket.ConnectAsync(
                    //    new HostName(deviceDataModel.RemoteHostName),
                    //    deviceDataModel.RemoteServiceName,
                    //    SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                    //this.writer = new DataWriter(this.socket.OutputStream);
                    //this.writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    //this.reader = new DataReader(this.socket.InputStream);
                    //this.reader.InputStreamOptions = InputStreamOptions.Partial;
                    //this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    //this.reader.ByteOrder = ByteOrder.LittleEndian;

                    //this.readCancelationToken = new CancellationTokenSource();
                    //this.readCancelationToken.Token.ThrowIfCancellationRequested();
                    //this.continueReading = true;

                    //this.Connected = true;
                    //this.LaunchReadTask();

                    //this.ConnectionCompleted?.Invoke(this, true);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "Connect Asyn Error", e);
                    this.ConnectionCompleted?.Invoke(this, false);
                }
            });
        }


    }
}
