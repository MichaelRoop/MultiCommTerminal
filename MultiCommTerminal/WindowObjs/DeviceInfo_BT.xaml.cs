using BluetoothCommon.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for DeviceInfo_BT.xaml</summary>
    public partial class DeviceInfo_BT : Window {

        #region Data

        private Window parent = null;

        #endregion


        public DeviceInfo_BT(Window parent, BTDeviceInfo info) {
            this.parent = parent;
            InitializeComponent();
            this.Init(info);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.CenterToParent(this.parent);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void Init(BTDeviceInfo info) {

            this.listboxMain.ItemsSource = DI.Wrapper.BT_GetDeviceInfoForDisplay(info);
            if (info.Radio.IsInitialized) {
                // Could have a radio info dialog
            }
            // TODO - assemble all the properties in a properties box

        }

    }
}
