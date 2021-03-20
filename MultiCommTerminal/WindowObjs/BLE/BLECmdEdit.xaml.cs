using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using VariousUtils.Net;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLECmdEdit.xaml</summary>
    public partial class BLECmdEdit : Window {

        private ClassLog log = new ClassLog("BLECmdEdit");
        private BLE_DataType dataType = BLE_DataType.Bool;
        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #region Constructors and window events

        public BLECmdEdit(Window parent, BLE_DataType dataType) {
            this.parent = parent;
            this.dataType = dataType;
            InitializeComponent();
            DI.Wrapper.BLE_GetRangeDisplay(
                this.dataType, str => this.txtRange.Content = str, this.onFailure);
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
            this.widthManager.PrepForChange();
            this.edBinEdit.SetValidator(this.ValidateRangeFunc);
            this.edBinEdit.OnValueChanged += this.onBinaryValueChanged;
            this.edBinEdit.OnValueEmpty += this.onBinaryValueEmpty;
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
            this.edBinEdit.OnValueChanged -= this.onBinaryValueChanged;
            this.edBinEdit.OnValueEmpty -= this.onBinaryValueEmpty;
        }

        #endregion

        #region Edit box events

        /// <summary>Prevent invalid characters ever getting entered</summary>
        /// <param name="sender">The EditBox</param>
        /// <param name="args">The event args</param>
        private void tbDec_PreviewKeyUp(object sender, KeyEventArgs args) {
            try {
                if (args.Key.IsUnsignedNumericForbidden()) {
                    args.Handled = true;
                }
                else {
                    if (args.Key.IsNumeric()) {
                        string s = args.Key.GetNumericValue();
                        this.ValidateRange(string.Format("{0}{1}", this.tbDec.Text, s), args);
                    }
                }
            }
            catch(Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }


        /// <summary>Invalid characters already filtered. Do translation</summary>
        /// <param name="sender">The EditBox</param>
        /// <param name="args">The event args</param>
        private void tbDec_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                // remove multiple leading zeros
                this.log.Info("tbDec_TextChanged", this.tbDec.Text);
                if (this.tbDec.Text.Length == 0) {
                    this.edHex.Text = "";
                    this.edBinEdit.SetEmpty();
                }
                else {
                    this.edHex.TextChanged -= this.edHex_TextChanged;
                    // Now translate to hex and binary
                    UInt32 val = UInt32.Parse(this.tbDec.Text);
                    this.edHex.Text = val.ToString("X");
                    this.edBinEdit.SetValue(val);
                    this.edHex.TextChanged += this.edHex_TextChanged;
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }


        private void edHex_PreviewKeyDown(object sender, KeyEventArgs args) {
            try {
                if (args.Key.IsHexNumericForbidden()) {
                    args.Handled = true;
                }
                else {
                    if (args.Key.IsHexDecimal()) {
                        var s = string.Format("{0}{1}", this.edHex.Text, args.Key.GetHexDecimalValue());

                        this.log.Info("edHex_PreviewKeyDown", () =>
                            string.Format("'{0}'  '{1}'  '{2}'  '{3}'", 
                            args.Key.ToString(), this.edHex.Text, args.Key.GetHexDecimalValue(), s));
                        
                        UInt32 tmp = Convert.ToUInt32(s, 16);
                        this.ValidateRange(tmp.ToString(), args);
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }

        }

        private void edHex_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                // remove multiple leading zeros
                this.log.Info("tbDec_TextChanged", this.edHex.Text);
                if (this.edHex.Text.Length == 0) {
                    this.tbDec.Text = "";
                    this.edBinEdit.SetEmpty();
                }
                else {
                    this.tbDec.TextChanged -= this.tbDec_TextChanged;
                    // Now translate to decimal and binary
                    UInt32 val = Convert.ToUInt32(this.edHex.Text, 16);
                    this.tbDec.Text = val.ToString();
                    this.edBinEdit.SetValue(val);
                    this.tbDec.TextChanged += this.tbDec_TextChanged;
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }

        #endregion


        #region Button events

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnOk_Click(object sender, RoutedEventArgs e) {

        }


        private void onFailure(string msg) {
            App.ShowMsg(msg);
            this.Close();
        }


        #endregion


        private void ValidateRange(string value, KeyEventArgs args) {
            DI.Wrapper.ValidateBLEValue(
                this.dataType, value, () => { },
                err => {
                    args.Handled = true;
                    App.ShowMsg(err);
                });
        }


        private bool ValidateRangeFunc(string value) {
            bool result = false;
            DI.Wrapper.ValidateBLEValue(
                this.dataType, value, () => { result = true; },
                err => {
                    App.ShowMsg(err);
                    result = false;
                });
            return result;
        }


        #region Custom EditBox events

        private void onBinaryValueEmpty(object sender, EventArgs e) {
            // TODO - eventually call their empty functions but for now handle directly
            this.tbDec.TextChanged -= this.tbDec_TextChanged;
            this.edHex.TextChanged -= this.edHex_TextChanged;
            this.edHex.Text = "";
            this.tbDec.Text = "";
            this.tbDec.TextChanged += this.tbDec_TextChanged;
            this.edHex.TextChanged += this.edHex_TextChanged;
        }


        private void onBinaryValueChanged(object sender, uint value) {
            // TODO - eventually call their SetValue functions but for now handle directly
            this.tbDec.TextChanged -= this.tbDec_TextChanged;
            this.edHex.TextChanged -= this.edHex_TextChanged;
            this.tbDec.Text = value.ToString();
            this.edHex.Text = UInt32.Parse(this.tbDec.Text).ToString("X");
            this.tbDec.TextChanged += this.tbDec_TextChanged;
            this.edHex.TextChanged += this.edHex_TextChanged;
        }


        #endregion

    }
}
