using BluetoothLE.Net.DataModels;
using MultiCommTerminal.WPF_Helpers;
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
using VariousUtils.Net;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BLE.xaml</summary>
    public partial class DeviceInfo_BLE : Window {

        public class DisplaySet {
            public string Description { get; set; } = string.Empty;
            public string Value { get; set; } = string.Empty;
            public string DataType { get; set; } = string.Empty;
            public DisplaySet() { }
            public DisplaySet(string desc, string value) {
                this.Description = desc;
                this.Value = value;
            }
            public DisplaySet(string desc, string value, string dataType) {
                this.Description = desc;
                this.Value = value;
                this.DataType = dataType;
            }


            public DisplaySet(string desc, BLE_PropertyDataModel data) {
                this.Description = desc;
                if (data.DataType == PropertyDataType.TypeString) {
                    this.Value = string.Format("\"{0}\"", data.Value.ToString());
                }
                else {
                    this.Value = data.Value.ToString();
                }
                this.DataType = data.DataType.ToFriendlyString();
            }
        }


        Window parent = null;
        ServiceTreeDict treeDict = new ServiceTreeDict();
        BluetoothLEDeviceInfo info = null;
        private List<DisplaySet> mainValues = new List<DisplaySet>();
        private List<DisplaySet> serviceProperties = new List<DisplaySet>();


        public static void ShowBox(Window parent, BluetoothLEDeviceInfo info) {
            DeviceInfo_BLE win = new DeviceInfo_BLE(parent, info);
            win.ShowDialog();
        }


        public DeviceInfo_BLE(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();

            #region Test data for info display
            // ---------------------------------------------
            BLE_ServiceDataModel service1 = new BLE_ServiceDataModel();
            service1.DisplayName = "Hogwarts 1 service";
            service1.Characteristics.Add("1", new BLE_CharacteristicDataModel());
            service1.Characteristics["1"].CharName = "George Characteristic";
            service1.Characteristics["1"].Descriptors.Add("1", new BLE_DescriptorDataModel());
            service1.Characteristics["1"].Descriptors["1"].DisplayName = "Output descriptor";
            service1.Characteristics.Add("2", new BLE_CharacteristicDataModel());
            service1.Characteristics["2"].CharName = "Fred Characteristic";
            service1.Characteristics["2"].Descriptors.Add("1", new BLE_DescriptorDataModel());
            service1.Characteristics["2"].Descriptors["1"].DisplayName = "Input descriptor";
            service1.Characteristics["2"].Descriptors.Add("2", new BLE_DescriptorDataModel());
            service1.Characteristics["2"].Descriptors["2"].DisplayName = "Name of stuff";
            this.treeDict.Add("1", service1);
            // ---------------------------------------------

            // ---------------------------------------------
            BLE_ServiceDataModel service2 = new BLE_ServiceDataModel();
            service2.DisplayName = "Hogwarts 2 service";
            service2.Characteristics.Add("1", new BLE_CharacteristicDataModel());
            service2.Characteristics["1"].CharName = "Hermioni characteristic";
            service2.Characteristics.Add("2", new BLE_CharacteristicDataModel());
            service2.Characteristics["2"].CharName = "Ginny characteristic";
            this.treeDict.Add("2", service2);

            //// Just pass list of Values to avoid headach in XAML    
            //this.treeServices.ItemsSource = this.treeDict.Values;
            #endregion

            this.info = info;
            this.mainValues.Add(new DisplaySet("Name", this.info.Name));
            this.mainValues.Add(new DisplaySet("Id", this.info.Id));
            this.mainValues.Add(new DisplaySet("Access Status", this.info.AccessStatus.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Address", this.info.AddressAsULong.ToString()));
            this.mainValues.Add(new DisplaySet("Address Type", this.info.AddressType.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Default", this.info.IsDefault.ToString()));
            this.mainValues.Add(new DisplaySet("Enabled", this.info.IsEnabled.ToString()));
            this.mainValues.Add(new DisplaySet("Kind", this.info.Kind.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Can Pair", this.info.CanPair.ToString()));
            this.mainValues.Add(new DisplaySet("Paired", this.info.IsPaired.ToString()));
            this.mainValues.Add(new DisplaySet("Paired using secure connection", this.info.WasPairedUsingSecureConnection.ToString()));
            this.mainValues.Add(new DisplaySet("Connectable", this.info.IsConnectable.ToString()));
            this.mainValues.Add(new DisplaySet("Connected", this.info.IsConnected.ToString()));
            this.mainValues.Add(new DisplaySet("Protection Level", this.info.ProtectionLevel.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Type", this.info.TypeBluetooth.ToString().CamelCaseToSpaces()));
            this.listboxMain.ItemsSource = this.mainValues;

            //this.info.EnclosureLocation

            foreach (var sp in this.info.ServiceProperties) {
                this.serviceProperties.Add(new DisplaySet(sp.Key, sp.Value));
            }
            this.listboxProperties.ItemsSource = this.serviceProperties;


            //this.info.Services
            if (this.info.Services == null || this.info.Services.Count == 0) {
                // Just pass list of Values to avoid headach in XAML    
                this.treeServices.ItemsSource = this.treeDict.Values;
            }
            else {
                // TODO - check if any previous selections
                this.treeServices.ItemsSource = this.info.Services.Values;
            }

            this.SizeToContent = SizeToContent.WidthAndHeight;

        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            // TODO REMOVE
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            // TODO REMOVE
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
