using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WifiCommon.Net.DataModels;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.WifiWins {

    /// <summary>Interaction logic for WifiSelect.xaml</summary>
    public partial class WifiSelect : Window {

        private Window parent = null;

        public WifiNetworkInfo SelectedWifi { get; private set; } = null;


        public static WifiNetworkInfo ShowBox(Window parent) {
            WifiSelect win = new WifiSelect(parent);
            win.ShowDialog();
            return win.SelectedWifi;
        }


        public WifiSelect(Window parent, bool paired = true) {
            this.parent = parent;
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

            DI.Wrapper.DiscoveredWifiNetworks += this.discoveredWifiNetworks;
            DI.Wrapper.OnWifiError += this.onWifiError;
            this.gridWait.Show();
            DI.Wrapper.WifiDiscoverAsync();
            // start the load

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            DI.Wrapper.DiscoveredWifiNetworks -= this.discoveredWifiNetworks;
            DI.Wrapper.OnWifiError -= this.onWifiError;
            this.lbWifi.SelectionChanged -= this.wifiSelectionChanged;
        }


        private void onWifiError(object sender, WifiError e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                App.ShowMsg(string.Format("{0} '{1}'", e.Code, e.ExtraInfo));
                this.Close();
            });
        }


        private void discoveredWifiNetworks(object sender, List<WifiNetworkInfo> e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (e.Count == 0) {
                    App.ShowMsgTitle("", DI.Wrapper.GetText(MsgCode.NotFound));
                    this.Close();
                }
                else {
                    this.lbWifi.ItemsSource = e;
                    this.lbWifi.SelectionChanged += this.wifiSelectionChanged;
                }
            });
        }


        private void wifiSelectionChanged(object sender, SelectionChangedEventArgs e) {
            WifiNetworkInfo info = this.lbWifi.SelectedItem as WifiNetworkInfo;
            if (info != null) {
                this.SelectedWifi = info;
                this.Close();
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
