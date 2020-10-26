using LogUtils.Net;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for AboutWin.xaml</summary>
    public partial class AboutWin : Window {

        #region Data

        private Window parent = null;

        #endregion


        public static void ShowBox(Window parent) {
            AboutWin win = new AboutWin(parent);
            win.ShowDialog();
        }

        public AboutWin(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.lblBuild.Content = App.Build;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void Hyperlink_RequestNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e) {
            try {
                Process.Start(new ProcessStartInfo(e.Uri.AbsoluteUri) { UseShellExecute = true });
            e.Handled = true;
            }
            catch (Exception ex) {
                Log.Exception(9999, "", ex);
            }
        }

    }
}
