using BluetoothCommon.Net;
using BluetoothLE.Net.DataModels;
using ChkUtils.Net;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window {

        #region Data

        private List<CommMedialDisplay> mediums = new List<CommMedialDisplay>();
        private List<BTDeviceInfo> infoList_BT = new List<BTDeviceInfo>();
        private List<BluetoothLEDeviceInfo> infoList_BLE = new List<BluetoothLEDeviceInfo>();
        private List<ScriptItem> scriptItems = new List<ScriptItem>();
        private List<WifiNetworkInfo> wifiNetworks = new List<WifiNetworkInfo>();
        private ButtonGroupSizeSyncManager buttonSizer_BT = null;
        private ButtonGroupSizeSyncManager buttonSizer_BLE = null;
        private ButtonGroupSizeSyncManager buttonSizer_WIFI = null;

        MenuWin menu = null;
        private ICommWrapper wrapper = null;
        private ClassLog log = new ClassLog("MainWindow");
        private ScrollViewer inScroll = null;

        #endregion

        #region Constructors and window events

        public MainWindow() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.OnStartupSuccess();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.menu = new MenuWin(this);
            this.menu.Collapse();
            this.inScroll = this.lbIncoming.GetScrollViewer();

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.OnTeardown();
        }


        /// <summary>Close opened menu window anywhere on window on mouse down</summary>
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.HideMenu();
        }


        /// <summary>Grab to window to move when click on title bar</summary>
        private void TitleBarBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.HideMenu();
            this.DragMove();
        }

        #endregion

        #region Button events

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        #endregion

        #region Bluetooth LE

        /// <summary>Event handler for Bluetooth LE device discovery. Adds one at a time</summary>
        /// <param name="sender">The sender of event</param>
        /// <param name="info">The information for discovered device</param>
        private void BLE_DeviceDiscoveredHandler(object sender, BluetoothLEDeviceInfo info) {
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device Discovered", () => {
                    lock (this.listBox_BLE) {
                        this.log.Info("BLE_DeviceDiscoveredHandler", () => string.Format("Adding '{0}' '{1}'", info.Name, info.Id));
                        this.RemoveIfFound(info.Id, false, true);
                        this.log.Info("BLE_DeviceDiscoveredHandler", () => string.Format("Adding DONE"));
                        this.listBox_BLE.Add(this.infoList_BLE, info);
                    }
                });
            });
        }


        /// <summary>BLE adds and removes devices on its own. Old copies are no longer be valid</summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void BLE_DeviceRemovedHandler(object sender, string id) {
            //this.log.Info("BLE_DeviceRemovedHandler", () => string.Format("**** ------- **** Searching to remove {0}", id));
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device Remove", () => {
                    lock (this.listBox_BLE) {
                        this.RemoveIfFound(id, false, true);
                    }
                });
            });
        }


        private void BLE_DeviceUpdatedHandler(object sender, BLE_PropertiesUpdateDataModel args) {
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device Updated", () => {
                    this.log.Info("", () => string.Format("Updating '{0}'", args.Id));
                    // Disconnect the list from control before changing. Maybe change to Observable collection
                    this.listBox_BLE.ItemsSource = null;
                    BluetoothLEDeviceInfo item = this.infoList_BLE.Find((x) => x.Id == args.Id);
                    if (item != null) {
                        // This will raise events on change. How to deal with that. Think only if it is the current on display
                        item.Update(args.ServiceProperties);
                    }
                    this.listBox_BLE.ItemsSource = this.infoList_BLE;
                });
            });
        }


        private void BLE_DeviceDiscoveryCompleteHandler(object sender, bool e) {
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device Discovery Complete", () => {
                    this.gridWait.Collapse();
                });
            });
        }


        private void Wrapper_BLE_DeviceInfoGatheredForConfig(object sender, BluetoothLEDeviceInfo info) {
            this.Dispatcher.Invoke(() => {
                WrapErr.ToErrReport(9999, "Failure on BLE Device info gathered Complete", () => {
                    this.gridWait.Collapse();
                    this.wrapper.BLE_DeviceInfoGathered -= this.Wrapper_BLE_DeviceInfoGatheredForConfig;
                    if (info != null) {
                        DeviceInfo_BLESerial bleInfo = new DeviceInfo_BLESerial(this, info);
                        bleInfo.ShowDialog();
                    }
                    else {
                        MsgBoxSimple.ShowBox(DI.Wrapper.GetText(MsgCode.Error), DI.Wrapper.GetText(MsgCode.LoadFailed));
                    }
                });
            });
        }


        // TODO - add the Update because of Characteristics can be added and removed
        private void RemoveIfFound(string id, bool postErrorNotFound, bool msgIfFound) {
            WrapErr.ToErrReport(9999, "Fail on Remove if found", () => {

                // Disconnect the list from control before changing. Maybe change to Observable collection
                this.listBox_BLE.ItemsSource = null;

                var item = this.infoList_BLE.Find((x) => x.Id == id);
                if (item != null) {
                    if (msgIfFound) {
                        this.log.Info("RemoveIfFound", () => string.Format("REMOVE DEVICE:{0}", id));
                    }
                    if (!this.infoList_BLE.Remove(item)) {
                        this.log.Error(9999, "BLE_DeviceRemovedHander", () => string.Format("Failed to remove '{0}'", id));
                    }
                }
                else {
                    if (postErrorNotFound) {
                        this.log.Error(9999, "BLE_DeviceRemovedHander", () => string.Format("Item not found to be removed '{0}'", id));
                    }
                }

                this.listBox_BLE.ItemsSource = this.infoList_BLE;
            });
        }



        /// <summary>Clear Bluetooth LE device list and Launch device discovery</summary>
        /// <param name="sender">Click sender</param>
        /// <param name="e">Routed argument. Not used</param>
        private void btnDiscoverLE_Click(object sender, RoutedEventArgs e) {
            this.listBox_BLE.ItemsSource = null;
            this.infoList_BLE.Clear();
            this.listBox_BLE.ItemsSource = this.infoList_BLE;
            this.grdMain.Show();
            this.wrapper.BLE_DiscoverAsync();
        }


        private void btnInfoLE_Click(object sender, RoutedEventArgs e) {
            if (this.listBox_BLE.SelectedItem != null) {
                this.wrapper.BLE_GetDbgInfoStringDump(this.listBox_BLE.SelectedItem, App.ShowMsgTitle);
            }
        }


        private void btnConfigureBLE_Click(object sender, RoutedEventArgs e) {
            this.listBox_BLE.GetSelected<BluetoothLEDeviceInfo>((info) => {
                try {
                    // Will connect to complete the info
                    this.gridWait.Show();
                    this.wrapper.BLE_DeviceInfoGathered += this.Wrapper_BLE_DeviceInfoGatheredForConfig;
                    DI.Wrapper.BLE_GetInfo(info);
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "Failed on BLE config access to device", ex);
                }
            });
        }


        private void btnLEConnect_Click(object sender, RoutedEventArgs e) {
            this.listBox_BLE.GetSelected<BluetoothLEDeviceInfo>((info) => {
                try {
                    this.wrapper.BLE_ConnectAsync(info);
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "Failed on BLE connect", ex);
                }
            });
        }

        #endregion

        #region Bluetooth

        #region Bluetooth Button handlers

        private void btnBTDiscover_Click(object sender, RoutedEventArgs e) {
            this.BT_ClearAllEntries();
            this.gridWait.Show();
            this.btnBTConnect.Collapse();
            this.wrapper.BTClassicDiscoverAsync(this.btPairCheck.IsChecked.GetValueOrDefault(false));
        }


        private void btnBTConnect_Click(object sender, RoutedEventArgs e) {
            this.log.InfoEntry("btnBTConnect_Click");
            this.listBox_BT.GetSelected<BTDeviceInfo>((item) => {
                this.log.Info("btnBTConnect_Click", "item not NULL - so call connect async");
                this.btnBTDisconnect.Collapse();
                this.btnBTConnect.Collapse();
                this.gridWait.Show();
                this.wrapper.BTClassicConnectAsync(item);
            });
        }


        private void btnBTDisconnect_Click(object sender, RoutedEventArgs e) {
            this.wrapper.BTClassicDisconnect();
            this.btnBTDisconnect.Collapse();
            this.btnBTConnect.Show();
        }


        private void btnInfoBT_Click(object sender, RoutedEventArgs e) {
            this.listBox_BT.GetSelected<BTDeviceInfo>((item) => {
                    // TODO - spinner
                    this.wrapper.BTClassicGetExtraInfoAsync(item);                    
            });
        }


        private void btnBTPair_Click(object sender, RoutedEventArgs e) {
            this.SetBTCheckUncheckButtons();
            this.log.InfoEntry("btnBTPair_Click");
            this.listBox_BT.GetSelected<BTDeviceInfo>((item) => {
                this.log.Info("btnBTConnect_Click", "item not NULL - so call connect async");
                this.gridWait.Show();
                this.wrapper.BTClassicPairAsync(item);
            });
        }


        private void btnBTUnPair_Click(object sender, RoutedEventArgs e) {
            this.SetBTCheckUncheckButtons();
            this.log.InfoEntry("btnBTUnPair_Click");
            this.listBox_BT.GetSelected<BTDeviceInfo>((item) => {
                this.log.Info("btnBTConnect_Click", "item not NULL - so call connect async");
                this.gridWait.Show();
                this.wrapper.BTClassicUnPairAsync(item);
            });
        }

        #endregion

        #region BT Event Handlers

        private void BT_DeviceDiscoveredHandler(object sender, BTDeviceInfo dev) {
            this.Dispatcher.Invoke(() => {
                this.listBox_BT.Add(this.infoList_BT, dev);
            });
        }


        private void BT_DiscoveryCompleteHandler(object sender, bool e) {
            this.log.InfoEntry("BT_DiscoveryCompleteHandler");
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                this.SetBTCheckUncheckButtons();
            });
        }


        private void BT_DeviceInfoGatheredHandler(object sender, BTDeviceInfo e) {
            this.Dispatcher.Invoke(() => {
                DeviceInfo_BT win = new DeviceInfo_BT(this, e);
                win.ShowDialog();
            });
        }


        private void BT_BytesReceivedHandler(object sender, string msg) {
            this.Dispatcher.Invoke(() => {
                this.lbIncoming.AddAndScroll(msg, this.inScroll, 100);
            });
        }


        private void BT_ConnectionCompletedHandler(object sender, bool isOk) {
            this.log.InfoEntry("BT_ConnectionCompletedHandler");
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (isOk) {
                    this.btnBTDisconnect.Show();
                }
                else {
                    this.btnBTConnect.Show();
                    MsgBoxSimple.ShowBox("Failed connection", "Reason unknown");
                }
            });
        }


        private void BT_PairInfoRequestedHandler(object sender, BT_PairingInfoDataModel info) {
            this.log.InfoEntry("BT_PairInfoRequestedHandler");
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (info.IsPinRequested) {
                    var result = MsgBoxEnterText.ShowBox(this, info.RequestTitle, info.RequestMsg);
                    info.PIN = result.Text;
                    info.HasUserConfirmed = (result.Result == MsgBoxEnterText.MsgBoxTextInputResult.OK);
                }
                else {
                    MsgBoxYesNo.MsgBoxResult result2 = MsgBoxYesNo.ShowBox(this, info.RequestMsg, info.RequestMsg);
                    info.HasUserConfirmed = (result2 == MsgBoxYesNo.MsgBoxResult.Yes);     
                }
            });
        }


        private void BT_UnPairStatusHandler(object sender, BTUnPairOperationStatus e) {
            this.log.InfoEntry("BT_UnPairStatusHandler");
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (e.IsSuccessful) {
                    this.BT_RemoveEntry(e.Name);
                    this.SetBTCheckUncheckButtons();
                }
                else {
                    this.ShowMsgBox(this.wrapper.GetText(MsgCode.Error), e.UnpairStatus.ToString());
                }
            });
        }


        private void BT_PairStatusHandler(object sender, BTPairOperationStatus e) {
            this.log.InfoEntry("BT_PairStatusHandler");
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (e.IsSuccessful) {
                    this.BT_RemoveEntry(e.Name);
                    this.SetBTCheckUncheckButtons();
                }
                else {
                    this.ShowMsgBox(this.wrapper.GetText(MsgCode.Error), e.PairStatus.ToString());
                }
            });
        }

        #endregion

        private void SetBTCheckUncheckButtons() {
            this.btnBTUnPair.Collapse();
            this.btnBTPair.Collapse();
            this.btnBTConnect.Collapse();
            this.btnInfoBT.Collapse();
            if (this.listBox_BT.Count() > 0) {
                this.btnInfoBT.Show();
                if (this.btPairCheck.IsChecked.GetValueOrDefault(false)) {
                    this.btnBTUnPair.Show();
                    this.btnBTConnect.Show();
                }
                else {
                    this.btnBTPair.Show();
                }
            }
        }


        private void BT_RemoveEntry(string name) {
            // TODO - just using the same list box. Will need more logic
            BTDeviceInfo item = this.infoList_BT.FirstOrDefault(x => x.Name == name);
            if (item != null) {
                this.listBox_BT.ItemsSource = null;
                this.infoList_BT.Remove(item);
                this.listBox_BT.ItemsSource = this.infoList_BT;
            }
        }


        private void btPairCheck_Checked(object sender, RoutedEventArgs e) {
            this.BT_ClearAllEntries();
            this.SetBTCheckUncheckButtons();
        }


        private void btPairCheck_Unchecked(object sender, RoutedEventArgs e) {
            this.BT_ClearAllEntries();
            this.SetBTCheckUncheckButtons();
        }


        private void BT_ClearAllEntries() {
            this.btnBTPair.Collapse();
            this.btnBTUnPair.Collapse();
            this.listBox_BT.Clear(this.infoList_BT);
        }

        #endregion

        #region Wifi

        private void btnWifiDiscover_Click(object sender, RoutedEventArgs e) {
            this.gridWait.Show();
            this.wrapper.WifiDiscoverAsync();
        }


        private void Wrapper_DiscoveredWifiNetworks(object sender, List<WifiNetworkInfo> networks) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                this.log.Info("Wrapper_DiscoveredNetworks", () => string.Format("Found {0} networks", networks.Count));
                this.lbWifi.SetNewSource(ref this.wifiNetworks, networks);
                this.btnWifiConnect.Show();
            });
        }


        private void Wrapper_OnWifiError(object sender, WifiError e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (e.Code != WifiErrorCode.UserCanceled) {
                    string err = string.Format("{0} ({1})", e.Code.ToString(), e.ExtraInfo.Length == 0 ? "--" : e.ExtraInfo);
                    App.ShowMsg(err);
                }
            });
        }

        private void Wifi_CredentialsRequestedEventHandler(object sender, WifiCredentials cred) {

            var result = MsgBoxWifiCred.ShowBox(this, cred.SSID, cred.RemoteHostName, cred.RemoteServiceName);
            cred.IsUserCanceled = !result.IsOk;
            cred.IsUserSaveRequest = result.Save;
            cred.RemoteHostName = result.HostName;
            cred.RemoteServiceName = result.ServiceName;
            cred.WifiPassword = result.Password;


            //cred.RemoteHostName = "192.168.4.1";
            //cred.RemoteServiceName = "80";
            //cred.WifiPassword = "1234567890";

            // TODO - implement dialog


            //App.ShowMsg("Returning cred for wifi");
            // Not using UserName
        }



        private void btnWifiConnect_Click(object sender, RoutedEventArgs e) {
            this.lbWifi.GetSelected<WifiNetworkInfo>((item) => {
                this.gridWait.Show();
                this.wrapper.WifiConnectAsync(item);
            });
        }


        private void btnWifiDisconnect_Click(object sender, RoutedEventArgs e) {
            this.wrapper.WifiDisconect();
            this.btnWifiDisconnect.Collapse();
        }


        private void Wrapper_OnWifiConnectionAttemptCompletedHandler(object sender, CommunicationStack.Net.DataModels.MsgPumpConnectResults e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (!e.IsSuccessful) {
                    App.ShowMsg("Failed connection");
                }
                else {
                    this.btnWifiDisconnect.Show();
                }
            });
        }


        private void Wrapper_Wifi_BytesReceivedHandler(object sender, string msg) {
            this.Dispatcher.Invoke(() => {
                this.lbIncoming.AddAndScroll(msg, this.inScroll, 100);
            });
        }

        #endregion

        #region Private Init and teardown

        private void OnStartupSuccess() {
            this.wrapper.CommMediumList((items) => {
                foreach (var item in items) {
                    if (item.MediumType != CommMediumType.None) {
                        // The cross platform wrapper only returns the immediate icon path
                        item.IconSource = string.Format("{0}{1}", IconBinder.GetIconPrefix(), item.IconSource);
                        this.mediums.Add(item);
                    }
                }
            });
            this.cbComm.ItemsSource = this.mediums;
            this.cbComm.SelectedIndex = 0;

            // Terminators
            this.wrapper.GetCurrentTerminator(this.GetTerminatorsOk, this.GetTerminatorsOnErr);
            this.wrapper.CurrentTerminatorChanged += Wrapper_CurrentTerminatorChanged;

            // Language
            this.wrapper.LanguageChanged += this.LanguageChangedHandler;

            // Scripts
            this.wrapper.CurrentScriptChanged += this.Wrapper_CurrentScriptChanged;

            // BLE
            this.wrapper.BLE_DeviceDiscovered += this.BLE_DeviceDiscoveredHandler;
            this.wrapper.BLE_DeviceRemoved += this.BLE_DeviceRemovedHandler;
            this.wrapper.BLE_DeviceUpdated += this.BLE_DeviceUpdatedHandler;
            this.wrapper.BLE_DeviceDiscoveryComplete += BLE_DeviceDiscoveryCompleteHandler;

            // BT
            this.wrapper.BT_DeviceDiscovered += this.BT_DeviceDiscoveredHandler;
            this.wrapper.BT_DiscoveryComplete += this.BT_DiscoveryCompleteHandler;
            this.wrapper.BT_DeviceInfoGathered += this.BT_DeviceInfoGatheredHandler;
            this.wrapper.BT_ConnectionCompleted += this.BT_ConnectionCompletedHandler;
            this.wrapper.BT_BytesReceived += this.BT_BytesReceivedHandler;
            this.wrapper.BT_PairInfoRequested += this.BT_PairInfoRequestedHandler;
            this.wrapper.BT_PairStatus += this.BT_PairStatusHandler;
            this.wrapper.BT_UnPairStatus += this.BT_UnPairStatusHandler;

            // WIFI
            this.wrapper.OnWifiError += this.Wrapper_OnWifiError;
            this.wrapper.DiscoveredWifiNetworks += this.Wrapper_DiscoveredWifiNetworks;
            this.wrapper.OnWifiConnectionAttemptCompleted += this.Wrapper_OnWifiConnectionAttemptCompletedHandler;
            this.wrapper.CredentialsRequestedEvent += this.Wifi_CredentialsRequestedEventHandler;
            this.wrapper.Wifi_BytesReceived += Wrapper_Wifi_BytesReceivedHandler;

            // Call before rendering which will trigger initial resize events
            this.buttonSizer_BT = new ButtonGroupSizeSyncManager(this.btnBTConnect, this.btnBTDiscover);
            this.buttonSizer_BT.PrepForChange();

            this.buttonSizer_BLE = new ButtonGroupSizeSyncManager(this.btnDiscoverLE, this.btnInfoLE, this.btnLEConnect);
            this.buttonSizer_BLE.PrepForChange();

            this.buttonSizer_WIFI = new ButtonGroupSizeSyncManager(this.btnWifiDiscover, this.btnWifiConnect, this.btnWifiDisconnect);
            this.buttonSizer_WIFI.PrepForChange();

            this.wrapper.GetCurrentScript(this.PopulateScriptData, WindowHelpers.ShowMsg);
        }


        private void OnTeardown() {
            this.buttonSizer_BT.Teardown();
            this.buttonSizer_BLE.Teardown();
            this.buttonSizer_WIFI.Teardown();

            // Languages
            this.wrapper.LanguageChanged -= this.LanguageChangedHandler;

            // Scripts
            this.wrapper.CurrentScriptChanged -= this.Wrapper_CurrentScriptChanged;

            // Terminators
            this.wrapper.CurrentTerminatorChanged += Wrapper_CurrentTerminatorChanged;

            // BLE
            this.wrapper.BLE_DeviceDiscovered -= this.BLE_DeviceDiscoveredHandler;
            this.wrapper.BLE_DeviceRemoved -= this.BLE_DeviceRemovedHandler;
            this.wrapper.BLE_DeviceUpdated -= this.BLE_DeviceUpdatedHandler;
            this.wrapper.BLE_DeviceDiscoveryComplete -= BLE_DeviceDiscoveryCompleteHandler;
            this.wrapper.BLE_Disconnect();

            //BT
            this.wrapper.BT_DeviceDiscovered -= this.BT_DeviceDiscoveredHandler;
            this.wrapper.BT_DiscoveryComplete -= this.BT_DiscoveryCompleteHandler;
            this.wrapper.BT_DeviceInfoGathered -= this.BT_DeviceInfoGatheredHandler;
            this.wrapper.BT_ConnectionCompleted -= this.BT_ConnectionCompletedHandler;
            this.wrapper.BT_BytesReceived -= this.BT_BytesReceivedHandler;
            this.wrapper.BT_PairInfoRequested -= this.BT_PairInfoRequestedHandler;
            this.wrapper.BT_PairStatus -= this.BT_PairStatusHandler;
            this.wrapper.BT_UnPairStatus -= this.BT_UnPairStatusHandler;
            this.wrapper.BTClassicDisconnect();

            // WIFI
            this.wrapper.OnWifiError -= this.Wrapper_OnWifiError;
            this.wrapper.DiscoveredWifiNetworks -= this.Wrapper_DiscoveredWifiNetworks;
            this.wrapper.OnWifiConnectionAttemptCompleted -= this.Wrapper_OnWifiConnectionAttemptCompletedHandler;
            this.wrapper.Wifi_BytesReceived -= Wrapper_Wifi_BytesReceivedHandler;
            this.wrapper.CredentialsRequestedEvent -= this.Wifi_CredentialsRequestedEventHandler;
            this.wrapper.WifiDisconect();

            // USB

            if (this.menu != null) {
                this.menu.Close();
            }

            this.wrapper.Teardown();
        }


        private void PopulateScriptData(ScriptDataModel dataModel) {
            this.outgoing.Clear(this.scriptItems);
            this.outgoing.Add(this.scriptItems, dataModel.Items);
        }


        #endregion

        #region Private Medium Selection

        private void SelectBTClassic() {
            this.spBluetooth.Show();
            this.btnBTDiscover.Show();
        }

        private void SelectLE() {
            this.spBluetoothLE.Show();
            this.btnDiscoverLE.Show();
            //this.btnLEConnect.Show();
        }


        private void SelectEthernet() {
            this.spEthernet.Show();
        }


        private void SelectWifi() {
            this.spWifi.Show();
        }

        private void SelectUSB() {
            // TODO USB functionality
        }


        private void UnselectBTClassic() {
            this.spBluetooth.Collapse();
            this.btnBTDiscover.Collapse();
            this.btnBTDisconnect.Collapse();
            this.listBox_BT.Clear(this.infoList_BT);
        }


        private void UnselectLE() {
            this.spBluetoothLE.Collapse();
            this.btnLEConnect.Collapse();
            this.btnDiscoverLE.Collapse();
            this.listBox_BLE.Clear(this.infoList_BLE);
        }


        private void UnselectEthernet() {
            this.spEthernet.Collapse();
        }


        private void UnselectWifi() {
            this.spWifi.Collapse();
            this.btnWifiConnect.Collapse();
            this.btnWifiDisconnect.Collapse();
            this.lbWifi.Clear(this.wifiNetworks);
            // TODO extra buttons
        }


        private void UnselectUsb() {
            // TODO USB functionality
        }


        private void cbComm_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.lbIncoming.Items.Clear();
            this.UnselectBTClassic();
            this.UnselectLE();
            this.UnselectEthernet();
            this.UnselectWifi();
            this.UnselectUsb();
            DI.Wrapper.DisconnectAll();
            this.cbComm.GetSelected<CommMedialDisplay>((media) => {
                switch (media.MediumType) {
                    case CommMediumType.Bluetooth:
                        this.SelectBTClassic();
                        break;
                    case CommMediumType.BluetoothLE:
                        this.SelectLE();
                        break;
                    case CommMediumType.Ethernet:
                        this.SelectEthernet();
                        break;
                    case CommMediumType.Wifi:
                        this.SelectWifi();
                        break;
                    case CommMediumType.Usb:
                        this.SelectUSB();
                        break;
                    default:
                        break;
                }
            });
        }

        #endregion

        #region Private

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            this.outgoing.GetSelected<ScriptItem>((item) => {
                this.cbComm.GetSelected<CommMedialDisplay>((media) => {
                    switch (media.MediumType) {
                        case CommMediumType.Bluetooth:
                            this.wrapper.BTClassicSend(item.Command);
                            break;
                        case CommMediumType.BluetoothLE:
                            break;
                        case CommMediumType.Ethernet:
                            break;
                        case CommMediumType.Wifi:
                            this.wrapper.WifiSend(item.Command);
                            break;
                        default:
                            break;
                    }
                });
            });
        }


        private void btnCommTypeHelp_Click(object sender, RoutedEventArgs e) {
            Help_CommunicationMediums win = new Help_CommunicationMediums(this);
            win.ShowDialog();
        }


        private void outgoing_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.btnSend.Show();
        }


        private void Wrapper_CurrentScriptChanged(object sender, ScriptDataModel data) {
            this.PopulateScriptData(data);
        }


        private void Wrapper_CurrentTerminatorChanged(object sender, TerminatorDataModel data) {
            this.terminatorView.Initialise(data);
        }


        private void GetTerminatorsOk(TerminatorDataModel data) {
            this.terminatorView.Initialise(data);
        }


        private void GetTerminatorsOnErr(string err) {
            App.ShowMsg(err);
            this.terminatorView.Initialise(new TerminatorDataModel());
        }

        #endregion

        #region Private Menu events

        /// <summary>Click event on the hamburger icon to toggle menu window</summary>
        private void imgMenu_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (this.menu.IsVisible) {
                this.menu.Hide();
            }
            else {
                // Need to get offset from current position of main window at click time
                this.menu.Left = this.Left;
                this.menu.Top = this.Top + this.taskBar.ActualHeight;
                this.menu.Show();
            }
        }


        /// <summary>Catch the MouseDown event on hamburg menu before it bubbles up to the window and closes the menu</summary>
        private void imgMenu_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            e.Handled = true;
        }


        /// <summary>Handle the language changed event to update controls text</summary>
        private void LanguageChangedHandler(object sender, LanguageFactory.Net.Messaging.SupportedLanguage lang) {
            // The button text change will trigger resize
            this.buttonSizer_BT.PrepForChange();
            this.buttonSizer_BLE.PrepForChange();
            this.buttonSizer_WIFI.PrepForChange();

            // Buttons
            this.btnExit.Content = lang.GetText(MsgCode.exit);
            this.btnSend.Content = lang.GetText(MsgCode.send);
            
            this.btnBTConnect.Content = lang.GetText(MsgCode.connect);
            this.btnInfoBT.Content = lang.GetText(MsgCode.info);
            this.btnBTDiscover.Content = lang.GetText(MsgCode.discover);

            this.btnLEConnect.Content = lang.GetText(MsgCode.connect);
            this.btnDiscoverLE.Content = lang.GetText(MsgCode.discover);
            this.btnInfoLE.Content =lang.GetText(MsgCode.info);

            this.btnBTPair.Content = lang.GetText(MsgCode.Pair);
            this.btnBTUnPair.Content = lang.GetText(MsgCode.Unpair);

            // Labels
            this.lbCommand.Content = lang.GetText(MsgCode.command);
            this.lbResponse.Content = lang.GetText(MsgCode.response);
            this.txtPairedDevices.Text = lang.GetText(MsgCode.PairedDevices);

            // TODO Other texts
        }

        private void HideMenu() {
            if (this.menu.IsVisible) {
                this.menu.Hide();
            }
        }

        #endregion


    }
}
