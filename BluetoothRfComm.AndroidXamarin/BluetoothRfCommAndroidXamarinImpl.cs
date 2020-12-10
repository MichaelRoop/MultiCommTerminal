using Android.Bluetooth;
using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using BluetoothCommonAndroidXamarin;
using LogUtils.Net;
using System;

namespace BluetoothRfComm.AndroidXamarin {

    public partial class BluetoothRfCommAndroidXamarinImpl : IBTInterface {

        #region Data

        private ClassLog log = new ClassLog("BluetoothRfCommAndroidXamarinImpl");
        private BluetoothCommonFunctionality common = new BluetoothCommonFunctionality(BluetoothDeviceType.Classic);

        #endregion

        #region IBTInterface events

        // TODO Implement other events
        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<BTDeviceInfo> BT_DeviceInfoGathered;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<BT_PairInfoRequest> BT_PairInfoRequested;
        public event EventHandler<BTPairOperationStatus> BT_PairStatus;
        public event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

        #endregion

        #region ICommStackChannel Events

        public event EventHandler<byte[]> MsgReceivedEvent;

        #endregion

        #region Constructors

        public BluetoothRfCommAndroidXamarinImpl() {
            this.common.DiscoveredBTDevice += this.Common_DiscoveredBTDevice;
            this.common.DiscoveryComplete += this.Common_DiscoveryComplete;
            this.common.BT_PairStatus += this.Common_BT_PairStatus;
            this.common.BT_UnPairStatus += this.Common_BT_UnPairStatus;
            this.common.MsgReceivedEvent += this.Common_MsgReceivedEvent;
            this.common.ConnectionCompleted += this.Common_ConnectionCompleted;
        }

        #endregion


        #region IBTInterface methods

        public void ConnectAsync(BTDeviceInfo device) {
            this.common.ConnectAsync(device.Name);
        }


        public void Disconnect() {
            this.common.Disconnect();
        }


        public void DiscoverDevicesAsync(bool paired) {
            this.common.DiscoverDevicesAsync(paired);
        }


        public void GetDeviceInfoAsync(BTDeviceInfo deviceDataModel) {
            throw new NotImplementedException();
        }


        public void PairgAsync(BTDeviceInfo info) {
            this.common.Pair(info.Name);
        }


        public void UnPairAsync(BTDeviceInfo info) {
            this.common.UnPairAsync(info.Name);
        }

        #endregion

        #region ICommStackChannel methods

        public bool SendOutMsg(byte[] msg) {
            this.common.SendOutMsg(msg);
            return true;
        }

        #endregion

        #region Event handlers

        private void Common_ConnectionCompleted(object sender, bool e) {
            this.ConnectionCompleted?.Invoke(this, e);
        }


        private void Common_MsgReceivedEvent(object sender, byte[] e) {
            this.MsgReceivedEvent?.Invoke(sender, e);
        }


        private void Common_DiscoveryComplete(object sender, bool e) {
            this.DiscoveryComplete?.Invoke(sender, e);
        }


        private void Common_DiscoveredBTDevice(object sender, BTDeviceInfo e) {
            this.DiscoveredBTDevice?.Invoke(sender, e);
        }


        private void Common_BT_UnPairStatus(object sender, BTUnPairOperationStatus e) {
            this.BT_UnPairStatus?.Invoke(sender, e);
        }


        private void Common_BT_PairStatus(object sender, BTPairOperationStatus e) {
            this.BT_PairStatus?.Invoke(sender, e);
        }


        private void ToEliminateCompilerWarnings() {
            // Potential future implementation
            this.BT_DeviceInfoGathered?.Invoke(this, new BTDeviceInfo());

            // Android raises its own PIN dialog
            this.BT_PairInfoRequested?.Invoke(this, new BT_PairInfoRequest() {
                DeviceName = "NOT IMPLEMENTED"
            });
        }

        #endregion

    }

}