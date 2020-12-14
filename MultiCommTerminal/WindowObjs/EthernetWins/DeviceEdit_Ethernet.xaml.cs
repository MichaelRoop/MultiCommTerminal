using Ethernet.Common.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for DeviceEdit_Ethernet.xaml</summary>
    public partial class DeviceEdit_Ethernet : Window {

        #region Data

        private enum Mode {
            Edit,
            Create,
        }

        private Mode mode = Mode.Edit;
        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private IIndexItem<DefaultFileExtraInfo> index = null;
        private EthernetParams data = new EthernetParams();

        #endregion

        #region Properties

        public bool IsChanged { get; set; } = false;

        #endregion

        #region Constructors and windows events

        public static bool ShowBoxEdit(Window parent, IIndexItem<DefaultFileExtraInfo> index) {
            DeviceEdit_Ethernet win = new DeviceEdit_Ethernet(parent, index);
            win.ShowDialog();
            return win.IsChanged;
        }


        public static bool ShowBoxAdd(Window parent, IIndexItem<DefaultFileExtraInfo> index) {
            return ShowBoxEdit(parent, null);
        }



        public DeviceEdit_Ethernet(Window parent, IIndexItem<DefaultFileExtraInfo> index) {
            this.parent = parent;
            this.index = index;
            this.mode = this.index == null ? Mode.Create : Mode.Edit;
            InitializeComponent();
            this.Init();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSave);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.widthManager.Teardown();
        }

        #endregion

        #region Button event handlers

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            this.data.Name = this.txtName.Text;
            this.data.EthernetAddress = this.txtHostName.Text;
            this.data.EthernetServiceName = this.txtServiceName.Text;

            switch (this.mode) {
                case Mode.Edit:
                    DI.Wrapper.SaveEthernetData(
                        this.index, this.data, this.OnSaveOk, this.OnFailure);
                    break;
                case Mode.Create:
                    DI.Wrapper.CreateNewEthernetData(this.data, this.OnSaveOk, this.OnFailure);
                    break;
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.IsChanged = false;
            this.Close();
        }

        #endregion

        #region Private

        private void Init() {
            switch (this.mode) {
                case Mode.Edit:
                    DI.Wrapper.RetrieveEthernetData(this.index,
                        (data) => {
                            this.data = data;
                            this.Title = DI.Wrapper.GetText(MsgCode.Edit);
                        },
                        this.OnFailure);
                    break;
                case Mode.Create:
                    this.data.Name = "SampleName";
                    this.data.EthernetAddress = "192.168.1.100";
                    this.data.EthernetServiceName = "9999";
                    this.Title = DI.Wrapper.GetText(MsgCode.Create);
                    break;
            }

            this.txtName.Text = this.data.Name;
            this.txtHostName.Text = this.data.EthernetAddress;
            this.txtServiceName.Text = this.data.EthernetServiceName;
        }


        private void OnSaveOk() {
            this.IsChanged = true;
            this.Close();
        }


        private void OnFailure(string err) {
            App.ShowMsg(err);
            this.Close();
        }

        #endregion

    }
}
