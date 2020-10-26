using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>List, add, modify and delete terminator data items</summary>
    public partial class TerminatorDataSelector : Window {

        private Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;

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

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            ReloadList(true);
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
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

        #endregion

        #region Window buttons

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            var item = this.listBoxTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                this.wrapper.SetCurrentTerminators(item, this.Close, App.ShowMsg);
            }
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
