using BluetoothLE.Net.DataModels;
using ChkUtils.Net;
using Common.Net.Network;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLESelect.xaml</summary>
    public partial class BLESelect : Window {

        #region Data and properties

        private Window parent = null;
        private bool pair = false;
        private List<BluetoothLEDeviceInfo> devices = new List<BluetoothLEDeviceInfo>();
        private ClassLog log = new ClassLog("BLESelect");

        public BluetoothLEDeviceInfo SelectedBLE { get; private set; } = null;

        #endregion

        #region Constructor and window event

        public static BluetoothLEDeviceInfo ShowBox(Window parent, bool pair = true) {
            BLESelect win = new BLESelect(parent, pair);
            win.ShowDialog();
            return win.SelectedBLE;
        }


        public BLESelect(Window parent, bool pair = true) {
            this.parent = parent;
            this.pair = pair;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            DI.Wrapper.BLE_DeviceDiscovered += this.deviceDiscovered;
            DI.Wrapper.BLE_DeviceRemoved += this.deviceRemoved;
            DI.Wrapper.BLE_DeviceUpdated += this.deviceUpdated;
            DI.Wrapper.BLE_DeviceDiscoveryComplete += this.deviceDiscoveryComplete;
            this.gridWait.Show();
            // TODO - modify to look for paired or unpaired
            DI.Wrapper.BLE_DiscoverAsync();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            DI.Wrapper.BLE_DeviceDiscovered -= this.deviceDiscovered;
            DI.Wrapper.BLE_DeviceRemoved -= this.deviceRemoved;
            DI.Wrapper.BLE_DeviceUpdated -= this.deviceUpdated;
            DI.Wrapper.BLE_DeviceDiscoveryComplete -= this.deviceDiscoveryComplete;
            this.listBox_BLE.SelectionChanged -= this.selectionChanged;
        }



        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void selectionChanged(object sender, SelectionChangedEventArgs e) {
            BluetoothLEDeviceInfo device = this.listBox_BLE.SelectedItem as BluetoothLEDeviceInfo;
            if (device != null) {
                this.SelectedBLE = device;
                this.Close();
            }
        }

        #endregion

        #region DI BLE events

        private void deviceDiscovered(object sender, BluetoothLEDeviceInfo device) {
            Log.Info("BLESelect", "deviceDiscovered", () => string.Format("Found {0}", device.Name));
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device Discovered", () => {
                    lock (this.listBox_BLE) {
                        this.listBox_BLE.SelectionChanged -= this.selectionChanged;
                        this.log.Info("deviceDiscovered", () => string.Format("Adding '{0}' '{1}'", device.Name, device.Id));
                        this.RemoveIfFound(device.Id);
                        this.log.Info("BLE_DeviceDiscoveredHandler", () => string.Format("Adding DONE"));
                        this.listBox_BLE.Add(this.devices, device);
                        this.listBox_BLE.SelectionChanged += this.selectionChanged;
                    }
                });
            });
        }


        private void deviceDiscoveryComplete(object sender, bool e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (this.devices.Count == 0) {
                    App.ShowMsgTitle("", DI.Wrapper.GetText(MsgCode.NotFound));
                    this.Close();
                }
                else {
                    this.listBox_BLE.SelectionChanged += this.selectionChanged;
                }
            });
        }


        private void deviceRemoved(object sender, string id) {
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device Remove", () => {
                    lock (this.listBox_BLE) {
                        this.listBox_BLE.SelectionChanged -= this.selectionChanged;
                        this.RemoveIfFound(id);
                        this.listBox_BLE.SelectionChanged += this.selectionChanged;
                    }
                });
            });
        }


        private void deviceUpdated(object sender, NetPropertiesUpdateDataModel dataModel) {
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device Updated", () => {
                    lock (this.listBox_BLE) {
                        this.log.Info("", () => string.Format("Updating '{0}'", dataModel.Id));
                        // Disconnect the list from control before changing. Maybe change to Observable collection
                        this.listBox_BLE.SelectionChanged -= this.selectionChanged;
                        this.listBox_BLE.ItemsSource = null;
                        BluetoothLEDeviceInfo item = this.devices.Find((x) => x.Id == dataModel.Id);
                        if (item != null) {
                            // This will raise events on change. How to deal with that. Think only if it is the current on display
                            item.Update(dataModel.ServiceProperties);
                        }
                        this.listBox_BLE.ItemsSource = this.devices;
                        this.listBox_BLE.SelectionChanged += this.selectionChanged;
                    }
                });
            });
        }

        #endregion

        #region Private 

        private void RemoveIfFound(string id) {
            WrapErr.ToErrReport(9999, "Fail on Remove if found", () => {
                this.listBox_BLE.ItemsSource = null;
                var item = this.devices.Find((x) => x.Id == id);
                if (item != null) {
                    if (!this.devices.Remove(item)) {
                        this.log.Error(9999, "RemoveIfFound", () => string.Format("Failed to remove '{0}'", id));
                    }
                }
                this.listBox_BLE.ItemsSource = this.devices;
            });
        }

        #endregion

    }

}
