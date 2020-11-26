using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using WifiCommon.Net.DataModels;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    public class WifiViewModel : BaseViewModel {

        public Command<WifiNetworkInfo> GoToRun;
        public Command GoToCredentials;

        public WifiViewModel() {
            this.GoToRun = new Command<WifiNetworkInfo>(this.OnGoToRun);
            this.GoToCredentials = new Command(this.OnGoToCredentials);
        }


        private async void OnGoToRun(WifiNetworkInfo info) {
            await Shell.Current.GoToAsync($"{nameof(WifiRunPage)}?WifiRunPage.WifiInfoAsString={JsonConvert.SerializeObject(info)}");
        }


        private async void OnGoToCredentials() {
            await Shell.Current.GoToAsync(nameof(WifiCredentialsPage));
        }

    }

}
