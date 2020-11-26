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

        private void OnLanguageChanged(object sender, SupportedLanguage e) {
            this.UpdateLanguage(e);
        }


        private void UpdateLanguage(SupportedLanguage language) {
            this.lbTitle.Text = language.GetText(MsgCode.About);
        }

        #endregion



    }
}