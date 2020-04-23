//#define USING_OLDER_UWP

using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;


// To use the full 


// Example code read write
// https://www.c-sharpcorner.com/code/1912/uwp-bluetooth-le-implementation
// https://github.com/nexussays/ble.net/blob/master/Readme.md

namespace BluetoothLE.Win32 {

    // Using the UWP Bluetooth LE calls from Win 32 desktop - NOT USING THIS ONE ANYMORE
    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/e261aeb5-104d-43ba-9b2b-97447614dec0/how-to-use-windowsdevices-api-in-a-c-winform-desktop-application-in-windows-10?forum=winforms
    // Add Reference to  \Program Files\Windows Kits\10\UnionMetadata\Windows.winmd
    // Add Reference to \Program Files\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll
    // Regardless what Target Framework i'm using, the only working version of System.Runtime.WindowsRuntime.dll was v4.5. All other version (wich corresponds to the Target Framework) caused runtime errors.

    
    // NOW USING THIS METHOD TO USE THE FULL POWER OF UWP    
    // https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance


    public partial class BluetoothLEImplWin32 : IBLETInterface {

        #region data
        
        private ManualResetEvent stopped = new ManualResetEvent(false);

        /// <summary>Allows discovery of device list</summary>
        private DeviceWatcher devWatcher = null;

        private BluetoothLEDevice currentDevice = null;
        private ClassLog log = new ClassLog("BluetoothLEImplWin32");

        #endregion

        #region IBLETInterface:ICommStackChannel methods


        public bool SendOutMsg(byte[] msg) {
            //throw new NotImplementedException();
            // TODO - send out by some kind of stream to BLE device - see classic
            return false;
        }

        #endregion

        #region Interface methods

        /// <summary>Interface call to discover devices</summary>
        /// <returns>A list of discovered devices.  However I am now using events so it is empty</returns>
        public void DiscoverDevices() {

            System.Diagnostics.Debug.WriteLine("Doing stuff...");

            this.DoLEWatcherSearch();
        }

        public void Disconnect() {
            if (this.currentDevice != null) {
                // Apparently do not need this. Dispose will do it
                //if (this.currentDevice.ConnectionStatus == BluetoothConnectionStatus.Connected) {
                //    // Disconnect & wait for disconnection
                //    //BluetoothLEDevice.
                //}

                this.DisconnectBTLEDeviceEvents();
                this.currentDevice.Dispose();
                this.currentDevice = null;
            }
        }


        string id = "";

        public void Connect(BluetoothLEDeviceInfo deviceInfo) {
            this.Disconnect();
            this.id = deviceInfo.Id;
            Task.Run(async () => await this.ConnectToDevice(deviceInfo));
        }
  

#endregion



#region Connection to device code

        private void ConnectBTLEDeviceEvents() {
            if (this.currentDevice != null) {
                this.currentDevice.ConnectionStatusChanged += this.CurrentDevice_ConnectionStatusChanged;
                this.currentDevice.GattServicesChanged += this.CurrentDevice_GattServicesChanged;
                this.currentDevice.NameChanged += this.CurrentDevice_NameChanged;
            }
        }


        private void DisconnectBTLEDeviceEvents() {
            if (this.currentDevice != null) {
                this.currentDevice.ConnectionStatusChanged -= this.CurrentDevice_ConnectionStatusChanged;
                this.currentDevice.GattServicesChanged -= this.CurrentDevice_GattServicesChanged;
                this.currentDevice.NameChanged -= this.CurrentDevice_NameChanged;
            }
        }



        private void CurrentDevice_NameChanged(BluetoothLEDevice sender, object args) {
            //throw new NotImplementedException();
        }

        private void CurrentDevice_GattServicesChanged(BluetoothLEDevice sender, object args) {
            //throw new NotImplementedException();
        }

        private void CurrentDevice_ConnectionStatusChanged(BluetoothLEDevice sender, object args) {
            //throw new NotImplementedException();
        }

#endregion

    }
}

