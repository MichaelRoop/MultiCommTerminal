using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.Terminators;
using MultiCommTerminal.NetCore.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>List, add, modify and delete terminator data items</summary>
    public partial class TerminatorDataSelector : Window {

        private Window parent = null;
        private ICommWrapper wrapper = null;

        #region Constructors and window events

        public static void ShowBox(Window parent) {
            TerminatorDataSelector win = new TerminatorDataSelector(parent);
            win.ShowDialog();
        }


        public TerminatorDataSelector(Window parent) {
            this.wrapper = DI.Wrapper;
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.spEditButtons.Visibility = Visibility.Collapsed;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ReloadList(true);
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        #endregion

        #region Sidebar buttons

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            TerminatorEditor win = new TerminatorEditor(this, null);
            win.ShowDialog();
            this.ReloadList(win.IsChanged);
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            var item = this.listBoxTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                TerminatorEditor win = new TerminatorEditor(this, item);
                win.ShowDialog();
                this.ReloadList(win.IsChanged);
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            var item = this.listBoxTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                if (MsgBoxYesNo.ShowBoxDelete(this, item.Display) == MsgBoxYesNo.MsgBoxResult.Yes) {
                    this.wrapper.DeleteTerminatorData(item, this.ReloadList, App.ShowMsg);
                }
            }
        }


        private void btnSettings_Click(object sender, RoutedEventArgs e) {
            this.ReloadList(TerminatorSettings.ShowBox(this));
        }

        #endregion

        #region Window buttons

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        #endregion

        private void listBoxTerminators_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.spEditButtons.Visibility = Visibility.Visible;

        }


        private void ReloadList(bool isChanged) {
            if (isChanged) {
                this.listBoxTerminators.SelectionChanged -= this.listBoxTerminators_SelectionChanged;
                this.wrapper.GetTerminatorList(
                    (items) => {
                        this.listBoxTerminators.ItemsSource = null;
                        this.listBoxTerminators.ItemsSource = items;
                    }, App.ShowMsg);
                this.listBoxTerminators.SelectionChanged += this.listBoxTerminators_SelectionChanged;
            }
        }

    }
}
