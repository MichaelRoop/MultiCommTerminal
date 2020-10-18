using BluetoothCommon.Net.interfaces;

namespace BluetoothRfComm.UWP.Core {

    public partial class BluetoothRfCommUwpCore : IBTInterface {

        public void Disconnect() {
            this.msgPump.Disconnect();
        }
    }
}
