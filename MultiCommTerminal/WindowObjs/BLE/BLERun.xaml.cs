using BluetoothLE.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.UserControls;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLERun.xaml</summary>
    public partial class BLERun : Window {

        private Window parent = null;
        private BluetoothLEDeviceInfo selectedDevice = null;

        public static int Instances { get; private set; }

        public event EventHandler<Type> CloseRequest;


        #region Constructors and window events

        public BLERun(Window parent) {
            this.parent = parent;
            Instances++;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.ui.ExitClicked += this.OnUiExit;
            this.ui.ConnectCicked += this.OnUiConnect;
            this.ui.DiscoverClicked += this.OnUiDiscover;
            this.ui.DisconnectClicked += this.OnUiDisconnect;
            this.ui.SendClicked += this.OnUiSend;
            this.ui.InfoClicked += this.OnUiInfo;
            this.ui.SettingsClicked += this.OnUiSettings;
            // Add connection, error and bytes received when supported
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.ui.OnLoad(this, CommMedium.BluetoothLE, new RunPageCtrlsEnabled() {
                Connect = false,
                Disconnect = false,
                Settings = false,
                Send = false,
            });
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.ui.ExitClicked -= this.OnUiExit;
            this.ui.ConnectCicked -= this.OnUiConnect;
            this.ui.DiscoverClicked -= this.OnUiDiscover;
            this.ui.DisconnectClicked -= this.OnUiDisconnect;
            this.ui.SendClicked -= this.OnUiSend;
            this.ui.InfoClicked -= this.OnUiInfo;
            this.ui.SettingsClicked -= this.OnUiSettings;
            // Add connection, error and bytes received when supported
            DI.Wrapper.BLE_DeviceInfoGathered -= deviceInfoGathered;
            this.ui.OnClosing();
            Instances--;
        }

        #endregion

        #region DI event handlers

        private void deviceInfoGathered(object sender, BluetoothLEDeviceInfo info) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                DI.Wrapper.BLE_DeviceInfoGathered -= deviceInfoGathered;
                DeviceInfo_BLE win = new DeviceInfo_BLE(this, info);
                win.ShowDialog();
            });
        }

        // Add other handlers when connection supported

        #endregion

        #region UI Event handlers

        private void OnUiExit(object sender, EventArgs e) {
            this.CloseRequest?.Invoke(this, typeof(BLERun));
        }

        private void OnUiDiscover(object sender, EventArgs e) {
            this.Title = "BLE";
            this.selectedDevice = BLESelect.ShowBox(this, true);
            if (this.selectedDevice != null) {
                this.Title = string.Format("(BLE) {0}", this.selectedDevice.Name);
            }
        }


        private void OnUiConnect(object sender, EventArgs e) {
            if (this.selectedDevice == null) {
                this.OnUiDiscover(sender, e);
            }
            if (this.selectedDevice != null) {
                //this.ui.IsBusy = true;
                DI.Wrapper.BLE_ConnectAsync(this.selectedDevice);
            }
        }


        private void OnUiDisconnect(object sender, EventArgs e) {
            DI.Wrapper.BLE_Disconnect();
        }


        private void OnUiSend(object sender, string msg) {
            //DI.Wrapper.BLE_Send(msg);
        }


        private void OnUiInfo(object sender, EventArgs e) {
            if (this.selectedDevice != null) {
                DI.Wrapper.BLE_DeviceInfoGathered += deviceInfoGathered;
                this.ui.IsBusy = true;
                DI.Wrapper.BLE_GetInfo(this.selectedDevice);
            }
            else {
                this.OnUiDiscover(sender, e);
                if (this.selectedDevice == null) {
                    App.ShowMsg(DI.Wrapper.GetText(MsgCode.NothingSelected));
                }
                else {
                    DI.Wrapper.BLE_DeviceInfoGathered += deviceInfoGathered;
                    this.ui.IsBusy = true;
                    DI.Wrapper.BLE_GetInfo(this.selectedDevice);
                }
            }
        }


        private void OnUiSettings(object sender, EventArgs e) {
            App.ShowMsgTitle("Bluetooth", "TBD");
        }

        #endregion

    }
}
