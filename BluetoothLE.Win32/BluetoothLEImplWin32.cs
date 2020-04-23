#define USING_OLDER_UWP

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



// Example code read write
// https://www.c-sharpcorner.com/code/1912/uwp-bluetooth-le-implementation
// https://github.com/nexussays/ble.net/blob/master/Readme.md

namespace BluetoothLE.Win32 {

    // Using the UWP Bluetooth LE calls from Win 32 desktop
    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/e261aeb5-104d-43ba-9b2b-97447614dec0/how-to-use-windowsdevices-api-in-a-c-winform-desktop-application-in-windows-10?forum=winforms
    // Add Reference to  \Program Files\Windows Kits\10\UnionMetadata\Windows.winmd
    // Add Reference to \Program Files\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll
    // Regardless what Target Framework i'm using, the only working version of System.Runtime.WindowsRuntime.dll was v4.5. All other version (wich corresponds to the Target Framework) caused runtime errors.

    public class BluetoothLEImplWin32 : IBLETInterface {

        #region data
        
        private ManualResetEvent stopped = new ManualResetEvent(false);

        /// <summary>Allows discovery of device list</summary>
        private DeviceWatcher devWatcher = null;

        private BluetoothLEDevice currentDevice = null;
        private ClassLog log = new ClassLog("BluetoothLEImplWin32");

        #endregion

        #region IBLETInterface:ICommStackChannel

        // TODO - the read thread on the BLE will raise this
        // When bytes come in
        public event EventHandler<byte[]> MsgReceivedEvent;

        public bool SendOutMsg(byte[] msg) {
            //throw new NotImplementedException();
            // TODO - send out by some kind of stream to BLE device - see classic
            return false;
        }

        #endregion



        #region Interface events

        public event EventHandler<BluetoothLEDeviceInfo> DeviceDiscovered;

        public event EventHandler<string> DeviceRemoved;

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
  

        private async Task ConnectToDevice(BluetoothLEDeviceInfo deviceInfo) {
            this.log.Info("ConnectToDevice", () => string.Format("Attempting connection to {0}: FromIdAsync({1})",
                deviceInfo.Name, deviceInfo.Id));

            try {
                // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs

                this.log.Info("ConnectToDevice", () => string.Format("Stored Device Info ID {0}", this.id));
                this.log.Info("ConnectToDevice", () => string.Format(" Param Device Info ID {0}", deviceInfo.Id));

                //this.currentDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                this.currentDevice = //await BluetoothLEDevice.FromIdAsync(this.id);
                    await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);


                if (this.currentDevice == null) {
                    this.log.Info("ConnectToDevice", "Connection failed" );
                }
                else {
                    this.log.Info("ConnectToDevice", "Connection ** OK **");
                }

                
                this.log.Info("ConnectToDevice", () => string.Format("Device {0} Connection status {1}", 
                    this.currentDevice.Name, this.currentDevice.ConnectionStatus.ToString()));

#if USING_OLDER_UWP
                this.log.Info("ConnectToDevice", () => string.Format("Device {0} Gatt services count {1}", 
                    this.currentDevice.Name, this.currentDevice.GattServices == null ? "NULL" : this.currentDevice.GattServices.Count.ToString() ));

                if (this.currentDevice.GattServices != null) {
                    foreach (var serv in this.currentDevice.GattServices) {
                        this.log.Info("ConnectToDevice", () => string.Format("Gatt service {0}", serv.Uuid.ToString()));
                        //GattDeviceService s = this.currentDevice.GetGattService(serv.Uuid);
                        this.log.Info("ConnectToDevice", "    CHARACTERISTICS");
                        foreach (var ch in serv.GetAllCharacteristics()) {
                            this.log.Info("ConnectToDevice", () => string.Format("  - Characteristic {0}", ch.UserDescription));
                            foreach (var desc in ch.GetAllDescriptors()) {
                                // descriptors have read and write
                                this.log.Info("ConnectToDevice", () => string.Format("      - Descriptors {0}", desc.Uuid.ToString()));
                            }
                        }

                    }
                }
#else

#endif


                this.log.Info("ConnectToDevice", "Get GATT services");
#if USING_OLDER_UWP
                //// This blows up with OLD and New
                //// ArgumentException : Value does not fall within the expected range.
                //GattDeviceService service = await GattDeviceService.FromIdAsync(this.id);
                //foreach (var s in service.GetAllCharacteristics()) {
                //    this.log.Info("ConnectToDevice", () => string.Format("Service Description: {0}",
                //        s.Uuid.ToString()));
                //}
#else
                GattDeviceServicesResult result = await this.currentDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                this.log.Info("ConnectToDevice", () => string.Format("Service search result {0}", result.Status.ToString()));
                foreach (var s in result.Services) {
                    this.log.Info("ConnectToDevice", () => string.Format("Service Description: {0}", s.Uuid.ToString()));
                }
#endif

            }
            catch (Exception e) {
                this.log.Exception(9999, "Exception", e);
            }

            if (this.currentDevice == null) {
                // report error
                this.log.Info("ConnectToDevice", () => string.Format("NULL device returned for {0}", deviceInfo.Id));
                return;
            }
            else {
                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                //GattDeviceServicesResult result =
                //    await this.currentDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                ////GattDeviceServicesResult result = await BluetoothLEDevice.FromIdAsync(this.currentDevice.DeviceId);
                //System.Diagnostics.Debug.WriteLine("Device Connected {0}", this.currentDevice.BluetoothAddress);
                this.log.Info("ConnectToDevice", () => string.Format("Device Connected {0}", this.currentDevice.BluetoothAddress));
            }

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

#region Discovery methods

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
            this.stopped.Set();
        }

        
        /// <summary>Event fired when the enumeration is complete</summary>
        private void DevWatcher_EnumerationCompleted(DeviceWatcher sender, object args) {
        }


        /// <summary>Fired when a device disappears</summary>
        /// <param name="sender"></param>
        /// <param name="updateInfo">The removed device. Use the ID</param>
        private void DevWatcher_Removed(DeviceWatcher sender, DeviceInformationUpdate updateInfo) {
            if (this.DeviceRemoved != null) {
                this.DeviceRemoved(sender, updateInfo.Id);
            }
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
            int number = device?.Properties.Count??-1;
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



#endregion

    }
}

#if false
//https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Devices.Enumeration;
using Windows.Security.Cryptography;
using Windows.Storage.Streams;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace SDKTemplate
{
    // This scenario connects to the device selected in the "Discover
    // GATT Servers" scenario and communicates with it.
    // Note that this scenario is rather artificial because it communicates
    // with an unknown service with unknown characteristics.
    // In practice, your app will be interested in a specific service with
    // a specific characteristic.
    public sealed partial class Scenario2_Client : Page
    {
        private MainPage rootPage = MainPage.Current;

        private BluetoothLEDevice bluetoothLeDevice = null;
        private GattCharacteristic selectedCharacteristic;

        // Only one registered characteristic at a time.
        private GattCharacteristic registeredCharacteristic;
        private GattPresentationFormat presentationFormat;

#region Error Codes
        readonly int E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED = unchecked((int)0x80650003);
        readonly int E_BLUETOOTH_ATT_INVALID_PDU = unchecked((int)0x80650004);
        readonly int E_ACCESSDENIED = unchecked((int)0x80070005);
        readonly int E_DEVICE_NOT_AVAILABLE = unchecked((int)0x800710df); // HRESULT_FROM_WIN32(ERROR_DEVICE_NOT_AVAILABLE)
#endregion

#region UI Code
        public Scenario2_Client()
        {
            InitializeComponent();
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            SelectedDeviceRun.Text = rootPage.SelectedBleDeviceName;
            if (string.IsNullOrEmpty(rootPage.SelectedBleDeviceId))
            {
                ConnectButton.IsEnabled = false;
            }
        }

        protected override async void OnNavigatedFrom(NavigationEventArgs e)
        {
            var success = await ClearBluetoothLEDeviceAsync();
            if (!success)
            {
                rootPage.NotifyUser("Error: Unable to reset app state", NotifyType.ErrorMessage);
            }
        }
#endregion

#region Enumerating Services
        private async Task<bool> ClearBluetoothLEDeviceAsync()
        {
            if (subscribedForNotifications)
            {
                // Need to clear the CCCD from the remote device so we stop receiving notifications
                var result = await registeredCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.None);
                if (result != GattCommunicationStatus.Success)
                {
                    return false;
                }
                else
                {
                    selectedCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                    subscribedForNotifications = false;
                }
            }
            bluetoothLeDevice?.Dispose();
            bluetoothLeDevice = null;
            return true;
        }

        private async void ConnectButton_Click()
        {
            ConnectButton.IsEnabled = false;

            if (!await ClearBluetoothLEDeviceAsync())
            {
                rootPage.NotifyUser("Error: Unable to reset state, try again.", NotifyType.ErrorMessage);
                ConnectButton.IsEnabled = true;
                return;
            }

            try
            {
                // BT_Code: BluetoothLEDevice.FromIdAsync must be called from a UI thread because it may prompt for consent.
                bluetoothLeDevice = await BluetoothLEDevice.FromIdAsync(rootPage.SelectedBleDeviceId);

                if (bluetoothLeDevice == null)
                {
                    rootPage.NotifyUser("Failed to connect to device.", NotifyType.ErrorMessage);
                }
            }
            catch (Exception ex) when (ex.HResult == E_DEVICE_NOT_AVAILABLE)
            {
                rootPage.NotifyUser("Bluetooth radio is not on.", NotifyType.ErrorMessage);
            }

            if (bluetoothLeDevice != null)
            {
                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                GattDeviceServicesResult result = await bluetoothLeDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    var services = result.Services;
                    rootPage.NotifyUser(String.Format("Found {0} services", services.Count), NotifyType.StatusMessage);
                    foreach (var service in services)
                    {
                        ServiceList.Items.Add(new ComboBoxItem { Content = DisplayHelpers.GetServiceName(service), Tag = service });
                    }
                    ConnectButton.Visibility = Visibility.Collapsed;
                    ServiceList.Visibility = Visibility.Visible;
                }
                else
                {
                    rootPage.NotifyUser("Device unreachable", NotifyType.ErrorMessage);
                }
            }
            ConnectButton.IsEnabled = true;
        }
#endregion

#region Enumerating Characteristics
        private async void ServiceList_SelectionChanged()
        {
            var service = (GattDeviceService)((ComboBoxItem)ServiceList.SelectedItem)?.Tag;

            CharacteristicList.Items.Clear();
            RemoveValueChangedHandler();

            IReadOnlyList<GattCharacteristic> characteristics = null;
            try
            {
                // Ensure we have access to the device.
                var accessStatus = await service.RequestAccessAsync();
                if (accessStatus == DeviceAccessStatus.Allowed)
                {
                    // BT_Code: Get all the child characteristics of a service. Use the cache mode to specify uncached characterstics only 
                    // and the new Async functions to get the characteristics of unpaired devices as well. 
                    var result = await service.GetCharacteristicsAsync(BluetoothCacheMode.Uncached);
                    if (result.Status == GattCommunicationStatus.Success)
                    {
                        characteristics = result.Characteristics;
                    }
                    else
                    {
                        rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                        // On error, act as if there are no characteristics.
                        characteristics = new List<GattCharacteristic>();
                    }
                }
                else
                {
                    // Not granted access
                    rootPage.NotifyUser("Error accessing service.", NotifyType.ErrorMessage);

                    // On error, act as if there are no characteristics.
                    characteristics = new List<GattCharacteristic>();

                }
            }
            catch (Exception ex)
            {
                rootPage.NotifyUser("Restricted service. Can't read characteristics: " + ex.Message,
                    NotifyType.ErrorMessage);
                // On error, act as if there are no characteristics.
                characteristics = new List<GattCharacteristic>();
            }

            foreach (GattCharacteristic c in characteristics)
            {
                CharacteristicList.Items.Add(new ComboBoxItem { Content = DisplayHelpers.GetCharacteristicName(c), Tag = c });
            }
            CharacteristicList.Visibility = Visibility.Visible;
        }
#endregion

        private void AddValueChangedHandler()
        {
            ValueChangedSubscribeToggle.Content = "Unsubscribe from value changes";
            if (!subscribedForNotifications)
            {
                registeredCharacteristic = selectedCharacteristic;
                registeredCharacteristic.ValueChanged += Characteristic_ValueChanged;
                subscribedForNotifications = true;
            }
        }

        private void RemoveValueChangedHandler()
        {
            ValueChangedSubscribeToggle.Content = "Subscribe to value changes";
            if (subscribedForNotifications)
            {
                registeredCharacteristic.ValueChanged -= Characteristic_ValueChanged;
                registeredCharacteristic = null;
                subscribedForNotifications = false;
            }
        }

        private async void CharacteristicList_SelectionChanged()
        {
            selectedCharacteristic = (GattCharacteristic)((ComboBoxItem)CharacteristicList.SelectedItem)?.Tag;
            if (selectedCharacteristic == null)
            {
                EnableCharacteristicPanels(GattCharacteristicProperties.None);
                rootPage.NotifyUser("No characteristic selected", NotifyType.ErrorMessage);
                return;
            }

            // Get all the child descriptors of a characteristics. Use the cache mode to specify uncached descriptors only 
            // and the new Async functions to get the descriptors of unpaired devices as well. 
            var result = await selectedCharacteristic.GetDescriptorsAsync(BluetoothCacheMode.Uncached);
            if (result.Status != GattCommunicationStatus.Success)
            {
                rootPage.NotifyUser("Descriptor read failure: " + result.Status.ToString(), NotifyType.ErrorMessage);
            }

            // BT_Code: There's no need to access presentation format unless there's at least one. 
            presentationFormat = null;
            if (selectedCharacteristic.PresentationFormats.Count > 0)
            {

                if (selectedCharacteristic.PresentationFormats.Count.Equals(1))
                {
                    // Get the presentation format since there's only one way of presenting it
                    presentationFormat = selectedCharacteristic.PresentationFormats[0];
                }
                else
                {
                    // It's difficult to figure out how to split up a characteristic and encode its different parts properly.
                    // In this case, we'll just encode the whole thing to a string to make it easy to print out.
                }
            }

            // Enable/disable operations based on the GattCharacteristicProperties.
            EnableCharacteristicPanels(selectedCharacteristic.CharacteristicProperties);
        }

        private void SetVisibility(UIElement element, bool visible)
        {
            element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;
        }

        private void EnableCharacteristicPanels(GattCharacteristicProperties properties)
        {
            // BT_Code: Hide the controls which do not apply to this characteristic.
            SetVisibility(CharacteristicReadButton, properties.HasFlag(GattCharacteristicProperties.Read));

            SetVisibility(CharacteristicWritePanel,
                properties.HasFlag(GattCharacteristicProperties.Write) ||
                properties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse));
            CharacteristicWriteValue.Text = "";

            SetVisibility(ValueChangedSubscribeToggle, properties.HasFlag(GattCharacteristicProperties.Indicate) ||
                                                       properties.HasFlag(GattCharacteristicProperties.Notify));

        }

        private async void CharacteristicReadButton_Click()
        {
            // BT_Code: Read the actual value from the device by using Uncached.
            GattReadResult result = await selectedCharacteristic.ReadValueAsync(BluetoothCacheMode.Uncached);
            if (result.Status == GattCommunicationStatus.Success)
            {
                string formattedResult = FormatValueByPresentation(result.Value, presentationFormat);
                rootPage.NotifyUser($"Read result: {formattedResult}", NotifyType.StatusMessage);
            }
            else
            {
                rootPage.NotifyUser($"Read failed: {result.Status}", NotifyType.ErrorMessage);
            }
        }

        private async void CharacteristicWriteButton_Click()
        {
            if (!String.IsNullOrEmpty(CharacteristicWriteValue.Text))
            {
                var writeBuffer = CryptographicBuffer.ConvertStringToBinary(CharacteristicWriteValue.Text,
                    BinaryStringEncoding.Utf8);

                var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writeBuffer);
            }
            else
            {
                rootPage.NotifyUser("No data to write to device", NotifyType.ErrorMessage);
            }
        }

        private async void CharacteristicWriteButtonInt_Click()
        {
            if (!String.IsNullOrEmpty(CharacteristicWriteValue.Text))
            {
                var isValidValue = Int32.TryParse(CharacteristicWriteValue.Text, out int readValue);
                if (isValidValue)
                {
                    var writer = new DataWriter();
                    writer.ByteOrder = ByteOrder.LittleEndian;
                    writer.WriteInt32(readValue);

                    var writeSuccessful = await WriteBufferToSelectedCharacteristicAsync(writer.DetachBuffer());
                }
                else
                {
                    rootPage.NotifyUser("Data to write has to be an int32", NotifyType.ErrorMessage);
                }
            }
            else
            {
                rootPage.NotifyUser("No data to write to device", NotifyType.ErrorMessage);
            }
        }

        private async Task<bool> WriteBufferToSelectedCharacteristicAsync(IBuffer buffer)
        {
            try
            {
                // BT_Code: Writes the value from the buffer to the characteristic.
                var result = await selectedCharacteristic.WriteValueWithResultAsync(buffer);

                if (result.Status == GattCommunicationStatus.Success)
                {
                    rootPage.NotifyUser("Successfully wrote value to device", NotifyType.StatusMessage);
                    return true;
                }
                else
                {
                    rootPage.NotifyUser($"Write failed: {result.Status}", NotifyType.ErrorMessage);
                    return false;
                }
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_INVALID_PDU)
            {
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                return false;
            }
            catch (Exception ex) when (ex.HResult == E_BLUETOOTH_ATT_WRITE_NOT_PERMITTED || ex.HResult == E_ACCESSDENIED)
            {
                // This usually happens when a device reports that it support writing, but it actually doesn't.
                rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                return false;
            }
        }

        private bool subscribedForNotifications = false;
        private async void ValueChangedSubscribeToggle_Click()
        {
            if (!subscribedForNotifications)
            {
                // initialize status
                GattCommunicationStatus status = GattCommunicationStatus.Unreachable;
                var cccdValue = GattClientCharacteristicConfigurationDescriptorValue.None;
                if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Indicate;
                }

                else if (selectedCharacteristic.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify))
                {
                    cccdValue = GattClientCharacteristicConfigurationDescriptorValue.Notify;
                }

                try
                {
                    // BT_Code: Must write the CCCD in order for server to send indications.
                    // We receive them in the ValueChanged event handler.
                    status = await selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(cccdValue);

                    if (status == GattCommunicationStatus.Success)
                    {
                        AddValueChangedHandler();
                        rootPage.NotifyUser("Successfully subscribed for value changes", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser($"Error registering for value changes: {status}", NotifyType.ErrorMessage);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support indicate, but it actually doesn't.
                    rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                }
            }
            else
            {
                try
                {
                    // BT_Code: Must write the CCCD in order for server to send notifications.
                    // We receive them in the ValueChanged event handler.
                    // Note that this sample configures either Indicate or Notify, but not both.
                    var result = await
                            selectedCharacteristic.WriteClientCharacteristicConfigurationDescriptorAsync(
                                GattClientCharacteristicConfigurationDescriptorValue.None);
                    if (result == GattCommunicationStatus.Success)
                    {
                        subscribedForNotifications = false;
                        RemoveValueChangedHandler();
                        rootPage.NotifyUser("Successfully un-registered for notifications", NotifyType.StatusMessage);
                    }
                    else
                    {
                        rootPage.NotifyUser($"Error un-registering for notifications: {result}", NotifyType.ErrorMessage);
                    }
                }
                catch (UnauthorizedAccessException ex)
                {
                    // This usually happens when a device reports that it support notify, but it actually doesn't.
                    rootPage.NotifyUser(ex.Message, NotifyType.ErrorMessage);
                }
            }
        }

        private async void Characteristic_ValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args)
        {
            // BT_Code: An Indicate or Notify reported that the value has changed.
            // Display the new value with a timestamp.
            var newValue = FormatValueByPresentation(args.CharacteristicValue, presentationFormat);
            var message = $"Value at {DateTime.Now:hh:mm:ss.FFF}: {newValue}";
            await Dispatcher.RunAsync(CoreDispatcherPriority.Normal,
                () => CharacteristicLatestValue.Text = message);
        }

        private string FormatValueByPresentation(IBuffer buffer, GattPresentationFormat format)
        {
            // BT_Code: For the purpose of this sample, this function converts only UInt32 and
            // UTF-8 buffers to readable text. It can be extended to support other formats if your app needs them.
            byte[] data;
            CryptographicBuffer.CopyToByteArray(buffer, out data);
            if (format != null)
            {
                if (format.FormatType == GattPresentationFormatTypes.UInt32 && data.Length >= 4)
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                else if (format.FormatType == GattPresentationFormatTypes.Utf8)
                {
                    try
                    {
                        return Encoding.UTF8.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        return "(error: Invalid UTF-8 string)";
                    }
                }
                else
                {
                    // Add support for other format types as needed.
                    return "Unsupported format: " + CryptographicBuffer.EncodeToHexString(buffer);
                }
            }
            else if (data != null)
            {
                // We don't know what format to use. Let's try some well-known profiles, or default back to UTF-8.
                if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.HeartRateMeasurement))
                {
                    try
                    {
                        return "Heart Rate: " + ParseHeartRateValue(data).ToString();
                    }
                    catch (ArgumentException)
                    {
                        return "Heart Rate: (unable to parse)";
                    }
                }
                else if (selectedCharacteristic.Uuid.Equals(GattCharacteristicUuids.BatteryLevel))
                {
                    try
                    {
                        // battery level is encoded as a percentage value in the first byte according to
                        // https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.battery_level.xml
                        return "Battery Level: " + data[0].ToString() + "%";
                    }
                    catch (ArgumentException)
                    {
                        return "Battery Level: (unable to parse)";
                    }
                }
                // This is our custom calc service Result UUID. Format it like an Int
                else if (selectedCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
                {
                    return BitConverter.ToInt32(data, 0).ToString();
                }
                // No guarantees on if a characteristic is registered for notifications.
                else if (registeredCharacteristic != null)
                {
                    // This is our custom calc service Result UUID. Format it like an Int
                    if (registeredCharacteristic.Uuid.Equals(Constants.ResultCharacteristicUuid))
                    {
                        return BitConverter.ToInt32(data, 0).ToString();
                    }
                }
                else
                {
                    try
                    {
                        return "Unknown format: " + Encoding.UTF8.GetString(data);
                    }
                    catch (ArgumentException)
                    {
                        return "Unknown format";
                    }
                }
            }
            else
            {
                return "Empty data received";
            }
            return "Unknown format";
        }

        /// <summary>
        /// Process the raw data received from the device into application usable data,
        /// according the the Bluetooth Heart Rate Profile.
        /// https://www.bluetooth.com/specifications/gatt/viewer?attributeXmlFile=org.bluetooth.characteristic.heart_rate_measurement.xml&u=org.bluetooth.characteristic.heart_rate_measurement.xml
        /// This function throws an exception if the data cannot be parsed.
        /// </summary>
        /// <param name="data">Raw data received from the heart rate monitor.</param>
        /// <returns>The heart rate measurement value.</returns>
        private static ushort ParseHeartRateValue(byte[] data)
        {
            // Heart Rate profile defined flag values
            const byte heartRateValueFormat = 0x01;

            byte flags = data[0];
            bool isHeartRateValueSizeLong = ((flags & heartRateValueFormat) != 0);

            if (isHeartRateValueSizeLong)
            {
                return BitConverter.ToUInt16(data, 1);
            }
            else
            {
                return data[1];
            }
        }
    }
}
© 2020 GitHub, Inc.

#endif

