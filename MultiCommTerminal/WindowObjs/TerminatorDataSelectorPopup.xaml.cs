using MultiCommData.Net.Enumerations;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Windows;
using System.Windows.Controls;
using WpfCustomControlLib.Core.Helpers;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for TerminatorDataSelectorPopup.xaml</summary>
    public partial class TerminatorDataSelectorPopup : Window {

        private Window parent = null;
        private CommMedium medium = CommMedium.None;

        #region Constructors and window events

        public static void ShowBox(Window parent, CommMedium medium) {
            TerminatorDataSelectorPopup win = new TerminatorDataSelectorPopup(parent, medium);
            win.ShowDialog();
        }


        public TerminatorDataSelectorPopup(Window parent, CommMedium medium) {
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
            this.LoadList();
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.listBoxTerminators.SelectionChanged -= this.listBoxTerminators_SelectionChanged;
        }

        #endregion

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void listBoxTerminators_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = this.listBoxTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            DI.Wrapper.SetCurrentTerminators(item, this.medium, this.Close, App.ShowMsg);
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

        #endregion

    }
}
