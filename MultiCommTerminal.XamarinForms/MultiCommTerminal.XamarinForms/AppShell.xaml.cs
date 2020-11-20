using MultiCommTerminal.XamarinForms.ViewModels;
using MultiCommTerminal.XamarinForms.Views;
using System;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms {
    public partial class AppShell : Xamarin.Forms.Shell {
        public AppShell() {
            InitializeComponent();
            Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));
            Routing.RegisterRoute(nameof(LanguagePage), typeof(LanguagePage));

            App.Wrapper.LanguageChanged += this.LanguageChangedHandler;

            this.flyLanguage.Title = App.Wrapper.GetText(LanguageFactory.Net.data.MsgCode.language);

        }

        private void LanguageChangedHandler(object sender, LanguageFactory.Net.Messaging.SupportedLanguage e) {
            this.flyLanguage.Title = App.Wrapper.GetText(LanguageFactory.Net.data.MsgCode.language);
        }

        private async void OnMenuItemClicked(object sender, EventArgs e) {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
