using Communications.UWP.Core.Extensions;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace Communications.UWP.Core.MsgPumps {
    public abstract class SocketMsgPumpBase : IMsgPump<SocketMsgPumpConnectData> {

        #region Data

        private ClassLog log = new ClassLog("SocketMsgPumpBase");
        private static bool continueReading = false;
        private static bool isConnected = false;

        private StreamSocket socket = null;
        private DataReader reader = null;
        private DataWriter writer = null;

        #endregion

        #region Properties
        public bool Connected {
            get {
                return isConnected;
            }
            private set {
                isConnected = value;
            }
        }

        #endregion

        #region Event Handlers 

        public event EventHandler<MsgPumpResults> MsgPumpConnectResultEvent;
        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        public SocketMsgPumpBase() { }


        public async Task ConnectAsync2(SocketMsgPumpConnectData paramsObj) {
            try {
                this.TearDown(true);
                this.log.Info("ConnectAsync2", () => string.Format(
                    "Host:{0} Service:{1}", paramsObj.RemoteHostName, paramsObj.ServiceName));

                this.socket = new StreamSocket();
                await socket.ConnectAsync(
                    new HostName(paramsObj.RemoteHostName),
                    paramsObj.ServiceName,
                    paramsObj.ProtectionLevel);

                StreamSocketInformation i = this.socket.Information;
                this.log.Info("ConnectAsync2", () => string.Format(
                    "Connected to socket Local {0}:{1} Remote {2}:{3} - {4} : Protection:{5}",
                    i.LocalAddress, i.LocalPort,
                    i.RemoteHostName, i.RemotePort, i.RemoteServiceName, i.ProtectionLevel));

                this.writer = new DataWriter(this.socket.OutputStream);
                this.writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                this.reader = new DataReader(socket.InputStream);
                this.reader.InputStreamOptions = InputStreamOptions.Partial;
                this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                this.reader.ByteOrder = ByteOrder.LittleEndian;

                this.CancelToken = new CancellationTokenSource();
                this.CancelToken.Token.ThrowIfCancellationRequested();
                continueReading = true;

                this.Connected = true;
                this.LaunchReadTask(paramsObj.MaxReadBufferSize);

                this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.Connected));
            }
            catch (Exception e) {
                this.log.Exception(9999, "Connect Asyn2 Error", e);
                this.MsgPumpConnectResultEvent?.Invoke(this, 
                    new MsgPumpResults(MsgPumpResultCode.ConnectionFailure, e.GetSocketCode()));
            }
        }


        public void ConnectAsync(SocketMsgPumpConnectData paramsObj) {
            this.log.InfoEntry("ConnectAsync");
            this.TearDown(true);
            Task.Run(async () => {
                try {
                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", paramsObj.RemoteHostName, paramsObj.ServiceName));

                    this.socket = new StreamSocket();
                    await socket.ConnectAsync(
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

                    this.reader = new DataReader(socket.InputStream);
                    this.reader.InputStreamOptions = InputStreamOptions.Partial;
                    this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    this.reader.ByteOrder = ByteOrder.LittleEndian;

                    this.CancelToken = new CancellationTokenSource();
                    this.CancelToken.Token.ThrowIfCancellationRequested();
                    continueReading = true;

                    this.Connected = true;
                    this.LaunchReadTask(paramsObj.MaxReadBufferSize);

                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.Connected));
                }
                catch (Exception e) {
                    this.log.Exception(9999, "Connect Asyn Error", e);
                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.ConnectionFailure));
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
                            await this.socket.OutputStream.FlushAsync();
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

        #region Abstract Properties

        /// <summary>Derived returns static AutoResetEvent to persiste in async methods</summary>
        /// <returns>The reset event</returns>
        protected abstract ManualResetEvent ReadFinishEvent { get; }

        /// <summary>Derived returns static cancelation token to persiste in async methods</summary>
        /// <returns>The cancelation token</returns>
        protected abstract CancellationTokenSource CancelToken { get; set; }

        #endregion

        #region Private

        private void LaunchReadTask(uint readBufferMaxSizer) {
            this.log.InfoEntry("DoReadTask +++");
            Task.Run(async () => {
                this.ReadFinishEvent.Reset();
                while (continueReading) {
                    try {
                        int count = (int)await this.reader.LoadAsync(readBufferMaxSizer).AsTask(this.CancelToken.Token);
                        if (count > 0) {
                            this.log.Error(9, "received");
                            if (count > 0) {
                                byte[] tmpBuff = new byte[count];
                                this.reader.ReadBytes(tmpBuff);
                                this.HandlerMsgReceived(this, tmpBuff);
                            }
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

                // Must clean up the reader, writer, socket here since they were 
                // created in the async space
                await this.CleanUpAsyncObjects();
            });
        }


        private async Task CleanUpAsyncObjects() {
            try {
                if (this.Connected) {
                    if (this.socket != null && false) {
                        // Put false to keep it from firing. 
                        //ObjectDisposedException : The object has been closed. (0x80000013)
                        // Not sure how it is disposed.
                        await this.socket.CancelIOAsync();
                    }

                    if (this.writer != null) {
                        try {
                            this.writer.DetachStream();
                        }
                        catch (Exception e) {
                            this.log.Exception(9999, "", e);
                        }
                        this.writer.Dispose();
                        this.writer = null;
                    }

                    if (this.reader != null) {
                        try {
                            this.reader.DetachStream();
                        }
                        catch (Exception e) {
                            this.log.Exception(9999, "", e);
                        }
                        this.reader.Dispose();
                        this.reader = null;
                    }

                    if (this.socket != null) {
                        this.socket.Dispose();
                        this.socket = null;
                    }
                    this.Connected = false;
                    this.ReadFinishEvent.Set();
                }
            }
            catch (Exception e) {
                this.log.Exception(7777, "", e);
            }
        }


        private void TearDown(bool sleepAfterSocketDispose) {
            try {
                this.log.InfoEntry("TearDown");
                #region Cancel Read Thread
                if (this.Connected) {
                    continueReading = false;
                    if (this.CancelToken != null) {
                        this.CancelToken.Cancel();
                        this.CancelToken.Dispose();
                        this.CancelToken = null;
                        if (!this.ReadFinishEvent.WaitOne(2000)) {
                            this.log.Error(9999, "Timed out waiting for read cancelation");
                        }
                    }

                    // Seems socket does not shut itself down fast enough before next call to connect
                    if (sleepAfterSocketDispose) {
                        Thread.Sleep(500);
                    }
                }
                #endregion
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
            this.log.InfoExit("TearDown");
        }


        private void HandlerMsgReceived(object sender, byte[] msg) {
            this.log.Info("HandlerMsgReceived", () => string.Format("Received:{0}", msg.ToFormatedByteString()));
            try {
                this.MsgReceivedEvent?.Invoke(sender, msg);
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }

        #endregion

    }
}
