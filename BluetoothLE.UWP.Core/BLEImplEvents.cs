﻿using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using Common.Net.Network;
using System;
using System.Threading.Tasks;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        #region IBLETInterface Interface events

        public event EventHandler<BluetoothLEDeviceInfo> DeviceDiscovered;

        public event EventHandler<string> DeviceRemoved;

        public event EventHandler<bool> DeviceDiscoveryCompleted;

        public event EventHandler<NetPropertiesUpdateDataModel> DeviceUpdated;

        public event EventHandler<BLEGetInfoStatus> DeviceInfoAssembled;

        public event EventHandler<BLE_ConnectStatusChangeInfo> ConnectionStatusChanged;

        public event EventHandler<BLEOperationStatus> BLE_Error;

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


        private void RaiseIfError(BLEOperationStatus status) {
            if (status != BLEOperationStatus.Success) {
                Task.Run(() => {
                    try {
                        BLE_Error?.Invoke(this, status);
                    }
                    catch(Exception e) {
                        this.log.Exception(9999, "Raise if error", "", e);
                    }
                });
            }
        }

    }

}
