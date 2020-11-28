using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.XamarinForms.Views;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms {
    public partial class AppShell : Xamarin.Forms.Shell {
        public AppShell() {

            InitializeComponent();

            // My pages
            Routing.RegisterRoute(nameof(AboutAppPage), typeof(AboutAppPage));
            
            Routing.RegisterRoute(nameof(BluetoothPage), typeof(BluetoothPage));
            Routing.RegisterRoute(nameof(BluetoothPairPage), typeof(BluetoothPairPage));
            Routing.RegisterRoute(nameof(BluetoothRunPage), typeof(BluetoothRunPage));

            Routing.RegisterRoute(nameof(WifiPage), typeof(WifiPage));
            Routing.RegisterRoute(nameof(WifiRunPage), typeof(WifiRunPage));
            Routing.RegisterRoute(nameof(WifiCredentialsPage), typeof(WifiCredentialsPage));

            // Define multiple routes for return of Wifi Credentials
            // This one is for pop up on connection when no creds identified
            Routing.RegisterRoute(
                string.Format("{0}/{1}", nameof(WifiRunPage), nameof(WifiCredentialsModalEditPage)), 
                typeof(WifiCredentialsModalEditPage));
            // This one when opening from Credential list page
            Routing.RegisterRoute(
                string.Format("{0}/{1}", nameof(WifiCredentialsPage), nameof(WifiCredentialsModalEditPage)),
                typeof(WifiCredentialsModalEditPage));

            Routing.RegisterRoute(nameof(LanguagePage), typeof(LanguagePage));

            Routing.RegisterRoute(nameof(TerminatorsPage), typeof(TerminatorsPage));
            Routing.RegisterRoute(nameof(TerminatorSetPage), typeof(TerminatorSetPage));

            Routing.RegisterRoute(nameof(CommandSetsPage), typeof(CommandSetsPage));
            Routing.RegisterRoute(nameof(CommandSetPage), typeof(CommandSetPage));
            Routing.RegisterRoute(nameof(CommandEditPage), typeof(CommandEditPage));



            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            App.Wrapper.LanguageChanged += this.LanguageChangedHandler;
        }


        private void LanguageChangedHandler(object sender, SupportedLanguage language) {
            this.UpdateLanguage(language);
        }


        private void UpdateLanguage(SupportedLanguage language) {
            this.flyAbout.Title = language.GetText(MsgCode.About);
            this.flyLanguage.Title = language.GetText(MsgCode.language);
            this.flyBluetooth.Title = "Bluetooth"; // Seems same in all languages
            this.flyWifi.Title = "WIFI"; // Same in all languages
            this.flyAbout.Title = language.GetText(MsgCode.About);
            this.flyTerminators.Title = language.GetText(MsgCode.Terminators);
            this.flyLanguage.Title = language.GetText(MsgCode.language);
            this.flyCommands.Title = language.GetText(MsgCode.commands);
        }


    }
}
