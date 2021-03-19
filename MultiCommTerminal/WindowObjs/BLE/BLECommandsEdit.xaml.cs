using BluetoothLE.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.UserControls.BLE;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLECommandsEdit.xaml</summary>
    public partial class BLECommandsEdit : Window {

        #region Data types

        /// <summary>Functionality differs by use type</summary>
        public enum UseType {
            View,
            Edit,
            New,
        }

        #endregion

        #region Data

        Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private BLECommandSetDataModel original = null;
        private BLECommandSetDataModel copy = null;
        private IIndexItem<BLECmdIndexExtraInfo> index = null;
        private UseType useType = UseType.View;
        private List<BLETypeDisplay> lbDataTypes = new List<BLETypeDisplay>();

        #endregion

        #region Constructors and window events

        public static void ShowBox(Window parent, IIndexItem<BLECmdIndexExtraInfo> index, UseType useType) {
            BLECommandsEdit win = new BLECommandsEdit(parent, index, useType);
            win.ShowDialog();
        }


        public BLECommandsEdit(Window parent, IIndexItem<BLECmdIndexExtraInfo> index, UseType useType) {
            this.parent = parent;
            this.index = index;
            this.useType = useType;
            InitializeComponent();
            this.PopulateDataFields();
            this.ShowControls();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (this.widthManager != null) {
                this.widthManager.Teardown();
            }
        }

        #endregion

        #region Controls events

        // TODO - Problem - If I change the data type here it would invalidate any checks done on individual commands.
        // Would need to iterate through the commands and flag error and refuse save

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            // Add a command
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            ScriptItem item = this.lbxCmds.SelectedItem as ScriptItem;
            if (item != null) {
                this.lbxCmds.ItemsSource = null;
                this.copy.Items.Remove(item);
                this.lbxCmds.ItemsSource = this.copy.Items;
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            // Save and close
        }


        private void cbDataTypes_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // TODO - Validate all the data accumulated so far
        }

        #endregion

        #region Private

        /// <summary>Populate controls in the dialog depending on use type</summary>
        private void PopulateDataFields() {
            this.lbDataTypes.Add(new BLETypeDisplay(BLE_DataType.Bool));
            this.lbDataTypes.Add(new BLETypeDisplay(BLE_DataType.UInt_8bit));
            this.lbDataTypes.Add(new BLETypeDisplay(BLE_DataType.UInt_16bit));
            this.cbDataTypes.ItemsSource = this.lbDataTypes;

            switch (this.useType) {
                case UseType.View:
                case UseType.Edit:
                    this.RetrieveScript();
                    break;
                case UseType.New:
                    this.InitializeNewScript();
                    break;
            }
            if (this.copy != null) {
                this.txtName.Text = this.copy.Display;
                this.lbxCmds.ItemsSource = copy.Items;
                var x = this.lbDataTypes.FirstOrDefault(x => x.DataType == this.copy.DataType);
                if (x != null) {
                    this.cbDataTypes.SelectedItem = x;
                }
            }
        }


        /// <summary>Show or hide controls based on use type</summary>
        private void ShowControls() {
            switch (this.useType) {
                case UseType.View:
                    this.btnCancel.Visibility = Visibility.Collapsed;
                    this.txtName.IsEnabled = false;
                    this.lbxCmds.IsEnabled = false;
                    this.stPanelSideButtons.Visibility = Visibility.Collapsed;
                    break;
                case UseType.Edit:
                case UseType.New:
                    // Call before rendering which will trigger initial resize events
                    this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
                    this.widthManager.PrepForChange();
                    break;
            }
        }


        /// <summary>Retrieve the script from storage</summary>
        private void RetrieveScript() {
            DI.Wrapper.RetrieveBLECmdSet(this.index, this.onRetrieveSuccess, this.onRetrieveFailure);
        }


        /// <summary>Create a new dummy script template</summary>
        private void InitializeNewScript() {
            List<ScriptItem> items = new List<ScriptItem>();
            items.Add(new ScriptItem("Open", "1"));
            items.Add(new ScriptItem("Close", "2"));
            this.copy = new BLECommandSetDataModel(
                items, "", BLE_DataType.UInt_8bit, "Sample Name");
        }

        #endregion

        #region Delegates

        /// <summary>On successful retrieval make a copy of the script</summary>
        /// <param name="dataModel">The script data to dopy</param>
        private void onRetrieveSuccess(BLECommandSetDataModel dataModel) {
            // Make a copy of the original to avoid changing it unless OK
            this.original = dataModel;
            this.copy = new BLECommandSetDataModel() {
                DataType = this.original.DataType,
                Display = this.original.Display,
                CharacteristicName = this.original.CharacteristicName,
                UId = this.original.UId,
            };
            foreach (var item in original.Items) {
                this.copy.Items.Add(new ScriptItem() {
                    Display = item.Display,
                    Command = item.Command,
                });
            }
        }


        /// <summary>On failure of retrieval post message and close</summary>
        /// <param name="msg">The message to display</param>
        private void onRetrieveFailure(string msg) {
            App.ShowMsg(msg);
            this.Close();
        }

        #endregion

    }
}
