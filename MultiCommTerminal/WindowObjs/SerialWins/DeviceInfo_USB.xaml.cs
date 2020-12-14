using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using SerialCommon.Net.DataModels;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.SerialWins {

    /// <summary>Interaction logic for DeviceInfo_USB.xaml</summary>
    public partial class DeviceInfo_USB : Window {

        #region Data

        private Window parent = null;
        private SerialDeviceInfo info = null;

        #endregion


        public static void ShowBox(Window parent, SerialDeviceInfo info) {
            DeviceInfo_USB win = new DeviceInfo_USB(parent, info);
            win.ShowDialog();
        }


        public DeviceInfo_USB(Window parent, SerialDeviceInfo info) {
            this.parent = parent;
            this.info = info;
            InitializeComponent();
            this.listboxMain.ItemsSource = DI.Wrapper.Serial_GetDeviceInfoForDisplay(info);
            this.Title = info.PortName;
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


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            // Will have to reload values after
            if (DeviceEdit_USB.ShowBox(this.parent, this.info)) {
                this.listboxMain.ItemsSource = null;
                this.listboxMain.ItemsSource = DI.Wrapper.Serial_GetDeviceInfoForDisplay(info);
            }
        }
    }
}
