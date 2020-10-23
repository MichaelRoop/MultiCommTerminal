using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for MsgBoxWifiCred.xaml</summary>
    public partial class MsgBoxWifiCred : Window {

        #region Data types

        public class WifiCredResult {
            public bool IsOk { get; set; } = false;
            public bool Save { get; set; } = false;
            public string HostName { get; set; } = string.Empty;
            public string ServiceName { get; set; } = string.Empty;
            public string Password { get; set; } = string.Empty;
        }

        #endregion

        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        // For when we are editing existing
        private IIndexItem<DefaultFileExtraInfo> index = null;
        WifiCredentialsDataModel originalData = null;

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
            box.ShowDialog();
            return box.Result;
        }


        /// <summary>Retrieve data from storage to edit and save</summary>
        /// <param name="parent">The parent window</param>
        /// <param name="index">The data item index object</param>
        public static bool ShowBox(Window parent, IIndexItem<DefaultFileExtraInfo> index) {
            MsgBoxWifiCred box = new MsgBoxWifiCred(parent, index);
            box.ShowDialog();
            return box.Result.IsOk;
        }

        #endregion


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
            this.chkSave.Collapse();
            this.txtChkSave.Collapse();

            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            this.HideTitleBarIcon();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            if (this.index == null) {
                // Validate entries in wrapper level
                this.Result.IsOk = true;
                this.Result.Save = this.chkSave.IsChecked.GetValueOrDefault(false);
                this.Result.HostName = txtHostName.Text;
                this.Result.ServiceName = txtServiceName.Text;
                this.Result.Password = this.txtPwd.Password;
                this.Close();
            }
            else {
                // This is an edit 
                // TODO - validation of entries?
                DI.Wrapper.SaveWifiCred(
                    this.index, this.originalData, this.OnSaveOk, this.OnDataErr);
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Result.IsOk = false;
            this.Close();
        }


        private void PopulateFields(IIndexItem<DefaultFileExtraInfo> itemIndex) {
            DI.Wrapper.RetrieveWifiCredData(itemIndex, this.OnRetrieveOk, this.OnDataErr);
        }


        private void OnRetrieveOk(WifiCredentialsDataModel data) {
            this.originalData = data;
            this.Title = data.SSID;
            this.txtPwd.Password = data.WifiPassword;
            this.txtHostName.Text = data.RemoteHostName;
            this.txtServiceName.Text = data.RemoteServiceName;
        }


        private void OnSaveOk() {
            this.Result.IsOk = true;
            this.Close();
        }


        private void OnDataErr(string err) {
            this.Result.IsOk = false;
            App.ShowMsg(err);
            Close();
        }



    }
}
