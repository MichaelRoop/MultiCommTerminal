using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        ClassLog log = new ClassLog("BluetoothRfCommImpl");

        public bool Connect(BTDeviceInfo device) {
            throw new NotImplementedException();
        }


        public void ConnectAsync(BTDeviceInfo device) {
            throw new NotImplementedException();
        }


        public void Disconnect() {
            throw new NotImplementedException();
        }


        public void DiscoverDevicesAsync() {
            Task.Run(() => {
                try {
                    this.DoDiscovery();
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                }
            });
        }


        /// <summary>Complete by connecting and filling in the device information</summary>
        /// <param name="device"></param>
        public void GetDeviceInfo(BTDeviceInfo deviceDataModel) {
            // TODO - add to interface. 
            // TODO - add event with newly populated device data model

            //Windows.UI.Core.CoreWindow.GetForCurrentThread().Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, )


            // This was a suggestion but does not work to get adapter or connect to Bluetooth Classic
            //Task.Run(async () => {
            //    try {
            //        await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(
            //            Windows.UI.Core.CoreDispatcherPriority.Normal, async () => {

            //            await this.HarvestInfo(deviceDataModel);
            //        });


            //        //await this.HarvestInfo(deviceDataModel);
            //    }
            //    catch (Exception e) {
            //        this.log.Exception(9999, "", e);
            //    }
            //});



            Task.Run(async () => {
                try {
                    await this.HarvestInfo(deviceDataModel);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                }
            });
        }



        public bool SendOutMsg(byte[] msg) {
            throw new NotImplementedException();
        }
    }
}
