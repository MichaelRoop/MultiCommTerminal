﻿using BluetoothCommon.Net;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BT.xaml</summary>
    public partial class DeviceInfo_BT : Window {

        #region Data

        private Window parent = null;
        private ICommWrapper wrapper = null;
        private BTDeviceInfo info = null;

        #endregion


        public DeviceInfo_BT(Window parent, BTDeviceInfo info) {
            this.parent = parent;
            this.info = info;
            InitializeComponent();
            this.Init(info);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.CenterToParent(this.parent);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void comboBoxFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.comboBoxFeatures.SelectedIndex = -1;
        }


        private void Init(BTDeviceInfo info) {
            this.lbName.Content = info.Name;
            this.lbAddress.Content = info.Address;
            this.lbDeviceClass.Content = string.Format("{0} ({1})", info.DeviceClassName, info.DeviceClassInt);
            this.lbServiceClass.Content = string.Format("{0} ({1})", info.ServiceClassName, info.ServiceClassInt);
            this.lbStrength.Content = info.Strength.ToString();
            this.lbLastSeen.Content = info.LastSeen.ToString();
            this.lbLastUsed.Content = info.LastUsed.ToString();
            this.lbRadioManufacturer.Content = info.Radio.Manufacturer;
            this.lbRadioLmp.Content = info.Radio.LinkManagerProtocol;
            this.comboBoxFeatures.ItemsSource = info.Radio.Features;
        }

    }
}