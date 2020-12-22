using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.UserDisplayData;
using MultiCommTerminal.XamarinForms.UIHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Resources;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class SamplesPage : ContentPage {

        public SamplesPage() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            base.OnAppearing();
        }

        private void btnBluetooth_Clicked(object sender, EventArgs e) {
            this.PostSample(CommMedium.Bluetooth);
        }

        private void btnWifi_Clicked(object sender, EventArgs e) {
            this.PostSample(CommMedium.Wifi);
        }

        private void btnCopy_Clicked(object sender, EventArgs e) {
            if (!string.IsNullOrWhiteSpace(this.edSample.Text)) {
                Device.BeginInvokeOnMainThread(async () => {
                    await Clipboard.SetTextAsync(this.edSample.Text);
                    // TODO - need msg box?
                });
            }
        }


        private void LanguageUpdate(SupportedLanguage l) {
            this.btnCopy.Text = l.GetText(MsgCode.copy);
            this.lbTitle.Text = l.GetText(MsgCode.select);
            AutomationProperties.SetIsInAccessibleTree(this.btnBluetooth, true);
            this.btnBluetooth.SetScreenReader("Bluetooth");
            this.btnWifi.SetScreenReader("WIFI");
        }


        private void PostSample(CommMedium commHelpType) {
            SampleLoader.Load(commHelpType, this.OnLoadOk, this.OnErr);
        }



        private void OnLoadOk(string txt) {
            this.edSample.Text = txt;
            this.scrlText.ScrollToAsync(0, 0, true);
        }


    }
}