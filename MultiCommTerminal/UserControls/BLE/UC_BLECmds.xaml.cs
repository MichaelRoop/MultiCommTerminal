using BluetoothLE.Net.DataModels;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.UserControls.BLE {

    /// <summary>Interaction logic for UC_BLECmds.xaml</summary>
    public partial class UC_BLECmds : UserControl {


        //private BLECommandSetDataModel general = null;
        //private BLECommandSetDataModel specific = null;

        private List<IIndexItem<BLECmdIndexExtraInfo>> general = new List<IIndexItem<BLECmdIndexExtraInfo>>();
        private List<IIndexItem<BLECmdIndexExtraInfo>> specific = new List<IIndexItem<BLECmdIndexExtraInfo>>();
        private BLE_CharacteristicDataModel characteristic = null;

        public event EventHandler<string> OnCmdSelected;


        public UC_BLECmds() {
            InitializeComponent();
            this.lblCmdDataTypeContent.Content = "";
        }


        public void Init(BLE_CharacteristicDataModel dm) {
            // The BLE tree updates continuously so need to check if same
            if (this.characteristic != null && this.characteristic.Uuid == dm.Uuid) {
                return;
            }

            this.Unsubscribe();
            this.lvBLECmdList.ItemsSource = null;
            this.lb_BLECmds.ItemsSource = null;
            this.lblCmdDataTypeContent.Content = "";
            this.characteristic = dm;
            if (this.characteristic.IsWritable) {
                this.lblCmdDataTypeContent.Content = dm.DataTypeDisplay;
                DI.Wrapper.GetFilteredBLECmdList(
                    this.characteristic.Parser.DataType,
                    this.characteristic.CharName,
                    (general, specific) => {
                        this.general = general;
                        this.specific = specific;
                        // We would do a check if specific exists then it becomes list and enable filter icon if general also has number > 1

                        // TODO - for now just show general. Need filter swap in UI if specific exists
                        this.lvBLECmdList.ItemsSource = this.general;
                        //this.ForceCmdListResize();
                        this.lvBLECmdList.ForceColumnResize();

                    }, App.ShowMsg);
            }
            this.Subscribe();
            if (this.lvBLECmdList.Items != null && this.lvBLECmdList.Count() > 0) {
                this.lvBLECmdList.SelectedIndex = 0;
                // Then select first of commands
                if (this.lb_BLECmds.Items != null && this.lb_BLECmds.Count() > 0) {
                    this.lb_BLECmds.SelectedIndex = 0;
                }

            }
        }


        #region Event handling

        private void Subscribe() {
            this.lvBLECmdList.SelectionChanged += this.cmdListSelectionChanged;
            this.lb_BLECmds.SelectionChanged += this.cmdsSelectionChanged;
        }


        private void Unsubscribe() {
            this.lvBLECmdList.SelectionChanged -= this.cmdListSelectionChanged;
            this.lb_BLECmds.SelectionChanged -= this.cmdsSelectionChanged;
        }


        private void cmdListSelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.lb_BLECmds.SelectionChanged -= this.cmdsSelectionChanged;
            this.lb_BLECmds.ItemsSource = null;
            //this.current = null;


            IIndexItem<BLECmdIndexExtraInfo> item = this.lvBLECmdList.SelectedItem as IIndexItem<BLECmdIndexExtraInfo>;
            if (item != null) {
                DI.Wrapper.RetrieveBLECmdSet(item, dm => {
                    //this.current = dm;
                    this.lb_BLECmds.ItemsSource = dm.Items;
                }, App.ShowMsg);
            }
            this.lb_BLECmds.SelectionChanged += this.cmdsSelectionChanged;
        }


        private void cmdsSelectionChanged(object sender, SelectionChangedEventArgs e) {
            ScriptItem item = this.lb_BLECmds.SelectedItem as ScriptItem;
            if (item != null) {
                this.OnCmdSelected?.Invoke(this, item.Command);
            }
        }

        #endregion

    }
}
