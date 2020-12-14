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
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {
    /// <summary>
    /// Interaction logic for WifiRun.xaml
    /// </summary>
    public partial class WifiRun : Window {

        Window parent = null;

        public WifiRun(Window parent) {
            this.parent = parent;
            InitializeComponent();

            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;


            this.ui.ExitClicked+= this.OnUiExit;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            this.HideTitleBarIcon();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.ui.OnLoad(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

            this.ui.OnClosing();
        }


        #region UI Event handlers

        //EventHandler

        private void OnUiExit(object sender, EventArgs e) {
            this.Close();
        }


        #endregion

    }
}
