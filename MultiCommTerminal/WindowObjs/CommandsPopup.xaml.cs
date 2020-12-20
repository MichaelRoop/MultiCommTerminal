using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
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
            DI.Wrapper.SetCurrentScript(this.original, this.medium, this.OnGetError);
            this.Close();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void selectionChangedHandler(object sender, SelectionChangedEventArgs e) {
            IIndexItem<DefaultFileExtraInfo> item = this.lbxCmds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            DI.Wrapper.SetCurrentScript(item, this.medium, () => { }, App.ShowMsg);
        }


        private void OnGetOk(SettingItems settings) {
            if (settings == null) {
                this.OnGetError("NULL COMMAND SET");
                return;
            }

            this.original = settings.CurrentScript;
            switch (this.medium) {
                case CommMedium.Bluetooth:
                    this.original = settings.CurrentScriptBT;
                    break;
                case CommMedium.BluetoothLE:
                    this.original = settings.CurrentScriptBLE;
                    break;
                case CommMedium.Ethernet:
                    this.original = settings.CurrentScriptEthernet;
                    break;
                case CommMedium.Usb:
                    this.original = settings.CurrentScriptUSB;
                    break;
                case CommMedium.Wifi:
                    this.original = settings.CurrentScriptWIFI;
                    break;
                default:
                    this.original = settings.CurrentScript;
                    break;
            }

            if (this.original == null) {
                this.original = settings.CurrentScript;
            }

            // This would be an error where both the specific and default is null. So, a read failure
            if (this.original == null) {
                this.OnGetError(DI.Wrapper.GetText(MsgCode.ReadFailure));
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
