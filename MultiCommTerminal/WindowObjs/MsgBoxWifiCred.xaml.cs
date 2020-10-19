using MultiCommTerminal.WPF_Helpers;
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
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for MsgBoxWifiCred.xaml</summary>
    public partial class MsgBoxWifiCred : Window {

        #region Data types

        public class WifiCredResult {
            public bool IsOk { get; set; } = false;
            public string HostName { get; set; } = string.Empty;
            public string ServiceName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        #endregion

        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #endregion

        public WifiCredResult Result { get; set; } = new WifiCredResult();

        #region Static methods

        public static WifiCredResult ShowBox(Window win, string title) {
            MsgBoxWifiCred box = new MsgBoxWifiCred(win, title);
            box.ShowDialog();
            return box.Result;
        }

        #endregion


        public MsgBoxWifiCred(Window parent, string title) {
            this.parent = parent;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            this.HideTitleBarIcon();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {

        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            // Validate entries in wrapper level
            this.Result.IsOk = true;
            this.Result.HostName = txtHostName.Text;
            this.Result.ServiceName = txtServiceName.Text;
            this.Result.Password = this.txtPwd.Password;
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Result.IsOk = false;
            this.Close();
        }
    }
}
