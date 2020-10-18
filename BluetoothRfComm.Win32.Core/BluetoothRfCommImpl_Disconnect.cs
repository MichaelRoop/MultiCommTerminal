using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;

namespace BluetoothRfComm.UWP.Core {
    
    public partial class BluetoothRfCommUwpCore : IBTInterface {

        public void Disconnect() {
            //this.TearDown(false);
            this.msgPump.Disconnect();
        }


        ///// <summary>Tear down any connections, dispose and reset all resources</summary>
        //private void TearDown(bool sleepAfterSocketDispose) {
        //    try {
        //        #region Cancel Read Thread
        //        this.continueReading = false;
        //        if (this.readCancelationToken != null) {
        //            this.readCancelationToken.Cancel();
        //            this.readCancelationToken.Dispose();
        //            this.readCancelationToken = null;
        //            if (!this.readFinishedEvent.WaitOne(2000)) {
        //                this.log.Error(9999, "Timed out waiting for read cancelation");
        //            }
        //        }
        //        #endregion

        //        #region Close Writer and Reader
        //        if (this.writer != null) {
        //            try { this.writer.DetachStream(); }
        //            catch (Exception e) { this.log.Exception(9999, "", e); }
        //            this.writer.Dispose();
        //            this.writer = null;
        //        }

        //        if (this.reader != null) {
        //            try { this.reader.DetachStream(); }
        //            catch (Exception e) { this.log.Exception(9999, "", e); }
        //            this.reader.Dispose();
        //            this.reader = null;
        //        }
        //        #endregion

        //        #region Close socket
        //        if (this.socket != null) {
        //            // The socket was closed so cannot cancel IO
        //            this.socket.Dispose();
        //            this.socket = null;
        //            // Seems socket does not shut itself down fast enough before next call to connect
        //            if (sleepAfterSocketDispose) {
        //                Thread.Sleep(500);
        //            }
        //        }
        //        #endregion
        //        this.Connected = false;
        //    }
        //    catch (Exception e) {
        //        this.log.Exception(9999, "", e);
        //    }
        //}

    }
}
