using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace BluetoothRfComm.UWP.Core {


    public partial class BluetoothRfCommUwpCore : IBTInterface {

        /// <summary>Write message from ICommStackChannel interface</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendOutMsg(byte[] msg) {
            this.WriteAsync(msg);
            return true;
        }



        /// <summary>Run an asynchronous write</summary>
        /// <param name="msg">The message bytes to write</param>
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
                this.log.Error(9999, "Not Connected");
            }
        }


        /// <summary>The read task</summary>
        private void LaunchReadTask() {
            Task.Run(async () => {
                this.log.InfoEntry("DoReadTask +++");
                this.readFinishedEvent.Reset();

                while (this.continueReading) {
                    try {
                        int count = (int)await this.reader.LoadAsync(READ_BUFF_MAX_SIZE).AsTask(this.readCancelationToken.Token);
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


        private void HandlerMsgReceived(object sender, byte[] e) {
            this.log.Info("BtWrapper_MsgReceivedEvent", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));

            this.MsgReceivedEvent?.Invoke(sender, e);
        }



    }
}
