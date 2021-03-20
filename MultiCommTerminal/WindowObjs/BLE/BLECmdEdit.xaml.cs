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
                    this.edBin.Text = "";
                }
                else {
                    this.edHex.TextChanged -= this.edHex_TextChanged;
                    this.edBin.TextChanged -= this.edBin_TextChanged;
                    // Now translate to hex and binary
                    this.edHex.Text = UInt32.Parse(this.tbDec.Text).ToString("X");
                    this.edBin.Text = Convert.ToUInt32(this.tbDec.Text).ToFormatedBinaryString();
                    this.edHex.TextChanged += this.edHex_TextChanged;
                    this.edBin.TextChanged += this.edBin_TextChanged;
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
                    this.edBin.Text = "";
                }
                else {
                    this.tbDec.TextChanged -= this.tbDec_TextChanged;
                    this.edBin.TextChanged -= this.edBin_TextChanged;
                    // Now translate to decimal and binary
                    UInt32 val = Convert.ToUInt32(this.edHex.Text, 16);
                    this.tbDec.Text = val.ToString();
                    this.edBin.Text = val.ToFormatedBinaryString();
                    this.tbDec.TextChanged += this.tbDec_TextChanged;
                    this.edBin.TextChanged += this.edBin_TextChanged;
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }


        private void edBin_PreviewKeyDown(object sender, KeyEventArgs args) {
            try {
                if (args.Key.IsBinaryNumericForbidden()) {
                    args.Handled = true;
                }
                else {
                    if (args.Key.IsNumeric()) {
                        string current = this.edBin.Text;
                        string extra = args.Key.GetNumericValue();
                        string newValue = string.Format("{0}{1}", current, extra);
                        // Need to convert to an actual value
                        newValue = newValue.Replace(" ", "");
                        if (newValue.Length > 0) {
                            this.ValidateRange(Convert.ToUInt32(newValue, 2).ToString(), args);
                        }
                        // TODO do I need an else?
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }

        private void edBin_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                // remove multiple leading zeros
                this.log.Info("tbDec_TextChanged", this.edHex.Text);
                if (this.edBin.Text.Length == 0) {
                    this.edHex.Text = "";
                    this.tbDec.Text = "";
                }
                else {
                    this.tbDec.TextChanged -= this.tbDec_TextChanged;
                    this.edHex.TextChanged -= this.edHex_TextChanged;

                    // Now translate to decimal and binary
                    string noSpaceBin = this.edBin.Text.Replace(" ", "");
                    UInt32 value = Convert.ToUInt32(noSpaceBin, 2);
                    this.tbDec.Text = value.ToString();
                    //this.edHex.Text = Convert.ToUInt32(this.edBin.Text, 16).ToString();
                    this.edHex.Text = UInt32.Parse(this.tbDec.Text).ToString("X");

                    this.edBin.TextChanged -= this.edBin_TextChanged;
                    int carretIndex = this.edBin.CaretIndex;
                    this.edBin.Text = value.ToFormatedBinaryString();
                    this.edBin.CaretIndex = carretIndex > this.edBin.Text.Length 
                        ? this.edBin.Text.Length 
                        : carretIndex;
                    this.edBin.TextChanged += this.edBin_TextChanged;
                    this.tbDec.TextChanged += this.tbDec_TextChanged;
                    this.edHex.TextChanged += this.edHex_TextChanged;
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


    }
}
