using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using MultiCommData.Net.Enumerations;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.WifiWins;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using WifiCommon.Net.DataModels;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for WifiRun.xaml</summary>
    public partial class WifiRun : Window {

        private Window parent = null;
        private WifiNetworkInfo selectedWifi = null;

        public static int Instances { get; private set; }

        public event EventHandler<Type> CloseRequest;


        public WifiRun(Window parent) {
            this.parent = parent;
            Instances++;
            InitializeComponent();

            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;


            this.ui.ExitClicked+= this.OnUiExit;
            this.ui.ConnectCicked += this.OnUiConnect;
            this.ui.DisconnectClicked += this.OnUiDisconnect;
            this.ui.SendClicked += this.OnUiSend;
            this.ui.InfoClicked += this.OnUiInfo;
            this.ui.SettingsClicked += this.OnUiSettings;
            DI.Wrapper.OnWifiError += this.onWifiError;
            DI.Wrapper.OnWifiConnectionAttemptCompleted += this.onWifiConnectionAttemptCompleted;
            DI.Wrapper.Wifi_BytesReceived += this.bytesReceived;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.ui.OnLoad(this, CommMedium.Wifi);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.ui.ExitClicked -= this.OnUiExit;
            this.ui.ConnectCicked -= this.OnUiConnect;
            this.ui.DisconnectClicked -= this.OnUiDisconnect;
            this.ui.SendClicked -= this.OnUiSend;
            this.ui.InfoClicked -= this.OnUiInfo;
            this.ui.SettingsClicked -= this.OnUiSettings;
            DI.Wrapper.OnWifiError -= this.onWifiError;
            DI.Wrapper.OnWifiConnectionAttemptCompleted -= this.onWifiConnectionAttemptCompleted;
            DI.Wrapper.Wifi_BytesReceived -= this.bytesReceived;

            this.ui.OnClosing();
            Instances--;
        }


        #region Wifi DI event handlers

        private void bytesReceived(object sender, string msg) {
            this.ui.AddResponse(msg);
        }


        private void onWifiConnectionAttemptCompleted(object sender, MsgPumpResults e) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                if (e.Code == MsgPumpResultCode.Connected) {
                    this.ui.SetConnected();
                }
                else {
                    App.ShowMsg(string.Format("{0} '{1}", e.Code, e.ErrorString));
                }
            });
        }


        private void onWifiError(object sender, WifiError e) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy =false;
                App.ShowMsg(string.Format("{0} '{1}'", e.Code, e.ExtraInfo));
            });
        }

        #endregion

        #region UI Event handlers

        private void OnUiExit(object sender, EventArgs e) {
            this.CloseRequest?.Invoke(this, typeof(WifiRun));
        }


        private void OnUiConnect(object sender, EventArgs e) {
            if (this.selectedWifi == null) {
                this.DoDiscovery();
            }
            if (this.selectedWifi != null) {
                this.ui.IsBusy = true;
                DI.Wrapper.WifiConnectAsync(this.selectedWifi);
            }
        }


        private void OnUiDisconnect(object sender, EventArgs e) {
            DI.Wrapper.WifiDisconect();
        }


        private void OnUiSend(object sender, string msg) {
            DI.Wrapper.WifiSend(msg);
        }


        private void OnUiInfo(object sender, EventArgs e) {
            //if (this.selectedWifi == null) {
            //    this.DoDiscovery(sender, e);
            //}
            if (this.selectedWifi != null) {
                WifiInfo.ShowBox(this, this.selectedWifi);
            }
        }


        private void OnUiSettings(object sender, EventArgs e) {
            WifiCredentialsWin.ShowBox(this);
        }

        #endregion

        #region Private

        private void DoDiscovery() {
            this.Title = "WIFI";
            this.selectedWifi = WifiSelect.ShowBox(this);
            if (this.selectedWifi != null) {
                this.Title = this.selectedWifi.SSID;
            }
        }

        #endregion

    }
}
