using LogUtils.Net;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using VariousUtils.Net;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.UserControls.EditBoxes {

    /// <summary>Interaction logic for UC_BinaryEditBox.xaml</summary>
    public partial class UC_BinaryEditBox : UC_UintEditBoxBase {

        private ClassLog log = new ClassLog("UC_BinaryEditBox");

        public UC_BinaryEditBox() : base() {
            InitializeComponent();
        }


        protected override void DoSetValue(UInt32 value) {
            this.tbEdit.TextChanged -= this.tbEdit_TextChanged;
            int carretIndex = this.tbEdit.CaretIndex;
            this.tbEdit.Text = value.ToFormatedBinaryString();
            this.tbEdit.CaretIndex = carretIndex > this.tbEdit.Text.Length
                ? this.tbEdit.Text.Length
                : carretIndex;
            this.tbEdit.TextChanged += this.tbEdit_TextChanged;
        }


        protected override void DoSetEmpty() {
            this.tbEdit.TextChanged -= this.tbEdit_TextChanged;
            this.tbEdit.Text = "";
            this.tbEdit.TextChanged += this.tbEdit_TextChanged;
        }


        private void tbEdit_PreviewKeyDown(object sender, KeyEventArgs args) {
            try {
                if (args.Key.IsBinaryNumericForbidden()) {
                    args.Handled = true;
                }
                else {
                    if (args.Key.IsNumeric()) {
                        string add = args.Key.GetNumericValue();
                        string newVal = tbEdit.PreviewKeyDownAssembleText(add);
                        newVal = newVal.Replace(" ", "");
                        this.log.Info("", () => string.Format("'{0}'  '{1}'  '{2}'", this.tbEdit, add, newVal));

                        if (newVal.Length > 0) {
                            this.ValidateRange(() => Convert.ToUInt32(newVal, 2).ToString(), args);
                        }
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }


        private void tbEdit_TextChanged(object sender, TextChangedEventArgs e) {
            try {
                this.log.Info("tbEdit_TextChanged", this.tbEdit.Text);
                if (this.tbEdit.Text.Length == 0) {
                    this.RaiseValueEmpty();
                }
                else {
                    UInt32 value = Convert.ToUInt32(this.tbEdit.Text.Replace(" ", ""), 2);
                    this.SetValue(value);
                    this.RaiseValueChanged(value);
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }

    }
}
