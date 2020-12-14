using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.SerialWins {

    /// <summary>Interaction logic for DeviceSelect_USB.xaml</summary>
    public partial class DeviceSelect_USB : Window {

        private Window parent = null;

        public static void ShowBox(Window parent) {
            DeviceSelect_USB win = new DeviceSelect_USB(parent);
            win.ShowDialog();
        }


        public DeviceSelect_USB(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.spEditButtons.Visibility = Visibility.Collapsed;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ReloadList(true);
            this.CenterToParent(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            // TODO remove
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            var item = this.lbUsb.SelectedItem as IIndexItem<SerialIndexExtraInfo>;
            if (item != null) {
                DI.Wrapper.RetrieveSerialCfg(item,
                    (device) => {
                        this.ReloadList(DeviceEdit_USB.ShowBox(this, device));
                    }, App.ShowMsg);
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            var item = this.lbUsb.SelectedItem as IIndexItem<SerialIndexExtraInfo>;
            if (item != null) {
                if (MsgBoxYesNo.ShowBoxDelete(this, item.Display) == MsgBoxYesNo.MsgBoxResult.Yes) {
                    DI.Wrapper.DeleteSerialCfg(item,  this.ReloadList, App.ShowMsg);
                }
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void lbUsb_SelectionChangedHander(object sender, SelectionChangedEventArgs e) {
            this.spEditButtons.Show();
        }


        private void ReloadList(bool isChanged) {
            if (isChanged) {
                this.lbUsb.SelectionChanged -= this.lbUsb_SelectionChangedHander;
                DI.Wrapper.GetSerialCfgList(
                    (list) => {
                        this.lbUsb.ItemsSource = null;
                        this.lbUsb.ItemsSource = list;

                    }, App.ShowMsg);
                this.lbUsb.SelectionChanged += this.lbUsb_SelectionChangedHander;
            }
        }

    }
}
