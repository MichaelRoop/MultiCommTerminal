using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class WifiCredentialsModalEditPage : ContentPage {

        public WifiCredentialsModalEditPage() {
            InitializeComponent();
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            base.OnAppearing();
        }


        private void btnSave_Clicked(object sender, EventArgs e) {

        }

        private void btnDelete_Clicked(object sender, EventArgs e) {

        }

        private void btnCancel_Clicked(object sender, EventArgs e) {

        }


        private void LanguageUpdate(SupportedLanguage l) {
            // TODO - any title

            this.lbSsid.Text = l.GetText(MsgCode.Name);
            this.lbPwd.Text = l.GetText(MsgCode.NetworkSecurityKey);
            this.lbHost.Text = l.GetText(MsgCode.HostName);
            this.lbPort.Text = l.GetText(MsgCode.Port);
        }

    }
}