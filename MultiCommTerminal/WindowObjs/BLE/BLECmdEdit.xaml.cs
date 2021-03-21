using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using WpfHelperClasses.Core;

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
            DI.Wrapper.BLE_GetRangeDisplay(
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
            this.edDecEdit.OnValueChanged -= this.onDecValueChanged;
            this.edHexEdit.OnValueChanged -= this.onHexValueChanged;
            this.edBinEdit.OnValueChanged -= this.onBinaryValueChanged;

            this.edDecEdit.OnValueEmpty -= this.onDecValueEmpty;
            this.edHexEdit.OnValueEmpty -= this.onHexValueEmpty;
            this.edBinEdit.OnValueEmpty -= this.onBinaryValueEmpty;
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

            UInt32 val = 0;
            if (cmdStr.Length > 0) {
                if (!UInt32.TryParse(cmdStr, out val)){
                    val = 0;
                }
            }
            this.edDecEdit.SetValue(val);
            this.edBinEdit.SetValue(val);
            this.edHexEdit.SetValue(val);

            this.edDecEdit.SetValidator(this.ValidateRangeFunc);
            this.edHexEdit.SetValidator(this.ValidateRangeFunc);
            this.edBinEdit.SetValidator(this.ValidateRangeFunc);

            this.edDecEdit.OnValueChanged += this.onDecValueChanged;
            this.edHexEdit.OnValueChanged += this.onHexValueChanged;
            this.edBinEdit.OnValueChanged += this.onBinaryValueChanged;

            this.edDecEdit.OnValueEmpty += this.onDecValueEmpty;
            this.edHexEdit.OnValueEmpty += this.onHexValueEmpty;
            this.edBinEdit.OnValueEmpty += this.onBinaryValueEmpty;
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

        #region Custom EditBox events

        private void onDecValueEmpty(object sender, EventArgs e) {
            this.edHexEdit.SetEmpty();
            this.edBinEdit.SetEmpty();
        }

        private void onHexValueEmpty(object sender, EventArgs e) {
            this.edDecEdit.SetEmpty();
            this.edBinEdit.SetEmpty();
        }

        private void onBinaryValueEmpty(object sender, EventArgs e) {
            this.edDecEdit.SetEmpty();
            this.edHexEdit.SetEmpty();
        }

        private void onDecValueChanged(object sender, uint value) {
            this.edHexEdit.SetValue(value);
            this.edBinEdit.SetValue(value);
        }

        private void onHexValueChanged(object sender, uint value) {
            this.edDecEdit.SetValue(value);
            this.edBinEdit.SetValue(value);
        }

        private void onBinaryValueChanged(object sender, uint value) {
            this.edDecEdit.SetValue(value);
            this.edHexEdit.SetValue(value);
        }

        #endregion

    }
}
