using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.Enumerations;
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

    /// <summary>Interaction logic for UC_BLESerial.xaml</summary>
    public partial class UC_BLESerial : UserControl {

        #region Data

        private ClassLog log = new ClassLog("UC_BLESerial");
        private ScrollViewer inScroll = null;
        private ScrollViewer logScroll = null;
        private Window parent = null;
        private ButtonGroupSizeSyncManager buttonSizer = null;
        private List<ScriptItem> scriptItems = new List<ScriptItem>();
        CommMedium medium = CommMedium.BluetoothLE;
        List<BLE_CharacteristicDataModel> computerToBLE = new List<BLE_CharacteristicDataModel>();
        List<BLE_CharacteristicDataModel> bleToComputer = new List<BLE_CharacteristicDataModel>();
        bool isBusy = false;

        #endregion

        #region Events

        public EventHandler ConnectCicked;
        public EventHandler<string> SendClicked;
        public EventHandler InfoClicked;
        public EventHandler SelectComputerEmittingClicked;
        public EventHandler SelectComputerReceivingClicked;
        public EventHandler ExitClicked;

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
        public UC_BLESerial() {
            InitializeComponent();
            this.lbCharacteristicBleToComputer.ItemsSource = this.bleToComputer;
            this.lbCharacteristicComputerToBle.ItemsSource = this.computerToBLE;
            this.buttonSizer = new ButtonGroupSizeSyncManager(
                this.btnConnect, this.btnExit, this.btnLog);
            this.buttonSizer.PrepForChange();
        }

        /// <summary>Do any initialization here. Should be called by window at load</summary>
        public void OnLoad(Window parent) {
            this.parent = parent;
            this.inScroll = this.lbResponse.GetScrollViewer();
            this.logScroll = this.lbLog.GetScrollViewer();
            this.lbLog.Collapse();

            this.AddEventHandlers();
            DI.Wrapper.CurrentSupportedLanguage(this.SetLanguage);
            DI.Wrapper.GetCurrentTerminator(this.medium, this.SetTerminators, App.ShowMsg);
            DI.Wrapper.GetCurrentScript(this.medium, this.PopulateScriptData, App.ShowMsg);
        }


        /// <summary>Do any teardown here. Should be called by window at closing</summary>
        public void OnClosing() {
            this.RemoveEventHandlers();
            this.buttonSizer.Teardown();
        }

        #endregion

        #region Public

        public void AddResponse(string response) {
            this.Dispatcher.Invoke(() => {
                lock (this.lbResponse) {
                    this.lbResponse.AddAndScroll(response, this.inScroll, 100);
                }
            });
        }


        public void SetConnected() {
            this.IsBusy = false;
            this.SetConnectState(true);
        }


        public void SetComputerToBLE(BLE_CharacteristicDataModel characteristic) {
            this.ClearComputerToBle();
            this.computerToBLE.Add(characteristic);
            this.lbCharacteristicComputerToBle.ItemsSource = this.computerToBLE;
        }


        public void SetBLEToComputer(BLE_CharacteristicDataModel characteristic) {
            this.ClearBleToComputer();
            characteristic.OnReadValueChanged += this.onBleReadValueChanged;
            this.bleToComputer.Add(characteristic);
        }


        /// <summary>Message with terminator</summary>
        /// <param name="msg">The terminated message to send</param>
        public void SendMessage(string msg) {
            if (this.computerToBLE.Count > 0) {
                this.computerToBLE[0].CharValue = msg;
            }
        }

        #endregion

        #region Button events

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            if (!string.IsNullOrWhiteSpace(this.txtCommmand.Text)) {
                this.SendClicked?.Invoke(this, this.txtCommmand.Text);
                // The receiver will add the terminators and call SendMessage(string)
            }
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            this.SetConnectState(false);
            this.ConnectCicked?.Invoke(this, new EventArgs());
        }


        private void btnLog_Click(object sender, RoutedEventArgs e) {
            this.lbLog.ToggleVisibility();
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            // TODO Make sure that the device is disconnected
            this.ExitClicked?.Invoke(this, new EventArgs());
        }


        private void btnInfo_Click(object sender, RoutedEventArgs e) {
            this.InfoClicked?.Invoke(this, new EventArgs());
        }

        private void btnClear_Click(object sender, RoutedEventArgs e) {
            this.ClearResponses();
        }

        private void btnSelectEmitting_Click(object sender, RoutedEventArgs e) {
            // Only clear on reception
            this.SelectComputerEmittingClicked?.Invoke(this, new EventArgs());
        }

        private void btnSelectReceiving_Click(object sender, RoutedEventArgs e) {
            // Only clear on reception
            this.SelectComputerReceivingClicked?.Invoke(this, new EventArgs());
        }

        #endregion

        #region Other controls events

        private void Commands_MouseLeftButtonUp(object sender, MouseButtonEventArgs e) {
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

        #endregion

        #region DI handlers

        private void AddEventHandlers() {
            DI.Wrapper.LanguageChanged += this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent += this.AppLogMsgEventHandler;
            DI.Wrapper.CurrentTerminatorChangedBLE += this.currentTerminatorChangedHandler;
            DI.Wrapper.CurrentScriptChangedBLE += this.currentScriptChanged;
        }


        private void RemoveEventHandlers() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent -= this.AppLogMsgEventHandler;
            DI.Wrapper.CurrentTerminatorChangedBLE -= this.currentTerminatorChangedHandler;
            DI.Wrapper.CurrentScriptChangedBLE -= this.currentScriptChanged;
            this.ClearBleToComputer();
        }


        private void SetLanguage(SupportedLanguage l) {
            this.btnSend.Content = l.GetText(MsgCode.send);
            this.btnConnect.Content = l.GetText(MsgCode.connect);
            this.btnLog.Content = l.GetText(MsgCode.Log);
            this.btnExit.Content = l.GetText(MsgCode.exit);
            this.btnSelectEmitting.Content = l.GetText(MsgCode.select);
            this.btnSelectReceiving.Content = l.GetText(MsgCode.select);
            this.btnClear.Content = l.GetText(MsgCode.Clear);
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


        /// <summary>Handle characteristic event directly</summary>
        /// <param name="sender">Sender of event</param>
        /// <param name="result">Object with result and data payload</param>
        private void onBleReadValueChanged(object sender, BLE_CharacteristicReadResult result) {
            this.Dispatcher.Invoke(() => {
                try {
                    if (result.Status == BLE_CharacteristicCommunicationStatus.Success) {
                        if (this.bleToComputer.Count > 0) {
                            // TODO this will show one part at a time for longer messages
                            // Need to decide if we just list them in the response list and get 
                            // the complete later on. This goes in both characteristic and response
                            this.lbCharacteristicBleToComputer.ItemsSource = null;
                            this.lbCharacteristicBleToComputer.ItemsSource = this.bleToComputer;
                            this.AddResponse(this.bleToComputer[0].CharValue);
                        }
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "onBleReadValueChanged", "", e);
                }
            });
        }

        #endregion

        #region Private

        private void SetConnectState(bool isConnected) {
            this.Dispatcher.Invoke(() => {
                if (isConnected) {
                    this.connectedOff.Collapse();
                    this.connectedOn.Show();
                }
                else {
                    this.connectedOn.Collapse();
                    this.connectedOff.Show();
                }

                this.ClearResponses();
                this.ClearBleToComputer();
                this.ClearComputerToBle();
            });
        }


        private void ClearResponses() {
            lock (this.lbResponse) {
                this.lbResponse.Items.Clear();
            }
        }


        private void ClearBleToComputer() {
            this.lbCharacteristicBleToComputer.ItemsSource = null;
            if (this.bleToComputer.Count > 0) {
                this.bleToComputer[0].OnReadValueChanged -= this.onBleReadValueChanged;
                this.bleToComputer.Clear();
            }
        }


        private void ClearComputerToBle() {
            this.lbCharacteristicComputerToBle.ItemsSource = null;
            this.computerToBLE.Clear();
        }

        #endregion

    }
}
