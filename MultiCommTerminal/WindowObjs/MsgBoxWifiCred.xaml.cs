using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.WifiWins;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for MsgBoxWifiCred.xaml</summary>
    public partial class MsgBoxWifiCred : Window {

        enum ProcessType {
            Edit,
            Init,
            Create,
        }


        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        // For when we are editing existing
        private IIndexItem<DefaultFileExtraInfo> index = null;
        private WifiCredentialsDataModel dataModelToEdit = null;
        private ProcessType processType = ProcessType.Edit;

        #endregion

        public WifiCredResult Result { get; set; } = new WifiCredResult();

        #region Static methods

        /// <summary>
        /// Get user data when using credentials for first time. Will not
        /// be save unless the connection is successful
        /// </summary>
        /// <param name="win">Parent window</param>
        /// <param name="ssid">SSID as the box title</param>
        /// <param name="host">Network host name or IP address</param>
        /// <param name="service">Network port</param>
        /// <returns></returns>
        public static WifiCredResult ShowBox(Window win, string ssid, string host, string service) {
            MsgBoxWifiCred box = new MsgBoxWifiCred(win, ssid, host, service);
            box.processType = ProcessType.Init;
            box.ShowDialog();
            return box.Result;
        }


        /// <summary>Retrieve data from storage to edit and save</summary>
        /// <param name="parent">The parent window</param>
        /// <param name="index">The data item index object</param>
        public static bool ShowBox(Window parent, IIndexItem<DefaultFileExtraInfo> index) {
            MsgBoxWifiCred box = new MsgBoxWifiCred(parent, index);
            box.processType = ProcessType.Edit;
            box.ShowDialog();
            return box.Result.IsChanged;
        }


        public static bool ShowBox(Window parent) {
            MsgBoxWifiCred box = new MsgBoxWifiCred(parent);
            box.processType = ProcessType.Create;
            box.ShowDialog();
            return box.Result.IsChanged;
        }

        #endregion


        public MsgBoxWifiCred(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.Title = DI.Wrapper.GetText(MsgCode.Create);
            this.lbSSID.Show();
            this.txtSSID.Show();

            this.txtHostName.Text = string.Empty;
            this.txtServiceName.Text = string.Empty;
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }


        public MsgBoxWifiCred(Window parent, string title, string host, string service) {
            this.parent = parent;
            InitializeComponent();
            this.Title = title;
            this.txtHostName.Text = host;
            this.txtServiceName.Text = service;
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }



        public MsgBoxWifiCred(Window parent, IIndexItem<DefaultFileExtraInfo> index) {
            this.parent = parent;
            this.index = index;
            this.InitializeComponent();

            this.PopulateFields(this.index);
            WPF_ControlHelpers.CenterChild(parent, this);
            this.btnOk.Content = DI.Wrapper.GetText(MsgCode.save);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            switch (this.processType) {
                case ProcessType.Edit:
                    // This is an edit of an existing
                    this.dataModelToEdit.RemoteHostName = this.txtHostName.Text;
                    this.dataModelToEdit.RemoteServiceName = this.txtServiceName.Text;
                    this.dataModelToEdit.WifiPassword = this.txtPwd.Password;
                    DI.Wrapper.SaveWifiCred(
                        this.index, this.dataModelToEdit, this.OnSaveOk, this.OnDataErr);
                    break;
                case ProcessType.Init:
                    if (this.index == null) {
                        // Validate entries in wrapper level
                        this.Result.IsChanged = true;
                        this.Result.HostName = txtHostName.Text;
                        this.Result.ServiceName = txtServiceName.Text;
                        this.Result.Password = this.txtPwd.Password;
                        this.Close();
                    }
                    break;
                case ProcessType.Create:
                    WifiCredentialsDataModel dm = new WifiCredentialsDataModel() {
                        SSID = this.txtSSID.Text,
                        RemoteHostName = this.txtHostName.Text,
                        RemoteServiceName = this.txtServiceName.Text,
                        WifiPassword = this.txtPwd.Password,
                    };
                    DI.Wrapper.CreateNewWifiCred(dm.SSID, dm, this.OnSaveOk, this.OnDataErr);
                    break;
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Result.IsChanged = false;
            this.Close();
        }


        private void PopulateFields(IIndexItem<DefaultFileExtraInfo> itemIndex) {
            DI.Wrapper.RetrieveWifiCredData(itemIndex, this.OnRetrieveOk, this.OnDataErrExit);
        }


        private void OnRetrieveOk(WifiCredentialsDataModel data) {
            this.dataModelToEdit = data;
            this.Title = data.SSID;
            this.txtPwd.Password = data.WifiPassword;
            this.txtHostName.Text = data.RemoteHostName;
            this.txtServiceName.Text = data.RemoteServiceName;
        }


        private void OnSaveOk() {
            this.Result.IsChanged = true;
            this.Close();
        }


        private void OnDataErr(string err) {
            App.ShowMsg(err);
        }


        private void OnDataErrExit(string err) {
            App.ShowMsg(err);
            this.Result.IsChanged = false;
            Close();
        }


    }
}
