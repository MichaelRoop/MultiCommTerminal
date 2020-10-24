using BluetoothLE.Net.DataModels;
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

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLE_ServicesDisplay.xaml</summary>
    public partial class BLE_ServicesDisplay : Window {

        Window parent = null;

        public static void ShowBox(Window parent, BluetoothLEDeviceInfo info) {
            BLE_ServicesDisplay win = new BLE_ServicesDisplay(parent, info);
            win.ShowDialog();
        }


        public BLE_ServicesDisplay(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.treeServices.ItemsSource = info.Services.Values;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
