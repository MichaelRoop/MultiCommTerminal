using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BluetoothLE.Win32 {

    public partial class BluetoothLEImplWin32 : IBLETInterface {

        /// <summary>Called from syncronous Interface method .. to call async methods
        /// 
        /// </summary>
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
                //// With properties only get 4
                //string[] prop = {
                //    "System.Devices.Aep.DeviceAddress",
                //    "System.Devices.Aep.IsConnected",
                //    "System.Devices.Aep.Bluetooth.Le.IsConnectable",
                //    "System.Devices.Aep.IsPaired"
                //};

                // with AQS Get 23 if I use properties, if null properties only 22
                //string aqsBTAddressNotNull = "System.Devices.Aep.DeviceAddress:<>[]";

                //this.devWatcher = DeviceInformation.CreateWatcher(
                //    BluetoothLEDevice.GetDeviceSelector(),  //aqsBTAddressNotNull, 
                //    prop, 
                //    DeviceInformationKind.AssociationEndpoint);

                // The GetDeviceSelector creates an aqs string that will return all LE devices. Do not need other parameters
                this.devWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelector());

                // See the GetDeviceSelectorFromAppearance to get specific kinds of LE devices

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
            this.log.Info("DevWatcher_Stopped", "*****");
            this.stopped.Set();
        }


        /// <summary>Event fired when the enumeration is complete</summary>
        private void DevWatcher_EnumerationCompleted(DeviceWatcher sender, object args) {
            this.log.Info("DevWatcher_EnumerationCompleted", "*****");
        }


        /// <summary>Fired when a device disappears</summary>
        /// <param name="sender"></param>
        /// <param name="updateInfo">The removed device. Use the ID</param>
        private void DevWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate updateInfo) {
            this.log.Info("DevWatcher_Removed", "*****");
            if (this.DeviceRemoved != null) {
                this.DeviceRemoved(sender, updateInfo.Id);
            }
        }


        /// <summary>Fired when the device is changed</summary>
        /// <param name="sender"></param>
        /// <param name="updateInfo"></param>
        private void DevWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate updateInfo) {
            this.log.Info("DevWatcher_Updated", "*****");
        }


        /// <summary>Fired when a device is discovered</summary>
        /// <param name="sender"></param>
        /// <param name="deviceInfo">Info on the discovered device</param>
        private void DevWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo) {
            this.log.Info("DevWatcher_Added", "*****");

            // TODO - look at separate objects for LE
            if (deviceInfo.Name.Length > 0) {
                //if (deviceInfo.Name == "itead") {

                this.DebugDumpDeviceInfo(deviceInfo);
                //}

                if (this.DeviceDiscovered != null) {

                    BluetoothLEDeviceInfo dev = new BluetoothLEDeviceInfo() {
                        Name = deviceInfo.Name,
                        Id = deviceInfo.Id,
                        IsDefault = deviceInfo.IsDefault,
                        CanPair = deviceInfo?.Pairing.CanPair ?? false,
                        IsPaired = deviceInfo?.Pairing.IsPaired ?? false,
                        // required for connecting ?
                        OSSpecificObj = deviceInfo,
                    };

                    if (deviceInfo.Properties != null) {
                        foreach (var p in deviceInfo.Properties) {
                            // The value is object. I have only seen strings to this point
                            object val = p.Value;
                            string s = val?.ToString() ?? "";

                            // Sometimes there is only the key
                            //dev.LEProperties.Add(new Tuple<string, string>(p.Key, p.Value.ToString()));
                            dev.LEProperties.Add(new Tuple<string, string>(p.Key, s));


                        }
                    }

                    this.DeviceDiscovered(sender, dev);

                    //this.DeviceDiscovered(sender, new BluetoothLEDeviceInfo() {
                    //    Name = deviceInfo.Name,
                    //    Id = deviceInfo.Id,
                    //    IsDefault = deviceInfo.IsDefault,
                    //    CanPair  = deviceInfo?.Pairing.CanPair ?? false,
                    //    IsPaired = deviceInfo?.Pairing.IsPaired ?? false,
                    //    //if (deviceInfo.Properties != null) {

                    //    //},


                    //    // required for connecting
                    //    OSSpecificObj = deviceInfo,
                    //});
                }
            }

        }


        private void DebugDumpDeviceInfo(DeviceInformation device) {
            //            System.Diagnostics.Debug.WriteLine("Doing stuff...");

            // Weird - prints value, then the string with {0}
            this.log.Info("Dump", () => string.Format("     Name: {0}", device.Name));
            this.log.Info("Dump", () => string.Format("       Id: {0}", device.Id));
            this.log.Info("Dump", () => string.Format("IsDefault: {0}", device.IsDefault));
            this.log.Info("Dump", () => string.Format("IsEnabled: {0}", device.IsEnabled));
            this.log.Info("Dump", () => string.Format("     Kind: {0}", device.Kind));

            this.log.Info("Dump", () => string.Format("PROPERTIES"));
            int number = device?.Properties.Count ?? -1;
            System.Diagnostics.Debug.WriteLine("Properties: {0}", number);
            if (number > 0) {
                foreach (var p in device.Properties) {
                    this.log.Info("Dump", () => string.Format("      Property: {0} : {1}", p.Key, p.Value));
                }
            }

            this.log.Info("Dump", () => string.Format("ENCLOSURE LOCATIONS"));
            if (device.EnclosureLocation != null) {
                this.log.Info("Dump", () => string.Format("     InDock: {0}", device.EnclosureLocation.InDock));
                this.log.Info("Dump", () => string.Format("      InLid: {0}", device.EnclosureLocation.InLid));
                this.log.Info("Dump", () => string.Format("      Panel: {0}", device.EnclosureLocation.Panel));
                this.log.Info("Dump", () => string.Format("      Angle: {0}", device.EnclosureLocation.RotationAngleInDegreesClockwise));
            }
            else {
                System.Diagnostics.Debug.WriteLine("EnclosureLocation: null");
            }

            this.log.Info("Dump", () => string.Format("PAIRING"));
            if (device.Pairing != null) {
                this.log.Info("Dump", () => string.Format("    CanPair: {0}", device.Pairing.CanPair));
                this.log.Info("Dump", () => string.Format("   IsPaired: {0}", device.Pairing.IsPaired));
                this.log.Info("Dump", () => string.Format(" Protection: {0}", device.Pairing.ProtectionLevel));
                if (device.Pairing.Custom != null) {
                    this.log.Info("Dump", () => string.Format("     Custom: not null"));
                }
                else {
                    this.log.Info("Dump", () => string.Format("Custom: null"));
                }
            }
            else {
                this.log.Info("Dump", () => string.Format("Custom: null"));
            }
            //this.log.Info("Dump", () => string.Format());
        }





    }
}
