using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Tools;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.BLE;
using System;
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
        private ClassLog log = new ClassLog("UC_BLEWriteGeneral");

        public bool Connected { get; set; } = false;

        public UC_BLEWriteGeneral() {
            InitializeComponent();
            this.SetEnabled(false, false);
        }

        public void OnStartup(Window parent) {
            this.parent = parent;
            DI.Wrapper.LanguageChanged += this.languageChangedHandler;

        }

        public void OnShutdown() {
            try {
                DI.Wrapper.LanguageChanged -= this.languageChangedHandler;
                if (this.selected != null) {
                    this.selected.OnReadValueChanged -= Selected_OnReadValueChanged;
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "OnShutdown", "", e);
            }
        }


        public void SetCharacteristics(List<BLE_CharacteristicDataModel> dataModels, bool readable, bool writable) {
            this.dataModels = dataModels;
            this.SetEnabled(readable, writable);
        }

        public void Reset() {
            this.selected = null;
            this.lblCharacteristicContent.Content = DI.Wrapper.GetText(MsgCode.NothingSelected);
            this.lblInfoContent.Content = "";
            this.lblCharacteristicDescription.Content = "";
            this.txtCommmand.Text = "";
            this.SetEnabled(false, false);
        }


        private void btnSend_Click(object sender, RoutedEventArgs e) {
            DI.Wrapper.BLE_Send(
                this.txtCommmand.Text, this.selected, () => { }, App.ShowMsg);
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            if (this.Connected) {
                //if (this.selected != null) {
                //    this.selected.OnReadValueChanged -= Selected_OnReadValueChanged;
                //}
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
                            this.lblValueContent.Content = this.selected.CharValue;
                            this.SetEnabled(this.selected.IsReadable, this.selected.IsWritable);
                        }
                    }
                }
            }
        }

        private void btnRead_Click(object sender, RoutedEventArgs arg) {
            try {
                this.lblValueContent.Content = "";
                if (this.selected != null) {
                    this.selected.Read();
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "btnRead_Click", "", ex);
            }
        }


        private void Selected_OnReadValueChanged(object sender, BluetoothLE.Net.Enumerations.BLE_CharacteristicReadResult e) {
            this.Dispatcher.Invoke(() => {
                try {
                    if (this.selected != null) {
                        DI.Wrapper.Translate(this.selected);
                        this.lblValueContent.Content = this.selected.CharValue;
                    }
                }
                catch (Exception e) {
                    Log.Exception(9999, "UC_BLEWriteGeneral", "Selected_OnReadValueChanged", "", e);
                }
            });
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
                this.btnRead.Content = l.GetText(MsgCode.Read);
                // Labels
                this.lblCharacteristic.Content = l.GetText(MsgCode.Characteristic);
                this.lblInfo.Content = l.GetText(MsgCode.info);
                // Content
                if (this.selected == null) {
                    this.lblCharacteristicContent.Content = l.GetText(MsgCode.NothingSelected);
                    this.lblValueContent.Content = "";
                }
                else {
                    // translation and assembly happen in wrapper
                    DI.Wrapper.BLE_GetRangeDisplay(this.selected, this.DelegateSelectSuccess, App.ShowMsg);
                    this.lblValueContent.Content = this.selected.CharValue;
                }
            });
        }


        private void SetEnabled(bool readable, bool writable) {
            this.IsEnabled = readable || writable;
            this.Opacity = this.IsEnabled ? 1 : 0.5;
            if (this.IsEnabled) {
                this.btnSend.IsEnabled = writable;
                this.btnSend.Opacity =  writable ? 1 : 0.5;
                this.txtCommmand.IsEnabled = writable;
                this.txtCommmand.Opacity = writable ? 1 : 0.5;
                
                this.btnRead.IsEnabled = readable;
                this.btnRead.Opacity = readable ? 1 : 0.5;
                if (readable) {
                    if (this.selected != null) {
                        this.selected.OnReadValueChanged -= Selected_OnReadValueChanged;
                        this.selected.OnReadValueChanged += Selected_OnReadValueChanged;
                    }
                    else {
                        this.log.Error(9999, "SetEnabled", "No selected");
                    }
                }
            }
        }

    }
    
}
