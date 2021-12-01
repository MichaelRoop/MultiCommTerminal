using BluetoothLE.Net.DataModels;
using LogUtils.Net;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLESelectCharacteristic.xaml</summary>
    public partial class BLESelectCharacteristic : Window {

        #region Data and properties

        public class SelectResult {
            public bool IsCanceled { get; set; } = false;
            public BLE_CharacteristicDataModel SelectedCharacteristic { get; set; } = null;

        }


        private Window parent = null;
        private List<BLE_CharacteristicDataModel> characteristics = new List<BLE_CharacteristicDataModel>();
        private ClassLog log = new ClassLog("BLESelectCharacteristic");

        public SelectResult Result { get; private set; } = new SelectResult();

        #endregion

        #region Constructor and window event

        public static SelectResult ShowBox(Window parent, List<BLE_CharacteristicDataModel> characteristics) {
            BLESelectCharacteristic win = new BLESelectCharacteristic(parent, characteristics);
            win.ShowDialog();
            return win.Result;
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
            this.Result.IsCanceled = true;
            this.Close();
        }


        private void ListBox_BLE_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.Result.SelectedCharacteristic = this.listBox_BLE.SelectedItem as BLE_CharacteristicDataModel;
            this.Result.IsCanceled = false;
            this.Close();
        }

        #endregion

    }
}
