using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.XamarinForms.UIHelpers;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class AboutAppPage : ContentPage {

        public ICommand TapLinkCmd => new Command<string>(async (url) => await Launcher.OpenAsync(url));

        #region Constructor and page envents

        public AboutAppPage() {
            InitializeComponent();
            BindingContext = this;
            App.Wrapper.LanguageChanged += this.OnLanguageChanged;
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


        private void UpdateLanguage(SupportedLanguage l) {

            this.lbTitle.Text = l.GetText(MsgCode.About);
            this.btnUserManual.Text = l.GetText(MsgCode.UserManual);
            this.lbAuthor.Text = l.GetText(MsgCode.Author);
            this.lbIcons.Text = l.GetText(MsgCode.Icons);
            this.txtSupport.Text = l.GetText(MsgCode.Support);
        }



        #endregion
    }
}