using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLE_Full.xaml</summary>
    public partial class BLE_Full : Window {

        #region Data

        private ClassLog log = new ClassLog("BLE_Full");
        private Window parent = null;
        private ButtonGroupSizeSyncManager buttonSizer = null;
        private ScrollViewer logScroll = null;
        BluetoothLEDeviceInfo currentDevice = null;
        public static int Instances { get; private set; } 
        bool isBusy = false;
        DispatcherTimer timer = null;


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

        #region Constructors and Window events

        public BLE_Full(Window parent) {
            Instances++;
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.buttonSizer = new ButtonGroupSizeSyncManager(
                this.btnConnect, this.btnExit, this.btnLog);
            this.buttonSizer.PrepForChange();
            this.timer = new DispatcherTimer(DispatcherPriority.Normal);
            this.timer.Interval = TimeSpan.FromMilliseconds(500);
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.logScroll = this.lbLog.GetScrollViewer();
            this.logSection.Collapse();
            this.AddEventHandlers();
            this.timer.Start();
            this.writeControl.OnStartup(this.parent);
            DI.Wrapper.CurrentSupportedLanguage(this.SetLanguage);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.timer.Stop();
            this.RemoveEventHandlers();
            this.writeControl.OnShutdown();
            this.buttonSizer.Teardown();
            DI.Wrapper.BLE_Disconnect();
            Instances--;
        }

        #endregion

        #region button handlers

        private void btnSend_Click(object sender, RoutedEventArgs e) {

        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void btnLog_Click(object sender, RoutedEventArgs e) {
            this.logSection.ToggleVisibility();
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            this.SetConnectState(false);
            this.writeControl.Reset();
            var device = BLESelect.ShowBox(this.parent, true);
            if (device != null) {
                this.IsBusy = true;
                DI.Wrapper.BLE_DeviceConnectResult += this.DeviceConnectResultHandler;
                DI.Wrapper.BLE_ConnectAsync(device);
            }
        }


        private void btnCopyLog_Click(object sender, RoutedEventArgs e) {
            try {
                lock (this.lbLog) {
                    StringBuilder sb = new StringBuilder();
                    this.lbLog.SelectAll();
                    foreach (var item in lbLog.SelectedItems) {
                        sb.AppendLine(item.ToString());
                    }
                    Clipboard.SetText(sb.ToString());
                    this.lbLog.SelectedItem = null;
                    this.lbLog.UnselectAll();
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "btnCopyLog_Click", "", ex);
            }
        }

        private void btnClearLog_Click(object sender, RoutedEventArgs e) {
            try {
                lock (this.lbLog) {
                    if (this.logScroll != null) {
                        this.lbLog.Items.Clear();
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "btnClearLog_Click", "", ex);
            }
        }

        #endregion

        #region Event handlers

        bool dataChanged = false;

        private void AddEventHandlers() {
            DI.Wrapper.LanguageChanged += this.languageChangedHandler;
            DI.Wrapper.BLE_CharacteristicReadValueChanged += this.characteristicReadValueChanged;
            this.timer.Tick += this.Timer_Tick;
            App.STATIC_APP.LogMsgEvent += this.AppLogMsgEventHandler;
        }


        private void RemoveEventHandlers() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
            App.STATIC_APP.LogMsgEvent -= this.AppLogMsgEventHandler;
            DI.Wrapper.BLE_DeviceConnectResult -= this.DeviceConnectResultHandler;
            DI.Wrapper.BLE_CharacteristicReadValueChanged -= this.characteristicReadValueChanged;
            DI.Wrapper.BLE_ConnectionStatusChanged -= this.connectionStatusChanged;
            this.timer.Tick -= this.Timer_Tick;
        }

        private void languageChangedHandler(object sender, SupportedLanguage language) {
            this.SetLanguage(language);
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


        private void DeviceConnectResultHandler(object sender, BLEGetInfoStatus info) {
            this.Dispatcher.Invoke(() => {
                this.IsBusy = false;
                DI.Wrapper.BLE_DeviceConnectResult -= this.DeviceConnectResultHandler;
                switch (info.Status) {
                    case BLEOperationStatus.Failed:
                    case BLEOperationStatus.UnhandledError:
                    case BLEOperationStatus.UnknownError:
                    case BLEOperationStatus.NotFound:
                    case BLEOperationStatus.NoServices:
                    case BLEOperationStatus.GetServicesFailed:
                        App.ShowMsg(info.Message);
                        break;
                    case BLEOperationStatus.Success:
                        this.SetConnectState(true);
                        this.currentDevice = info.DeviceInfo;
                        this.Title = string.Format("(BLE) {0}", this.currentDevice.Name);
                        this.treeServices.ItemsSource = this.currentDevice.Services;
                        this.PopulateWriteControl(this.currentDevice);
                        break;
                }
            });
        }


        private void PopulateWriteControl(BluetoothLEDeviceInfo device) {
            List<BLE_CharacteristicDataModel> list = new List<BLE_CharacteristicDataModel>();
            foreach (BLE_ServiceDataModel service in device.Services) {
                foreach (BLE_CharacteristicDataModel characteristic in service.Characteristics) {
                    if (characteristic.IsWritable) {
                        list.Add(characteristic);
                    }
                }
            }
            this.writeControl.SetCharacteristics(list);
        }



        private void connectionStatusChanged(object sender, BLE_ConnectStatusChangeInfo e) {
            this.Dispatcher.Invoke(() => {
                if (e.Status == BLE_ConnectStatus.Disconnected) {
                    this.SetConnectState(false);
                    App.ShowMsg(e.Message);
                }
            });
        }

        private void characteristicReadValueChanged(object sender, BLE_CharacteristicReadResult e) {
            this.Dispatcher.Invoke(() => {
                lock (this.treeServices) {
                    this.dataChanged = true;
                }
            });
        }


        #endregion

        #region Private

        private void SetConnectState(bool isConnected) {
            this.Dispatcher.Invoke(() => {
                this.writeControl.Connected = isConnected;
                if (isConnected) {
                    this.connectedOff.Collapse();
                    this.connectedOn.Show();
                    DI.Wrapper.BLE_ConnectionStatusChanged += this.connectionStatusChanged;
                }
                else {
                    this.connectedOn.Collapse();
                    this.connectedOff.Show();
                    this.treeServices.ItemsSource = null;
                    this.currentDevice = null;
                    DI.Wrapper.BLE_ConnectionStatusChanged -= this.connectionStatusChanged;
                    DI.Wrapper.BLE_Disconnect();
                }
            });
        }


        private void SetLanguage(SupportedLanguage l) {
            this.Dispatcher.Invoke(() => {
                this.buttonSizer.PrepForChange();
                this.InvalidateVisual();
                this.btnSend.Content = l.GetText(MsgCode.send);
                this.btnConnect.Content = l.GetText(MsgCode.connect);
                this.btnLog.Content = l.GetText(MsgCode.Log);
                this.btnExit.Content = l.GetText(MsgCode.exit);
                DI.Wrapper.Translate(this.currentDevice);
                this.dataChanged = true;
            });
        }


        private void Timer_Tick(object sender, EventArgs args) {
            lock (this.treeServices) {
                if (this.dataChanged) {
                    this.dataChanged = false;
                    try {
                        this.treeServices.RefreshAndExpand();
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "characteristicReadValueChanged", "", e);
                    }
                }
            }
        }

        #endregion

    }

}
