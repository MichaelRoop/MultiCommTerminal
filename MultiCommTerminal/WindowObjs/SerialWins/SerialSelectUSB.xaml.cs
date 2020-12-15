using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using SerialCommon.Net.DataModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.SerialWins {

    /// <summary>Interaction logic for SerialSelectUSB.xaml</summary>
    public partial class SerialSelectUSB : Window {

        private Window parent = null;

        public SerialDeviceInfo SerialInfo { get; private set; } = null;


        public static SerialDeviceInfo ShowBox(Window parent) {
            SerialSelectUSB win = new SerialSelectUSB(parent);
            win.ShowDialog();
            return win.SerialInfo;
        }


        public SerialSelectUSB(Window parent) {
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
            DI.Wrapper.SerialDiscoveredDevices += this.discoveredDevices;
            DI.Wrapper.SerialOnError += this.onSerialOnError;
            this.gridWait.Show();
            DI.Wrapper.SerialUsbDiscoverAsync();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            DI.Wrapper.SerialDiscoveredDevices -= this.discoveredDevices;
            DI.Wrapper.SerialOnError -= this.onSerialOnError;
            this.lbUsb.SelectionChanged -= this.usbSelectionChanged;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        #region Event handlers

        private void onSerialOnError(object sender, SerialUsbError e) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                App.ShowMsgTitle(e.PortName, string.Format("{0} '{1}'", e.Code, e.Message));
                this.Close();
            });
        }


        private void discoveredDevices(object sender, List<SerialDeviceInfo> items) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (items.Count == 0) {
                    App.ShowMsgTitle("", DI.Wrapper.GetText(MsgCode.NotFound));
                    this.Close();
                }
                else {
                    this.lbUsb.ItemsSource = items;
                    this.lbUsb.SelectionChanged += this.usbSelectionChanged;
                }
            });
        }


        private void usbSelectionChanged(object sender, SelectionChangedEventArgs e) {
            SerialDeviceInfo info = this.lbUsb.SelectedItem as SerialDeviceInfo;
            if (info != null) {
                this.SerialInfo = info;
                this.Close();
            }
        }

        #endregion

    }
}
