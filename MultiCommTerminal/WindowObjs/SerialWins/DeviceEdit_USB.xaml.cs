using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.SerialWins {

    /// <summary>Interaction logic for DeviceEdit_USB.xamlsummary>
    public partial class DeviceEdit_USB : Window {

        private Window parent = null;
        private SerialDeviceInfo info = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        public bool Changed { get; set; } = false;

        public static bool ShowBox(Window parent, SerialDeviceInfo info) {
            DeviceEdit_USB win = new DeviceEdit_USB(parent, info);
            win.ShowDialog();
            return win.Changed;
        }


        public DeviceEdit_USB(Window parent, SerialDeviceInfo info) {
            this.parent = parent;
            this.info = info;
            InitializeComponent();
            this.Init();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.Title = info.PortName;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            // Save the values in the SerialDeviceInfo
            this.info.Baud = (uint)this.cbBaud.SelectedItem;
            this.info.DataBits = (ushort)this.cbDataBits.SelectedItem;
            this.info.StopBits = (this.cbStopBits.SelectedItem as StopBitDisplayClass).StopBits;
            this.info.Parity = (this.cbParity.SelectedItem as SerialParityDisplayClass).ParityType;
            this.info.FlowHandshake = (this.cbFlowControl.SelectedItem as FlowControlDisplay).FlowControl;
            this.info.ReadTimeout = this.GetValue(this.txtReadTimeout);
            this.info.WriteTimeout = this.GetValue(this.txtWriteTimeout);
            this.Changed = true;
            DI.Wrapper.CreateOrSaveSerialCfg(this.info.PortName, this.info, this.Close, this.OnSaveError);
        }

        private TimeSpan GetValue(TextBox txtBox) {
            if (txtBox.Text.Length == 0) {
                return TimeSpan.FromMilliseconds(0);
            }
            return TimeSpan.FromMilliseconds(int.Parse(txtBox.Text));
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        // Temp. get the values from wrapper

        private void Init() {
            DI.Wrapper.Serial_GetBaudsForDisplay(this.info, (bauds, ndx) => {
                this.cbBaud.ItemsSource = bauds;
                this.cbBaud.SelectedIndex = ndx;
            });
            DI.Wrapper.Serial_GetDataBitsForDisplay(this.info, (source, ndx) => {
                this.cbDataBits.ItemsSource = source;
                this.cbDataBits.SelectedIndex = ndx;
            });
            DI.Wrapper.Serial_GetStopBitsForDisplay(this.info, (source, ndx) => {
                this.cbStopBits.ItemsSource = source;
                this.cbStopBits.SelectedIndex = ndx;
            });
            DI.Wrapper.Serial_GetParitysForDisplay(this.info, (source, ndx) => {
                this.cbParity.ItemsSource = source;
                this.cbParity.SelectedIndex = ndx;
            });
            DI.Wrapper.Serial_FlowControlForDisplay(this.info, (source, ndx) => {
                this.cbFlowControl.ItemsSource = source;
                this.cbFlowControl.SelectedIndex = ndx;
            });
            this.txtReadTimeout.Text = this.info.ReadTimeout.TotalMilliseconds.ToString();
            this.txtWriteTimeout.Text = this.info.WriteTimeout.TotalMilliseconds.ToString();
        }


        private readonly Regex numOnly = new Regex("[^0-9]");
        private void txtWriteTimeout_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = numOnly.IsMatch(e.Text);
        }

        private void txtReadTimeout_PreviewTextInput(object sender, TextCompositionEventArgs e) {
            e.Handled = numOnly.IsMatch(e.Text);
        }


        private void OnSaveError(string err) {
            App.ShowMsg(err);
            this.Close();
        }

    }
}
