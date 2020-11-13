using System.Threading;

namespace Communications.UWP.Core.MsgPumps {

    /// <summary>
    /// Derived instance of SocketMsgPumpBase for WIFI that passes its 
    /// static members to base to enable base to share with async methods.
    /// You can only have one instance in an application because of the 
    /// statics
    /// </summary>
    public class SocketMsgPumpWifi : SocketMsgPumpBase {

        #region static members

        // Must provide statics to kill the read thread since it is triggered from 
        // a different thread in base async methods

        private static CancellationTokenSource CANCEL_TOKEN = null;
        private static ManualResetEvent FINISH_READ_EVENT = new ManualResetEvent(false);

        #endregion

        #region SocketMsgPumpBase overrides for base to use its statics

        protected override ManualResetEvent ReadFinishEvent {
            get { return FINISH_READ_EVENT; }
        }


        protected override CancellationTokenSource CancelToken {
            get { return CANCEL_TOKEN; }
            set { CANCEL_TOKEN = value; }
        }

        #endregion

    }
}
