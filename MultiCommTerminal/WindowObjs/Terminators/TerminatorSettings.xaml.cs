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
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.Terminators {
    /// <summary>
    /// Interaction logic for TerminatorSettings.xaml
    /// </summary>
    public partial class TerminatorSettings : Window {

        Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        public bool IsChanged { get; set; } = false;

        #region Constructors and window events

        public static bool ShowBox(Window parent) {
            TerminatorSettings win = new TerminatorSettings(parent);
            win.ShowDialog();
            return win.IsChanged;
        }


        public TerminatorSettings(Window parent) {
            this.parent = parent;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.btnDefault.Content = string.Format("{0} \\n\\r", DI.Wrapper.GetText(MsgCode.Default));
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.widthManager = new ButtonGroupSizeSyncManager(
                this.btnExit, this.btnArduino);
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

        private void btnDefault_Click(object sender, RoutedEventArgs e) {
            if (MsgBoxYesNo.ShowBox(
                this, DI.Wrapper.GetText(MsgCode.Create),
                string.Format("{0} \\n\\r", DI.Wrapper.GetText(MsgCode.Default))) == MsgBoxYesNo.MsgBoxResult.Yes) {
                this.IsChanged = true;
                DI.Wrapper.CreateDefaultTerminators(this.Close, App.ShowMsg);
            }
        }


        private void btnArduino_Click(object sender, RoutedEventArgs e) {
            if (MsgBoxYesNo.ShowBox(
                this, DI.Wrapper.GetText(MsgCode.Create),
                string.Format("{0} (Arduino)", DI.Wrapper.GetText(MsgCode.Terminators))) == MsgBoxYesNo.MsgBoxResult.Yes) {
                this.IsChanged = true;
                DI.Wrapper.CreateArduinoTerminators(this.Close, App.ShowMsg);
            }
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
