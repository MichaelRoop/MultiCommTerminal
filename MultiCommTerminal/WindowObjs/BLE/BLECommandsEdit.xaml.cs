using BluetoothLE.Net.Enumerations;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using System.Collections.Generic;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLECommandsEdit.xaml</summary>
    public partial class BLECommandsEdit : Window {

        #region Data types

        /// <summary>Functionality differs by use type</summary>
        private enum UseType {
            Edit,
            New,
        }

        #endregion

        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private BLECommandSetDataModel original = null;
        private BLECommandSetDataModel copy = null;
        private IIndexItem<BLECmdIndexExtraInfo> index = null;
        private UseType useType = UseType.New;
        private BLE_DataType dataType = BLE_DataType.Bool;

        #endregion

        #region Constructors and window events

        public static void ShowBox(Window parent, IIndexItem<BLECmdIndexExtraInfo> index) {
            BLECommandsEdit win = new (parent, index, UseType.Edit, index.ExtraInfoObj.DataType);
            win.ShowDialog();
        }


        public static void ShowBox(Window parent, BLE_DataType dataType) {
            BLECommandsEdit win = new (parent, null, UseType.New, dataType);
            win.ShowDialog();
        }



        private BLECommandsEdit(Window parent, IIndexItem<BLECmdIndexExtraInfo> index, UseType useType, BLE_DataType dataType) {
            this.parent = parent;
            this.index = index;
            this.useType = useType;
            this.dataType = dataType;
            InitializeComponent();
            this.Init();
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
            this.widthManager.PrepForChange();
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

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            //// TODO Command editor binary, hex, dec
            ScriptItem item = new (DI.Wrapper.GetText(MsgCode.Default), "0");
            if (BLECmdEdit.ShowBox(this, this.dataType, item)) {
                this.lbxCmds.ItemsSource = null;
                this.copy.Items.Add(item);
                this.lbxCmds.ItemsSource = this.copy.Items;
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            ScriptItem item = this.lbxCmds.SelectedItem as ScriptItem;
            if (item != null) {
                // TODO - determine if we need to get it from the actual List<>
                if (BLECmdEdit.ShowBox(this, this.dataType, item)) {
                    this.lbxCmds.ItemsSource = null;
                    this.lbxCmds.ItemsSource = this.copy.Items;
                }
            }
            else {
                App.ShowMsg(DI.Wrapper.GetText(MsgCode.NothingSelected));
            }
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
            this.CopyBackToOriginal();
            if (this.useType == UseType.Edit) {
                DI.Wrapper.SaveBLECmdSet(this.index, this.original, this.Close, this.onFailure);
            }
            else {
                DI.Wrapper.CreateBLECmdSet(this.original, (idx) => { this.Close(); }, this.onFailure);
            }
        }

        #endregion

        #region Private

        private void Init() {
            this.original = this.useType == UseType.Edit
                ? this.RetrieveOriginal()
                : this.CreateNewOriginal();
            this.copy = this.MakeWorkingCopy(this.original);
            if (this.copy != null) {
                this.txtName.Text = this.copy.Display;
                this.lbxCmds.ItemsSource = copy.Items;
                this.lbDataType.Content = string.Format("{0} : {1}", 
                    DI.Wrapper.GetText(MsgCode.DataType), this.copy.DataType.ToStr());
            }
        }


        /// <summary>Create a new dummy script template</summary>
        private BLECommandSetDataModel CreateNewOriginal() {
            List<ScriptItem> items = new ();
            items.Add(new ScriptItem("Open", "1"));
            return new BLECommandSetDataModel(items, "", this.dataType, "Sample Name");
        }


        private BLECommandSetDataModel RetrieveOriginal() {
            BLECommandSetDataModel tmp = null;
            DI.Wrapper.RetrieveBLECmdSet(this.index, dm => { tmp = dm; }, this.onFailure);
            return tmp;
        }


        private BLECommandSetDataModel MakeWorkingCopy(BLECommandSetDataModel source) {
            if (source != null) {
                var target = new BLECommandSetDataModel() {
                    DataType = source.DataType,
                    Display = source.Display,
                    CharacteristicName = source.CharacteristicName,
                    UId = source.UId,
                };
                foreach (var item in original.Items) {
                    target.Items.Add(new ScriptItem(item.Display, item.Command));
                }
                return target; 
            }
            return null;
        }

        private void CopyBackToOriginal() {
            this.original.Display = this.txtName.Text;
            this.original.Items.Clear();
            //UUid already there. No need to transfer data type. cannot be changed
            foreach (var obj in this.lbxCmds.Items) {
                ScriptItem item = obj as ScriptItem;
                if (item != null) {
                    this.original.Items.Add(new ScriptItem(item.Display, item.Command));
                }
            }
        }




        #endregion

        #region Delegates


        /// <summary>On failure of retrieval post message and close</summary>
        /// <param name="msg">The message to display</param>
        private void onFailure(string msg) {
            App.ShowMsg(msg);
            this.Close();
        }

        #endregion

    }
}
