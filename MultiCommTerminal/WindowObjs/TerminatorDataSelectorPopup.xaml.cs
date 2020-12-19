using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
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
            DI.Wrapper.SetCurrentTerminators(this.original, this.medium, this.HandleError);
            this.Close();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void listBoxTerminators_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            var item = this.listBoxTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            DI.Wrapper.SetCurrentTerminators(item, this.medium, () => { }, App.ShowMsg);
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
            switch (this.medium) {
                case CommMedium.Bluetooth:
                    this.original = settings.CurrentTerminatorBT;
                    break;
                case CommMedium.BluetoothLE:
                    this.original = settings.CurrentTerminatorBLE;
                    break;
                case CommMedium.Ethernet:
                    this.original = settings.CurrentTerminatorEthernet;
                    break;
                case CommMedium.Usb:
                    this.original = settings.CurrentTerminatorUSB;
                    break;
                case CommMedium.Wifi:
                    this.original = settings.CurrentTerminatorWIFI;
                    break;
                default:
                    break;
            }

            // Default with a know terminator set
            if (this.original == null) {
                this.original = settings.CurrentTerminator;
            }

            // This would be an error where both the specific and default is null. So, a read failure
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
