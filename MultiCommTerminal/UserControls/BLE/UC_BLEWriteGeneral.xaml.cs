using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Tools;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
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


        public bool Connected { get; set; } = false;

        public UC_BLEWriteGeneral() {
            InitializeComponent();
            this.SetEnabled(false);
        }

        public void OnStartup(Window parent) {
            this.parent = parent;
            DI.Wrapper.LanguageChanged += this.languageChangedHandler;
            
        }

        public void OnShutdown() {
            DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
        }


        public void SetCharacteristics(List<BLE_CharacteristicDataModel> dataModels) {
            this.dataModels = dataModels;
            this.SetEnabled(this.dataModels.Count > 0);
        }

        public void Reset() {
            this.selected = null;
            this.lblCharacteristicContent.Content = DI.Wrapper.GetText(MsgCode.NothingSelected);
            this.lblInfoContent.Content = "";
            this.lblCharacteristicDescription.Content = "";
            this.txtCommmand.Text = "";
            this.SetEnabled(false);
        }


        private void btnSend_Click(object sender, RoutedEventArgs e) {
            DI.Wrapper.BLE_Send(
                this.txtCommmand.Text, this.selected, () => { }, App.ShowMsg);
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            if (this.Connected) {
                // Should never be called since disabled but double check
                if (this.dataModels.Count == 0) {
                    this.Reset();
                    App.ShowMsg(DI.Wrapper.GetText(MsgCode.ReadOnly));
                }
                else {
                    BLESelectCharacteristic.SelectResult result = 
                        BLESelectCharacteristic.ShowBox(this.parent, this.dataModels);
                    if (!result.IsCanceled) {
                        if (this.selected != result.SelectedCharacteristic) {
                            this.Reset();
                            this.selected = result.SelectedCharacteristic;
                            this.lblCharacteristicDescription.Content = this.selected.UserDescription;
                            DI.Wrapper.BLE_GetRangeDisplay(this.selected, this.DelegateSelectSuccess, App.ShowMsg);
                            this.SetEnabled(true);
                        }
                    }
                }
            }
        }


        private void DelegateSelectSuccess(string characteristicName, string dataInfo) {
            this.lblCharacteristicContent.Content = characteristicName;
            this.lblInfoContent.Content = dataInfo;
        }


        private void languageChangedHandler(object sender, SupportedLanguage l) {
            Dispatcher.Invoke(() => {
                // Buttons
                this.btnSelect.Content = l.GetText(MsgCode.select);
                this.btnSend.Content = l.GetText(MsgCode.send);
                // Labels
                this.lblCharacteristic.Content = l.GetText(MsgCode.Characteristic);
                this.lblInfo.Content = l.GetText(MsgCode.info);
                this.lblWrite.Content = l.GetText(MsgCode.Write);
                // Content
                if (this.selected == null) {
                    this.lblCharacteristicContent.Content = l.GetText(MsgCode.NothingSelected);
                }
                else {
                    // translation and assembly happen in wrapper
                    DI.Wrapper.BLE_GetRangeDisplay(this.selected, this.DelegateSelectSuccess, App.ShowMsg);
                }
            });
        }


        private void SetEnabled(bool active) {
            this.IsEnabled = active;
            this.Opacity = active ? 1 : 0.5;
        }


    }
}
