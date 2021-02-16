using BluetoothLE.Net.DataModels;
using LogUtils.Net;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLESelectCharacteristic.xaml</summary>
    public partial class BLESelectCharacteristic : Window {

        #region Data and properties

        private Window parent = null;
        private List<BLE_CharacteristicDataModel> characteristics = new List<BLE_CharacteristicDataModel>();
        private ClassLog log = new ClassLog("BLESelectCharacteristic");

        public BLE_CharacteristicDataModel SelectedCharacteristic { get; private set; } = null;

        #endregion

        #region Constructor and window event

        public static BLE_CharacteristicDataModel ShowBox(Window parent, List<BLE_CharacteristicDataModel> characteristics) {
            BLESelectCharacteristic win = new BLESelectCharacteristic(parent, characteristics);
            win.ShowDialog();
            return win.SelectedCharacteristic;
        }


        public BLESelectCharacteristic(Window parent, List<BLE_CharacteristicDataModel> characteristics) {
            this.parent = parent;
            this.characteristics = characteristics;
            InitializeComponent();
            this.listBox_BLE.ItemsSource = characteristics;
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.listBox_BLE.SelectionChanged += this.ListBox_BLE_SelectionChanged;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.listBox_BLE.SelectionChanged -= this.ListBox_BLE_SelectionChanged;
        }

        #endregion

        #region Control events

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void ListBox_BLE_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.SelectedCharacteristic = this.listBox_BLE.SelectedItem as BLE_CharacteristicDataModel;
            this.Close();
        }

        #endregion

    }
}
