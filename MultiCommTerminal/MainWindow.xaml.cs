﻿using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using MultiCommData.UserDisplayData;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal {

    /// <summary>Interaction logic for MainWindow.xaml</summary>
    public partial class MainWindow : Window {

        #region Data

        private MediumGroup mediumGroup = new MediumGroup();
        //private CommMediumType currentMedium = CommMediumType.None;
        private List<BTDeviceInfo> btInfoList = new List<BTDeviceInfo>();
        private List<BluetoothLEDeviceInfo> btInfoListLE = new List<BluetoothLEDeviceInfo>();

        // TODO move out of UI
        private IBTInterface blueTooth = new BluetoothClassic.BluetoothClassicImpl();
        private IBLETInterface blueToothLE = new BluetoothLE.Win32.BluetoothLEImplWin32();

        #endregion

        #region Constructors and window events

        public MainWindow() {
            InitializeComponent();
            this.OnStartupSuccess();
            this.blueToothLE.DeviceDiscovered += this.BlueToothLE_DeviceDiscovered;
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            // Must force the window size down
            this.Width = this.grdMain.ActualWidth;
            this.Height = this.grdMain.ActualHeight + 40; // TODO Weird have to add this
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
            this.blueToothLE.DiscoverDevices();
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
                    this.blueToothLE.Connect(info);
                }
                catch (Exception ex) {
                    System.Diagnostics.Debug.WriteLine(ex.Message);
                }
            }
        }



        #endregion

        #region Bluetooth

        private void btnDiscover_Click(object sender, RoutedEventArgs e) {
            this.btInfoList.Clear();
            this.lbBluetooth.ItemsSource = null;

            // Move out of UI
            this.btInfoList = this.blueTooth.DiscoverDevices();
            this.lbBluetooth.ItemsSource = this.btInfoList;

            if (this.btInfoList.Count == 0) {
                MessageBox.Show(string.Format("Number of devices {0}", this.btInfoList.Count));
            }
        }

        #endregion

        #region Private

        private void OnStartupSuccess() {
            // TODO for now init manually
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("BluetoothLE", CommMediumType.BluetoothLE));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Bluetooth Classic", CommMediumType.Bluetooth));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Ethernet", CommMediumType.Ethernet));
            this.mediumGroup.Mediums.Add(new CommMedialDisplay("Wifi", CommMediumType.Wifi));
            this.cbComm.ItemsSource = this.mediumGroup.Mediums;
            this.cbComm.SelectedIndex = 0;
        }


        private void cbComm_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            //MessageBox.Show(String.Format("Enum selected:{0}",
            //    (this.cbComm.SelectedItem as CommMedialDisplay).MediumType.ToString()));

            // Hide all the options
            this.spBluetooth.Visibility = Visibility.Collapsed;
            this.spBluetoothLE.Visibility = Visibility.Collapsed;
            this.btnLEConnect.Visibility = Visibility.Collapsed;
            this.spEthernet.Visibility = Visibility.Collapsed;
            this.spWifi.Visibility = Visibility.Collapsed;

            this.btnDiscover.Visibility = Visibility.Collapsed;
            this.btnDiscoverLE.Visibility = Visibility.Collapsed;

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

        #endregion
    }
}
