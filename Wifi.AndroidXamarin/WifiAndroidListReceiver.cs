using Android.Content;
using Android.Net.Wifi;
using LogUtils.Net;
using System;
using System.Collections.Generic;

namespace Wifi.AndroidXamarin {

    public class WifiAndroidListReceiver : BroadcastReceiver {

        private Action<IList<ScanResult>> onSuccess = null;
        private ClassLog log = new ClassLog("WifiAndroidListReceiver");

        public WifiAndroidListReceiver(Action<IList<ScanResult>> onSuccess) {
            this.onSuccess = onSuccess;
        }


        public override void OnReceive(Context context, Intent intent) {
            IList<ScanResult> scanResults = new List<ScanResult>();
            WifiManager manager = (WifiManager)context.GetSystemService(Context.WifiService);
            foreach (var result in manager.ScanResults) {
                scanResults.Add(result);
            }
            this.onSuccess.Invoke(scanResults);
        }

    }

}