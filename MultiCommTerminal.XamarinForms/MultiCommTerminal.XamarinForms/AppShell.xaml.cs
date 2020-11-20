using IconFactory.Net.data;
using LanguageFactory.Net.data;
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
            //this.flyLanguage.Icon = App.GetImageSource(UIIcon.Language);

            // DOES NOT WORK
            //this.flyLanguage.Icon = ImageSource.FromResource("icons8_close_window_50_noborder.png");

            //ImageSource.FromResource("icons8_language_50.png");

        }

        private void LanguageChangedHandler(object sender, LanguageFactory.Net.Messaging.SupportedLanguage e) {
            this.flyLanguage.Title = App.Wrapper.GetText(MsgCode.language);
        }

        private async void OnMenuItemClicked(object sender, EventArgs e) {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
