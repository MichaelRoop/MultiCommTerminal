using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Threading;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLE_Full.xaml</summary>
    public partial class BLE_Full : Window {

        #region Data

        private ClassLog log = new ClassLog("BLE_Full");
        private Window parent = null;
        private ButtonGroupSizeSyncManager buttonSizer = null;
        BluetoothLEDeviceInfo currentDevice = null;
        //BLE_CharacteristicDataModel currentCharacteristic = null;
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
            this.buttonSizer = new ButtonGroupSizeSyncManager(
                this.btnConnect, this.btnDisconnect, this.btnExit, this.btnLog, this.btnCommands);
            this.buttonSizer.PrepForChange();
            this.timer = new DispatcherTimer(DispatcherPriority.Normal);
            this.timer.Interval = TimeSpan.FromMilliseconds(500);
            //this.CenterToParent(this.parent);
            // TODO - set the title on commands
            //this.lblCmdDataTypeContent.Content = BLE_DataType.Reserved.ToStr();
            //this.ucLogger.Collapse();
            //this.ucCmds.Collapse();

            this.treeServices.SizeChanged += TreeServices_SizeChanged;
        }

        private void TreeServices_SizeChanged(object sender, SizeChangedEventArgs e) {
            //this.ucLogger.Width = this.Width;
            //if (this.ucLogger.IsVisible) {
            //    this.ucLogger.ToggleVisibility();
            //    this.ucLogger.MaxWidth = this.Width;
            //    this.ucLogger.ToggleVisibility();
            //}
            //else {
            //    this.ucLogger.MaxWidth = this.Width;
            //}
            //this.ucLogger.InvalidateVisual();
            //this.ucLogger.InvalidateMeasure();
            this.ResizeOnNormal();
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.AddEventHandlers();
            this.timer.Start();
            this.writeControl.OnStartup(this.parent);
            DI.Wrapper.CurrentSupportedLanguage(this.SetLanguage);
            this.ucLogger.Show();
            this.ucLogger.OnStartup();
            this.ucLogger.Collapse();
            this.ucCmds.Show();
            this.ucCmds.OnStartup();
            this.ucCmds.Collapse();
            this.SizeToContent = SizeToContent.WidthAndHeight;
//            this.SizeToContent = SizeToContent.Manual;
            this.CenterToParent(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.timer.Stop();
            this.RemoveEventHandlers();
            this.writeControl.OnShutdown();
            this.ucCmds.OnShutdown();
            this.ucLogger.OnShutdown();
            this.buttonSizer.Teardown();
            DI.Wrapper.BLE_Disconnect();
            Instances--;
        }

        #endregion

        #region button handlers

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void btnLog_Click(object sender, RoutedEventArgs e) {
            // Shrink logger before hidding. Set to full width on going visible
            //this.ucLogger.Width = ucLogger.IsVisible ? 400 : this.Width;
            this.ucLogger.ToggleVisibility();
            this.ResizeOnNormal();
        }


        private void btnCommands_Click(object sender, RoutedEventArgs e) {
            //this.ucLogger.Width = this.ucCmds.IsVisible ? 400 : this.Width;
            this.ucCmds.ToggleVisibility();
            this.ResizeOnNormal();
            //this.ucLogger.Width = this.Width;
        }


        private void btnConnect_Click(object sender, RoutedEventArgs e) {
            var device = BLESelect.ShowBox(this.parent, true);
            if (device != null) {
                this.IsBusy = true;
                DI.Wrapper.BLE_DeviceConnectResult += this.DeviceConnectResultHandler;
                DI.Wrapper.BLE_ConnectAsync(device);
            }
        }


        private void btnDisconnect_Click(object sender, RoutedEventArgs e) {
            this.writeControl.Reset();
            // This will call the disconnect as well as set the visuals
            this.SetConnectState(false);
            this.ResizeOnNormal();
        }


        private void btnSettings_Click(object sender, RoutedEventArgs e) {
            BLECommands.ShowBox(this);
            //this.Update(true);

            // reload the list if showing in the commands window
        }


        #endregion

        #region Event handlers

        bool dataChanged = false;

        private void AddEventHandlers() {
            DI.Wrapper.LanguageChanged += this.languageChangedHandler;
            DI.Wrapper.BLE_CharacteristicReadValueChanged += this.characteristicReadValueChanged;
            this.ucLogger.OnMsgReceived += this.logger_OnMsgReceived;
            this.ucCmds.OnCmdSelected += this.onCmdSelected;
            this.timer.Tick += this.Timer_Tick;
        }


        private void onCmdSelected(object sender, string command) {
            this.writeControl.SetCommandText(command);
        }


        private void RemoveEventHandlers() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
            DI.Wrapper.BLE_DeviceConnectResult -= this.DeviceConnectResultHandler;
            DI.Wrapper.BLE_CharacteristicReadValueChanged -= this.characteristicReadValueChanged;
            DI.Wrapper.BLE_ConnectionStatusChanged -= this.connectionStatusChanged;
            this.ucLogger.OnMsgReceived -= this.logger_OnMsgReceived;
            this.ucCmds.OnCmdSelected -= this.onCmdSelected;
            this.timer.Tick -= this.Timer_Tick;
            this.treeServices.SelectedItemChanged -= this.treeServices_SelectedItemChanged;
        }


        private void languageChangedHandler(object sender, SupportedLanguage language) {
            this.SetLanguage(language);
        }


        private void DeviceConnectResultHandler(object sender, BLEGetInfoStatus info) {
            this.Dispatcher.Invoke(() => {
                this.IsBusy = false;
                DI.Wrapper.BLE_DeviceConnectResult -= this.DeviceConnectResultHandler;
                this.ResizeOnNormal();
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
                        break;
                }
            });
        }


        private void connectionStatusChanged(object sender, BLE_ConnectStatusChangeInfo e) {
            this.Dispatcher.Invoke(() => {
                if (e.Status == BLE_ConnectStatus.Disconnected) {
                    this.SetConnectState(false);
                    this.writeControl.Reset();
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


        //private void Update(bool force) {
        //    try {
        //        if (this.currentCharacteristic != null) {
        //            this.writeControl.SetCharacteristic(this.currentCharacteristic);
        //            this.ucCmds.Init(this.currentCharacteristic, force);
        //        }
        //        else {
        //            this.writeControl.Reset();
        //        }
        //        this.ResizeOnNormal();
        //    }
        //    catch(Exception ex) {
        //        this.log.Exception(9999, "", "Update", ex);
        //    }
        //}


        private void treeServices_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e) {
            try {
                if (e.NewValue is BLE_CharacteristicDataModel) {
                    //this.currentCharacteristic = e.NewValue as BLE_CharacteristicDataModel;

                    BLE_CharacteristicDataModel dm = e.NewValue as BLE_CharacteristicDataModel;
                    this.writeControl.SetCharacteristic(dm);
                    this.ucCmds.Init(dm);
                    //this.ucLogger.Width = 400;
                    this.ResizeOnNormal();
                    //this.ucLogger.Width = this.Width;
                    if (dm != null) {
                    //    //this.lblCmdDataTypeContent.Content = dm.DataTypeDisplay;
                    //    // TODO load list of commands based on data type

                    }

                }
                //// Cannot reset if not Characteristic since that happens on update
                else if (e.NewValue is BLE_ServiceDataModel || e.NewValue is BLE_DescriptorDataModel) {
                    this.log.Info("------------------------------------------------------", e.NewValue.GetType().Name);
                    //this.currentCharacteristic = null;
                    this.writeControl.Reset();
                    //this.ucLogger.Width = 400;
                    this.ResizeOnNormal();
                    //this.ucLogger.Width = this.Width;
                    //// TODO - Reset the commands title
                    ////this.lblCmdDataTypeContent.Content = BLE_DataType.Reserved.ToStr();
                }
                //this.Update(false);
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", "treeServices_SelectedItemChanged", ex);
            }
        }

        #endregion

        #region Private

        private void SetConnectState(bool isConnected) {
            this.Dispatcher.Invoke(() => {
                this.writeControl.Connected = isConnected;
                if (isConnected) {
                    this.connectedOff.Collapse();
                    this.connectedOn.Show();
                    this.btnConnect.Collapse();
                    this.btnDisconnect.Show();
                    DI.Wrapper.BLE_ConnectionStatusChanged += this.connectionStatusChanged;
                }
                else {
                    this.connectedOn.Collapse();
                    this.connectedOff.Show();
                    this.btnDisconnect.Collapse();
                    this.btnConnect.Show();
                    this.treeServices.ItemsSource = null;
                    this.currentDevice = null;
                    //this.currentCharacteristic = null;
                    DI.Wrapper.BLE_ConnectionStatusChanged -= this.connectionStatusChanged;
                    DI.Wrapper.BLE_Disconnect();
                    // Reset title
                    //this.lblCmdDataTypeContent.Content = BLE_DataType.Reserved.ToStr();
                }
                this.ResizeOnNormal();
            });
        }


        private void SetLanguage(SupportedLanguage l) {
            this.Dispatcher.Invoke(() => {
                try {
                    this.buttonSizer.PrepForChange();
                    this.btnConnect.Content = l.GetText(MsgCode.connect);
                    this.btnDisconnect.Content = l.GetText(MsgCode.Disconnect);
                    this.btnLog.Content = l.GetText(MsgCode.Log);
                    this.btnExit.Content = l.GetText(MsgCode.exit);
                    this.btnCommands.Content = l.GetText(MsgCode.commands);
                    DI.Wrapper.Translate(this.currentDevice);
                    this.dataChanged = true;
                    this.ResizeOnNormal();
                }
                catch (Exception e) {
                    this.log.Exception(9999, "SetLanguage", "", e);
                }
            });
        }


        private void Timer_Tick(object sender, EventArgs args) {
            lock (this.treeServices) {
                if (this.dataChanged) {
                    this.dataChanged = false;
                    try {
                        this.treeServices.SelectedItemChanged -= this.treeServices_SelectedItemChanged;
                        List<string> selected = this.treeServices.GetSelected();
                        this.treeServices.RefreshAndExpand();
                        this.treeServices.RestoreSelected(selected);
                        this.treeServices.SelectedItemChanged += this.treeServices_SelectedItemChanged;
                        this.ResizeOnNormal();
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "characteristicReadValueChanged", "", e);
                    }
                }
            }
        }


        private void logger_OnMsgReceived(object sender, EventArgs e) {
            if (this.ucLogger.IsVisible) {
                //this.ResizeOnNormal();
            }
        }


        private void ResizeOnNormal() {
            if (this.WindowState == WindowState.Normal) {
                //this.ucLogger.Width = 100;
                this.SizeToContent = SizeToContent.WidthAndHeight;
                this.InvalidateVisual();
//                this.SizeToContent = SizeToContent.Manual;
            }
            //this.ucLogger.MaxWidth = this.Width;
        }


        private void Window_SizeChanged(object sender, SizeChangedEventArgs e) {
            //this.ucLogger.Width = this.Width;
            //if (this.ucLogger.IsVisible) {
            //    this.ucLogger.ToggleVisibility();
            //    //this.ucLogger.MaxWidth = this.Width;
            //    this.ucLogger.ToggleVisibility();
            //}
            //else {
            //    //this.ucLogger.MaxWidth = this.Width;
            //}


            //this.ucLogger.InvalidateVisual();
            //this.ucLogger.InvalidateMeasure();
            //this.ResizeOnNormal();
        }


        #endregion

    }

}
