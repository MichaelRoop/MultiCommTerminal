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
            //Routing.RegisterRoute(nameof(ItemDetailPage), typeof(ItemDetailPage));
            //Routing.RegisterRoute(nameof(NewItemPage), typeof(NewItemPage));

            // My pages
            Routing.RegisterRoute(nameof(AboutAppPage), typeof(AboutAppPage));
            Routing.RegisterRoute(nameof(BluetoothPage), typeof(BluetoothPage));
            Routing.RegisterRoute(nameof(BluetoothPairPage), typeof(BluetoothPairPage));
            Routing.RegisterRoute(nameof(BluetoothRunPage), typeof(BluetoothRunPage));
            Routing.RegisterRoute(nameof(LanguagePage), typeof(LanguagePage));
            Routing.RegisterRoute(nameof(TerminatorsPage), typeof(TerminatorsPage));

            Routing.RegisterRoute(nameof(CommandSetsViewModel), typeof(CommandSetsViewModel));



            App.Wrapper.LanguageChanged += this.LanguageChangedHandler;

            this.flyLanguage.Title = App.Wrapper.GetText(MsgCode.language);
            //this.flyBluetooth.Title = App.Wrapper.GetText(MsgCode.blue)


            // DOES NOT WORK
            //this.flyLanguage.Icon = App.GetImageSource(UIIcon.Language);
            //this.flyLanguage.Icon = ImageSource.FromResource("icons8_close_window_50_noborder.png");
            //ImageSource.FromResource("icons8_language_50.png");
        }

        private void LanguageChangedHandler(object sender, LanguageFactory.Net.Messaging.SupportedLanguage e) {
            this.flyLanguage.Title = App.GetText(MsgCode.language);
            this.flyBluetooth.Title = "Bluetooth"; // Seems same in all languages
            this.flyAbout.Title = App.GetText(MsgCode.About);
            this.flyTerminators.Title = App.GetText(MsgCode.Terminators);
            this.flyLanguage.Title = App.GetText(MsgCode.language);
            this.flyCommands.Title = App.GetText(MsgCode.command);

        }

        private async void OnMenuItemClicked(object sender, EventArgs e) {
            await Shell.Current.GoToAsync("//LoginPage");
        }
    }
}
