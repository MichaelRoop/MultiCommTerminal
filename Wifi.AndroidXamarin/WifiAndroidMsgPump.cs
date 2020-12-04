using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using Java.Net;
using LogUtils.Net;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace Wifi.AndroidXamarin {

    public class WifiAndroidMsgPump : IMsgPump<WifiAndroidMsgPumpConnectData> {

        #region Properties

        public bool Connected {
            get {
                return this.socket == null ? false : this.socket.IsConnected;
            }
        }

        #endregion

        #region Data

        private const int READ_BUFF_SIZE = 256;
        private ClassLog log = new ClassLog("WifiAndroidMsgPump");
        private Socket socket = null;
        private CancellationTokenSource readCancelSource = null;
        private CancellationTokenSource writeCancelSource = null;
        private bool continueReading = false;
        private AutoResetEvent readFinishedEvent = new AutoResetEvent(false);
        byte[] readBuff = new byte[READ_BUFF_SIZE];

        #endregion

        #region Events

        public event EventHandler<MsgPumpResults> MsgPumpConnectResultEvent;
        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        #region Public

        /// <summary>Connect async wrapper</summary>
        /// <param name="paramsObj">The object with connection parameters</param>
        public void ConnectAsync(WifiAndroidMsgPumpConnectData paramsObj) {
            this.ConnectAsync2(paramsObj);
        }


        /// <summary>Awaitable asynchronous connection</summary>
        /// <param name="paramsObj">The object with connection parameters</param>
        /// <returns>The task to await</returns>
        public Task ConnectAsync2(WifiAndroidMsgPumpConnectData paramsObj) {
            return Task.Run(async () => {
                try {
                    if (this.socket != null && this.socket.IsConnected) {
                        this.DisconnectAsync();
                    }
                    this.InitForConnection();
                    this.log.Info("ConnectAsync2", "Before connect");

                    this.socket = paramsObj.DiscoveredNetwork.SocketFactory.CreateSocket();
                    await this.socket.ConnectAsync(new InetSocketAddress(paramsObj.HostName, paramsObj.Port), 30000);

                    this.log.Info("ConnectAsync2", "After msg pump connect");
                    this.LaunchReadThread();
                    this.log.Info("ConnectAsync2", "After read thread launch");
                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.Connected));
                }
                catch (ConnectException ce) {
                    this.log.Exception(9999, "ConnectAsync2", "Connect Exception", ce);
                    // Bug with multiple firing of OnAvailable with ConnectionCallback where
                    // sockets not finished closing. Ignore 'E'rror 'CONN'ection 'A'borted
                    if (!ce.Message.Contains("ECONNABORTED")) {
                        this.MsgPumpConnectResultEvent?.Invoke(this,
                            new MsgPumpResults(MsgPumpResultCode.ConnectionFailure, ce.Message));
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "ConnectAsync2", "", e);
                    this.MsgPumpConnectResultEvent?.Invoke(this,
                        new MsgPumpResults(MsgPumpResultCode.ConnectionFailure, e.Message));
                }
            });
        }


        /// <summary>Calls the disconnect async</summary>
        public void Disconnect() {
            this.DisconnectAsync();
        }


        /// <summary>Do an asynchronous write</summary>
        /// <param name="msg">The message bytes to write</param>
        public void WriteAsync(byte[] msg) {
            Task.Run(async () => {
                try {
                    if (this.socket != null) {
                        if (this.socket.IsConnected) {
                            this.log.Info("WriteAsync", () => string.Format("WriteString:{0}", msg.ToAsciiString()));
                            await this.socket.OutputStream.WriteAsync(msg, 0, msg.Length, this.writeCancelSource.Token);
                            await this.socket.OutputStream.FlushAsync(this.writeCancelSource.Token);
                            this.log.Info("WriteAsync", "After write and flush");
                        }
                        else {
                            this.log.Error(9999, "WriteAsync", "Not connected");
                        }
                    }
                    else {
                        this.log.Error(9999, "WriteAsync", "No socket created");
                    }
                }
                catch (TaskCanceledException) {
                    this.log.Info("WriteAsync", "Write canceled");
                }
                catch (Exception e) {
                    this.log.Exception(9999, "WriteAsync", "", e);
                }
            });
        }

        #endregion

        #region Private

        /// <summary>Awaitable disconnect</summary>
        /// <returns>The task to wait upon</returns>
        private void DisconnectAsync() {
            try {
                this.continueReading = false;
                // Cancelling readCancelSource does not throw. So we close socket to abort immediately
                if (this.socket != null) {
                    this.socket.Close();
                    if (!this.readFinishedEvent.WaitOne(500)) {
                        this.log.Error(1111, "DisconnectAsync", "Timed out waiting for read cancelation FIRST CLOSE");
                    }
                    else {
                        this.log.Info("DisconnectAsync", "READ THREAD SUCCESSFULY ENDED");
                    }
                }

                try {
                    if (this.readCancelSource != null) {
                        this.log.Info("DisconnectAsync", "Before read cancel");
                        this.readCancelSource.Cancel();
                        if (!this.readFinishedEvent.WaitOne(100)) {
                            this.log.Error(1111, "DisconnectAsync", "Timed out waiting for read cancelation");
                        }
                        this.log.Info("DisconnectAsync", "After wait on readFinishedEvent");
                        this.readCancelSource.Dispose();
                        this.readCancelSource = null;
                    }
                }
                catch (Exception e) {
                    this.log.Exception(1, "On read cancel token", e);
                }

                try {
                    if (this.writeCancelSource != null) {
                        this.log.Info("DisconnectAsync", "Write cancel");
                        this.writeCancelSource.Cancel();
                        this.writeCancelSource.Dispose();
                        this.writeCancelSource = null;
                    }
                }
                catch (Exception e) {
                    this.log.Exception(1, "On write cancel token", e);
                }

                try {
                    if (this.socket != null) {
                        try { this.socket.Close(); } catch { }
                        this.socket.Dispose();
                        this.socket = null;
                    }
                }
                catch (Exception e) {
                    this.log.Exception(1, "On socket close", e);
                }

            }
            catch (Exception e) {
                this.log.Exception(9898, "Disconnect", "", e);
            }
        }


        /// <summary>Initialise the objects for a new connection</summary>
        private void InitForConnection() {
            this.readCancelSource = new CancellationTokenSource();
            this.readCancelSource.Token.ThrowIfCancellationRequested();
            this.writeCancelSource = new CancellationTokenSource();
            this.writeCancelSource.Token.ThrowIfCancellationRequested();
            this.continueReading = true;
            this.readFinishedEvent.Reset();
        }


        /// <summary>Launch asynchronous read thread operating in a loop</summary>
        private void LaunchReadThread() {
            Task.Run(async () => {
                this.log.InfoEntry("DoReadTask +++");
                while (this.continueReading) {
                    try {
                        int count = await this.socket.InputStream.ReadAsync(
                            this.readBuff, 0, READ_BUFF_SIZE,  this.readCancelSource.Token);
                        if (continueReading) {
                            if (count > 0) {
                                byte[] tmpBuff = new byte[count];
                                Array.Copy(this.readBuff, tmpBuff, count);
                                this.MsgReceivedHandler(this, tmpBuff);
                            }
                        }
                    }
                    catch (TaskCanceledException) {
                        if (this.continueReading) {
                            // Does not fire when canceling the token
                            this.log.Info("DoReadTask", "Cancelation");
                        }
                        break;
                    }
                    catch (IOException ioe) {
                        if (this.continueReading) {
                            this.log.Exception(9999, "ReadThread", "", ioe);
                            this.MsgPumpConnectResultEvent?.Invoke(this,
                                new MsgPumpResults(MsgPumpResultCode.ReadFailure));
                        }
                        break;
                    }
                    catch (Exception e) {
                        if (this.continueReading) {
                            this.log.Exception(9999, "ReadThread", "", e);
                            this.MsgPumpConnectResultEvent?.Invoke(this,
                                new MsgPumpResults(MsgPumpResultCode.ReadFailure));
                        }
                        break;
                    }
                }
                this.log.InfoExit("DoReadTask ---");
                this.readFinishedEvent.Set();
            });
        }


        /// <summary>Calls the outgoing MsgReceiveEvent</summary>
        /// <param name="sender">this</param>
        /// <param name="msg">The message bytes to raise</param>
        private void MsgReceivedHandler(object sender, byte[] msg) {
            this.log.Info("HandlerMsgReceived", () => string.Format("Received:{0}", msg.ToFormatedByteString()));
            try {
                this.MsgReceivedEvent?.Invoke(sender, msg);
            }
            catch (Exception e) {
                this.log.Exception(9999, "MsgReceivedHandler", "", e);
            }
        }

        #endregion





    }
}