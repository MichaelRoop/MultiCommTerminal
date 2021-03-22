using LogUtils.Net;
using System;
using System.Windows.Controls;
using System.Windows.Input;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.UserControls.EditBoxes {

    /// <summary>Interaction logic for UC_UIntEditBox.xaml</summary>
    public partial class UC_UIntEditBox : UC_UintEditBoxBase {

        private ClassLog log = new ClassLog("UC_UIntEditBox");


        public string Text { get { return this.tbEdit.Text; } }


        public UC_UIntEditBox() : base() {
            InitializeComponent();
        }


        protected override void DoSetValue(UInt64 value) {
            this.tbEdit.TextChanged -= this.tbEdit_TextChanged;
            int carretIndex = this.tbEdit.CaretIndex;
            this.tbEdit.Text = value.ToString();
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
                if (args.Key.IsUnsignedNumericForbidden()) {
                    args.Handled = true;
                }
                else {
                    if (args.Key.IsNumeric()) {
                        string add = args.Key.GetNumericValue();
                        string newVal = this.tbEdit.PreviewKeyDownAssembleText(add);
                        this.log.Info("", () => string.Format("'{0}'  '{1}'  '{2}'", this.tbEdit, add, newVal));
                        if (newVal.Length > 0) {
                            this.ValidateRange(() => { return newVal; }, args);
                        }
                    }
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }


        private void tbEdit_TextChanged(object sender, TextChangedEventArgs args) {
            try {
                this.log.Info("tbEdit_TextChanged", this.tbEdit.Text);
                if (this.tbEdit.Text.Length == 0) {
                    this.RaiseValueEmpty();
                }
                else {
                    this.RaiseValueChanged(Convert.ToUInt64(this.tbEdit.Text));
                }
            }
            catch (Exception ex) {
                this.log.Exception(9999, "", ex);
            }
        }

    }

}
