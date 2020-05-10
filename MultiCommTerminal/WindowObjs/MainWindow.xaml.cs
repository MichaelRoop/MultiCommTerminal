using BluetoothCommon.Net;
using BluetoothLE.Net.DataModels;
using ChkUtils.Net;
using LanguageFactory.data;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window {

        #region Data

        private List<CommMedialDisplay> mediums = new List<CommMedialDisplay>();
        private List<BTDeviceInfo> infoList_BT = new List<BTDeviceInfo>();
        private List<BluetoothLEDeviceInfo> infoList_BLE = new List<BluetoothLEDeviceInfo>();
        private ButtonGroupSizeSyncManager buttonSizer_BT = null;
        private ButtonGroupSizeSyncManager buttonSizer_BLE = null;

        MenuWin menu = null;
        private ICommWrapper wrapper = null;
        private ClassLog log = new ClassLog("MainWindow");

        #endregion

        #region Constructors and window events

        public MainWindow() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.wrapper.GetCurrentTerminator(
                (data) => { 
                    this.terminatorView.Initialise(data);
                }, 
                (err) => {
                    App.ShowMsg(err);
                    this.terminatorView.Initialise(new TerminatorDataModel());
                });
            this.wrapper.CurrentTerminatorChanged += Wrapper_CurrentTerminatorChanged;

            this.OnStartupSuccess();
            this.SizeToContent = SizeToContent.WidthAndHeight;

        }


        private void Wrapper_CurrentTerminatorChanged(object sender, TerminatorDataModel data) {
            this.terminatorView.Initialise(data);
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.menu = new MenuWin(this);
            this.menu.Visibility = Visibility.Collapsed;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.buttonSizer_BT.Teardown();
            this.buttonSizer_BLE.Teardown();
            
            this.wrapper.BLE_DeviceDiscovered -= this.BLE_DeviceDiscoveredHandler;
            this.wrapper.BLE_DeviceRemoved -= this.BLE_DeviceRemovedHandler;
            this.wrapper.BLE_DeviceUpdated -= this.BLE_DeviceUpdatedHandler;
            this.wrapper.BLE_DeviceDiscoveryComplete -= BLE_DeviceDiscoveryCompleteHandler;
            this.wrapper.BLE_Disconnect();

            this.wrapper.BT_DeviceDiscovered -= this.BT_DeviceDiscoveredHandler;
            this.wrapper.BT_DiscoveryComplete -= this.BT_DiscoveryCompleteHandler;
            this.wrapper.BT_ConnectionCompleted -= this.BT_ConnectionCompletedHandler;
            this.wrapper.BT_BytesReceived -= this.BT_BytesReceivedHandler;
            this.wrapper.BTClassicDisconnect();

            if (this.menu != null) {
                this.menu.Close();
            }

            this.wrapper.Teardown();
        }

        /// <summary>Close opened menu window anywhere on window on mouse down</summary>
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.HideMenu();
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
                        // Disconnect the list from control before changing. Maybe change to Observable collection
                        this.listBox_BLE.ItemsSource = null;
                        this.infoList_BLE.Add(info);
                        this.log.Info("BLE_DeviceDiscoveredHandler", () => string.Format("Adding DONE"));
                        this.listBox_BLE.ItemsSource = this.infoList_BLE;
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
                    this.gridWait.Visibility = Visibility.Collapsed;
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
            this.grdMain.Visibility = Visibility.Visible;
            this.wrapper.BLE_DiscoverAsync();
        }


        private void btnInfoLE_Click(object sender, RoutedEventArgs e) {
            if (this.listBox_BLE.SelectedItem != null) {
                this.wrapper.BLE_GetDbgInfoStringDump(this.listBox_BLE.SelectedItem, App.ShowMsgTitle);
            }
        }


        private void btnLEConnect_Click(object sender, RoutedEventArgs e) {
            if (this.listBox_BLE.SelectedItem != null) {
                try {
                    BluetoothLEDeviceInfo info = this.listBox_BLE.SelectedItem as BluetoothLEDeviceInfo;
                    this.wrapper.BLE_ConnectAsync(info);
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "Failed on BLE connect", ex);
                }
            }
        }

        #endregion

        #region Bluetooth

        private void btnBTDiscover_Click(object sender, RoutedEventArgs e) {
            // Asynchronous call
            this.listBox_BT.ItemsSource = null;
            this.infoList_BT.Clear();
            this.listBox_BT.ItemsSource = this.infoList_BT;
            this.gridWait.Visibility = Visibility.Visible;
            this.wrapper.BTClassicDiscoverAsync();
        }

        private void btnBTConnect_Click(object sender, RoutedEventArgs e) {
            this.log.InfoEntry("btnBTConnect_Click");
            BTDeviceInfo item = this.listBox_BT.SelectedItem as BTDeviceInfo;
            if (item != null) {
                this.log.Info("btnBTConnect_Click", "item not NULL - so call connect async");
                this.gridWait.Visibility = Visibility.Visible;
                this.wrapper.BTClassicConnectAsync(item);
            }
        }

        private void btnInfoBT_Click(object sender, RoutedEventArgs e) {
            if (this.listBox_BT.SelectedItem != null) {
                BTDeviceInfo item = this.listBox_BT.SelectedItem as BTDeviceInfo;
                if (item != null) {
                    DeviceInfo_BT win = new DeviceInfo_BT(this, item);
                    win.ShowDialog();
                }
            }
        }


        private void BT_DeviceDiscoveredHandler(object sender, BTDeviceInfo dev) {
            this.Dispatcher.Invoke(() => {
                this.listBox_BT.ItemsSource = null;
                this.infoList_BT.Add(dev);
                this.listBox_BT.ItemsSource = this.infoList_BT;
            });
        }

        private void BT_DiscoveryCompleteHandler(object sender, bool e) {
            this.log.InfoEntry("BT_DiscoveryCompleteHandler");
            this.Dispatcher.Invoke(() => {
                this.gridWait.Visibility = Visibility.Collapsed;
            });
        }


        private void BT_BytesReceivedHandler(object sender, string msg) {
            this.Dispatcher.Invoke(() => {
                this.lbIncoming.Items.Add(msg);
            });
        }


        private void BT_ConnectionCompletedHandler(object sender, bool e) {
            this.log.InfoEntry("BT_ConnectionCompletedHandler");
            this.Dispatcher.Invoke(() => {
                this.gridWait.Visibility = Visibility.Collapsed;
            });
        }

        #endregion

        #region Private Init and teardown

        private void OnStartupSuccess() {
            this.wrapper.CommMediumList((items) => {
                foreach (var item in items) {
                    this.mediums.Add(item);
                }
            });
            this.cbComm.ItemsSource = this.mediums;
            this.cbComm.SelectedIndex = 0;
            this.SizeToContent = SizeToContent.WidthAndHeight;

            this.wrapper.LanguageChanged += this.LanguageChangedHandler;

            // Bluetooth LE
            this.wrapper.BLE_DeviceDiscovered += this.BLE_DeviceDiscoveredHandler;
            this.wrapper.BLE_DeviceRemoved += this.BLE_DeviceRemovedHandler;
            this.wrapper.BLE_DeviceUpdated += this.BLE_DeviceUpdatedHandler;
            this.wrapper.BLE_DeviceDiscoveryComplete += BLE_DeviceDiscoveryCompleteHandler;

            //Bluetooth Classic
            this.wrapper.BT_DeviceDiscovered += this.BT_DeviceDiscoveredHandler;
            this.wrapper.BT_DiscoveryComplete += this.BT_DiscoveryCompleteHandler;
            this.wrapper.BT_ConnectionCompleted += this.BT_ConnectionCompletedHandler;
            this.wrapper.BT_BytesReceived += this.BT_BytesReceivedHandler;
            // Call before rendering which will trigger initial resize events
            buttonSizer_BT = new ButtonGroupSizeSyncManager(this.btnBTConnect, this.btnBTDiscover);
            this.buttonSizer_BT.PrepForChange();

            buttonSizer_BLE = new ButtonGroupSizeSyncManager(this.btnDiscoverLE, this.btnInfoLE, this.btnLEConnect);
            buttonSizer_BLE.PrepForChange();

            // TODO - remove - temp populate command box
            this.outgoing.Items.Add("First msg");
            this.outgoing.Items.Add("Second msg");
            this.outgoing.Items.Add("A bit of nothing");
            this.outgoing.Items.Add("Start doing something");
            this.outgoing.Items.Add("Stop doing int");

        }

        #endregion

        #region Private

        private void cbComm_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Hide all the options
            this.spBluetooth.Visibility = Visibility.Collapsed;
            this.spBluetoothLE.Visibility = Visibility.Collapsed;
            this.btnLEConnect.Visibility = Visibility.Collapsed;
            this.spEthernet.Visibility = Visibility.Collapsed;
            this.spWifi.Visibility = Visibility.Collapsed;

            this.btnBTDiscover.Visibility = Visibility.Collapsed;
            this.btnDiscoverLE.Visibility = Visibility.Collapsed;

            // Disconnect on switch
            DI.Wrapper.DisconnectAll();

            switch ((this.cbComm.SelectedItem as CommMedialDisplay).MediumType) {
                case CommMediumType.Bluetooth:
                    this.spBluetooth.Visibility = Visibility.Visible;
                    this.btnBTDiscover.Visibility = Visibility.Visible;
                    break;
                case CommMediumType.BluetoothLE:
                    this.spBluetoothLE.Visibility = Visibility.Visible;
                    this.btnDiscoverLE.Visibility = Visibility.Visible;
                    this.btnLEConnect.Visibility = Visibility.Visible;
                    break;
                case CommMediumType.Ethernet:
                    this.spEthernet.Visibility = Visibility.Visible;
                    break;
                case CommMediumType.Wifi:
                    this.spWifi.Visibility = Visibility.Visible;
                    break;
                default:
                    break;
            }
        }

        // This will be moved out of UI
        //private void DoDisconnect(CommMediumType newMedium) {
        //    if (this.currentMedium != CommMediumType.None && 
        //        this.currentMedium != newMedium) {
        //        // Disconnect whatever we are connected to
        //        switch (this.currentMedium) {
        //            case CommMediumType.Bluetooth:
        //                break;
        //            case CommMediumType.BluetoothLE:
        //                break;
        //            case CommMediumType.Ethernet:
        //                break;
        //            case CommMediumType.Wifi:
        //                break;
        //            default:
        //                break;
        //        }
        //        this.currentMedium = CommMediumType.None;
        //        this.btnConnect.Visibility = Visibility.Collapsed;
        //    }
        //}

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            string cmd = this.outgoing.SelectedItem as string;
            if (cmd != null) {
                // TODO - send to current device
                switch ((this.cbComm.SelectedItem as CommMedialDisplay).MediumType) {
                    case CommMediumType.Bluetooth:
                        this.wrapper.BTClassicSend(cmd);
                        break;
                    case CommMediumType.BluetoothLE:
                        break;
                    case CommMediumType.Ethernet:
                        break;
                    case CommMediumType.Wifi:
                        break;
                    default:
                        break;
                }
            }
        }


        private void outgoing_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.btnSend.Visibility = Visibility.Visible;
            // TODO modify so that only when connected
        }



        #endregion

        #region Menu events

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
        private void LanguageChangedHandler(object sender, LanguageFactory.Messaging.SupportedLanguage lang) {
            // The button text change will trigger resize
            this.buttonSizer_BT.PrepForChange();
            this.buttonSizer_BLE.PrepForChange();

            // Buttons
            this.btnExit.Content = lang.GetText(MsgCode.exit);
            this.btnSend.Content = lang.GetText(MsgCode.send);
            
            this.btnBTConnect.Content = lang.GetText(MsgCode.connect);
            this.btnInfoBT.Content = lang.GetText(MsgCode.info);
            this.btnBTDiscover.Content = lang.GetText(MsgCode.discover);

            this.btnLEConnect.Content = lang.GetText(MsgCode.connect);
            this.btnDiscoverLE.Content = lang.GetText(MsgCode.discover);
            this.btnInfoLE.Content =lang.GetText(MsgCode.info);

            // Labels
            this.lbCommand.Content = lang.GetText(MsgCode.command);
            this.lbResponse.Content = lang.GetText(MsgCode.response);

            // TODO Other texts
        }


        private void TitleBarBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.HideMenu();
            this.DragMove();
        }


        private void HideMenu() {
            if (this.menu.IsVisible) {
                this.menu.Hide();
            }
        }

        #endregion

        private void btnCommTypeHelp_Click(object sender, RoutedEventArgs e) {
            Help_CommunicationMediums win = new Help_CommunicationMediums(this);
            win.ShowDialog();
        }
    }
}
