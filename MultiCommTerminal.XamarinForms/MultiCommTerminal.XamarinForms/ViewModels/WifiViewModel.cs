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
        //public Command GetWifiPermissions;
        //private bool isGranted = false;


        //private bool WifiPermissionsGranted { get; private set; }

        //private async Task SetIsGranted(bool isGranted) {
        //    await Task.Run(() => {
        //        this.isGranted = isGranted;
        //    });
        //}

        //public async Task<bool> GetIsGranted() {
        //    return await Task<bool>.Run(() => {
        //        return this.isGranted;
        //    });
        //}


        public WifiViewModel() {
            this.GoToRun = new Command<WifiNetworkInfo>(this.OnGoToRun);
            this.GoToCredentials = new Command(this.OnGoToCredentials);
            //this.GetWifiPermissions = new Command(this.ChkWifiPermissions);
        }


        private async void OnGoToRun(WifiNetworkInfo info) {
            await Shell.Current.GoToAsync($"{nameof(WifiRunPage)}?WifiRunPage.WifiInfoAsString={JsonConvert.SerializeObject(info)}");
        }


        private async void OnGoToCredentials() {
            await Shell.Current.GoToAsync(nameof(WifiCredentialsPage));
        }


        //private async void ChkWifiPermissions() {
        //    try {
        //        //this.WifiPermissionsGranted = false;
        //        await this.SetIsGranted(false);
        //        var wifiPermissions = DependencyService.Get<ILocationWhileInUsePermission>();
        //        PermissionStatus status;
        //        status = await wifiPermissions.CheckStatusAsync();
        //        LogUtils.Net.Log.Error(77777, () => string.Format("********************************Current wifi status:{0}", status));
        //        if (status != PermissionStatus.Granted) {
        //            status = await wifiPermissions.RequestAsync();
        //            if (status != PermissionStatus.Granted) {
        //                //this.WifiPermissionsGranted = false;
        //                await this.SetIsGranted(false);
        //                return;
        //            }
        //        }
        //        await this.SetIsGranted(true);
        //    }
        //    catch (Exception) {
        //        await this.SetIsGranted(false);
        //        return;
        //    }
        //}


        //public async Task<bool> ChkWifiPermissions2() {
        //    try {
        //        //this.WifiPermissionsGranted = false;
        //        await this.SetIsGranted(false);
        //        var wifiPermissions = DependencyService.Get<ILocationWhileInUsePermission>();
        //        PermissionStatus status;
        //        status = await wifiPermissions.CheckStatusAsync();
        //        LogUtils.Net.Log.Error(77777, () => string.Format("********************************Current wifi status:{0}", status));
        //        if (status != PermissionStatus.Granted) {
        //            status = await wifiPermissions.RequestAsync();
        //            if (status != PermissionStatus.Granted) {
        //                //this.WifiPermissionsGranted = false;
        //                await this.SetIsGranted(false);
        //                return await this.GetIsGranted();
        //            }
        //        }
        //        await this.SetIsGranted(true);
        //        return await this.GetIsGranted();
        //    }
        //    catch (Exception) {
        //        await this.SetIsGranted(false);
        //        return await this.GetIsGranted();
        //    }
        //}





    }

}
