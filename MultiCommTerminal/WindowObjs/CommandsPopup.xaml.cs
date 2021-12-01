using MultiCommData.Net.Enumerations;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Popup to select different command set</summary>
    public partial class CommandsPopup : Window {

        private Window parent = null;
        private CommMedium medium = CommMedium.None;

        public static void ShowBox(Window parent, CommMedium medium) {
            CommandsPopup win = new CommandsPopup(parent, medium);
            win.ShowDialog();
        }


        private CommandsPopup(Window parent, CommMedium medium) {
            this.parent = parent;
            this.medium = medium;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            DI.Wrapper.GetScriptList(this.LoadList, this.OnGetError);
            this.lbxCmds.SelectionChanged += this.selectionChangedHandler;
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbxCmds.SelectionChanged -= this.selectionChangedHandler;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void selectionChangedHandler(object sender, SelectionChangedEventArgs e) {
            IIndexItem<DefaultFileExtraInfo> item = this.lbxCmds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            DI.Wrapper.SetCurrentScript(item, this.medium, this.Close, App.ShowMsg);
        }


        private void OnGetError(string err) {
            App.ShowMsg(err);
            this.Close();
        }


        private void LoadList(List<IIndexItem<DefaultFileExtraInfo>> items) {
            this.lbxCmds.ItemsSource = items;
        }


    }
}
