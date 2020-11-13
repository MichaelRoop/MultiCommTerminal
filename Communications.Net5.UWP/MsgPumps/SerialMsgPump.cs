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
using Windows.Storage.Streams;

namespace Communications.UWP.Core.MsgPumps {

    //https://stackoverflow.com/questions/44467220/uwp-serial-port-communication-for-character-write-and-read-uwp-and-arduino/44490311

    public class SerialMsgPump : IMsgPump<SerialMsgPumpConnectData> {

        #region Data

        private ClassLog log = new ClassLog("SerialMsgPump");
        private IInputStream inStream = null;
        private IOutputStream outStream = null;

        private DataWriter writer = null;
        private DataReader reader = null;
        private CancellationTokenSource readCancelationToken = null;
        private bool continueReading = false;
        private uint readBufferMaxSizer = 256;
        private ManualResetEvent readFinishedEvent = new ManualResetEvent(false);


        #endregion

        public bool Connected { get; private set; } = false;

        public event EventHandler<MsgPumpResults> MsgPumpConnectResultEvent;
        public event EventHandler<byte[]> MsgReceivedEvent;



        public void ConnectAsync(SerialMsgPumpConnectData paramsObj) {
            // Really too fast to be async but conforms to interface
            Task.Run(() => {
                try {
                    this.Teardown();
                    this.readBufferMaxSizer = paramsObj.MaxReadBufferSize;
                    this.inStream = paramsObj.InStream;
                    this.outStream = paramsObj.OutStream;
                    
                    this.writer = new DataWriter(this.outStream);
                    this.writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;

                    this.reader = new DataReader(this.inStream);
                    this.reader.InputStreamOptions = InputStreamOptions.Partial;
                    this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    this.reader.ByteOrder = ByteOrder.LittleEndian;
                    this.reader.InputStreamOptions = InputStreamOptions.Partial;

                    this.readCancelationToken = new CancellationTokenSource();
                    this.readCancelationToken.Token.ThrowIfCancellationRequested();
                    this.continueReading = true;
                    this.Connected = true;
                    this.LaunchReadTask();
                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.Connected));
                }
                catch(Exception e) {
                    this.log.Exception(9999, "", e);
                    this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.ConnectionFailure));
                }
            });
        }


        public void Disconnect() {
            this.Teardown();
        }


        public void WriteAsync(byte[] msg) {
            if (this.Connected) {
                Task.Run(async () => {
                    try {
                        this.log.Info("WriteAsync", () =>
                            string.Format("Sent:{0}", msg.ToFormatedByteString()));
                        this.writer.WriteBytes(msg);
                        // returns 24 - number of bytes sent
                        uint result = await this.writer.StoreAsync(); // This is if underlying is a stream
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "", e);
                    }
                });
            }
            else {
                this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.NotConnected));
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
                        int count = (int)await this.reader.LoadAsync(
                            this.readBufferMaxSizer).AsTask(this.readCancelationToken.Token);
                        //this.log.Info("Launch Read Task", () => string.Format("Received:{0} bytes", count));
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
                        this.MsgPumpConnectResultEvent?.Invoke(this, new MsgPumpResults(MsgPumpResultCode.ReadFailure));
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
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }



        private void Teardown() {
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
                #endregion

                this.Connected = false;
            }
            catch(Exception e) {
                this.log.Exception(9999, "", e);
            }
        }

        public Task ConnectAsync2(SerialMsgPumpConnectData paramsObj) {
            throw new NotImplementedException();
        }

        #endregion

    }

}
