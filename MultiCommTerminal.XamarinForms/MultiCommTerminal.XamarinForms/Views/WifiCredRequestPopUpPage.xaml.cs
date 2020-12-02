using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
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


        WifiCredentials cred;

        public WifiCredRequestPopUpPage(WifiCredentials cred) {
            this.cred = cred;
            InitializeComponent();
            this.InitiEditBoxes(this.cred);
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
        }


        private void btnSave_Clicked(object sender, EventArgs e) {
            // TODO - validate data
            this.cred.SSID = this.edSsid.Text;
            this.cred.WifiPassword = this.edPwd.Text;
            this.cred.RemoteHostName = this.edHost.Text;
            this.cred.RemoteServiceName = this.edPort.Text;

            // Then close the edit box
            // May be a problem with the call being async. Might have to make it sync to hold
            // up wrapper until complete
            // Now close the popup
            // await PopupNavigation.Instance.PushAsync(new WifiCredRequestPopUpPage(cred));
            this.cred.CompletedEvent.Set();
            PopupNavigation.Instance.PopAsync(true);

        }


        private void UpdateLanguage(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.Credentials);
            this.lbSsid.Text = l.GetText(MsgCode.Name);
            this.lbPwd.Text = l.GetText(MsgCode.NetworkSecurityKey);
            this.lbHost.Text = l.GetText(MsgCode.HostName);
            this.lbPort.Text = l.GetText(MsgCode.Port);
            this.btnSave.Text = l.GetText(MsgCode.save);
        }


        private void InitiEditBoxes(WifiCredentials cred) {
            this.edHost.Text = cred.RemoteHostName;
            this.edPort.Text = cred.RemoteServiceName;
            this.edPwd.Text = cred.WifiPassword;
            this.edSsid.Text = cred.SSID;
        }


    }

}