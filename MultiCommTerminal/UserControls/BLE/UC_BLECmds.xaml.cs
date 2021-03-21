using BluetoothLE.Net.DataModels;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.UserControls.BLE {

    /// <summary>Interaction logic for UC_BLECmds.xaml</summary>
    public partial class UC_BLECmds : UserControl {

        #region Data

        private List<IIndexItem<BLECmdIndexExtraInfo>> general = new List<IIndexItem<BLECmdIndexExtraInfo>>();
        private BLE_CharacteristicDataModel characteristic = null;
        private string currentCmd = string.Empty;
        private ClassLog log = new ClassLog("UC_BLECmds");

        #endregion

        #region Public

        public event EventHandler<string> OnCmdSelected;

        public UC_BLECmds() {
            InitializeComponent();
            this.lblCmdDataTypeContent.Content = "";
        }

        public void OnStartup() {
            DI.Wrapper.LanguageChanged += languageChanged;
        }


        private void languageChanged(object sender, SupportedLanguage l) {
            this.lblCmdDataTypeLabel.Content = l.GetText(MsgCode.DataType);
        }


        public void OnShutdown() {
            DI.Wrapper.LanguageChanged -= languageChanged;
        }


        public void Init(BLE_CharacteristicDataModel dm, bool force = false) {
            try {
                // The BLE tree updates continuously so need to check if same
                if (!force) {
                    if (this.characteristic != null && this.characteristic.Uuid == dm.Uuid) {
                        return;
                    }
                }

                this.Unsubscribe();
                this.lbCmdList.ItemsSource = null;
                this.lb_BLECmds.ItemsSource = null;
                this.lblCmdDataTypeContent.Content = "";
                this.characteristic = dm;
                if (this.characteristic.IsWritable) {
                    this.lblCmdDataTypeContent.Content = dm.DataTypeDisplay;
                    DI.Wrapper.GetFilteredBLECmdList(
                        this.characteristic.Parser.DataType,
                        this.characteristic.CharName,
                        (general, specific) => {
                        // Only using general for now. Not filtering on specific characteristic name
                        this.general = general;
                            this.lbCmdList.ItemsSource = this.general;
                        }, App.ShowMsg);
                }
                this.Subscribe();
                if (this.lbCmdList.Items != null && this.lbCmdList.Count() > 0) {
                    this.lbCmdList.SelectedIndex = 0;
                    // Then select first of commands
                    if (this.lb_BLECmds.Items != null && this.lb_BLECmds.Count() > 0) {
                        this.lb_BLECmds.SelectedIndex = 0;
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "Init", "", ex);
            }
        }






        #endregion

        #region Event handling

        private void Subscribe() {
            this.lbCmdList.SelectionChanged += this.lbCmdList_SelectionChanged;
            this.lb_BLECmds.SelectionChanged += this.cmdsSelectionChanged;
        }


        private void Unsubscribe() {
            this.lbCmdList.SelectionChanged -= this.lbCmdList_SelectionChanged;
            this.lb_BLECmds.SelectionChanged -= this.cmdsSelectionChanged;
        }


        private void lbCmdList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                this.lb_BLECmds.SelectionChanged -= this.cmdsSelectionChanged;
                this.lb_BLECmds.ItemsSource = null;
                DI.Wrapper.RetrieveBLECmdSet(
                    this.lbCmdList.SelectedItem as IIndexItem<BLECmdIndexExtraInfo>,
                    this.OnCmdListChanged,
                    App.ShowMsg);
            }
            catch (Exception ex) {
                this.log.Exception(9999, "lbCmdList_SelectionChanged", "", ex);
            }
            finally {
                this.lb_BLECmds.SelectionChanged += this.cmdsSelectionChanged;
            }
        }


        private void cmdsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            try {
                ScriptItem item = this.lb_BLECmds.SelectedItem as ScriptItem;
                if (item != null) {
                    this.currentCmd = item.Command;
                    if (this.IsVisible) {
                        this.OnCmdSelected?.Invoke(this, this.currentCmd);
                    }
                }
            }
            catch(Exception ex) {
                this.log.Exception(9999, "cmdsSelectionChanged", "", ex);
            }
        }


        private void UC_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs args) {
            if (this.IsVisible && this.currentCmd != string.Empty) {
                this.OnCmdSelected?.Invoke(this, this.currentCmd);
            }
            else {
                this.OnCmdSelected?.Invoke(this, string.Empty);
            }
        }


        private void OnCmdListChanged(BLECommandSetDataModel dm) {
            if (dm != null) {
                this.lb_BLECmds.ItemsSource = dm.Items;
            }
        }

        #endregion

    }
}
