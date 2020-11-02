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


        public class StopBitDisplayClass {
            public string Display { get; set; } = string.Empty;
            public SerialStopBits StopBits { get; set; } = SerialStopBits.One;
            public StopBitDisplayClass(SerialStopBits sb) {
                this.Display = sb.Display();
                this.StopBits = sb;
            }
        }

        public class SerialParityDisplayClass {
            public string Display { get; set; } = string.Empty;
            public SerialParityType ParityType { get; set; } = SerialParityType.None;
            public SerialParityDisplayClass(SerialParityType pt) {
                this.Display = pt.Display();
                this.ParityType = pt;
            }
        }



        List<uint> bauds = new List<uint>();
        List<ushort> dataBits = new List<ushort>();
        List<StopBitDisplayClass> stopBits = new List<StopBitDisplayClass>();
        List<SerialParityDisplayClass> paritys = new List<SerialParityDisplayClass>();


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
            this.bauds.Add(300);
            this.bauds.Add(600);
            this.bauds.Add(1200);
            this.bauds.Add(2400);
            this.bauds.Add(4800);
            this.bauds.Add(9600);
            this.bauds.Add(14400);
            this.bauds.Add(19200);
            this.bauds.Add(28800);
            this.bauds.Add(31250);
            this.bauds.Add(38400);
            this.bauds.Add(57600);
            this.bauds.Add(115200);
            this.cbBaud.ItemsSource = this.bauds;
            this.cbBaud.SelectedIndex = 0; 
            // Set from incoming

        }


        private void InitDataBits() {
            // Must be 5-8
            this.dataBits.Add(5);
            this.dataBits.Add(6);
            this.dataBits.Add(7);
            this.dataBits.Add(8);
            this.cbDataBits.ItemsSource = this.dataBits;
            this.cbDataBits.SelectedIndex = 0;
        }


        private void InitStopBits() {
            this.stopBits.Add(new StopBitDisplayClass(SerialStopBits.One));
            this.stopBits.Add(new StopBitDisplayClass(SerialStopBits.OnePointFive));
            this.stopBits.Add(new StopBitDisplayClass(SerialStopBits.Two));
            this.cbStopBits.ItemsSource = this.stopBits;
            this.cbStopBits.SelectedIndex = 0;
        }

        private void InitSerialParityTypes() {
            this.paritys.Add(new SerialParityDisplayClass(SerialParityType.None));
            this.paritys.Add(new SerialParityDisplayClass(SerialParityType.Even));
            this.paritys.Add(new SerialParityDisplayClass(SerialParityType.Odd));
            this.paritys.Add(new SerialParityDisplayClass(SerialParityType.Mark));
            this.paritys.Add(new SerialParityDisplayClass(SerialParityType.Space));
            this.cbParity.ItemsSource = this.paritys;
            this.cbParity.SelectedIndex = 0;
        }
    }
}
