using BluetoothLE.Net.DataModels;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Text;
using System.Threading;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Events and their handlers

        /// <summary>Event raised when a device is discovered</summary>
        public event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceDiscovered;

        /// <summary>Event raised when a device is removed</summary>
        public event EventHandler<string> BLE_DeviceRemoved;

        /// <summary>Event raised when device discovery complete</summary>
        public event EventHandler<bool> BLE_DeviceDiscoveryComplete;

        /// <summary>Event raised when BLE device properties change</summary>
        public event EventHandler<BLE_PropertiesUpdateDataModel> BLE_DeviceUpdated;

        /// <summary>Raised when BLE info on a device is finished gathering</summary>
        public event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceInfoGathered;


        private void BLE_DeviceDiscoveredHandler(object sender, BluetoothLEDeviceInfo e) {
            if (this.BLE_DeviceDiscovered != null) {
                this.BLE_DeviceDiscovered(this, e);
            }
        }


        private void BLE_DeviceRemovedHandler(object sender, string e) {
            if (this.BLE_DeviceRemoved != null) {
                this.BLE_DeviceRemoved(this, e);
            }
        }


        private void BLE_DeviceUpdatedHandler(object sender, BLE_PropertiesUpdateDataModel args) {
            this.BLE_DeviceUpdated?.Invoke(this, args);
        }


        private void BLE_DeviceDiscoveryCompleted(object sender, bool e) {
            if (this.BLE_DeviceDiscoveryComplete != null) {
                this.BLE_DeviceDiscoveryComplete.Invoke(this, e);
            }
        }


        private void BleStack_MsgReceived(object sender, byte[] e) {
            throw new NotImplementedException();
        }

        #endregion

        #region Public

        public void BLE_DiscoverAsync() {
            this.DisconnectAll();
            this.bleBluetooth.DiscoverDevices();
        }

        public void BLE_ConnectAsync(BluetoothLEDeviceInfo device) {
            this.bleBluetooth.Connect(device);
        }



        public void BLE_GetInfo(BluetoothLEDeviceInfo device) {
            this.bleBluetooth.GetInfo(device);
        }


        private void BleBluetooth_DeviceInfoAssembled(object sender, BluetoothLEDeviceInfo e) {
            this.BLE_DeviceInfoGathered?.Invoke(this, e);
        }


        public void BLE_GetDbgInfoStringDump(object obj, Action<string, string> onComplete) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "", () => {
                BluetoothLEDeviceInfo info = obj as BluetoothLEDeviceInfo;
                if (info == null) {
                    onComplete("** NOT  A BLE DEVICE **", "** NOT  A BLE DEVICE **");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("Id: {0}", info.Id)).AppendLine();
                sb.Append(string.Format("IsDefault: {0}", info.IsDefault)).AppendLine();
                sb.Append(string.Format("IsEnabled: {0}", info.IsEnabled)).AppendLine();
                //sb.Append("     Kind: {0}", device.Kind);
                // Properties
                sb.Append(string.Format("Properties: ({0})", info.ServiceProperties.Count)).AppendLine();
                foreach (var p in info.ServiceProperties) {
                    sb.Append(p.Key).Append(" : ").Append(p.Value.Value.ToString()).AppendLine();
                }
                #region OS Specific to implement in the abstract
                //// Enclosure location
                //if (device.EnclosureLocation != null) {
                //    System.Diagnostics.Debug.WriteLine("EnclosureLocation:");
                //    System.Diagnostics.Debug.WriteLine("     InDock: {0}", device.EnclosureLocation.InDock);
                //    System.Diagnostics.Debug.WriteLine("      InLid: {0}", device.EnclosureLocation.InLid);
                //    System.Diagnostics.Debug.WriteLine("      Panel: {0}", device.EnclosureLocation.Panel);
                //    System.Diagnostics.Debug.WriteLine("      Angle: {0}", device.EnclosureLocation.RotationAngleInDegreesClockwise);
                //}
                //else {
                //    System.Diagnostics.Debug.WriteLine("EnclosureLocation: null");
                //}
                //// Pairing
                //if (device.Pairing != null) {
                //    System.Diagnostics.Debug.WriteLine("Pairing:");
                //    System.Diagnostics.Debug.WriteLine("    CanPair: {0}", device.Pairing.CanPair);
                //    System.Diagnostics.Debug.WriteLine("   IsPaired: {0}", device.Pairing.IsPaired);
                //    System.Diagnostics.Debug.WriteLine(" Protection: {0}", device.Pairing.ProtectionLevel);
                //    if (device.Pairing.Custom != null) {
                //        System.Diagnostics.Debug.WriteLine("     Custom: not null");
                //    }
                //    else {
                //        System.Diagnostics.Debug.WriteLine("Custom: null");
                //    }
                //}
                //else {
                //    System.Diagnostics.Debug.WriteLine("Custom: null");
                //}

                //MessageBox.Show(sb.ToString(), info.Name);

                //return "";
                #endregion
                onComplete.Invoke(info.Name, sb.ToString());
            });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => onComplete("** FAILED **", "** FAILED **"));
            }
        }


        public void BLE_Disconnect() {
            this.bleBluetooth.Disconnect();
        }

        #endregion

        #region Init and teardown

        private void BLE_Init() {
            this.bleStack.SetCommChannel(this.bleBluetooth);
            this.bleStack.InTerminators = "\n\r".ToAsciiByteArray();
            this.bleStack.OutTerminators = "\n\r".ToAsciiByteArray();
            this.bleStack.MsgReceived += this.BleStack_MsgReceived;
            this.bleBluetooth.DeviceDiscovered += this.BLE_DeviceDiscoveredHandler;
            this.bleBluetooth.DeviceRemoved += this.BLE_DeviceRemovedHandler;
            this.bleBluetooth.DeviceUpdated += BLE_DeviceUpdatedHandler;
            this.bleBluetooth.DeviceDiscoveryCompleted += this.BLE_DeviceDiscoveryCompleted;
            this.bleBluetooth.DeviceInfoAssembled += this.BleBluetooth_DeviceInfoAssembled;
        }


        private void BLE_TearDown() {
            this.bleBluetooth.Disconnect();
            this.bleStack.MsgReceived -= this.BleStack_MsgReceived;
            this.bleBluetooth.DeviceDiscovered -= this.BLE_DeviceDiscoveredHandler;
            this.bleBluetooth.DeviceRemoved -= this.BLE_DeviceRemovedHandler;
            this.bleBluetooth.DeviceUpdated -= BLE_DeviceUpdatedHandler;
            this.bleBluetooth.DeviceDiscoveryCompleted -= this.BLE_DeviceDiscoveryCompleted;
            this.bleBluetooth.DeviceInfoAssembled -= this.BleBluetooth_DeviceInfoAssembled;
        }

        #endregion

    }
}
