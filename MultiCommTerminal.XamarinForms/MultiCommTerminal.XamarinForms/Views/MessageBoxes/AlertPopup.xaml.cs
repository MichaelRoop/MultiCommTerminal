using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using Rg.Plugins.Popup.Services;
using System;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views.MessageBoxes {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class AlertPopup : Rg.Plugins.Popup.Pages.PopupPage {

        public AlertPopup(string title, string msg) {
            InitializeComponent();
            this.CloseWhenBackgroundIsClicked = false;
            this.lbTitle.Text = title;
            this.lbMsg.Text = msg;
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.updateLanguage);
            base.OnAppearing();
        }


        protected override bool OnBackButtonPressed() {
            return true;
        }


        private void btnOk_Clicked(object sender, EventArgs e) {
            PopupNavigation.Instance.PopAsync(true);
        }


        private void updateLanguage(SupportedLanguage l) {
            this.btnOk.Text = l.GetText(MsgCode.Ok);
        }


    }
}