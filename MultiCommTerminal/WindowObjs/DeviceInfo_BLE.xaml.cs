using BluetoothLE.Net.DataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BLE.xaml</summary>
    public partial class DeviceInfo_BLE : Window {


        Window parent = null;


        public static void ShowBox(Window parent, BluetoothLEDeviceInfo info) {
            DeviceInfo_BLE win = new DeviceInfo_BLE(parent, info);
            win.ShowDialog();
        }


        public DeviceInfo_BLE(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();
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


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void PopulateFields(BluetoothLEDeviceInfo info) {
            this.listboxMain.ItemsSource = DI.Wrapper.BLE_GetDeviceInfoForDisplay(info);
            this.listboxProperties.ItemsSource = DI.Wrapper.BLE_GetServiceProperties(info);
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
