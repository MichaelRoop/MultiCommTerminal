using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommWrapper.Net.Helpers;
using Rg.Plugins.Popup.Services;
using System;
using WifiCommon.Net.DataModels;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    /// <summary>Popup to get user input for Wifi credentials</summary>
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiCredRequestPopUpPage : Rg.Plugins.Popup.Pages.PopupPage {

        #region Data

        WifiCredAndIndex cred;
        WifiNetworkInfo discoverData;
        Action<WifiNetworkInfo> connectAction;

        #endregion

        public WifiCredRequestPopUpPage(
            WifiCredAndIndex cred, 
            WifiNetworkInfo discoverData,
            Action<WifiNetworkInfo> connectAction) {

            this.cred = cred;
            this.discoverData = discoverData;
            this.connectAction = connectAction;
            InitializeComponent();
            this.CloseWhenBackgroundIsClicked = false;
            this.InitiEditBoxes(this.cred.Data);
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
        }


        protected override bool OnBackButtonPressed() {
            return true;
            //return base.OnBackButtonPressed();
        }


        #region Button handlers

        private void btnSave_Clicked(object sender, EventArgs e) {
            this.cred.Data.SSID = this.edSsid.Text;
            this.cred.Data.WifiPassword = this.edPwd.Text;
            this.cred.Data.RemoteHostName = this.edHost.Text;
            this.cred.Data.RemoteServiceName = this.edPort.Text;

            App.Wrapper.SaveWifiCred(
                this.cred.Index, 
                this.cred.Data, 
                () => {
                    // Initialise the discovery data passed in. Used for actual connection
                    this.discoverData.RemoteHostName = cred.Data.RemoteHostName;
                    this.discoverData.RemoteServiceName = cred.Data.RemoteServiceName;
                    this.discoverData.Password = cred.Data.WifiPassword;
                    this.connectAction(this.discoverData);
                    PopupNavigation.Instance.PopAsync(true);
                }, 
                this.OnErr);
        }


        private void btnCancel_Clicked(object sender, EventArgs e) {
            PopupNavigation.Instance.PopAsync(true);
        }

        #endregion

        #region Private

        private void UpdateLanguage(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.Credentials);
            this.lbSsid.Text = l.GetText(MsgCode.Name);
            this.lbPwd.Text = l.GetText(MsgCode.NetworkSecurityKey);
            this.lbHost.Text = l.GetText(MsgCode.HostName);
            this.lbPort.Text = l.GetText(MsgCode.Port);
            this.btnSave.Text = l.GetText(MsgCode.save);
            this.btnCancel.Text = l.GetText(MsgCode.cancel);
        }


        private void InitiEditBoxes(WifiCredentialsDataModel cred) {
            this.edHost.Text = cred.RemoteHostName;
            this.edPort.Text = cred.RemoteServiceName;
            this.edPwd.Text = cred.WifiPassword;
            this.edSsid.Text = cred.SSID;
        }

        #endregion

    }

}