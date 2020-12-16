﻿using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using Ethernet.Common.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for EthernetRun.xaml</summary>
    public partial class EthernetRun : Window {

        private Window parent = null;
        private EthernetSelectResult selectedEthernet = null;

        #region Constructors and windows events

        public EthernetRun(Window parent) {
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
            DI.Wrapper.EthernetParamsRequestedEvent += this.paramsRequestedEventHandler;
            DI.Wrapper.Ethernet_BytesReceived += this.bytesReceivedHandler;
            DI.Wrapper.OnEthernetError += this.onEthernetError;
            DI.Wrapper.OnEthernetListChange += this.onEthernetListChange;
            DI.Wrapper.OnEthernetConnectionAttemptCompleted += this.onEthernetConnectionAttemptCompleted;
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
            DI.Wrapper.EthernetParamsRequestedEvent -= this.paramsRequestedEventHandler;
            DI.Wrapper.Ethernet_BytesReceived -= this.bytesReceivedHandler;
            DI.Wrapper.OnEthernetError -= this.onEthernetError;
            DI.Wrapper.OnEthernetListChange -= this.onEthernetListChange;
            DI.Wrapper.OnEthernetConnectionAttemptCompleted -= this.onEthernetConnectionAttemptCompleted;
            this.ui.OnClosing();
        }

        #endregion

        #region DI event wrappers
        
        private void bytesReceivedHandler(object sender, string msg) {
            this.ui.AddResponse(msg);
        }


        private void onEthernetListChange(object sender, List<StorageFactory.Net.interfaces.IIndexItem<StorageFactory.Net.StorageManagers.DefaultFileExtraInfo>> e) {
            // Irrelevant at this level since we only work with the current selected connection
        }


        private void onEthernetError(object sender, CommunicationStack.Net.DataModels.MsgPumpResults e) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                App.ShowMsg(string.Format("{0} '{1}'", e.Code, e.ErrorString));
            });
        }


        private void paramsRequestedEventHandler(object sender, EthernetParams e) {
            EthernetEditRequest.ShowBox(this, this.selectedEthernet);
        }


        private void onEthernetConnectionAttemptCompleted(object sender, MsgPumpResults e) {
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

        #endregion

        #region UI Event handlers

        private void OnUiExit(object sender, EventArgs e) {
            this.Close();
        }


        private void OnUiDiscover(object sender, EventArgs e) {
            this.Title = DI.Wrapper.GetText(MsgCode.Ethernet);
            this.selectedEthernet = EthernetSelect.ShowBox(this);
            if (this.selectedEthernet != null) {
                this.Title = this.selectedEthernet.DataModel.DisplayString;
            }
            else {
                // No entries. Create one
                DeviceEdit_Ethernet.ShowBoxAdd(this, null);
            }
        }


        private void OnUiConnect(object sender, EventArgs e) {
            if (this.selectedEthernet == null) {
                this.OnUiDiscover(sender, e);
            }
            if (this.selectedEthernet != null) {
                this.ui.IsBusy = true;
                DI.Wrapper.EthernetConnectAsync(this.selectedEthernet.DataModel);
            }
        }


        private void OnUiDisconnect(object sender, EventArgs e) {
            DI.Wrapper.EthernetDisconnect();
        }


        private void OnUiSend(object sender, string msg) {
            DI.Wrapper.EthernetSend(msg);
        }


        private void OnUiInfo(object sender, EventArgs e) {
            App.ShowMsgTitle(DI.Wrapper.GetText(MsgCode.Ethernet), "TBD");
        }


        private void OnUiSettings(object sender, EventArgs e) {
            DeviceSelect_Ethernet.ShowBox(this);
        }

        #endregion

    }
}