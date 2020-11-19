using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BluetoothCommon.Net;
using Common.Net.Network.Enumerations;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using Java.Util;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace BluetoothRfComm.AndroidXamarin {
    
    public class BTAndroidMsgPump : IMsgPump<BTAndroidMsgPumpConnectData> {

        #region Properties

        public bool Connected {
            get {
                return this.socket == null ? false : this.socket.IsConnected;
            }
        }

        #endregion

        #region Data

        private ClassLog log = new ClassLog("BluetoothAndroidMsgPump");
        private BluetoothSocket socket = null;
        private CancellationTokenSource readCancelSource = null;
        private CancellationTokenSource writeCancelSource = null;
        private bool continueReading = false;
        private AutoResetEvent readFinishedEvent = new AutoResetEvent(false);

        #endregion

        #region Events

        public event EventHandler<MsgPumpResults> MsgPumpConnectResultEvent;
        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        #region Public


        public void ConnectAsync(BTAndroidMsgPumpConnectData paramsObj) {
            this.ConnectAsync2(paramsObj);
        }


        public Task ConnectAsync2(BTAndroidMsgPumpConnectData paramsObj) {
            return Task.Run(async () => {
                try {
                    this.Disconnect();
                    this.InitForConnection();
                    this.socket = paramsObj.Device.CreateRfcommSocketToServiceRecord(
                        UUID.FromString(BT_Ids.SerialServiceGuid));
                    // TODO - look if we set values on input and output streams
                    await this.socket.ConnectAsync();
                    this.LaunchReadThread();
                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.Connected));
                }
                catch (Exception e) {
                    this.log.Exception(9999, "ConnectAsync", "", e);
                    this.MsgPumpConnectResultEvent?.Invoke(this, 
                        new MsgPumpResults(MsgPumpResultCode.ConnectionFailure, e.Message));
                }
            });
        }


        public void Disconnect() {
            try {
                //if (this.Connected) {
                this.continueReading = false;
                if (this.readCancelSource != null) {
                    this.readCancelSource.Cancel();
                    this.readCancelSource.Dispose();
                    this.readCancelSource = null;
                    if (!this.readFinishedEvent.WaitOne(2000)) {
                        this.log.Error(9999, "Timed out waiting for read cancelation");
                    }
                }
                if (this.writeCancelSource != null) {
                    this.writeCancelSource.Cancel();
                    this.writeCancelSource.Dispose();
                    this.writeCancelSource = null;
                }
                if (this.socket != null) {
                    this.socket.Close();
                    this.socket.Dispose();
                    this.socket = null;
                }
                //}
            }
            catch (Exception e) {
                this.log.Exception(9898, "Disconnect", "", e);
            }
        }


        public void WriteAsync(byte[] msg) {
            Task.Run(async () => {
                try {
                    if (this.socket != null) {
                        if (this.socket.IsConnected) {
                            await this.socket.OutputStream.WriteAsync(msg, 0, msg.Length, this.writeCancelSource.Token);
                            await this.socket.OutputStream.FlushAsync(this.writeCancelSource.Token);
                        }
                        else {
                            // Error not connected
                        }
                    }
                    else {
                        // Error. No socket
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

        private void InitForConnection() {
            this.readCancelSource = new CancellationTokenSource();
            this.readCancelSource.Token.ThrowIfCancellationRequested();
            this.writeCancelSource = new CancellationTokenSource();
            this.writeCancelSource.Token.ThrowIfCancellationRequested();
            this.continueReading = true;
            this.readFinishedEvent.Reset();
        }

        byte[] readBuff = new byte[256];


        private void LaunchReadThread() {
            Task.Run(async () => {
                this.log.InfoEntry("DoReadTask +++");
                while (this.continueReading) {
                    try {
                        int count = await this.socket.InputStream.ReadAsync(
                            this.readBuff, 0, 256, this.readCancelSource.Token);
                        if (count > 0) {
                            byte[] tmpBuff = new byte[count];
                            Array.Copy(this.readBuff, tmpBuff, count);
                            this.MsgReceivedHandler(this, tmpBuff);
                        }
                    }
                    catch (TaskCanceledException) {
                        this.log.Info("DoReadTask", "Cancelation");
                        break;
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "", e);
                        this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.ReadFailure));
                        break;
                    }
                }
                this.log.InfoExit("DoReadTask ---");
                this.readFinishedEvent.Set();
            });
        }


        private void MsgReceivedHandler(object sender, byte[] msg) {
            this.log.Info("HandlerMsgReceived", () => string.Format("Received:{0}", msg.ToFormatedByteString()));
            try {
                this.MsgReceivedEvent?.Invoke(sender, msg);
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }



    }
}