using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for WifiCredentials.xaml</summary>
    public partial class WifiCredentialsWin : Window {

        private Window parent = null;
        List<IIndexItem<DefaultFileExtraInfo>> indexItems = new List<IIndexItem<DefaultFileExtraInfo>>();


        public static void ShowBox(Window parent) {
            WifiCredentialsWin win = new WifiCredentialsWin(parent);
            win.ShowDialog();
        }


        /// <summary>List the stored WIFI credentials for edit or delete</summary>
        public WifiCredentialsWin(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.btnDelete.Collapse();
            this.btnEdit.Collapse();
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.ReloadList(true);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.CenterToParent(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.listBoxCreds.SelectionChanged -= this.listBoxCreds_SelectionChanged;
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            var item = this.listBoxCreds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                this.ReloadList(MsgBoxWifiCred.ShowBox(this.parent, item));
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            var item = this.listBoxCreds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                // move decision to wrapper
                if (MsgBoxYesNo.ShowBoxDelete(this, item.Display) == MsgBoxYesNo.MsgBoxResult.Yes) {
                    DI.Wrapper.DeleteWifiCredData(item, this.ReloadList, App.ShowMsg);
                }
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void listBoxCreds_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.btnEdit.Show();
            this.btnDelete.Show();
        }


        private void ReloadList(bool changed) {
            if (changed) {
                this.listBoxCreds.SelectionChanged -= this.listBoxCreds_SelectionChanged;
                DI.Wrapper.GetWifiCredList(this.OnRetrieveOk, this.OnDataError);
                this.listBoxCreds.SelectionChanged += this.listBoxCreds_SelectionChanged;

            }
        }


        private void OnRetrieveOk(List<IIndexItem<DefaultFileExtraInfo>> items) {
            this.listBoxCreds.SetNewSource(ref this.indexItems, items);
        }


        private void OnDataError(string msg) {
            App.ShowMsg(msg);
            this.Close();
        }

    }
}
