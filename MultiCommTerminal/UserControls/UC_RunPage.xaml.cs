﻿using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.UserControls {

    /// <summary>
    /// Interaction logic for UC_RunPage.xaml
    /// </summary>
    public partial class UC_RunPage : UserControl {
        #region Data

        private ClassLog log = new ClassLog("UC_RunPage");
        private ScrollViewer inScroll = null;
        private ScrollViewer logScroll = null;
        private Window parent = null;
        private ButtonGroupSizeSyncManager buttonSizer = null;
        private List<ScriptItem> scriptItems = new List<ScriptItem>();

        // TODO size buttons

        #endregion

        #region Events

        public EventHandler DiscoverClicked;
        public EventHandler ExitClicked;
        public EventHandler ConnectCicked;
        public EventHandler DisconnectClicked;
        public EventHandler<string> SendClicked;

        #endregion

        public UC_RunPage() {
            InitializeComponent();
            this.buttonSizer = new ButtonGroupSizeSyncManager(
                this.btnBTDiscover, this.btnConnect, this.btnDisconnect, this.btnExit, this.btnLog);
            this.buttonSizer.PrepForChange();
        }



        /// <summary>Do any initialization here. Should be called by window at load</summary>
        public void OnLoad(Window parent) {
            this.parent = parent;
            this.inScroll = this.lbIncoming.GetScrollViewer();
            this.logScroll = this.lbLog.GetScrollViewer();
            this.lbLog.Collapse();

            DI.Wrapper.CurrentSupportedLanguage(this.SetLanguage);
            DI.Wrapper.GetCurrentTerminator(this.SetTerminators, App.ShowMsg);
            DI.Wrapper.GetCurrentScript(this.PopulateScriptData, App.ShowMsg);

            DI.Wrapper.LanguageChanged += this.languageChangedHandler;
            DI.Wrapper.CurrentTerminatorChanged += this.currentTerminatorChangedHandler;
            DI.Wrapper.CurrentScriptChanged += this.currentScriptChanged;
            App.STATIC_APP.LogMsgEvent += this.AppLogMsgEventHandler;
        }



        /// <summary>Do any teardown here. Should be called by window at closing</summary>
        public void OnClosing() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
            DI.Wrapper.CurrentTerminatorChanged -= this.currentTerminatorChangedHandler;
            App.STATIC_APP.LogMsgEvent -= this.AppLogMsgEventHandler;
            DI.Wrapper.CurrentScriptChanged -= this.currentScriptChanged;
            this.buttonSizer.Teardown();
        }


        public void AddResponse(string response) {
            lock (this.lbIncoming) {
                this.lbIncoming.AddAndScroll(response, this.inScroll, 100);
            }
        }


        public bool IsBusy {
            get { return this.gridWait.Visibility == Visibility.Visible; }
            set {
                if (value) {
                    this.gridWait.Show();
                }
                else {
                    this.gridWait.Collapse();
                }
            }
        }



        #region Button events

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(this.txtCommmand.Text)) {
                this.SendClicked?.Invoke(this, this.txtCommmand.Text);
            }
        }


        private void btnBTDiscover_Click(object sender, RoutedEventArgs e) {
            this.ClearResponses();
            this.DiscoverClicked?.Invoke(this, new EventArgs());
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            this.ConnectCicked?.Invoke(this, new EventArgs());
        }


        private void btnDisconnect_Click(object sender, RoutedEventArgs e) {
            this.DisconnectClicked?.Invoke(this, new EventArgs());
        }


        private void btnLog_Click(object sender, RoutedEventArgs e) {
            this.lbLog.ToggleVisibility();
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.ExitClicked?.Invoke(this, new EventArgs());
        }

        #endregion


        private void brdResponse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            this.ClearResponses();
        }


        private void brdCommands_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            CommandsPopup.ShowBox(this.parent);
        }


        private void terminatorView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            TerminatorDataSelectorPopup.ShowBox(this.parent);
        }


        private void outgoing_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.outgoing.GetSelected<ScriptItem>((item) => {
                this.txtCommmand.Text = item.Command;
            });
        }

        #region Wrapper event handlers

        private void languageChangedHandler(object sender, SupportedLanguage language) {
            this.SetLanguage(language);
        }


        private void currentTerminatorChangedHandler(object sender, TerminatorDataModel terminators) {
            this.SetTerminators(terminators);
        }


        private void AppLogMsgEventHandler(object sender, string msg) {
            // Race condition with messages coming before window rendered
            try {
                if (this.logScroll != null) {
                    this.lbLog.AddAndScroll(msg, this.logScroll, 400);
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region Delegates

        private void SetLanguage(SupportedLanguage l) {
            this.lbResponse.Content = l.GetText(MsgCode.response);
            this.btnSend.Content = l.GetText(MsgCode.send);
            this.btnBTDiscover.Content = l.GetText(MsgCode.discover);
            this.btnConnect.Content = l.GetText(MsgCode.connect);
            this.btnDisconnect.Content = l.GetText(MsgCode.Disconnect);
            this.btnLog.Content = l.GetText(MsgCode.Log);
            this.btnExit.Content = l.GetText(MsgCode.exit);
        }

        private void SetTerminators(TerminatorDataModel data) {
            this.terminatorView.Initialise(data);
        }


        private void PopulateScriptData(ScriptDataModel dataModel) {
            this.outgoing.Clear(this.scriptItems);
            this.outgoing.Add(this.scriptItems, dataModel.Items);
            this.lbCommandListName.Content = dataModel.Display;
            this.txtCommmand.Text = "";
        }


        private void currentScriptChanged(object sender, ScriptDataModel data) {
            this.PopulateScriptData(data);
        }


        #endregion



        private void ClearResponses() {
            lock (this.lbIncoming) {
                this.lbIncoming.Items.Clear();
            }
        }

    }
}