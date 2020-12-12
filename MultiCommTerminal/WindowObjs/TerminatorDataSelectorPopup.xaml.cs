using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for TerminatorDataSelectorPopup.xaml</summary>
    public partial class TerminatorDataSelectorPopup : Window {

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private TerminatorDataModel original = null;

        #region Constructors and window events

        public static void ShowBox(Window parent) {
            TerminatorDataSelectorPopup win = new TerminatorDataSelectorPopup(parent);
            win.ShowDialog();
        }


        public TerminatorDataSelectorPopup(Window parent) {
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
            // Get the original in case of cancel
            DI.Wrapper.GetSettings(this.GetOriginal, this.HandleError);
            this.LoadList();
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            DI.Wrapper.SetCurrentTerminators(this.original, this.HandleError);
            this.Close();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void listBoxTerminators_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = this.listBoxTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            DI.Wrapper.SetCurrentTerminators(item, () => { }, App.ShowMsg);
        }


        #region Private

        private void LoadList() {
            this.listBoxTerminators.SelectionChanged -= this.listBoxTerminators_SelectionChanged;
            DI.Wrapper.GetTerminatorList(
                (items) => {
                    this.listBoxTerminators.ItemsSource = null;
                    this.listBoxTerminators.ItemsSource = items;
                }, App.ShowMsg);
            this.listBoxTerminators.SelectionChanged += this.listBoxTerminators_SelectionChanged;
        }


        private void GetOriginal(SettingItems settings) {
            this.original = settings.CurrentTerminator;
            if (this.original == null) {
                this.HandleError(DI.Wrapper.GetText(MsgCode.ReadFailure));
            }
        }

        private void HandleError(string err) {
            App.ShowMsg(err);
            this.Close();
        }


        #endregion


    }
}
