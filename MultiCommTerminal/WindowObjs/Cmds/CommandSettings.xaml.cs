using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Windows;
using WpfCustomControlLib.Core.Helpers;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.Cmds {

    /// <summary>Interaction logic for CommandSettings.xaml</summary>
    public partial class CommandSettings : Window {

        Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        public bool IsChanged { get; set; } = false;


        #region Constructors and window events

        public static bool ShowBox(Window parent) {
            CommandSettings win = new CommandSettings(parent);
            win.ShowDialog();
            return win.IsChanged;
        }


        public CommandSettings(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.btnCmdsHc05.Content = string.Format("{0} (HC-05)", DI.Wrapper.GetText(MsgCode.commands));
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.widthManager = new ButtonGroupSizeSyncManager(
                this.btnExit, this.btnCmdsHc05);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
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

        private void btnCmdsHc05_Click(object sender, RoutedEventArgs e) {
            if (MsgBoxYesNo.ShowBox(
                this, DI.Wrapper.GetText(MsgCode.Create),
                string.Format("{0} (HC-05)", DI.Wrapper.GetText(MsgCode.commands))) == MsgBoxYesNo.MsgBoxResult.Yes) {
                this.IsChanged = true;
                DI.Wrapper.CreateHC05AtCmds(this.Close, App.ShowMsg);
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
