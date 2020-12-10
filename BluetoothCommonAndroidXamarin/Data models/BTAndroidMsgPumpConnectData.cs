using Android.Bluetooth;

namespace BluetoothCommonAndroidXamarin.Data_models {

    /// <summary>Connection data for Bluetooth</summary>
    public class BTAndroidMsgPumpConnectData {

        public BluetoothDevice Device { get; set; } = null;

        public BTAndroidMsgPumpConnectData() { }

        public BTAndroidMsgPumpConnectData(BluetoothDevice device) {
            this.Device = device;
        }

    }
}