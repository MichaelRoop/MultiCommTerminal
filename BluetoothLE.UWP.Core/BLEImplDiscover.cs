using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using ChkUtils.Net;
using Common.Net.Network;
using Windows.Devices.Enumeration;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        /// <summary>Called from syncronous Interface method .. to call async methods</summary>
        private void DoLEWatcherSearch() {
            this.TearDownWatcher();
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
                        break;
                    case DeviceWatcherStatus.EnumerationCompleted:
                        // Call Stop and wait on stopped event
                        this.stopped.Reset();
                        this.devWatcher.Stop();
                        if (this.stopped.WaitOne(500)) {
                            this.log.Info("DoLEWatcherSearch", "THE WATCHER IS RESTARTED");
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
                //https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario1_Discovery.xaml.cs
                // This will return all BLE devices, paired or unpaired
                string aqsBLEProtocol = "(System.Devices.Aep.ProtocolId:=\"{bb7bb05e-5972-42b5-94fc-76eaa7084d49}\")";
                this.devWatcher = DeviceInformation.CreateWatcher(
                    aqsBLEProtocol, 
                    null, // no extra properties in the query
                    DeviceInformationKind.AssociationEndpoint);

                // First only returns ones that are paired, the second only those not paired
                //this.devWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelector());
                //this.devWatcher = DeviceInformation.CreateWatcher(BluetoothLEDevice.GetDeviceSelectorFromPairingState(false));

                // Hook up the events
                this.devWatcher.Added += DevWatcher_Added;
                this.devWatcher.Updated += DevWatcher_Updated;
                this.devWatcher.Removed += DevWatcher_Removed;
                this.devWatcher.EnumerationCompleted += DevWatcher_EnumerationCompleted;
                this.devWatcher.Stopped += DevWatcher_Stopped;
            }
        }


        // Events stay all the time. Only added once. Caller can disable as required
        private void TearDownWatcher() {
            if (devWatcher != null) {
                this.devWatcher.Added -= DevWatcher_Added;
                this.devWatcher.Updated -= DevWatcher_Updated;
                this.devWatcher.Removed -= DevWatcher_Removed;
                this.devWatcher.EnumerationCompleted -= DevWatcher_EnumerationCompleted;
                this.devWatcher.Stopped -= DevWatcher_Stopped;
                this.devWatcher.Stop();
                this.devWatcher = null;
                
            }
        }


        /// <summary>Event fired when the watcher is stopped</summary>
        private void DevWatcher_Stopped(DeviceWatcher sender, object args) {
            this.log.Info("DevWatcher_Stopped", "*****");
            this.stopped.Set();
        }


        /// <summary>Event fired when the enumeration is complete</summary>
        private void DevWatcher_EnumerationCompleted(DeviceWatcher sender, object args) {
            this.log.Info("DevWatcher_EnumerationCompleted", "*****");
            // TODO - implement
            if (this.DeviceDiscoveryCompleted != null) {
                this.DeviceDiscoveryCompleted.Invoke(this, true);
            }
        }


        /// <summary>Fired when a device disappears</summary>
        /// <param name="sender"></param>
        /// <param name="updateInfo">The removed device. Use the ID</param>
        private void DevWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate updateInfo) {
            if (this.DeviceRemoved != null) {
                //this.log.Info("DevWatcher_Removed", () => string.Format("----- {0}", updateInfo.Id));
                this.DeviceRemoved(sender, updateInfo.Id);
            }
        }


        /// <summary>Fired when the device is changed</summary>
        /// <param name="sender"></param>
        /// <param name="updateInfo"></param>
        private void DevWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate updateInfo) {
            this.log.Info("DevWatcher_Updated", "------------------------------ START -----------------------------------------------------");
            this.log.Info("DevWatcher_Updated", () => string.Format("***** {0}", updateInfo.Id));
            WrapErr.ToErrReport(9999, () => {

                NetPropertiesUpdateDataModel dm = updateInfo.CreatePropertiesUpdateData();
                this.DeviceUpdated?.Invoke(this, dm);

                //this.devic

                //if (updateInfo.Properties != null) {
                //    foreach (var p in updateInfo.Properties) {
                //        string value = "Unhandled";
                //        switch (p.Key) {
                //            case "System.Devices.Aep.ContainerId":
                //            case "Id":
                //            case "System.ItemNameDisplay": // *
                //            case "System.Devices.Icon":
                //            case "System.Devices.GlyphIcon":
                //                value = (string)p.Value;
                //                break;
                //            case "System.Devices.Aep.IsPaired": // *
                //            case "System.Devices.Aep.IsConnected":
                //            case "System.Devices.Aep.IsConnectable":
                //            case "System.Devices.Aep.CanPair": // *
                //                value = string.Format("{0}", (bool)p.Value);
                //                break;
                //                // "System.Devices.Icon"
                //                // "System.Devices.GlyphIcon"

                //        }
                //        this.log.Info("DevWatcher_Updated", () => string.Format("    Property:{0}  Value:{1}", p.Key, value));
                //    }
                //}
            });
            this.log.Info("DevWatcher_Updated", "------------------------------ END -----------------------------------------------------");
        }


        /// <summary>Fired when a device is discovered</summary>
        /// <param name="sender"></param>
        /// <param name="deviceInfo">Info on the discovered device</param>
        private void DevWatcher_Added(DeviceWatcher sender, DeviceInformation deviceInfo) {
            if (deviceInfo.Name.Length > 0) {
                WrapErr.ToErrReport(9999, () => {

                    // TODO - find out what comes in with no name
                    this.log.Info("DevWatcher_Added", () => string.Format("+++++ {0} : {1}", deviceInfo.Name, deviceInfo.Id));
                    this.DebugDumpDeviceInfo(deviceInfo);
                    
                    if (this.DeviceDiscovered != null) {
                        BluetoothLEDeviceInfo dev = new BluetoothLEDeviceInfo(new Bluetooth_WinPropertyKeys()) {
                            Name = deviceInfo.Name,
                            Id = deviceInfo.Id,
                            IsDefault = deviceInfo.IsDefault,
                            CanPair = deviceInfo?.Pairing.CanPair ?? false,
                            IsPaired = deviceInfo?.Pairing.IsPaired ?? false,
                            IsConnected = false,
                            ServiceProperties = deviceInfo.CreatePropertiesDictionary(),
                            // This would be the DeviceInformation object. Not required
                            // but handy for updates in Windows
                            OSSpecificObj = deviceInfo,
                        };
                        this.DeviceDiscovered(sender, dev);
                    }
                });
            }
        }


        private void DebugDumpDeviceInfo(DeviceInformation device) {
            this.log.Info("Dump", () => string.Format("     Name: {0}", device.Name));
            this.log.Info("Dump", () => string.Format("       Id: {0}", device.Id));
            this.log.Info("Dump", () => string.Format("IsDefault: {0}", device.IsDefault));
            this.log.Info("Dump", () => string.Format("IsEnabled: {0}", device.IsEnabled));
            this.log.Info("Dump", () => string.Format("     Kind: {0}", device.Kind));

            int number = device?.Properties.Count ?? -1;
            if (number > 0) {
                this.log.Info("Dump", () => string.Format("PROPERTIES ({0})", number));
                foreach (var p in device.Properties) {
                    this.log.Info("Dump", () => string.Format("      Property: {0} : {1}", p.Key, p.Value));
                }
            }

            if (device.EnclosureLocation != null) {
                this.log.Info("Dump", () => string.Format("ENCLOSURE LOCATIONS"));
                this.log.Info("Dump", () => string.Format("     InDock: {0}", device.EnclosureLocation.InDock));
                this.log.Info("Dump", () => string.Format("      InLid: {0}", device.EnclosureLocation.InLid));
                this.log.Info("Dump", () => string.Format("      Panel: {0}", device.EnclosureLocation.Panel));
                this.log.Info("Dump", () => string.Format("      Angle: {0}", device.EnclosureLocation.RotationAngleInDegreesClockwise));
            }
            else {
                this.log.Error(9999, "EnclosureLocation: null");
            }

            if (device.Pairing != null) {
                this.log.Info("Dump", () => string.Format("PAIRING"));
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
        }

    }
}
