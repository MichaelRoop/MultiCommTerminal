using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BluetoothLE.Win32 {

    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/e261aeb5-104d-43ba-9b2b-97447614dec0/how-to-use-windowsdevices-api-in-a-c-winform-desktop-application-in-windows-10?forum=winforms
    //    The solution i found for my problem is as follows:
    // Create a new Empty Desktop Application
    // Add Reference to  \Program Files\Windows Kits\10\UnionMetadata\Windows.winmd
    // Add Reference to \Program Files\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll
    // Regardless what Target Framework i'm using, the only working version of System.Runtime.WindowsRuntime.dll was v4.5. All other version (wich corresponds to the Target Framework) caused runtime errors.


    public class BluetoothLEImplWin32 : IBTInterface {


        public event EventHandler<BTDeviceInfo> DiscoveredDevice;


        /// <summary>Interface call
        /// 
        /// </summary>
        /// <returns></returns>
        List<BTDeviceInfo> IBTInterface.DiscoverDevices() {
            this.DoLEWatcherSearch();
            return this.infoListMain;


            //return this.DiscoverDevices1();
            //return this.DiscoverDevices3();

            ////this.infoListMain.Clear();
            ////Task<List<BTDeviceInfo>> task = this.GetBTDevices().Wait(4000);

            //Task<List<BTDeviceInfo>> task = Task.Run<List<BTDeviceInfo>>(async () => await GetBTDevices());

            //var result = this.GetBTDevices().Wait(4000);


            //List<BTDeviceInfo> infoList = new List<BTDeviceInfo>();
            //foreach (BTDeviceInfo d in this.infoListMain) {
            //    BTDeviceInfo info = new BTDeviceInfo() {
            //        Name = d.Name,
            //        Connected = d.Connected,
            //        Authenticated = d.Authenticated,
            //        Address = d.Address,
            //        DeviceClassInt = d.DeviceClassInt,
            //        DeviceClassName = d.DeviceClassName,
            //        ServiceClassInt = d.ServiceClassInt,
            //        ServiceClassName = d.ServiceClassName,
            //    };
            //    infoList.Add(info);
            //}




            //return infoList;
        }

        #region Watcher approach

        DeviceWatcher devWatcher = null;

        ManualResetEvent done = new ManualResetEvent(false);


        private void DoLEWatcherSearch() {
            lock (this.infoListMain) {
                this.infoListMain.Clear();
            }
            this.SetupWatcher();
            //this.SetupEvents();
            //this.done.Reset();

            // Need to properly shut down watcher if it is up and running


            if (devWatcher.Status != DeviceWatcherStatus.Started) {
                this.devWatcher.Start();
            }

            //if (devWatcher.Status == DeviceWatcherStatus.Started) {
            //    this.done.Reset();
            //    this.devWatcher.Stop();
            //    this.done.WaitOne(5000);
            //}
            //this.devWatcher.Start();


            //// problem - the enumeration end never happens. Cannot set too high
            //if (this.done.WaitOne(30000)) {

            //}
            //this.TearDownEvents();
            //this.devWatcher.Stop();
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


                this.SetupEvents();
            }
        }

        private void SetupEvents() {
            devWatcher.Added += DevWatcher_Added;
            devWatcher.Updated += DevWatcher_Updated;
            devWatcher.Removed += DevWatcher_Removed;
            devWatcher.EnumerationCompleted += DevWatcher_EnumerationCompleted;
            devWatcher.Stopped += DevWatcher_Stopped;
        }

        private void TearDownEvents() {
            devWatcher.Added -= DevWatcher_Added;
            devWatcher.Updated -= DevWatcher_Updated;
            devWatcher.Removed -= DevWatcher_Removed;
            devWatcher.EnumerationCompleted -= DevWatcher_EnumerationCompleted;
            devWatcher.Stopped -= DevWatcher_Stopped;
        }




        private void DevWatcher_Stopped(DeviceWatcher sender, object args) {
            //throw new NotImplementedException();
            this.done.Set();
        }

        private void DevWatcher_EnumerationCompleted(DeviceWatcher sender, object args) {
            //this.done.Set();
            //if (this.devWatcher.Status == DeviceWatcherStatus.Started) {
            //    this.devWatcher.Stop();
            //}
        }

        private void DevWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate args) {
            //throw new NotImplementedException();
        }

        private void DevWatcher_Updated(DeviceWatcher sender, DeviceInformationUpdate args) {
            //throw new NotImplementedException();
        }

        private void DevWatcher_Added(DeviceWatcher sender, DeviceInformation args) {
            //lock (this.infoListMain) { // Locking the container causes the list not to populate past first
            //    if (args.Name.Length > 0) {
            //        this.infoListMain.Add(new BTDeviceInfo() {
            //            Name = args.Name,
            //            Address = args.Id,
            //            // Does not apply for bluetooth LE
            //            Authenticated = true,
            //            // the aqs query only returns the connected ones
            //            Connected = true, 
            //            DeviceClassInt = 0,
            //            DeviceClassName = "0",
            //            ServiceClassInt = 0,
            //            ServiceClassName = "0",
            //        });
            //    }
            //}

            if (args.Name.Length > 0) {
                if (this.DiscoveredDevice != null) {
                    this.DiscoveredDevice(this, new BTDeviceInfo() {
                        Name = args.Name,
                        Address = args.Id,
                        // Does not apply for bluetooth LE
                        Authenticated = true,
                        // the aqs query only returns the connected ones
                        Connected = true,
                        DeviceClassInt = 0,
                        DeviceClassName = "0",
                        ServiceClassInt = 0,
                        ServiceClassName = "0",
                    });
                }
            }

        }

        #endregion


        #region Async Calls with wait. Quarky

        List<BTDeviceInfo> infoListMain = new List<BTDeviceInfo>();

        DeviceInformationCollection found = null;


        List<BTDeviceInfo> DiscoverDevices1() {
            this.infoListMain.Clear();
            bool result = this.GetBTDevices1().Wait(8000);
            List<BTDeviceInfo> infoList = new List<BTDeviceInfo>();
            foreach (BTDeviceInfo d in this.infoListMain) {
                BTDeviceInfo info = new BTDeviceInfo() {
                    Name = d.Name,
                    Connected = d.Connected,
                    Authenticated = d.Authenticated,
                    Address = d.Address,
                    DeviceClassInt = d.DeviceClassInt,
                    DeviceClassName = d.DeviceClassName,
                    ServiceClassInt = d.ServiceClassInt,
                    ServiceClassName = d.ServiceClassName,
                };
                infoList.Add(info);
            }
            return infoList;
        }


        List<BTDeviceInfo> DiscoverDevices3() {
            bool result = this.GetBTDevices3().Wait(4000);
            //List<BTDeviceInfo> infoList = new List<BTDeviceInfo>();

            //// Hack for now until I can syn a return
            //if (this.found == null) {
            //    result = this.GetBTDevices3().Wait(4000);
            //}
            //int i = 0;

            if (this.found != null) {
                foreach (DeviceInformation d in this.found) {
                    //if (i++ > 15) {
                    //    //break;
                    //}

                    //// Get All info from one (itead)
                    //if (d.Name == "itead") {
                    //    string info = string.Format("");
                    //    this.infoListMain.Add(new BTDeviceInfo() {
                    //        Name = info,
                    //    });
                    //}





                    // They are all .DeviceInterface
                    //if (d.Kind == DeviceInformationKind.Device) {

                    // filters down to 216
                    //if (this.infoListMain.Find(x => x.Name == d.Name) == null) {

                    // They all 1413 have unique IDs even if same name
                    //if (this.infoListMain.Find(x => x.Address == d.Id) == null) {

                    // Pairing almost always null
                    //if (d.Pairing != null /*&& d.Pairing.IsPaired*/) {

                    //if (d.Id.Contains("BluetoothDevice")) {

                    BTDeviceInfo info = new BTDeviceInfo() {
                        Name = d.Name,
                        Connected = d.IsEnabled,//.Connected,
                        Authenticated = false, //d.Pairing.IsPaired,//.Authenticated,
                        Address = "", // d.Id,//.Address, // TODO - put back ID
                        DeviceClassInt = 0,// d.DeviceClassInt,
                        DeviceClassName = "0",// d.DeviceClassName,
                        ServiceClassInt = 0,// d.ServiceClassInt,
                        ServiceClassName = "0",// d.ServiceClassName,
                    };
                    this.infoListMain.Add(info);
                    //}
                }
            }
            else {
                //this.infoListMain.Add(new BTDeviceInfo() {
                //    Name = "blah",
                //    Address = "blahAddress",
                //});
            }


            return infoListMain;
        }

        async Task GetBTDevices1() {
            //await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync();
            //eviceClass dc = new DeviceClass();
            int i = 0;

            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.All);
            foreach (DeviceInformation d in devices) {
                BTDeviceInfo info = new BTDeviceInfo() {
                    Name = d.Name,
                    Connected = d.IsEnabled,//.Connected,
                    Authenticated = d.Pairing.IsPaired,//.Authenticated,
                    Address = d.Id,//.Address,
                    DeviceClassInt = 0,// d.DeviceClassInt,
                    DeviceClassName = "0",// d.DeviceClassName,
                    ServiceClassInt = 0,// d.ServiceClassInt,
                    ServiceClassName = "0",// d.ServiceClassName,
                };
                this.infoListMain.Add(info);
            }

            //foreach (BluetoothDeviceInfo dev in /*cl.PairedDevices*/ cl.DiscoverDevices()) {
            //    BTDeviceInfo info = new BTDeviceInfo() {
            //        Name = dev.DeviceName,
            //        Connected = dev.Connected,
            //        Authenticated = dev.Authenticated,
            //        Address = dev.DeviceAddress.ToString(),
            //        DeviceClassInt = dev.ClassOfDevice.Value,
            //        DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
            //        ServiceClassInt = (int)dev.ClassOfDevice.Service,
            //        ServiceClassName = dev.ClassOfDevice.Service.ToString(),
            //    };

            //    infoList.Add(info);
            //}

            }

        async Task GetBTDevices3() {
            //aqs syntax
            //https://docs.microsoft.com/en-us/uwp/api/windows.devices.enumeration.deviceinformation.createwatcher

            //this.found = await DeviceInformation.FindAllAsync(DeviceClass.All);

            // Has IP address "System.Devices.IpAddress:<>[]"
            // No IP address "System.Devices.IpAddress:=[]" - however it filters out the itead. Maybe because it has an ethernet shield?
            // Bluetooth LE address "System.Devices.Aep.DeviceAddress:<>[]"
            // LE Is Connected "System.Devices.Aep.IsConnected:true"  - does not work

            this.found = await DeviceInformation.FindAllAsync(
                "System.Devices.Aep.DeviceAddress:<>[]", null, DeviceInformationKind.Device);

            // for a watcher
            //"System.Devices.Aep.DeviceAddress", "System.Devices.Aep.IsConnected" };
        }

        async Task<List<BTDeviceInfo>> GetBTDevices2() {
            //await Windows.Devices.Enumeration.DeviceInformation.FindAllAsync();
            //eviceClass dc = new DeviceClass();

            DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(DeviceClass.All);
            List<BTDeviceInfo> infoList = new List<BTDeviceInfo>();
            foreach (DeviceInformation d in devices) {

                BTDeviceInfo info = new BTDeviceInfo() {
                    Name = d.Name,
                    Connected = d.IsEnabled,//.Connected,
                    Authenticated = d.Pairing.IsPaired,//.Authenticated,
                    Address = d.Id,//.Address,
                    DeviceClassInt = 0,// d.DeviceClassInt,
                    DeviceClassName = "0",// d.DeviceClassName,
                    ServiceClassInt = 0,// d.ServiceClassInt,
                    ServiceClassName = "0",// d.ServiceClassName,
                };
                infoList.Add(info);
            }
            return infoList;

            //foreach (BluetoothDeviceInfo dev in /*cl.PairedDevices*/ cl.DiscoverDevices()) {
            //    BTDeviceInfo info = new BTDeviceInfo() {
            //        Name = dev.DeviceName,
            //        Connected = dev.Connected,
            //        Authenticated = dev.Authenticated,
            //        Address = dev.DeviceAddress.ToString(),
            //        DeviceClassInt = dev.ClassOfDevice.Value,
            //        DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
            //        ServiceClassInt = (int)dev.ClassOfDevice.Service,
            //        ServiceClassName = dev.ClassOfDevice.Service.ToString(),
            //    };

            //    infoList.Add(info);
            //}



        }

        #endregion



    }
}
