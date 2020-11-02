using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.Enumerations;
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
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            this.HideTitleBarIcon();
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
            // TODO - persistance if the 'Save' is checked


            this.Changed = true;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        // Temp. get the values from wrapper

        private void Init() {
            // Load the drop downs
            // Select the values from serial device if legit (i.e. not 0?)
            this.InitBauds();
            this.InitDataBits();
            this.InitStopBits();
            this.InitSerialParityTypes();
        }





        private void InitBauds() {
            this.cbBaud.ItemsSource = DI.Wrapper.Serial_GetBaudsForDisplay();
            this.cbBaud.SelectedIndex = 0; 
            // Set from incoming
        }


        private void InitDataBits() {
            // Must be 5-8
            this.cbDataBits.ItemsSource = DI.Wrapper.Serial_GetDataBits();
            this.cbDataBits.SelectedIndex = 0;
        }


        private void InitStopBits() {
            this.cbStopBits.ItemsSource = DI.Wrapper.Serial_GetStopBitsForDisplay();
            this.cbStopBits.SelectedIndex = 0;
        }

        private void InitSerialParityTypes() {
            this.cbParity.ItemsSource = DI.Wrapper.Serial_GetParitysForDisplay();
            this.cbParity.SelectedIndex = 0;
        }
    }
}
