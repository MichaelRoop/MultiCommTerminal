using Android.Bluetooth;
using Android.Content;
using BluetoothCommon.Net;
using BluetoothCommon.Net.Enumerations;
using BluetoothCommon.Net.interfaces;
using BluetoothCommonAndroidXamarin;
using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.interfaces;
using LogUtils.Net;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace BluetoothRfComm.AndroidXamarin {

    public partial class BluetoothRfCommAndroidXamarinImpl : IBTInterface {

        #region Data

        private ClassLog log = new ClassLog("BluetoothRfCommAndroidXamarinImpl");
        private IMsgPump<BTAndroidMsgPumpConnectData> msgPump = new BTAndroidMsgPump();
        private bool connected = false;
        private BluetoothDevice device = null;

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
            this.msgPump.MsgPumpConnectResultEvent += this.MsgPumpConnectResultEventHandler;
            this.msgPump.MsgReceivedEvent += this.MsgReceivedEventHandler;
            this.common.DiscoveredBTDevice += Common_DiscoveredBTDevice;
            this.common.DiscoveryComplete += Common_DiscoveryComplete;
        }

        #endregion


        #region IBTInterface methods

        public void ConnectAsync(BTDeviceInfo device) {
            Task.Run(() => {
                try {
                    this.Disconnect();
                    this.device = BluetoothAdapter.DefaultAdapter.BondedDevices.FirstOrDefault(d => d.Name == device.Name);
                    this.msgPump.ConnectAsync2(new BTAndroidMsgPumpConnectData(this.device));
                }
                catch (Exception e) {
                    this.log.Exception(9999, "ConnectAsync", "", e);
                }
            });
        }


        public void Disconnect() {
            try {
                if (this.connected) {
                    this.msgPump.Disconnect();
                    // Never dispose the device since it came from the Adaptor Bonded list
                    if (this.device != null) {
                        this.device = null;
                    }
                    this.connected = false;
                }
            }
            catch (Exception e) {
                this.log.Exception(9898, "Disconnect", "", e);
            }
            this.connected = false;
        }


        public void DiscoverDevicesAsync(bool paired) {
            this.common.DiscoverDevicesAsync(paired);
        }


        public void GetDeviceInfoAsync(BTDeviceInfo deviceDataModel) {
            throw new NotImplementedException();
        }


        public void PairgAsync(BTDeviceInfo info) {
            BluetoothDevice unbonded = this.common.UnBondedDevices.FirstOrDefault(device => device.Name == info.Name);
            // Status is failed by default
            BTPairOperationStatus status = new BTPairOperationStatus() {
                Name = info.Name,
            };
            try {
                if (unbonded != null) {
                    if (unbonded.CreateBond()) {
                        status.IsSuccessful = true;
                        status.PairStatus = BT_PairingStatus.Paired;
                    }
                    else {
                        status.PairStatus = BT_PairingStatus.AuthenticationFailure;
                    }
                }
                else {
                    status.PairStatus = BT_PairingStatus.NoParingObject;
                }
            }
            catch(Exception e) {
                this.log.Exception(9999, "PairgAsync", "", e);
                status.PairStatus = BT_PairingStatus.Failed;
            }

            this.BT_PairStatus?.Invoke(this, status);
        }


        public void UnPairAsync(BTDeviceInfo info) {
            // Also need to check if current device.
            // Status is failed by default
            BTUnPairOperationStatus status = new BTUnPairOperationStatus() {
                Name = info.Name,
            };
            Task.Run(() => {
                try {
                    var d = BluetoothAdapter.DefaultAdapter.BondedDevices.FirstOrDefault(dev => dev != null && dev.Name == info.Name);
                    if (d != null) {
                        var mi = d.Class.GetMethod("removeBond", null);
                        var sdfd = mi.Invoke(d, null);
                        // Need to sleep a bit for the method invocation to complete
                        Thread.Sleep(200);
                        // Suppose it to be successful if exception not thrown. Reload list to see if it is still there
                        status.IsSuccessful = true;
                        status.UnpairStatus = BT_UnpairingStatus.Success;
                    }
                    else {
                        this.log.Error(9999, "UnPairAsync", "Already unpaired - null device or not of name");
                        status.UnpairStatus = BT_UnpairingStatus.AlreadyUnPaired;
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "UnPairAsync", "", e);
                    status.UnpairStatus = BT_UnpairingStatus.Failed;
                }
                this.BT_UnPairStatus?.Invoke(this, status);
            });
        }

        #endregion

        #region ICommStackChannel methods

        public bool SendOutMsg(byte[] msg) {
            this.msgPump.WriteAsync(msg);
            return true;
        }

        #endregion

        #region Event handlers

        private void MsgReceivedEventHandler(object sender, byte[] e) {
            this.log.Info("MsgReceivedEventHandler", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));
            this.MsgReceivedEvent?.Invoke(sender, e);
        }


        private void MsgPumpConnectResultEventHandler(object sender, CommunicationStack.Net.DataModels.MsgPumpResults results) {
            this.connected = results.Code == MsgPumpResultCode.Connected;

            this.log.Info("", () => string.Format(
                "Connect result:{0}  Socket:{1} Msg:{2}",
                results.Code, results.SocketErr, results.ErrorString));
            this.ConnectionCompleted?.Invoke(this, this.connected);
        }


        private void Common_DiscoveryComplete(object sender, bool e) {
            this.DiscoveryComplete?.Invoke(sender, e);
        }

        private void Common_DiscoveredBTDevice(object sender, BTDeviceInfo e) {
            this.DiscoveredBTDevice?.Invoke(sender, e);
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

        #region Private

        private Context GetContext() {
            return Android.App.Application.Context;
        }

        #endregion

    }

}