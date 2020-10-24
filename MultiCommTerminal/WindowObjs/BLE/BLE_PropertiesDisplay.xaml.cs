﻿using BluetoothLE.Net.DataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLE_PropertiesDisplay.xaml</summary>
    public partial class BLE_PropertiesDisplay : Window {

        private Window parent = null;

        public static void ShowBox(Window parent, BluetoothLEDeviceInfo info) {
            BLE_PropertiesDisplay win = new BLE_PropertiesDisplay(parent, info);
            win.ShowDialog();
        }


        public BLE_PropertiesDisplay(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();
            this.listboxProperties.ItemsSource = DI.Wrapper.BLE_GetServiceProperties(info);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}