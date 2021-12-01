using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLECmdEdit.xaml</summary>
    public partial class BLECmdEdit : Window {

        private ClassLog log = new ClassLog("BLECmdEdit");
        private BLE_DataType dataType = BLE_DataType.Bool;
        private Window parent = null;
        private ScriptItem cmdItem = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private bool saveItem = false;

        #region Constructors and window events

        public static bool ShowBox(Window parent, BLE_DataType dataType, ScriptItem cmdItem) {
            BLECmdEdit win = new BLECmdEdit(parent, dataType, cmdItem);
            win.ShowDialog();
            return win.saveItem;
        }


        private BLECmdEdit(Window parent, BLE_DataType dataType, ScriptItem cmdItem) {
            this.parent = parent;
            this.dataType = dataType;
            this.cmdItem = cmdItem;
            InitializeComponent();
            if (this.dataType == BLE_DataType.UInt_32bit) {
                this.grdValueCol.MinWidth = 320;
            }

            DI.Wrapper.BLE_GetShortRangeDisplay(
                this.dataType, str => this.txtRange.Content = str, this.onFailure);
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
            this.widthManager.PrepForChange();
            this.Setup();
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

        #region Button events

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.saveItem = false;
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            // Make copy to validate so as not to change the inputed item
            ScriptItem tmp = new ScriptItem(this.txtName.Text, this.edDecEdit.Text);
            DI.Wrapper.ValidateBLECmdItem(
                this.dataType,
                tmp,
                () => {
                    this.saveItem = true;
                    this.cmdItem.Display = tmp.Display;
                    this.cmdItem.Command = tmp.Command;
                    this.Close();
                },
                err => {
                    this.saveItem = false;
                    App.ShowMsg(err);
                });
        }

        #endregion

        #region Private

        private void Setup() {
            this.txtName.Text = this.cmdItem.Display;
            string cmdStr = cmdItem.Command;

            UInt64 val = 0;
            if (cmdStr.Length > 0) {
                if (!UInt64.TryParse(cmdStr, out val)){
                    val = 0;
                }
            }

            this.edDecEdit.SetValidator(this.ValidateRangeFunc);
            this.edHexEdit.SetValidator(this.ValidateRangeFunc);
            this.edBinEdit.SetValidator(this.ValidateRangeFunc);

            this.edDecEdit.SetValue(val);
            this.edBinEdit.SetValue(val);
            this.edHexEdit.SetValue(val);

            this.edDecEdit.RegisterDependant(this.edBinEdit);
            this.edDecEdit.RegisterDependant(this.edHexEdit);

            this.edHexEdit.RegisterDependant(this.edDecEdit);
            this.edHexEdit.RegisterDependant(this.edBinEdit);

            this.edBinEdit.RegisterDependant(this.edDecEdit);
            this.edBinEdit.RegisterDependant(this.edHexEdit);
        }


        private void onFailure(string msg) {
            this.saveItem = false;
            App.ShowMsg(msg);
            this.Close();
        }


        private bool ValidateRangeFunc(string value) {
            bool result = false;
            DI.Wrapper.ValidateBLEValue(
                this.dataType, value, () => {
                    result = true;
                },
                err => {
                    App.ShowMsg(err);
                    result = false;
                });
            return result;
        }

        #endregion

    }
}
