using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Tools;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.NetCore.UserControls.BLE {

    /// <summary>Interaction logic for UC_BLEWriteGeneral.xaml</summary>
    public partial class UC_BLEWriteGeneral : UserControl {

        #region Data

        private BLE_CharacteristicDataModel selected = null;
        private BLERangeValidator validator = new BLERangeValidator();
        private Window parent = null;
        private ClassLog log = new ClassLog("UC_BLEWriteGeneral");

        #endregion

        #region Public

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


        public void SetCommandText(string command) {
            this.txtCommmand.Text = command;
        }

        public void Reset() {
            try {
                if (this.selected != null) {
                    this.selected.OnReadValueChanged -= Selected_OnReadValueChanged;
                }
                this.selected = null;
                this.lblServiceContent.Content = "";
                this.lblCharacteristicName.Content = "";
                this.lblInfoContent.Content = "";
                this.txtCommmand.Text = "";
                this.SetEnabled(false, false);
            }
            catch(Exception e) {
                this.log.Exception(9999, "Reset", "", e);
            }
        }

        #endregion

        #region Private

        private void btnSend_Click(object sender, RoutedEventArgs e) {
            DI.Wrapper.BLE_Send(this.txtCommmand.Text, this.selected, () => { }, App.ShowMsg);
        }


        public void SetCharacteristic(BLE_CharacteristicDataModel dm) {
            try {
                if (this.selected != dm) {
                    this.Reset();
                    this.selected = dm;
                    this.lblServiceContent.Content = this.selected.Service.DisplayName;
                    DI.Wrapper.BLE_GetRangeDisplay(
                        this.selected, this.DelegateSelectSuccess, App.ShowMsg);
                    this.SetEnabled(this.selected.IsReadable, this.selected.IsWritable);
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "SetCharacteristic", "", e);
            }
        }


        private void btnRead_Click(object sender, RoutedEventArgs arg) {
            try {
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
                        this.AssembleCharacteristicLine();
                    }
                }
                catch (Exception e) {
                    Log.Exception(9999, "UC_BLEWriteGeneral", "Selected_OnReadValueChanged", "", e);
                }
            });
        }


        private void DelegateSelectSuccess(string characteristicName, string dataInfo) {
            this.lblInfoContent.Content = dataInfo;
            this.AssembleCharacteristicLine();
        }


        private void languageChangedHandler(object sender, SupportedLanguage l) {
            Dispatcher.Invoke(() => {
                try {
                    // Buttons
                    this.btnSend.Content = l.GetText(MsgCode.Write);
                    this.btnRead.Content = l.GetText(MsgCode.Read);
                    // Labels
                    this.lblServiceLabel.Content = l.GetText(MsgCode.Service);
                    this.lblCharacteristicLabel.Content = l.GetText(MsgCode.Characteristic);
                    this.lblInfoLabel.Content = l.GetText(MsgCode.info);
                    // Content
                    if (this.selected == null) {
                        this.lblCharacteristicName.Content = "";
                    }
                    else {
                        // translation and assembly happen in wrapper
                        DI.Wrapper.BLE_GetRangeDisplay(
                            this.selected, this.DelegateSelectSuccess, App.ShowMsg);
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "LanguageChangeHandler", "", e);
                }
            });
        }


        private void SetEnabled(bool readable, bool writable) {
            try {
                this.IsEnabled = readable || writable;
                this.Opacity = this.IsEnabled ? 1 : 0.5;
                if (this.IsEnabled) {
                    this.btnSend.IsEnabled = writable;
                    this.btnSend.Opacity = writable ? 1 : 0.5;
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
            catch (Exception ex) {
                this.log.Exception(9999, "SetEnabled", "", ex);
            }
        }


        private void AssembleCharacteristicLine() {
            try {
                if (this.selected != null) {
                    this.lblCharacteristicName.Content =
                        string.Format("{0}  {1}  {2}",
                        this.selected.CharName,
                        this.selected.CharValue,
                        this.selected.UserDescription);
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "AssembleCharacteristicLine", "", e);
            }
        }

        #endregion

    }

}
