using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using Common.Net.Network;
using System;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        #region IBLETInterface Interface events

        public event EventHandler<BluetoothLEDeviceInfo> DeviceDiscovered;

        public event EventHandler<string> DeviceRemoved;

        public event EventHandler<bool> DeviceDiscoveryCompleted;

        public event EventHandler<NetPropertiesUpdateDataModel> DeviceUpdated;

        public event EventHandler<BluetoothLEDeviceInfo> DeviceInfoAssembled;

        #endregion

        #region IBLETInterface:ICommStackChannel events

        // TODO - the read thread on the BLE will raise this
        // When bytes come in
        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion



        private void ToStatisfyCompilerWarnings() {
            // TODO use this later
            this.MsgReceivedEvent?.Invoke(this, null);
        }


    }

}
