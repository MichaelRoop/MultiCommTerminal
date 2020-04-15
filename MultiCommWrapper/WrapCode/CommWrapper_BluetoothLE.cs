﻿using BluetoothCommon.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using VariousUtils;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Events and their handlers

        /// <summary>Event raised when a device is discovered</summary>
        public event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceDiscovered;


        private void BLE_DeviceDiscoveredHandler(object sender, BluetoothLEDeviceInfo e) {
            if (this.BLE_DeviceDiscovered != null) {
                this.BLE_DeviceDiscovered(this, e);
            }
        }

        #endregion

        #region Public

        public void BLE_DiscoverAsync() {
            this.bleBluetooth.DiscoverDevices();
        }

        public void BLE_ConnectAsync(BluetoothLEDeviceInfo device) {
            this.bleBluetooth.Connect(device);
        }

        #endregion

        #region Init and teardown

        private void BLE_Init() {
            this.bleStack.SetCommChannel(this.bleBluetooth);
            this.bleStack.InTerminators = "\n\r".ToAsciiByteArray();
            this.bleStack.OutTerminators = "\n\r".ToAsciiByteArray();
            this.bleStack.MsgReceived += this.BleStack_MsgReceived;
            this.bleBluetooth.DeviceDiscovered += this.BLE_DeviceDiscoveredHandler;
        }

        private void BleStack_MsgReceived(object sender, byte[] e) {
            throw new NotImplementedException();
        }

        private void BLE_TearDown() {
            this.bleStack.MsgReceived -= this.BleStack_MsgReceived;
            this.bleBluetooth.DeviceDiscovered -= this.BLE_DeviceDiscoveredHandler;
        }

        #endregion

    }
}
