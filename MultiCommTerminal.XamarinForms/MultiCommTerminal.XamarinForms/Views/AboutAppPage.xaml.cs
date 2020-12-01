using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.XamarinForms.UIHelpers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class AboutAppPage : ContentPage {

        #region Constructor and page envents

        public AboutAppPage() {
            InitializeComponent();
            App.LanguageUpdated += this.OnLanguageChanged;             
        }

        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            base.OnAppearing();
        }

        #endregion


        #region Wrapper event handlers

        private void btnUserManual_Clicked(object sender, EventArgs e) {
            SampleLoader.LoadUserManual(this.OnErr);
        }


        private void OnLanguageChanged(object sender, SupportedLanguage e) {
            this.UpdateLanguage(e);
        }


        private void UpdateLanguage(SupportedLanguage language) {
            this.lbTitle.Text = language.GetText(MsgCode.About);
            this.btnUserManual.Text = App.GetText(MsgCode.UserManual);
        }



        #endregion
    }
}