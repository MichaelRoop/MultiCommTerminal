using Android.Bluetooth;
using BluetoothCommon.Net;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using Java.Util;
using LogUtils.Net;
using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace BluetoothRfComm.AndroidXamarin {

    public class BTAndroidMsgPump : IMsgPump<BTAndroidMsgPumpConnectData> {

        #region Properties


        bool connected = false;

        public bool Connected {
            get {
                return this.socket == null ? false : this.socket.IsConnected;
            }
        }

        #endregion

        #region Data

        private const int READ_BUFF_SIZE = 256;
        private ClassLog log = new ClassLog("BluetoothAndroidMsgPump");
        private BluetoothSocket socket = null;
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
        public void ConnectAsync(BTAndroidMsgPumpConnectData paramsObj) {
            this.ConnectAsync2(paramsObj);
        }


        /// <summary>Awaitable asynchronous connection</summary>
        /// <param name="paramsObj">The object with connection parameters</param>
        /// <returns>The task to await</returns>
        public Task ConnectAsync2(BTAndroidMsgPumpConnectData paramsObj) {
            return Task.Run(async () => {
                try {
                    await this.DisconnectAsync();
                    this.InitForConnection();
                    this.socket = paramsObj.Device.CreateRfcommSocketToServiceRecord(
                        UUID.FromString(BT_Ids.SerialServiceGuid));
                    // TODO - look if we set values on input and output streams
                    await this.socket.ConnectAsync();
                    this.LaunchReadThread();
                    this.connected = true;
                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.Connected));
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
                            await this.socket.OutputStream.WriteAsync(msg, 0, msg.Length, this.writeCancelSource.Token);
                            await this.socket.OutputStream.FlushAsync(this.writeCancelSource.Token);
                        }
                        else {
                            this.log.Error(9999, "WriteAsync", "Not connected");
                        }
                    }
                    else {
                        this.log.Error(9999, "WriteAsync", "No socket created");
                    }
                }
                catch(TaskCanceledException) {
                    this.log.Info("WriteAsync", "Write canceled");
                }
                catch(Exception e) {
                    this.log.Exception(9999, "WriteAsync", "", e);
                }
            });
        }

        #endregion

        #region Private

        /// <summary>Awaitable disconnect</summary>
        /// <returns>The task to wait upon</returns>
        private Task DisconnectAsync() {
            return Task.Run(() => {
                try {
                    this.continueReading = false;
                    // Cancelling readCancelSource does not throw. So we close socket to abort immediately
                    if (this.socket != null) {
                        this.socket.Close();
                    }

                    if (this.readCancelSource != null) {
                        this.log.Info("DisconnectAsync", "Before read cancel");
                        this.readCancelSource.Cancel();
                        this.log.Info("DisconnectAsync", "After read cancel");
                        if (!this.readFinishedEvent.WaitOne(2000)) {
                            this.log.Error(1111, "DisconnectAsync", "Timed out waiting for read cancelation");
                        }
                        this.log.Info("DisconnectAsync", "After wait on readFinishedEvent");
                        this.readCancelSource.Dispose();
                        this.readCancelSource = null;
                    }
                    if (this.writeCancelSource != null) {
                        this.writeCancelSource.Cancel();
                        this.writeCancelSource.Dispose();
                        this.writeCancelSource = null;
                    }
                    if (this.socket != null) {
                        //this.socket.Close();
                        this.socket.Dispose();
                        this.socket = null;
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9898, "Disconnect", "", e);
                }
            });
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
                            this.readBuff, 0, READ_BUFF_SIZE, this.readCancelSource.Token);
                        if (count > 0) {
                            byte[] tmpBuff = new byte[count];
                            Array.Copy(this.readBuff, tmpBuff, count);
                            this.MsgReceivedHandler(this, tmpBuff);
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