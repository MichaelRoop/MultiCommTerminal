using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Popup to select different command set</summary>
    public partial class CommandsPopup : Window {

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private ScriptDataModel original = null;


        public static void ShowBox(Window parent) {
            CommandsPopup win = new CommandsPopup(parent);
            win.ShowDialog();
        }


        private CommandsPopup(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            DI.Wrapper.GetSettings(this.OnGetOk, this.OnGetError);
            DI.Wrapper.GetScriptList(this.LoadList, this.OnGetError);
            this.lbxCmds.SelectionChanged += this.selectionChangedHandler;
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbxCmds.SelectionChanged -= this.selectionChangedHandler;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            DI.Wrapper.SetCurrentScript(this.original, this.OnGetError);
            this.Close();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void selectionChangedHandler(object sender, SelectionChangedEventArgs e) {
            IIndexItem<DefaultFileExtraInfo> item = this.lbxCmds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            DI.Wrapper.SetCurrentScript(item, () => { }, App.ShowMsg);
        }


        private void OnGetOk(SettingItems items) {
            if (items == null || items.CurrentScript == null) {
                this.OnGetError("NULL COMMAND SET");
            }
            else {
                this.original = items.CurrentScript;
            }
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
