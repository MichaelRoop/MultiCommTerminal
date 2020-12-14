using LogUtils.Net;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Diagnostics;
using System.Windows;
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


        /// <summary>Opens the browser because of the execution of a hyperlink</summary>
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
