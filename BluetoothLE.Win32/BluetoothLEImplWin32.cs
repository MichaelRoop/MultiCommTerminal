using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;

namespace BluetoothLE.Win32 {

    // Using the UWP Bluetooth LE calls from Win 32 desktop
    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/e261aeb5-104d-43ba-9b2b-97447614dec0/how-to-use-windowsdevices-api-in-a-c-winform-desktop-application-in-windows-10?forum=winforms
    // Add Reference to  \Program Files\Windows Kits\10\UnionMetadata\Windows.winmd
    // Add Reference to \Program Files\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll
    // Regardless what Target Framework i'm using, the only working version of System.Runtime.WindowsRuntime.dll was v4.5. All other version (wich corresponds to the Target Framework) caused runtime errors.

    public class BluetoothLEImplWin32 : IBLETInterface {

        #region data
        
        private ManualResetEvent stopped = new ManualResetEvent(false);

        #endregion

        #region Interface events

        public event EventHandler<BluetoothLEDeviceInfo> DeviceDiscovered;

        public event EventHandler<string> DeviceRemoved;

        #endregion



        /// <summary>Interface call to discover devices</summary>
        /// <returns>A list of discovered devices.  However I am now using events so it is empty</returns>
        void IBLETInterface.DiscoverDevices() {
            this.DoLEWatcherSearch();
        }

        #region Watcher approach

        DeviceWatcher devWatcher = null;

        private void DoLEWatcherSearch() {
            if (this.devWatcher == null) {
                this.SetupWatcher();
                this.devWatcher.Start();
            }
            else {
                // Subsequent calls
                // https://docs.microsoft.com/en-us/uwp/api/windows.devices.enumeration.devicewatcherstatus
                switch (this.devWatcher.Status) {
                    case DeviceWatcherStatus.Created:
                    case DeviceWatcherStatus.Stopped:
                    case DeviceWatcherStatus.Aborted:
                        this.devWatcher.Start();
                        break;
                    case DeviceWatcherStatus.Started:
                        // Already started but enumeration not completed
                    case DeviceWatcherStatus.EnumerationCompleted:
                        // Call Stop and wait on stopped event
                        this.stopped.Reset();
                        this.devWatcher.Stop();
                        if (this.stopped.WaitOne(5000)) {
                            this.devWatcher.Start();
                        }
                        break;
                    case DeviceWatcherStatus.Stopping:
                        // Wait on stopped event
                        break;
                }

            }
        }


        private void SetupWatcher() {
            // https://stackoverflow.com/questions/48909496/aqs-syntax-when-providing-a-filter-for-ble-scanning-in-uwp
            // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/DeviceEnumerationAndPairing/cs/Scenario7_DeviceInformationKind.xaml.cs
            // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/DeviceEnumerationAndPairing/cs/DeviceWatcherHelper.cs

            if (this.devWatcher == null) {
                // With properties only get 4
                string[] prop = {
                    "System.Devices.Aep.DeviceAddress",
                    "System.Devices.Aep.IsConnected",
                    "System.Devices.Aep.Bluetooth.Le.IsConnectable",
                    "System.Devices.Aep.IsPaired"
                };
                #region failed experiments
                //// Even with null properties I get 4
                //this.devWatcher = DeviceInformation.CreateWatcher(
                //    BluetoothLEDevice.GetDeviceSelectorFromPairingState(false), prop, .AssociationEndpoint);

                //// Gets Ergonomic keyboard only 
                //this.devWatcher = DeviceInformation.CreateWatcher(
                //    BluetoothLEDevice.GetDeviceSelector(), prop, DeviceInformationKind.AssociationEndpoint);

                // 1413 devices, duplicates, etc
                //this.devWatcher = DeviceInformation.CreateWatcher(DeviceClass.All);

                // 0 devices
                //this.devWatcher = DeviceInformation.CreateWatcher("System.Devices.Aep.DeviceAddress:<>[]");

                // 0 devices
                //this.devWatcher = DeviceInformation.CreateWatcher("System.Devices.Aep.DeviceAddress:<>[]", reqProperties);
                #endregion

                // with AQS Get 23 if I use properties, if null properties only 22
                string aqsBTAddressNotNull = "System.Devices.Aep.DeviceAddress:<>[]";
                this.devWatcher = DeviceInformation.CreateWatcher(
                    aqsBTAddressNotNull, prop, DeviceInformationKind.AssociationEndpoint);

                // Hook up the events
                devWatcher.Added += DevWatcher_Added;
                devWatcher.Updated += DevWatcher_Updated;
                devWatcher.Removed += DevWatcher_Removed;
                devWatcher.EnumerationCompleted += DevWatcher_EnumerationCompleted;
                devWatcher.Stopped += DevWatcher_Stopped;
            }
        }


        //private void TearDownEvents() {
        //    devWatcher.Added -= DevWatcher_Added;
        //    devWatcher.Updated -= DevWatcher_Updated;
        //    devWatcher.Removed -= DevWatcher_Removed;
        //    devWatcher.EnumerationCompleted -= DevWatcher_EnumerationCompleted;
        //    devWatcher.Stopped -= DevWatcher_Stopped;
        //}


        /// <summary>Event fired when the watcher is stopped</summary>
        private void DevWatcher_Stopped(DeviceWatcher sender, object args) {
            this.stopped.Set();
        }

        
        /// <summary>Event fired when the enumeration is complete</summary>
        private void DevWatcher_EnumerationCompleted(DeviceWatcher sender, object args) {
        }


        /// <summary>Fired when a device disappears</summary>
        /// <param name="sender"></param>
        /// <param name="updateInfo">The removed device. Use the ID</param>
        private void DevWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate updateInfo) {
        }


        /// <summary>Fired when the device is changed</summary>
        /// <param name="sender"></param>
        /// <param name="updateInfo"></param>
        private void DevWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate updateInfo) {
        }


        /// <summary>Fired when a device is discovered</summary>
        /// <param name="sender"></param>
        /// <param name="deviceInfo">Info on the discovered device</param>
        private void DevWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo) {
            // TODO - look at separate objects for LE
            if (deviceInfo.Name.Length > 0) {
                if (this.DeviceDiscovered != null) {
                    this.DeviceDiscovered(this, new BluetoothLEDeviceInfo() {
                        Name = deviceInfo.Name,
                        Id = deviceInfo.Id,
                        IsDefault = deviceInfo.IsDefault,
                        CanPair  = deviceInfo?.Pairing.CanPair ?? false,
                        IsPaired = deviceInfo?.Pairing.IsPaired ?? false,
                    });
                }
            }

        }

        #endregion

    }
}
