using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Windows;
using WifiCommon.Net.DataModels;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.WifiWins {

    /// <summary>Interaction logic for WifiInfo.xaml</summary>
    public partial class WifiInfo : Window {

        private Window parent = null;
        private WifiNetworkInfo info = null;


        public static void ShowBox(Window parent, WifiNetworkInfo info) {
            WifiInfo win = new WifiInfo(parent, info);
            win.ShowDialog();
        }


        public WifiInfo(Window parent, WifiNetworkInfo info) {
            this.parent = parent;
            this.info = info;
            InitializeComponent();
            this.listboxMain.ItemsSource = DI.Wrapper.Wifi_GetDeviceInfoForDisplay(info);
            this.Title = info.SSID;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
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
