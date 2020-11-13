using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Communications.UWP.Core.MsgPumps {

    public class SocketMsgPump : IMsgPump<SocketMsgPumpConnectData> {

        #region Data

        private ClassLog log = new ClassLog("SocketMsgPump");
        private StreamSocket socket = null;
        private DataWriter writer = null;
        private DataReader reader = null;
        private CancellationTokenSource readCancelationToken = null;
        private bool continueReading = false;
        private uint readBufferMaxSizer = 256;
        private ManualResetEvent readFinishedEvent = new ManualResetEvent(false);

        #endregion

        #region Properties

        public bool Connected { get; private set; } = false;

        #endregion

        #region Events

        public event EventHandler<MsgPumpResults> MsgPumpConnectResultEvent;
        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion


        public void ConnectAsync(SocketMsgPumpConnectData paramsObj) {
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    this.TearDown(true);
                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", paramsObj.RemoteHostName, paramsObj.ServiceName));

                    this.readBufferMaxSizer = paramsObj.MaxReadBufferSize;
                    this.socket = new StreamSocket();
                    await this.socket.ConnectAsync(
                        new HostName(paramsObj.RemoteHostName),
                        paramsObj.ServiceName,
                        paramsObj.ProtectionLevel);

                    StreamSocketInformation i = this.socket.Information;
                    this.log.Info("ConnectAsync", () => string.Format(
                        "Connected to socket Local {0}:{1} Remote {2}:{3} - {4} : Protection:{5}",
                        i.LocalAddress, i.LocalPort, 
                        i.RemoteHostName, i.RemotePort, i.RemoteServiceName, i.ProtectionLevel));



                    this.writer = new DataWriter(this.socket.OutputStream);
                    this.writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    this.reader = new DataReader(this.socket.InputStream);
                    this.reader.InputStreamOptions = InputStreamOptions.Partial;
                    this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    this.reader.ByteOrder = ByteOrder.LittleEndian;

                    this.readCancelationToken = new CancellationTokenSource();
                    this.readCancelationToken.Token.ThrowIfCancellationRequested();
                    this.continueReading = true;

                    this.Connected = true;
                    this.LaunchReadTask();

                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.Connected));
                }
                catch (Exception e) {
                    this.log.Exception(9999, "Connect Asyn Error", e);
                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults( MsgPumpResultCode.ConnectionFailure));
                }
            });
        }


        public void Disconnect() {
            this.TearDown(false);
        }


        public void WriteAsync(byte[] msg) {
            if (this.Connected) {
                if (this.socket != null) {
                    Task.Run(async () => {
                        try {
                            this.log.Info("WriteAsync", () =>
                                string.Format("Sent:{0}", msg.ToFormatedByteString()));

                            this.writer.WriteBytes(msg);
                            await this.socket.OutputStream.WriteAsync(this.writer.DetachBuffer());
                        }
                        catch (Exception e) {
                            this.log.Exception(9999, "", e);
                        }
                    });
                }
                else {
                    this.log.Error(9999, "Socket is null");
                }
            }
            else {
                // TODO - add events for error
                this.log.Error(9999, "Not Connected");
            }
        }

        #region Private

        private void LaunchReadTask() {
            Task.Run(async () => {
                this.log.InfoEntry("DoReadTask +++");
                this.readFinishedEvent.Reset();

                while (this.continueReading) {
                    try {
                        int count = (int)await this.reader.LoadAsync(this.readBufferMaxSizer).AsTask(this.readCancelationToken.Token);

                        this.log.Error(9, "received");
                        
                        if (count > 0) {
                            byte[] tmpBuff = new byte[count];
                            this.reader.ReadBytes(tmpBuff);
                            this.HandlerMsgReceived(this, tmpBuff);
                        }
                    }
                    catch (TaskCanceledException) {
                        this.log.Info("DoReadTask", "Cancelation");
                        break;
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "", e);
                        break;
                    }
                }
                this.log.InfoExit("DoReadTask ---");
                this.readFinishedEvent.Set();
            });
        }


        private void HandlerMsgReceived(object sender, byte[] msg) {
            this.log.Info("HandlerMsgReceived", () => string.Format("Received:{0}", msg.ToFormatedByteString()));
            try {
                this.MsgReceivedEvent?.Invoke(sender, msg);
            }
            catch(Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        /// <summary>Tear down any connections, dispose and reset all resources</summary>
        private void TearDown(bool sleepAfterSocketDispose) {
            try {
                #region Cancel Read Thread
                this.continueReading = false;
                if (this.readCancelationToken != null) {
                    this.readCancelationToken.Cancel();
                    this.readCancelationToken.Dispose();
                    this.readCancelationToken = null;
                    if (!this.readFinishedEvent.WaitOne(2000)) {
                        this.log.Error(9999, "Timed out waiting for read cancelation");
                    }
                }
                #endregion

                #region Close Writer and Reader
                if (this.writer != null) {
                    try { this.writer.DetachStream(); }
                    catch (Exception e) { this.log.Exception(9999, "", e); }
                    this.writer.Dispose();
                    this.writer = null;
                }

                if (this.reader != null) {
                    try { this.reader.DetachStream(); }
                    catch (Exception e) { this.log.Exception(9999, "", e); }
                    this.reader.Dispose();
                    this.reader = null;
                }
                #endregion

                #region Close socket
                if (this.socket != null) {
                    // The socket was closed so cannot cancel IO
                    this.socket.Dispose();
                    this.socket = null;
                    // Seems socket does not shut itself down fast enough before next call to connect
                    if (sleepAfterSocketDispose) {
                        Thread.Sleep(500);
                    }
                }
                #endregion
                this.Connected = false;
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }

        public Task ConnectAsync2(SocketMsgPumpConnectData paramsObj) {
            throw new NotImplementedException();
        }

        #endregion

    }
}
