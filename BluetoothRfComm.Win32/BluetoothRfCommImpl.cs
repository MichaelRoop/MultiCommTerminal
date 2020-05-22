#define USE_BT_WRAPPER

using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VariousUtils;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Foundation;

namespace BluetoothRfComm.Win32 {


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


    public partial class BluetoothRfCommImpl : IBTInterface {

        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<byte[]> MsgReceivedEvent;

        private ClassLog log = new ClassLog("BluetoothRfCommImpl");
        private BluetoothClassicUwpWrapper btWrapper = null;



        public BluetoothRfCommImpl() {
            this.btWrapper = new BluetoothClassicUwpWrapper();
            this.btWrapper.DiscoveryComplete += BtWrapper_DiscoveryComplete;
            this.btWrapper.DeviceDiscovered += BtWrapper_DeviceDiscovered;
            this.btWrapper.ConnectionCompleted += BtWrapper_ConnectionCompleted;
            this.btWrapper.MsgReceivedEvent += BtWrapper_MsgReceivedEvent;
        }


        public bool Connect(BTDeviceInfo device) {
            throw new NotImplementedException();
        }


        public void ConnectAsync(BTDeviceInfo device) {
#if USE_BT_WRAPPER
            this.btWrapper.ConnectAsync(device);
#endif
        }


        public void Disconnect() {
#if USE_BT_WRAPPER
            this.btWrapper.Disconnect();
#else
            throw new NotImplementedException();
#endif
        }


        public void DiscoverDevicesAsync() {
#if USE_BT_WRAPPER
            this.btWrapper.DiscoverPairedDevicesAsync();
#else
            Task.Run(() => {
                try {
                    this.DoDiscovery();
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                }
            });
#endif
        }


        /// <summary>Complete by connecting and filling in the device information</summary>
        /// <param name="device"></param>
        public void GetDeviceInfo(BTDeviceInfo deviceDataModel) {
            // TODO - add to interface. 
            // TODO - add event with newly populated device data model
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



        public bool SendOutMsg(byte[] msg) {
#if USE_BT_WRAPPER
            this.btWrapper.WriteAsync(msg);
            return true;
#else
            throw new NotImplementedException();
#endif
        }

#region Private

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

        #endregion

    }
}
