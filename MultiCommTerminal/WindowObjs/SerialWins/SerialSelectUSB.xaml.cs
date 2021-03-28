using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using SerialCommon.Net.DataModels;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using WpfCustomControlLib.Core.Helpers;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.SerialWins {

    /// <summary>Interaction logic for SerialSelectUSB.xaml</summary>
    public partial class SerialSelectUSB : Window {

        private Window parent = null;
        private List<SerialDeviceInfo> usbItems = new List<SerialDeviceInfo>();
        private AutoResetEvent doneLoading = new AutoResetEvent(false);

        public SerialDeviceInfo SerialInfo { get; private set; } = null;

        #region Constructors and window events

        public static SerialDeviceInfo ShowBox(Window parent, bool isSelect) {
            SerialSelectUSB win = new SerialSelectUSB(parent, isSelect);
            win.ShowDialog();
            return win.SerialInfo;
        }


        public SerialSelectUSB(Window parent, bool isSelect) {
            this.parent = parent;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            if (!isSelect) {
                this.btnSelect.Collapse();
                this.btnCancel.Collapse();
                this.btnExit.Show();
            }
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
            this.ReloadList();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            DI.Wrapper.SerialDiscoveredDevices -= this.discoveredDevices;
            DI.Wrapper.SerialOnError -= this.onSerialOnError;
        }

        #endregion

        #region Button events

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            SerialDeviceInfo info = this.lbUsb.SelectedItem as SerialDeviceInfo;
            if (info != null) {
                this.SerialInfo = info;
                this.Close();
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            SerialDeviceInfo info = this.lbUsb.SelectedItem as SerialDeviceInfo;
            if (info != null) {
                int index = this.lbUsb.SelectedIndex;
                if (DeviceEdit_USB.ShowBox(this, info)) {
                    this.ReloadList();
                    this.WaitForLoad(index);
                }
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            SerialDeviceInfo info = this.lbUsb.SelectedItem as SerialDeviceInfo;
            DI.Wrapper.DeleteSerialCfg(info, this.DeleteDecision, this.ReloadList, App.ShowMsg);
        }

        #endregion

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
                    this.usbItems = items;
                    this.lbUsb.ItemsSource = this.usbItems;
                    this.doneLoading.Set();
                }
            });
        }

        #endregion

        #region Private

        /// <summary>Get spinner going and reload list of USB devices</summary>
        /// <param name="dummy">Dummy parameter so it can be used as a delegate</param>
        private void ReloadList(bool dummy = true) {
            this.doneLoading.Reset();
            //this.gridWait.Focus();
            this.gridWait.Show();
            DI.Wrapper.SerialUsbDiscoverAsync();
        }


        private bool DeleteDecision(string portName) {
            return MsgBoxYesNo.ShowBoxDelete(this, portName) == MsgBoxYesNo.MsgBoxResult.Yes;
        }


        /// <summary>Wait for the list of USB to load again from OS with added params</summary>
        /// <param name="ndx">Set the same selected as before the operation</param>
        private void WaitForLoad(int ndx) {
            Task.Run(() => {
                try {
                    if (this.doneLoading.WaitOne(2000)) {
                        this.Dispatcher.Invoke(() => {
                            try {
                                if (this.usbItems.Count > ndx) {
                                    this.lbUsb.SelectedIndex = ndx;
                                }
                            }
                            catch (Exception) { }
                        });
                    }
                }
                catch (Exception) { }
            });
        }

        #endregion

    }
}
