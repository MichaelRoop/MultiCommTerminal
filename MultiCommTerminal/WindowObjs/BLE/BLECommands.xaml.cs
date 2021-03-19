using LanguageFactory.Net.data;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BLE {

    /// <summary>Interaction logic for BLECommands.xaml</summary>
    public partial class BLECommands : Window {

        private Window parent = null;

        #region Constructors

        public static void ShowBox(Window parent) {
            BLECommands win = new BLECommands(parent);
            win.ShowDialog();
        }


        public BLECommands(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.spEditButtons.Collapse();
        }

        #endregion

        #region Windows events

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.ReloadList(true);
            this.CenterToParent(this.parent);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbxCmds.SelectionChanged -= this.selectionChanged;
        }

        #endregion

        #region Control events

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            BLESelectDataType.ShowBox(this);
            this.ReloadList(true);
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            var item = this.lbxCmds.SelectedItem as IIndexItem<BLECmdIndexExtraInfo>;
            if (item != null) {
                BLECommandsEdit.ShowBox(this, item);
            }
            else {
                App.ShowMsg(DI.Wrapper.GetText(MsgCode.NothingSelected));
            }
            this.ReloadList(true);
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            var item = this.lbxCmds.SelectedItem as IIndexItem<BLECmdIndexExtraInfo>;
            if (item != null) {
                if (MsgBoxYesNo.ShowBoxDelete(this, item.Display) == MsgBoxYesNo.MsgBoxResult.Yes) {
                    DI.Wrapper.DeleteBLECmdSet(item, this.ReloadList, App.ShowMsg);
                }
            }
        }


        private void btnSettings_Click(object sender, RoutedEventArgs e) {
            // No needed?
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void selectionChanged(object sender, SelectionChangedEventArgs e) {
            this.spEditButtons.Visibility = Visibility.Visible;
        }

        #endregion

        #region Private

        private void ReloadList(bool isChanged) {
            if (isChanged) {
                this.lbxCmds.SelectionChanged -= this.selectionChanged;
                DI.Wrapper.GetBLECmdList(list => {
                    this.lbxCmds.ItemsSource = null;
                    this.lbxCmds.SelectedItem = null;
                    this.lbxCmds.ItemsSource = list;
                }, App.ShowMsg);
                this.lbxCmds.SelectionChanged += this.selectionChanged;
            }
        }

        #endregion

    }
}
