using BluetoothLE.Net.DataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.BLE;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BLE.xaml</summary>
    public partial class DeviceInfo_BLE : Window {


        Window parent = null;
        BluetoothLEDeviceInfo info = null;
        private ButtonGroupSizeSyncManager widthManager = null;


        public static void ShowBox(Window parent, BluetoothLEDeviceInfo info) {
            DeviceInfo_BLE win = new DeviceInfo_BLE(parent, info);
            win.ShowDialog();
        }


        public DeviceInfo_BLE(Window parent, BluetoothLEDeviceInfo info) {
            this.parent = parent;
            this.info = info;
            InitializeComponent();
            this.PopulateFields();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


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


        private void btnServices_Click(object sender, RoutedEventArgs e) {
            BLE_ServicesDisplay.ShowBox(this, this.info);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void PopulateFields() {
            this.listboxMain.ItemsSource = DI.Wrapper.BLE_GetDeviceInfoForDisplay(info);
            if (info.Services.Count == 0) {
                this.btnServices.Collapse();
                this.widthManager = new ButtonGroupSizeSyncManager(this.btnProperties, this.btnExit);
            }
            else {
                this.widthManager = new ButtonGroupSizeSyncManager(this.btnProperties, this.btnServices, this.btnExit);
            }
            this.widthManager.PrepForChange();
        }


        private void btnProperties_Click(object sender, RoutedEventArgs e) {
            BLE_PropertiesDisplay.ShowBox(this.parent, this.info);

        }
    }
}
