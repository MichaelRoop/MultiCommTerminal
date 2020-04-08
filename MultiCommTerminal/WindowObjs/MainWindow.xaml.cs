using BluetoothCommon.Net;
using LanguageFactory.data;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window {

        #region Data

        private MediumGroup mediumGroup = new MediumGroup();
        private List<BTDeviceInfo> btInfoList = new List<BTDeviceInfo>();
        private List<BluetoothLEDeviceInfo> btInfoListLE = new List<BluetoothLEDeviceInfo>();

        MenuWin menu = null;
        private ICommWrapper wrapper = null;

        #endregion

        #region Constructors and window events

        public MainWindow() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.OnStartupSuccess();
            this.wrapper.LanguageChanged += this.Languages_LanguageChanged;

            this.wrapper.BLE_DeviceDiscovered += this.BlueToothLE_DeviceDiscovered;

            //this.wrapper.Blu
            this.wrapper.BTClassicDeviceDiscovered += this.BlueTooth_DiscoveredBTDevice;
            this.wrapper.BTClassicDiscoveryComplete += this.BlueTooth_DiscoveryComplete;
            this.wrapper.BTClassicConnectionCompleted += this.BlueTooth_ConnectionCompleted;
            this.wrapper.BTClassicBytesReceived += this.BlueTooth_BytesReceived;

            // TODO - remove - temp populate command box
            this.outgoing.Items.Add("First msg");
            this.outgoing.Items.Add("Second msg");
            this.outgoing.Items.Add("A bit of nothing");
            this.outgoing.Items.Add("Start doing something");
            this.outgoing.Items.Add("Stop doing int");

        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.menu = new MenuWin();
            this.menu.Visibility = Visibility.Collapsed;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            // TODO call the wrapper teardown

            this.wrapper.BLE_DeviceDiscovered -= this.BlueToothLE_DeviceDiscovered;
            this.wrapper.BTClassicDeviceDiscovered -= this.BlueTooth_DiscoveredBTDevice;
            this.wrapper.BTClassicDiscoveryComplete -= this.BlueTooth_DiscoveryComplete;
            this.wrapper.BTClassicConnectionCompleted -= this.BlueTooth_ConnectionCompleted;
            this.wrapper.BTClassicBytesReceived -= this.BlueTooth_BytesReceived;


            if (this.menu != null) {
                this.menu.Close();
            }

            this.wrapper.Teardown();
        }

        /// <summary>Close opened menu window anywhere on window on mouse down</summary>
        private void Window_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            if (this.menu.IsVisible) {
                this.menu.Hide();
            }
        }

        #endregion

        #region Button events

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            // TODO need to disconnect any connected medium
            this.Close();
        }

        #endregion

        #region Bluetooth LE

        /// <summary>Event handler for Bluetooth LE device discovery. Adds one at a time</summary>
        /// <param name="sender">The sender of event</param>
        /// <param name="info">The information for discovered device</param>
        private void BlueToothLE_DeviceDiscovered(object sender, BluetoothLEDeviceInfo info) {
            this.Dispatcher.Invoke(() => {
                // Disconnect the list from control before changing. Maybe change to Observable collection
                this.lbBluetoothLE.ItemsSource = null;
                this.btInfoListLE.Add(info);
                this.lbBluetoothLE.ItemsSource = this.btInfoListLE;
            });
        }


        /// <summary>Clear Bluetooth LE device list and Launch device discovery</summary>
        /// <param name="sender">Click sender</param>
        /// <param name="e">Routed argument. Not used</param>
        private void btnDiscoverLE_Click(object sender, RoutedEventArgs e) {
            this.lbBluetoothLE.ItemsSource = null;
            this.btInfoListLE.Clear();
            this.lbBluetoothLE.ItemsSource = this.btInfoListLE;
            this.wrapper.BLE_DiscoverAsync();
        }

        private void btnInfoLE_Click(object sender, RoutedEventArgs e) {
            if (this.lbBluetoothLE.SelectedItem != null) {
                BluetoothLEDeviceInfo info = this.lbBluetoothLE.SelectedItem as BluetoothLEDeviceInfo;
                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("       Id: {0}\n", info.Id));//.Append("\n");
                sb.Append(string.Format("IsDefault: {0}\n", info.IsDefault));
                sb.Append(string.Format("IsEnabled: {0}\n", info.IsEnabled));
                //sb.Append("     Kind: {0}", device.Kind);
                // Properties
                sb.Append(string.Format("Properties: ({0})\n", info.LEProperties.Count));
                foreach (var p in info.LEProperties) {
                    if (p.Item2.Length > 0) {
                        sb.Append(string.Format("   {0} : {1}\n", p.Item1, p.Item2));
                    }
                    else {
                        sb.Append(string.Format("   {0}\n", p.Item1));
                    }
                }
                //// Enclosure location
                //if (device.EnclosureLocation != null) {
                //    System.Diagnostics.Debug.WriteLine("EnclosureLocation:");
                //    System.Diagnostics.Debug.WriteLine("     InDock: {0}", device.EnclosureLocation.InDock);
                //    System.Diagnostics.Debug.WriteLine("      InLid: {0}", device.EnclosureLocation.InLid);
                //    System.Diagnostics.Debug.WriteLine("      Panel: {0}", device.EnclosureLocation.Panel);
                //    System.Diagnostics.Debug.WriteLine("      Angle: {0}", device.EnclosureLocation.RotationAngleInDegreesClockwise);
                //}
                //else {
                //    System.Diagnostics.Debug.WriteLine("EnclosureLocation: null");
                //}
                //// Pairing
                //if (device.Pairing != null) {
                //    System.Diagnostics.Debug.WriteLine("Pairing:");
                //    System.Diagnostics.Debug.WriteLine("    CanPair: {0}", device.Pairing.CanPair);
                //    System.Diagnostics.Debug.WriteLine("   IsPaired: {0}", device.Pairing.IsPaired);
                //    System.Diagnostics.Debug.WriteLine(" Protection: {0}", device.Pairing.ProtectionLevel);
                //    if (device.Pairing.Custom != null) {
                //        System.Diagnostics.Debug.WriteLine("     Custom: not null");
                //    }
                //    else {
                //        System.Diagnostics.Debug.WriteLine("Custom: null");
                //    }
                //}
                //else {
                //    System.Diagnostics.Debug.WriteLine("Custom: null");
                //}

                MessageBox.Show(sb.ToString(), info.Name);
            }

        }

        private void btnLEConnect_Click(object sender, RoutedEventArgs e) {
            if (this.lbBluetoothLE.SelectedItem != null) {
                try {
                    BluetoothLEDeviceInfo info = this.lbBluetoothLE.SelectedItem as BluetoothLEDeviceInfo;
                    this.wrapper.BLE_ConnectAsync(info);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }

        #endregion

        #region Bluetooth

        private void btnDiscover_Click(object sender, RoutedEventArgs e) {
            // Asynchronous call
            this.lbBluetooth.ItemsSource = null;
            this.btInfoList.Clear();
            this.lbBluetooth.ItemsSource = this.btInfoList;
            this.gridWait.Visibility = Visibility.Visible;
            this.wrapper.BTClassicDiscoverAsync();
        }

        private void btnBTConnect_Click(object sender, RoutedEventArgs e) {
            BTDeviceInfo item = this.lbBluetooth.SelectedItem as BTDeviceInfo;
            if (item != null) {
                this.gridWait.Visibility = Visibility.Visible;
                this.wrapper.BTClassicConnectAsync(item);
            }
        }


        private void BlueTooth_DiscoveredBTDevice(object sender, BTDeviceInfo dev) {
            this.Dispatcher.Invoke(() => {
                this.lbBluetooth.ItemsSource = null;
                this.btInfoList.Add(dev);
                this.lbBluetooth.ItemsSource = this.btInfoList;
            });
        }

        private void BlueTooth_DiscoveryComplete(object sender, bool e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Visibility = Visibility.Collapsed;
            });
        }


        private void BlueTooth_BytesReceived(object sender, string msg) {
            this.Dispatcher.Invoke(() => {
                this.lbIncoming.Items.Add(msg);
            });
        }


        private void BlueTooth_ConnectionCompleted(object sender, bool e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Visibility = Visibility.Collapsed;
            });
        }


        #endregion

        #region Private

        private void OnStartupSuccess() {
            // TODO for now init manually
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Bluetooth Classic", CommMediumType.Bluetooth));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("BluetoothLE", CommMediumType.BluetoothLE));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Ethernet", CommMediumType.Ethernet));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Wifi", CommMediumType.Wifi));
            this.cbComm.ItemsSource = this.mediumGroup.Mediums;
            this.cbComm.SelectedIndex = 0;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        private void cbComm_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Hide all the options
            this.spBluetooth.Visibility = Visibility.Collapsed;
            this.spBluetoothLE.Visibility = Visibility.Collapsed;
            this.btnLEConnect.Visibility = Visibility.Collapsed;
            this.spEthernet.Visibility = Visibility.Collapsed;
            this.spWifi.Visibility = Visibility.Collapsed;

            this.btnDiscover.Visibility = Visibility.Collapsed;
            this.btnDiscoverLE.Visibility = Visibility.Collapsed;

            // TODO - disconnect on switch
            switch ((this.cbComm.SelectedItem as CommMedialDisplay).MediumType) {
                case CommMediumType.Bluetooth:
                    this.spBluetooth.Visibility = Visibility.Visible;
                    this.btnDiscover.Visibility = Visibility.Visible;
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

        #endregion

        #region Title bar events

        /// <summary>Grab window to start drag move when mouse clicks down on my title bar</summary>
        private void lbTitle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            App.Current.MainWindow.DragMove();
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
        private void Languages_LanguageChanged(object sender, LanguageFactory.Messaging.SupportedLanguage lang) {
            // Buttons
            this.btnExit.Content = lang.GetText(MsgCode.exit);
            this.btnSend.Content = lang.GetText(MsgCode.send);
            this.btnBTConnect.Content = lang.GetText(MsgCode.connect);
            this.btnLEConnect.Content = lang.GetText(MsgCode.connect);
            this.btnDiscover.Content = lang.GetText(MsgCode.discover);
            this.btnDiscoverLE.Content = lang.GetText(MsgCode.discover);
            this.btnInfoLE.Content =lang.GetText(MsgCode.info);

            // Labels
            this.lbCommand.Content = lang.GetText(MsgCode.command);
            this.lbResponse.Content = lang.GetText(MsgCode.response);

            // TODO Other texts
        }

        #endregion

    }
}
