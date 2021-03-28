using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
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
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for MainSettings.xaml</summary>
    public partial class MainSettings : Window {

        Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #region Constructors and window events

        public static void ShowBox(Window parent) {
            MainSettings win = new MainSettings(parent);
            win.ShowDialog();
        }


        public MainSettings(Window parent) {
            this.parent = parent;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.widthManager = new ButtonGroupSizeSyncManager(
                this.btnExit, this.btnResetAll);
            this.widthManager.PrepForChange();
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }

        #endregion

        private void btnResetAll_Click(object sender, RoutedEventArgs e) {
            if (MsgBoxYesNo.ShowBox(
                this, 
                DI.Wrapper.GetText(MsgCode.Warning),
                DI.Wrapper.GetText(MsgCode.Continue), true) == MsgBoxYesNo.MsgBoxResult.Yes) {
                DI.Wrapper.RebuildAllData();
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }
    }
}
