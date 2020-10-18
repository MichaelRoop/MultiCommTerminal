using BluetoothCommon.Net.interfaces;
using VariousUtils.Net;

namespace BluetoothRfComm.UWP.Core {


    public partial class BluetoothRfCommUwpCore : IBTInterface {

        /// <summary>Write message from ICommStackChannel interface</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
            return true;
        }

        private void MsgPump_MsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("BtWrapper_MsgReceivedEvent", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }

    }
}
