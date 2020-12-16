using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.UserControls;
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


        public WifiRun(Window parent) {
            this.parent = parent;
            InitializeComponent();

            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;


            this.ui.ExitClicked+= this.OnUiExit;
            this.ui.ConnectCicked += this.OnUiConnect;
            this.ui.DiscoverClicked += this.OnUiDiscover;
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
            this.ui.OnLoad(this.parent, new RunPageCtrlsEnabled() {
                Info = false,
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
            DI.Wrapper.OnWifiError -= this.onWifiError;
            DI.Wrapper.OnWifiConnectionAttemptCompleted -= this.onWifiConnectionAttemptCompleted;
            DI.Wrapper.Wifi_BytesReceived -= this.bytesReceived;

            this.ui.OnClosing();
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
            this.Close();
        }

        private void OnUiDiscover(object sender, EventArgs e) {
            this.Title = "WIFI";
            this.selectedWifi = WifiSelect.ShowBox(this);
            if (this.selectedWifi != null) {
                this.Title = this.selectedWifi.SSID;
            }
        }


        private void OnUiConnect(object sender, EventArgs e) {
            if (this.selectedWifi == null) {
                this.OnUiDiscover(sender, e);
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
            App.ShowMsgTitle(DI.Wrapper.GetText(MsgCode.Ethernet), "TBD");
        }


        private void OnUiSettings(object sender, EventArgs e) {
            WifiCredentialsWin.ShowBox(this);
        }

        #endregion

    }
}
