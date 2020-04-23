using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BluetoothLE.Win32 {

    public partial class BluetoothLEImplWin32 : IBLETInterface {

        #region IBLETInterface Interface events

        public event EventHandler<BluetoothLEDeviceInfo> DeviceDiscovered;

        public event EventHandler<string> DeviceRemoved;

        #endregion

        #region IBLETInterface:ICommStackChannel events

        // TODO - the read thread on the BLE will raise this
        // When bytes come in
        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion



    }

}
