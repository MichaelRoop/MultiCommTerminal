using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BTWins {

    /// <summary>Interaction logic for BWSelect.xaml</summary>
    public partial class BTSelect : Window {

        private Window parent = null;
        private bool pair = false;
        private List<BTDeviceInfo> items = new List<BTDeviceInfo>();

        public BTDeviceInfo SelectedBT { get; private set; } = null;


        public static BTDeviceInfo ShowBox(Window parent, bool pair = true) {
            BTSelect win = new BTSelect(parent, pair);
            win.ShowDialog();
            return win.SelectedBT;
        }


        public BTSelect(Window parent, bool pair = true) {
            this.parent = parent;
            this.pair = pair;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            DI.Wrapper.BT_DeviceDiscovered += this.deviceDiscovered;
            DI.Wrapper.BT_DiscoveryComplete += this.discoveryComplete;
            this.gridWait.Show();
            DI.Wrapper.BTClassicDiscoverAsync(this.pair);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            DI.Wrapper.BT_DeviceDiscovered -= this.deviceDiscovered;
            DI.Wrapper.BT_DiscoveryComplete -= this.discoveryComplete;
            this.lbBluetooth.SelectionChanged -= this.selectionChanged;
        }


        private void deviceDiscovered(object sender, BTDeviceInfo device) {
            Log.Info("BTSelect", "deviceDiscovered", () => string.Format("Found {0}", device.Name));
            this.Dispatcher.Invoke(() => this.lbBluetooth.Add(this.items, device));
        }


        private void discoveryComplete(object sender, bool e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (this.items.Count == 0) {
                    App.ShowMsgTitle("", DI.Wrapper.GetText(MsgCode.NotFound));
                    this.Close();
                }
                else {
                    this.lbBluetooth.SelectionChanged += this.selectionChanged;
                }
            });
        }


        private void selectionChanged(object sender, SelectionChangedEventArgs e) {
            BTDeviceInfo info = this.lbBluetooth.SelectedItem as BTDeviceInfo;
            if (info != null) {
                this.SelectedBT = info;
                this.Close();
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
