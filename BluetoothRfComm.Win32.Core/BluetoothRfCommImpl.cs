#define USE_BT_WRAPPER

using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;

namespace BluetoothRfComm.UWP.Core {


    // STEPS TO USE UWP libs in Win32
    // Windows.Foundation.UniversalApiContract
    // C:\Program Files(x86)\Windows Kits\10\References\10.0.18362.0\Windows.Foundation.UniversalApiContract\8.0.0.0\Windows.Foundation.UniversalApiContract.winmd

    //Windows.Foundation.FoundationContract
    //C:\Program Files (x86)\Windows Kits\10\References\10.0.18362.0\Windows.Foundation.FoundationContract\3.0.0.0\Windows.Foundation.FoundationContract.winmd

    //Windows
    //C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.18362.0\Facade\windows.winmd

    //System.Runtime.WindowsRuntime
    //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll

    //System.Runtime.InteropServices.WindowsRuntime
    //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Facades\System.Runtime.InteropServices.WindowsRuntime.dll

    // Add to manifest
    //https://stackoverflow.com/questions/38845320/uwp-serialdevice-fromidasync-throws-element-not-found-exception-from-hresult



    // UPDATE: It looks like you only have to add the package:
    // Microsoft.Windows.SDK.Contracts(nn.n.nnnn.n)
    // You do this through the NuGet package manager. Not sure if you need to add to manifest. If so, would only be in main App


    public partial class BluetoothRfCommUwpCore : IBTInterface {

        #region Data

        private ClassLog log = new ClassLog("BluetoothRfCommImpl");
        private BluetoothRfCommWrapperUwpCore btWrapper = null;

        private readonly string KEY_CAN_PAIR = "System.Devices.Aep.CanPair";
        private readonly string KEY_IS_PAIRED = "System.Devices.Aep.IsPaired";

        #endregion

        #region Events

        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<byte[]> MsgReceivedEvent;
        public event EventHandler<BT_PairInfoRequest> BT_PairInfoRequested;
        public event EventHandler<BTPairOperationStatus> BT_PairStatus;
        public event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

        #endregion

        #region Constructors

        public BluetoothRfCommUwpCore() {
            this.btWrapper = new BluetoothRfCommWrapperUwpCore();
            this.btWrapper.ConnectionCompleted += BtWrapper_ConnectionCompleted;
            this.btWrapper.MsgReceivedEvent += BtWrapper_MsgReceivedEvent;
            this.btWrapper.BT_PairInfoRequested += BtWrapper_BT_PairInfoRequested;
            this.btWrapper.BT_PairStatus += this.BtWrapper_BT_PairStatus;
            this.btWrapper.BT_UnPairStatus += this.BtWrapper_BT_UnPairStatus;
        }

        #endregion

        #region Public IBTInterface

        /// <summary>Synchronous connection</summary>
        /// <param name="deviceDataModel">Data model with device information</param>
        /// <returns>true on connection, false on failure</returns>
        public bool Connect(BTDeviceInfo deviceDataModel) {
            AutoResetEvent done = new AutoResetEvent(false);
            bool result = false;
            this.btWrapper.ConnectionCompleted += (sender, isOk) => {
                result = isOk;
                done.Set();
            };
            this.ConnectAsync(deviceDataModel);
            if (!done.WaitOne(5000)) {
                result = false;
            }
            return result;
        }


        public void ConnectAsync(BTDeviceInfo device) {
            this.btWrapper.ConnectAsync(device);
        }


        public void Disconnect() {
            try {
                this.btWrapper.Disconnect();
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        public void DiscoverDevicesAsync(bool paired) {
            try {
                Task.Run(() => {
                    try {
                        this.DoDiscovery(paired);
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "", e);
                    }
                });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        // TODO - ADD THIS TO INTERFACE AND MAKE IT ASYNC LIKE THE BLE IMPLEMENTATION
        /// <summary>Complete by connecting and filling in the device information</summary>
        /// <param name="device"></param>
        public void GetDeviceInfo(BTDeviceInfo deviceDataModel) {
#if USE_BT_WRAPPER
            //this.btWrapper.DiscoverPairedDevicesAsync();
#else
            Task.Run(async () => {
                try {
                    await this.HarvestInfo(deviceDataModel);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                }
            });
#endif
        }


        public void UnPairAsync(BTDeviceInfo info) {
            Task.Run(async () => {
                await this.btWrapper.DoUnPairing(info);
            });
            return;
        }


        public void PairgAsync(BTDeviceInfo info) {
            Task.Run(async () => {
                await this.btWrapper.DoPairing(info);
            });
            return;
        }

        #endregion

        #region Public ICommStackChannel interface

        public bool SendOutMsg(byte[] msg) {
            this.btWrapper.WriteAsync(msg);
            return true;
        }

        #endregion

        #region Private Bluetooth Wrapper event handlers

        private void BtWrapper_ConnectionCompleted(object sender, bool e) {
            this.ConnectionCompleted?.Invoke(sender, e);
        }


        private void BtWrapper_DeviceDiscovered(object sender, BTDeviceInfo e) {
            this.DiscoveredBTDevice?.Invoke(sender, e);
        }


        private void BtWrapper_DiscoveryComplete(object sender, bool e) {
            this.DiscoveryComplete?.Invoke(this, e);
        }


        private void BtWrapper_MsgReceivedEvent(object sender, byte[] e) {
            this.log.Info("BtWrapper_MsgReceivedEvent", () =>
                string.Format("Received:{0}", e.ToFormatedByteString()));

            this.MsgReceivedEvent?.Invoke(sender, e);
        }


        private void BtWrapper_BT_PairInfoRequested(object sender, BT_PairInfoRequest e) {
            this.log.Info("BtWrapper_BT_PairInfoRequested", () => string.Format("Received:{0}", e.Pin));
            this.BT_PairInfoRequested?.Invoke(sender, e);
        }


        private void BtWrapper_BT_UnPairStatus(object sender, BTUnPairOperationStatus args) {
            this.log.Info("BtWrapper_BT_UnPairStatus", () => string.Format("{0} - {1}", args.Name, args.UnpairStatus));
            this.BT_UnPairStatus?.Invoke(sender, args);
        }


        private void BtWrapper_BT_PairStatus(object sender, BTPairOperationStatus args) {
            this.log.Info("BtWrapper_BT_PairStatus", () => string.Format("{0} - {1}", args.Name, args.PairStatus));
            this.BT_PairStatus?.Invoke(sender, args);
        }

        #endregion

        #region Private tools

        /// <summary>Get the boolean value from a Device Information property</summary>
        /// <param name="property">The device information property</param>
        /// <param name="key">The property key to lookup</param>
        /// <param name="defaultValue">Default value on error</param>
        /// <returns></returns>
        private bool GetBoolProperty(IReadOnlyDictionary<string, object> property, string key, bool defaultValue) {
            if (property.ContainsKey(key)) {
                if (property[key] is Boolean) {
                    return (bool)property[key];
                }
                this.log.Error(9999, () => string.Format(
                    "{0} Property is {1} rather than Boolean", key, property[key].GetType().Name));
            }
            return defaultValue;
        }


        #endregion

    }
}
