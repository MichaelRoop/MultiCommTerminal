using BluetoothLE.Net.DataModels;
using LogUtils.Net;
using MultiCommTerminal.DependencyInjection;
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
        //BluetoothLEDeviceInfo info = null;
        private List<DisplaySet> mainValues = new List<DisplaySet>();
        private List<DisplaySet> serviceProperties = new List<DisplaySet>();


        public static void ShowBox(Window parent, BluetoothLEDeviceInfo info) {
            DeviceInfo_BLE win = new DeviceInfo_BLE(parent, info);
            win.ShowDialog();
        }


        public DeviceInfo_BLE(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();
            //this.info = info;
            this.PopulateFields(info);
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


        private void PopulateFields(BluetoothLEDeviceInfo info) {
            this.mainValues.Add(new DisplaySet("Name", info.Name));
            this.mainValues.Add(new DisplaySet("Id", info.Id));
            this.mainValues.Add(new DisplaySet("Access Status", info.AccessStatus.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Address", info.AddressAsULong.ToString()));
            this.mainValues.Add(new DisplaySet("Address Type", info.AddressType.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Default", info.IsDefault.ToString()));
            this.mainValues.Add(new DisplaySet("Enabled", info.IsEnabled.ToString()));
            this.mainValues.Add(new DisplaySet("Kind", info.Kind.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Can Pair", info.CanPair.ToString()));
            this.mainValues.Add(new DisplaySet("Paired", info.IsPaired.ToString()));
            this.mainValues.Add(new DisplaySet("Paired using secure connection", info.WasPairedUsingSecureConnection.ToString()));
            this.mainValues.Add(new DisplaySet("Connectable", info.IsConnectable.ToString()));
            this.mainValues.Add(new DisplaySet("Connected", info.IsConnected.ToString()));
            this.mainValues.Add(new DisplaySet("Protection Level", info.ProtectionLevel.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Type", info.TypeBluetooth.ToString().CamelCaseToSpaces()));
            this.mainValues.Add(new DisplaySet("Enclosure Location - Dock", info.EnclosureLocation.InDock.ToString()));
            this.mainValues.Add(new DisplaySet("Enclosure Location - Lid", info.EnclosureLocation.InLid.ToString()));
            this.mainValues.Add(new DisplaySet("Enclosure Location - Clockwise Rotation", info.EnclosureLocation.ClockWiseRotationInDegrees.ToString()));
            this.mainValues.Add(new DisplaySet("Enclosure Panel Location", info.EnclosureLocation.Location.ToString()));
            this.listboxMain.ItemsSource = this.mainValues;


            foreach (var sp in info.ServiceProperties) {
                this.serviceProperties.Add(new DisplaySet(sp.Key, sp.Value));
            }
            this.listboxProperties.ItemsSource = this.serviceProperties;


            //this.info.Services
            if (info.Services == null || info.Services.Count == 0) {
                this.treeServices.Collapse();
                this.txtServices.Collapse();
            }
            else {
                this.treeServices.ItemsSource = info.Services.Values;
            }
        }


        #region Debug code for dev

        ServiceTreeDict treeDict = new ServiceTreeDict();
        private void CreateDebugTree() {
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

            BLE_ServiceDataModel service2 = new BLE_ServiceDataModel();
            service2.DisplayName = "Hogwarts 2 service";
            service2.Characteristics.Add("1", new BLE_CharacteristicDataModel());
            service2.Characteristics["1"].CharName = "Hermioni characteristic";
            service2.Characteristics.Add("2", new BLE_CharacteristicDataModel());
            service2.Characteristics["2"].CharName = "Ginny characteristic";
            this.treeDict.Add("2", service2);

            // Just pass list of Values to avoid headach in XAML    
            this.treeServices.ItemsSource = this.treeDict.Values;
        }


        #endregion

    }
}
