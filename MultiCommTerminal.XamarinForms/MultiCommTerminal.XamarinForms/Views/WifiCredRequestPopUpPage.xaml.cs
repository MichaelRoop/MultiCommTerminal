using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommWrapper.Net.Helpers;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WifiCommon.Net.DataModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiCredRequestPopUpPage : Rg.Plugins.Popup.Pages.PopupPage {


        WifiCredAndIndex cred;
        WifiNetworkInfo discoverData;


        public WifiCredRequestPopUpPage(WifiCredAndIndex cred, WifiNetworkInfo discoverData) {
            this.cred = cred;
            this.discoverData = discoverData;
            InitializeComponent();
            this.InitiEditBoxes(this.cred.Data);
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
        }


        private void btnSave_Clicked(object sender, EventArgs e) {
            // TODO - validate data
            this.cred.Data.SSID = this.edSsid.Text;
            this.cred.Data.WifiPassword = this.edPwd.Text;
            this.cred.Data.RemoteHostName = this.edHost.Text;
            this.cred.Data.RemoteServiceName = this.edPort.Text;

            // Then close the edit box
            // May be a problem with the call being async. Might have to make it sync to hold
            // up wrapper until complete
            // Now close the popup
            // await PopupNavigation.Instance.PushAsync(new WifiCredRequestPopUpPage(cred));
            App.Wrapper.SaveWifiCred(
                this.cred.Index, 
                this.cred.Data, 
                () => {
                    // Initialise the discovery data passed in. It will be passed in for connection
                    this.discoverData.RemoteHostName = cred.Data.RemoteHostName;
                    this.discoverData.RemoteServiceName = cred.Data.RemoteServiceName;
                    this.discoverData.Password = cred.Data.WifiPassword;
                    PopupNavigation.Instance.PopAsync(true);
                }, 
                this.OnErr);





        }


        private void UpdateLanguage(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.Credentials);
            this.lbSsid.Text = l.GetText(MsgCode.Name);
            this.lbPwd.Text = l.GetText(MsgCode.NetworkSecurityKey);
            this.lbHost.Text = l.GetText(MsgCode.HostName);
            this.lbPort.Text = l.GetText(MsgCode.Port);
            this.btnSave.Text = l.GetText(MsgCode.save);
        }


        private void InitiEditBoxes(WifiCredentialsDataModel cred) {
            this.edHost.Text = cred.RemoteHostName;
            this.edPort.Text = cred.RemoteServiceName;
            this.edPwd.Text = cred.WifiPassword;
            this.edSsid.Text = cred.SSID;
        }


    }

}