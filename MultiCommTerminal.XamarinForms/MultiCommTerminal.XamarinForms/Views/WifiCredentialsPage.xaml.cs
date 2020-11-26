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

    public partial class WifiCredentialsPage : ContentPage {

        #region Constructors and overrides

        public WifiCredentialsPage() {
            InitializeComponent();
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            base.OnAppearing();
        }


        #endregion

        #region Event handlers

        private void LanguageUpdate(SupportedLanguage language) {
            this.lbTitle.Text = language.GetText(MsgCode.Credentials);

            //this.btnDiscover.Text = language.GetText(MsgCode.discover);
            //this.btnSelect.Text = language.GetText(MsgCode.select);
        }


        #endregion

    }
}