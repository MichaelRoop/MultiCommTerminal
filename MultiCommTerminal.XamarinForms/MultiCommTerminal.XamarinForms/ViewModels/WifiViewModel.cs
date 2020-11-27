using MultiCommTerminal.XamarinForms.interfaces;
using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using System;
using System.Threading.Tasks;
using WifiCommon.Net.DataModels;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    public class WifiViewModel : BaseViewModel {

        public Command<WifiNetworkInfo> GoToRun;
        public Command GoToCredentials;
        public Command GetWifiPermissions;

        public bool WifiPermissionsGranted { get; private set; }

        public WifiViewModel() {
            this.GoToRun = new Command<WifiNetworkInfo>(this.OnGoToRun);
            this.GoToCredentials = new Command(this.OnGoToCredentials);
            this.GetWifiPermissions = new Command(this.ChkWifiPermissions);
        }


        private async void OnGoToRun(WifiNetworkInfo info) {
            await Shell.Current.GoToAsync($"{nameof(WifiRunPage)}?WifiRunPage.WifiInfoAsString={JsonConvert.SerializeObject(info)}");
        }


        private async void OnGoToCredentials() {
            await Shell.Current.GoToAsync(nameof(WifiCredentialsPage));
        }



        private async void ChkWifiPermissions() {
            try {
                this.WifiPermissionsGranted = false;
                var wifiPermissions = DependencyService.Get<ILocationWhileInUsePermission>();
                PermissionStatus status;
                status = await wifiPermissions.CheckStatusAsync();
                if (status != PermissionStatus.Granted) {
                    status = await wifiPermissions.RequestAsync();
                    if (status != PermissionStatus.Granted) {
                        this.WifiPermissionsGranted = false;
                        return;
                    }
                }
                this.WifiPermissionsGranted = true;
            }
            catch (Exception) {
                this.WifiPermissionsGranted = false;
                return;
            }
        }




    }

}
