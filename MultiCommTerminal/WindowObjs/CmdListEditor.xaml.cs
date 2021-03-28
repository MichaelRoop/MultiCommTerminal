using MultiCommTerminal.NetCore.WPF_Helpers;
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
using System.Windows.Shapes;
using WpfCustomControlLib.Core.Helpers;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for CmdListEditor.xaml</summary>
    public partial class CmdListEditor : Window {

        public CmdListEditor() {
            InitializeComponent();
            // TODO - Look at XAML
            // Title bar title bound to Win.Title
            // Title bar icon bound to Win.Icon
        }


        /// <summary>MouseDown event mapped to template title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e) {
            DragMove();
        }


        private void btnExis_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
