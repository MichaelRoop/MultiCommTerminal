using BluetoothCommon.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.BLE;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BT.xaml</summary>
    public partial class DeviceInfo_BT : Window {

        #region Data

        private Window parent = null;
        private BTDeviceInfo info = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #endregion


        public DeviceInfo_BT(Window parent, BTDeviceInfo info) {
            this.parent = parent;
            this.info = info;
            InitializeComponent();
            this.Init();

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnExit, this.btnProperties);
            this.widthManager.PrepForChange();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        private void btnProperties_Click(object sender, RoutedEventArgs e) {
            BLE_PropertiesDisplay.ShowBox(this, this.info);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void Init() {

            this.listboxMain.ItemsSource = DI.Wrapper.BT_GetDeviceInfoForDisplay(this.info);
            if (this.info.Radio.IsInitialized) {
                // Could have a radio info dialog
            }
            // TODO - assemble all the properties in a properties box

        }

    }
}
