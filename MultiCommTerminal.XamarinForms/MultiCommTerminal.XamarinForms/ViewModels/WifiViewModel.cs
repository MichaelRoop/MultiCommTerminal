using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using WifiCommon.Net.DataModels;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    public class WifiViewModel {

        public Command<WifiNetworkInfo> GoToRun;


        public WifiViewModel() {
            this.GoToRun = new Command<WifiNetworkInfo>(this.OnGoToRun);
        }


        private async void OnGoToRun(WifiNetworkInfo info) {
            await Shell.Current.GoToAsync($"{nameof(WifiRunPage)}?WifiRunPage.WifiInfoAsString={JsonConvert.SerializeObject(info)}");
        }


    }

}
