using BluetoothCommon.Net;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
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

        private void comboBoxFeatures_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.comboBoxFeatures.SelectedIndex = -1;
        }


        private void Init(BTDeviceInfo info) {
            this.lbName.Content = info.Name;
            this.lbAddress.Content = info.Address;
            this.lbDeviceClass.Content = string.Format("{0} ({1})", info.DeviceClassName, info.DeviceClassInt);
            this.lbServiceClass.Content = string.Format("{0} (0x{1:X})", info.ServiceClassName, info.ServiceClassInt);
            this.lbStrength.Content = info.Strength.ToString();
            this.lbLastSeen.Content = info.LastSeen.ToString();
            this.lbLastUsed.Content = info.LastUsed.ToString();
            if (info.Radio.IsInitialized) {
                this.lbRadioManufacturer.Content = info.Radio.Manufacturer;
                this.lbRadioLmp.Content = info.Radio.LinkManagerProtocol;
                this.comboBoxFeatures.ItemsSource = info.Radio.Features;
            }
            else {
                // TODO Disable radio display
                this.lblRadioLabel.Collapse();
                this.lblRadioFeaturesLable.Collapse();
                this.lblRadioProtocolLabel.Collapse();
                this.lbRadioLmp.Collapse();
                this.lbRadioManufacturerLabel.Collapse();
                this.lbRadioManufacturer.Collapse();
                this.comboBoxFeatures.Collapse();
            }

            // Cannot get RSSI for now
            this.lbStrength.Collapse();
            this.lblStrengthLabel.Collapse();
            // Not displaying properties for the moment
            this.lblProperties.Collapse();
            this.comboBoxProperties.Collapse();
        }

    }
}
