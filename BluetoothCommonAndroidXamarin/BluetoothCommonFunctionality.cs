using Android.Bluetooth;
using BluetoothCommon.Net;
using BluetoothCommonAndroidXamarin.Data_models;
using BluetoothCommonAndroidXamarin.Messaging;
using BluetoothLE.Net.DataModels;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;

namespace BluetoothCommonAndroidXamarin {

    public partial class BluetoothCommonFunctionality {

        #region Data

        private ClassLog log = new ClassLog("BluetoothCommonFunctionality");
        private BluetoothDeviceType deviceType = BluetoothDeviceType.Classic;
        private List<BluetoothDevice> unBondedDevices = new List<BluetoothDevice>();
        private IMsgPump<BTAndroidMsgPumpConnectData> msgPump = new BTAndroidMsgPump();
        private bool connected = false;
        private BluetoothDevice device = null;

        #endregion

        #region Events

        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<BTPairOperationStatus> BT_PairStatus;
        public event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

        public event EventHandler<BluetoothLEDeviceInfo> DiscoveredBLEDevice;
        // TODO - determine if there is paring in BLE

        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        #region Properties

        public List<BluetoothDevice> UnBondedDevices { get { return this.unBondedDevices; } }
        public bool Connected { get { return this.connected; }  }

        #endregion


        public BluetoothCommonFunctionality(BluetoothDeviceType deviceType) {
            this.deviceType = deviceType;
            this.msgPump.MsgPumpConnectResultEvent += this.ConnectResultHandler;
            this.msgPump.MsgReceivedEvent += this.MsgReceivedHandler;
        }


    }
}