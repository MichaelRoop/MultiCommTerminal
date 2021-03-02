using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.UserControls {

    /// <summary>Interaction logic for common UC_RunPage.xaml user control</summary>
    public partial class UC_RunPage : UserControl {

        #region Data

        private ClassLog log = new ClassLog("UC_RunPage");
        private ScrollViewer inScroll = null;
        private ScrollViewer logScroll = null;
        private Window parent = null;
        private ButtonGroupSizeSyncManager buttonSizer = null;
        private List<ScriptItem> scriptItems = new List<ScriptItem>();
        bool isBusy = false;
        CommMedium medium = CommMedium.None;

        // TODO size buttons

        #endregion

        #region Events

        public EventHandler ExitClicked;
        public EventHandler ConnectCicked;
        public EventHandler DisconnectClicked;
        public EventHandler<string> SendClicked;
        public EventHandler InfoClicked;
        public EventHandler SettingsClicked;

        #endregion

        #region Properties

        public bool IsBusy {
            get { lock (this) { return this.isBusy; } }
            set {
                lock (this) {
                    this.isBusy = value;
                    this.Dispatcher.Invoke(() => {
                        if (this.isBusy) {
                            this.gridWait.Show();
                        }
                        else {
                            this.gridWait.Collapse();
                        }
                    });
                }
            }
        }

        #endregion

        #region Constructors and window events

        /// <summary>Default constructor required for designer load</summary>
        public UC_RunPage() {
            InitializeComponent();
            this.buttonSizer = new ButtonGroupSizeSyncManager(
                this.btnConnect, this.btnDisconnect, this.btnExit, this.btnLog);
            this.buttonSizer.PrepForChange();
        }


        /// <summary>Do any initialization here. Should be called by window at load</summary>
        public void OnLoad(Window parent, CommMedium medium, RunPageCtrlsEnabled enableList = null) {
            this.parent = parent;
            this.medium = medium;
            // TODO - remove the enable list
            if (enableList != null) {
                this.btnInfo.SetVisualEnabled(enableList.Info);
                this.btnSettings.SetVisualEnabled(enableList.Settings);
                this.btnConnect.SetVisualEnabled(enableList.Connect);
                this.connectedOff.SetVisualEnabled(enableList.Connect);
                this.connectedOn.SetVisualEnabled(enableList.Connect);
                this.btnDisconnect.SetVisualEnabled(enableList.Disconnect);
                this.btnSend.SetVisualEnabled(enableList.Send);
            }

            this.inScroll = this.lbIncoming.GetScrollViewer();
            this.logScroll = this.lbLog.GetScrollViewer();
            this.logSection.Collapse();
            this.SetConnectState(false);

            this.AddEventHandlers();
            DI.Wrapper.CurrentSupportedLanguage(this.SetLanguage);
            DI.Wrapper.GetCurrentTerminator(this.medium, this.SetTerminators, App.ShowMsg);
            DI.Wrapper.GetCurrentScript(this.medium, this.PopulateScriptData, App.ShowMsg);
        }



        /// <summary>Do any teardown here. Should be called by window at closing</summary>
        public void OnClosing() {
            this.RemoveEventHandlers();
            this.buttonSizer.Teardown();
            this.DisconnectClicked?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Public

        public void AddResponse(string response) {
            this.Dispatcher.Invoke(() => {
                lock (this.lbIncoming) {
                    this.lbIncoming.AddAndScroll(response, this.inScroll, 100);
                }
            });
        }


        public void SetConnected() {
            this.IsBusy = false;
            this.SetConnectState(true);
        }

        #endregion

        #region Button events

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(this.txtCommmand.Text)) {
                this.SendClicked?.Invoke(this, this.txtCommmand.Text);
            }
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            this.SetConnectState(false);
            this.ConnectCicked?.Invoke(this, new EventArgs());
        }


        private void btnDisconnect_Click(object sender, RoutedEventArgs e) {
            this.SetConnectState(false);
            this.DisconnectClicked?.Invoke(this, new EventArgs());
        }


        private void btnLog_Click(object sender, RoutedEventArgs e) {
            this.logSection.ToggleVisibility();
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            // Make sure that the device is disconnected
            btnDisconnect_Click(sender, e);
            this.ExitClicked?.Invoke(this, new EventArgs());
        }


        private void btnSettings_Click(object sender, RoutedEventArgs e) {
            this.SettingsClicked?.Invoke(this, new EventArgs());
        }


        private void btnInfo_Click(object sender, RoutedEventArgs e) {
            this.InfoClicked?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Other UI controls events

        private void brdResponse_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            this.ClearResponses();
        }


        private void brdCommands_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            CommandsPopup.ShowBox(this.parent, this.medium);
        }


        private void terminatorView_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
            TerminatorDataSelectorPopup.ShowBox(this.parent, this.medium);
        }


        private void outgoing_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.outgoing.GetSelected<ScriptItem>((item) => {
                this.txtCommmand.Text = item.Command;
            });
        }


        private void btnCopyLog_Click(object sender, RoutedEventArgs e) {
            try {
                lock (this.lbLog) {
                    StringBuilder sb = new StringBuilder();
                    this.lbLog.SelectAll();
                    foreach (object row in lbLog.SelectedItems) {
                        sb.AppendLine(row.ToString());
                    }
                    //sb.Remove(sb.Length - 1, 1); // Just to avoid copying last empty row
                    Clipboard.SetText(sb.ToString());
                    this.lbLog.UnselectAll();
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "btnCopyLog_Click", "", ex);
            }
        }


        private void btnClearLog_Click(object sender, RoutedEventArgs e) {
            lock (this.lbLog) {
                try {
                    this.lbLog.Items.Clear();
                }
                catch(Exception ex) {
                    this.log.Exception(9999, "btnClearLog_Click", "", ex);
                }
            }
        }


        #endregion

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
                lock (this.lbLog) {
                    if (this.logScroll != null) {
                        this.lbLog.AddAndScroll(msg, this.logScroll, 400);
                    }
                }
            }
            catch (Exception) { }
        }

        #endregion

        #region Delegates

        private void SetLanguage(SupportedLanguage l) {
            this.buttonSizer.PrepForChange();
            this.InvalidateVisual();
            this.lbResponse.Content = l.GetText(MsgCode.response);
            this.btnSend.Content = l.GetText(MsgCode.send);
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
            this.lbCommandListName.Text = dataModel.Display;
            this.txtCommmand.Text = "";
        }


        private void currentScriptChanged(object sender, ScriptDataModel data) {
            this.PopulateScriptData(data);
        }

        #endregion

        #region Private

        private void SetConnectState(bool isConnected) {
            this.Dispatcher.Invoke(() => {
                if (isConnected) {
                    this.btnConnect.Collapse();
                    this.btnDisconnect.Show();
                    this.btnInfo.Enable();
                    this.connectedOff.Collapse();
                    this.connectedOn.Show();
                }
                else {
                    this.ClearResponses();
                    this.btnInfo.Disable();
                    this.btnDisconnect.Collapse();
                    this.btnConnect.Show();
                    this.connectedOn.Collapse();
                    this.connectedOff.Show();
                }
            });
        }


        private void ClearResponses() {
            lock (this.lbIncoming) {
                this.lbIncoming.Items.Clear();
            }
        }


        private void AddEventHandlers() {
            DI.Wrapper.LanguageChanged += this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent += this.AppLogMsgEventHandler;
            
            switch (this.medium) {
                case CommMedium.Bluetooth:
                    DI.Wrapper.CurrentTerminatorChangedBT += this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedBT += this.currentScriptChanged;
                    break;
                case CommMedium.BluetoothLE:
                    DI.Wrapper.CurrentTerminatorChangedBLE += this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedBLE += this.currentScriptChanged;
                    break;
                case CommMedium.Ethernet:
                    DI.Wrapper.CurrentTerminatorChangedEthernet += this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedEthernet += this.currentScriptChanged;
                    break;
                case CommMedium.Usb:
                    DI.Wrapper.CurrentTerminatorChangedUSB += this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedUSB += this.currentScriptChanged;
                    break;
                case CommMedium.Wifi:
                    DI.Wrapper.CurrentTerminatorChangedWIFI += this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedWIFI += this.currentScriptChanged;
                    break;
                default:
                    DI.Wrapper.CurrentTerminatorChanged += this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChanged += this.currentScriptChanged;
                    break;
            }
        }



        private void RemoveEventHandlers() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent -= this.AppLogMsgEventHandler;

            switch (this.medium) {
                case CommMedium.Bluetooth:
                    DI.Wrapper.CurrentTerminatorChangedBT -= this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedBT -= this.currentScriptChanged;
                    break;
                case CommMedium.BluetoothLE:
                    DI.Wrapper.CurrentTerminatorChangedBLE -= this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedBLE -= this.currentScriptChanged;
                    break;
                case CommMedium.Ethernet:
                    DI.Wrapper.CurrentTerminatorChangedEthernet -= this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedEthernet -= this.currentScriptChanged;
                    break;
                case CommMedium.Usb:
                    DI.Wrapper.CurrentTerminatorChangedUSB -= this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedUSB -= this.currentScriptChanged;
                    break;
                case CommMedium.Wifi:
                    DI.Wrapper.CurrentTerminatorChangedWIFI -= this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChangedWIFI -= this.currentScriptChanged;
                    break;
                default:
                    DI.Wrapper.CurrentTerminatorChanged -= this.currentTerminatorChangedHandler;
                    DI.Wrapper.CurrentScriptChanged -= this.currentScriptChanged;
                    break;
            }
        }


        #endregion

    }
}
