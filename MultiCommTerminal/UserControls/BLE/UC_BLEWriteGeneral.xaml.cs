using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Tools;
using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.BLE;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.NetCore.UserControls.BLE {

    /// <summary>Interaction logic for UC_BLEWriteGeneral.xaml</summary>
    public partial class UC_BLEWriteGeneral : UserControl {

        private List<BLE_CharacteristicDataModel> dataModels = new List<BLE_CharacteristicDataModel>();
        private BLE_CharacteristicDataModel selected = null;
        private BLERangeValidator validator = new BLERangeValidator();
        private Window parent = null;

        public UC_BLEWriteGeneral() {
            InitializeComponent();
        }

        public void OnStartup(Window parent) {
            this.parent = parent;
        }

        public void SetCharacteristics(List<BLE_CharacteristicDataModel> dataModels) {
            this.dataModels = dataModels;
        }

        public void Reset() {
            this.selected = null;
            this.lblCharacteristic.Content = DI.Wrapper.GetText(MsgCode.NothingSelected);
            this.lblInfo.Content = "";
        }


        private void btnSend_Click(object sender, RoutedEventArgs e) {
            DI.Wrapper.BLE_Send(
                this.txtCommmand.Text, this.selected, () => { }, App.ShowMsg);
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.Reset();
            if (this.dataModels.Count == 0) {
                App.ShowMsg(DI.Wrapper.GetText(MsgCode.None)); // TODO Translation
            }
            else {
                this.selected = BLESelectCharacteristic.ShowBox(this.parent, this.dataModels);
                DI.Wrapper.BLE_GetRangeDisplay(this.selected, this.DelegateSelectSuccess, App.ShowMsg);
            }
        }


        private void DelegateSelectSuccess(string characteristicName, string dataInfo) {
            this.lblCharacteristic.Content = characteristicName;
            this.lblInfo.Content = dataInfo;
        }


    }
}
