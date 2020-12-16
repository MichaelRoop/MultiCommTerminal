﻿using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BTWins {

    /// <summary>Interaction logic for BTRun.xaml</summary>
    public partial class BTRun : Window {

        private Window parent = null;
        private BTDeviceInfo selectedDevice = null;


        public BTRun(Window parent) {
            this.parent = parent;
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
            DI.Wrapper.BT_ConnectionCompleted += this.connectionCompleted;
            DI.Wrapper.BT_BytesReceived += this.bytesReceived;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.ui.OnLoad(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.ui.ExitClicked -= this.OnUiExit;
            this.ui.ConnectCicked -= this.OnUiConnect;
            this.ui.DiscoverClicked -= this.OnUiDiscover;
            this.ui.DisconnectClicked -= this.OnUiDisconnect;
            this.ui.SendClicked -= this.OnUiSend;
            this.ui.InfoClicked -= this.OnUiInfo;
            this.ui.SettingsClicked -= this.OnUiSettings;
            DI.Wrapper.BT_ConnectionCompleted -= this.connectionCompleted;
            DI.Wrapper.BT_BytesReceived -= this.bytesReceived;
            DI.Wrapper.BT_DeviceInfoGathered -= deviceInfoGathered;
            this.ui.OnClosing();
        }


        #region Wrapper events

        private void bytesReceived(object sender, string msg) {
            this.ui.AddResponse(msg);
        }


        private void connectionCompleted(object sender, bool ok) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                if (ok) {
                    this.ui.SetConnected();
                }
                else {
                    App.ShowMsg(DI.Wrapper.GetText(MsgCode.ConnectionFailure));
                }
            });
        }

        #endregion

        #region UI Event handlers

        private void OnUiExit(object sender, EventArgs e) {
            this.Close();
        }

        private void OnUiDiscover(object sender, EventArgs e) {
            this.Title = "Bluetooth";
            this.selectedDevice = BTSelect.ShowBox(this, true);
            if (this.selectedDevice != null) {
                this.Title = this.selectedDevice.Name;
            }
        }


        private void OnUiConnect(object sender, EventArgs e) {
            if (this.selectedDevice == null) {
                this.OnUiDiscover(sender, e);
            }
            if (this.selectedDevice != null) {
                this.ui.IsBusy = true;
                DI.Wrapper.BTClassicConnectAsync(this.selectedDevice);
            }
        }


        private void OnUiDisconnect(object sender, EventArgs e) {
            DI.Wrapper.BTClassicDisconnect();
        }


        private void OnUiSend(object sender, string msg) {
            DI.Wrapper.BTClassicSend(msg);
        }


        private void OnUiInfo(object sender, EventArgs e) {
            if (this.selectedDevice != null) {
                DI.Wrapper.BT_DeviceInfoGathered += deviceInfoGathered;
                this.ui.IsBusy = true;
                DI.Wrapper.BTClassicGetExtraInfoAsync(this.selectedDevice);
            }
            else {
                this.OnUiDiscover(sender, e);
                if (this.selectedDevice == null) {
                    App.ShowMsg(DI.Wrapper.GetText(MsgCode.NothingSelected));
                }
                else {
                    DI.Wrapper.BT_DeviceInfoGathered += deviceInfoGathered;
                    this.ui.IsBusy = true;
                    DI.Wrapper.BTClassicGetExtraInfoAsync(this.selectedDevice);
                }
            }
        }


        private void OnUiSettings(object sender, EventArgs e) {
            BTSettings.ShowBox(this);
        }


        private void deviceInfoGathered(object sender, BTDeviceInfo e) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                DI.Wrapper.BT_DeviceInfoGathered -= deviceInfoGathered;
                DeviceInfo_BT win = new DeviceInfo_BT(this, e);
                win.ShowDialog();
            });
        }

        #endregion
    }
}
