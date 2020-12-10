using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VariousUtils.Net;

namespace BluetoothCommonAndroidXamarin {

    public partial class BluetoothCommonFunctionality {



        public void SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
        }


        #region Event Handlers

        private void MsgReceivedHandler(object sender, byte[] e) {
            this.log.Info("MsgReceivedEventHandler", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }

        #endregion

    }

}