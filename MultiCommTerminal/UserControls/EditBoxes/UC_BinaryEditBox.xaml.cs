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

namespace MultiCommTerminal.NetCore.UserControls.EditBoxes {
    
    /// <summary>Interaction logic for UC_BinaryEditBox.xaml</summary>
    public partial class UC_BinaryEditBox : UserControl {

        public string Text {
            get {
                return this.tbEdit.Text;
            }
            set {
                this.tbEdit.TextChanged -= this.tbEdit_TextChanged;
                this.tbEdit.Text = value;
                this.tbEdit.TextChanged += this.tbEdit_TextChanged;
            }
        }


        public UC_BinaryEditBox() {
            InitializeComponent();
        }


        private void tbEdit_PreviewKeyDown(object sender, KeyEventArgs e) {

        }

        private void tbEdit_TextChanged(object sender, TextChangedEventArgs e) {

        }



        private bool IsForbidden(Key key) {
            return
                key.IsNumericForbidden()
                || key.IsNonBinaryNumeric()
                || key.IsWhitespace()
                || key.IsLetter()
                || key.IsDecimal();

                

        }


    }
}
