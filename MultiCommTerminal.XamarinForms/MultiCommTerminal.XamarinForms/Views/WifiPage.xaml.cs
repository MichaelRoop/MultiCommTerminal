using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.interfaces;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommTerminal.XamarinForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WifiCommon.Net.DataModels;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiPage : ContentPage {

        #region Data

        private ClassLog log = new ClassLog("WifiPage");
        private List<WifiNetworkInfo> networks = new List<WifiNetworkInfo>();
        private WifiViewModel viewModel;

        #endregion

        public WifiPage() {
            InitializeComponent();
            this.BindingContext = this.viewModel = new WifiViewModel();
            App.Wrapper.DiscoveredWifiNetworks += this.DiscoveredWifiNetworksHandler;
            App.Wrapper.OnWifiError += OnWifiErrorHandler;
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            this.lstWifi.ItemsSource = this.networks;
            this.btnSelect.IsVisible = false;
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            this.lstWifi.SelectedItem = null;
            // loops on Deny and hanges on dont ask again
            //this.viewModel.GetWifiPermissions.Execute(null);
            base.OnAppearing();
        }

        #region Controls events

        private void btnDiscover_Clicked(object sender, EventArgs e) {
            //this.viewModel.GetWifiPermissions.Execute(null);
            //if (this.viewModel.WifiPermissionsGranted) {
                this.btnSelect.IsVisible = false;
                this.ResetWifiList(new List<WifiNetworkInfo>());
                this.IsBusy = true;
                this.viewModel.IsBusy = true;
                this.activity.IsRunning = true;
                App.Wrapper.WifiDiscoverAsync();
            //}
            //else {
            //    this.OnErr("Insufficient permissions to continue");
            //}
        }


        private void btnSelect_Clicked(object sender, EventArgs e) {
            // Get selected and call view model command to move to run page
            WifiNetworkInfo info = this.lstWifi.SelectedItem as WifiNetworkInfo;
            if (info != null) {
                // TODO - remove hack
                info.RemoteHostName = "192.168.4.1";
                info.RemoteServiceName = "80";
                info.Password = "1234567890";

                this.viewModel.GoToRun.Execute(info);
            }
            else {
                this.OnErr(App.Wrapper.GetText(MsgCode.NothingSelected));
            }
        }


        private void btnCredentials_Clicked(object sender, EventArgs e) {
            this.viewModel.GoToCredentials.Execute(null);
        }

        #endregion

        #region Private event handlers

        private void OnWifiErrorHandler(object sender, WifiCommon.Net.DataModels.WifiError e) {
            Device.BeginInvokeOnMainThread(() => {
                this.IsBusy = false;
                this.viewModel.IsBusy = false;
                this.activity.IsRunning = false;
                this.OnErr(e.ExtraInfo.Length > 0 ? e.ExtraInfo : e.Code.ToString());
            });
        }

        private void DiscoveredWifiNetworksHandler(object sender, List<WifiNetworkInfo> e) {
            Device.BeginInvokeOnMainThread(() => {
                this.IsBusy = false;
                this.viewModel.IsBusy = false;
                this.activity.IsRunning = false;
                if (e.Count > 0) {
                    this.btnSelect.IsVisible = true;
                }
                this.ResetWifiList(e);
            });
        }



        private void LanguageUpdate(SupportedLanguage language) {
            this.lbTitle.Text = "WIFI";
        }


        private void ResetWifiList(List<WifiNetworkInfo> infos) {
            this.lstWifi.ItemsSource = null;
            this.lstWifi.SelectedItem = null;
            this.networks.Clear();
            this.networks = infos;
            this.lstWifi.ItemsSource = this.networks;
        }


        #endregion

        #region WifiPermission checks

        protected async Task<bool> ChkWifiPermissions() {
            try {
                var wifiPermissions = DependencyService.Get<ILocationWhileInUsePermission>();
                PermissionStatus status;
                status = await wifiPermissions.CheckStatusAsync();
                if (status != PermissionStatus.Granted) {
                    status = await wifiPermissions.RequestAsync();
                    if (status != PermissionStatus.Granted) {
                        this.OnErr("Network State permission required");
                        return false;
                    }
                }
                return true;
            }
            catch (Exception e) {
                this.log.Exception(55555, "", e);
                this.OnErr(string.Format("Permissions error:{0}", e.Message));
                return false;
            }
        }



        //protected bool ChkWifiPermissions() {
        //    try {
        //        var wifiPermissions = DependencyService.Get<ILocationWhileInUsePermission>();
        //        Task<PermissionStatus> status;
        //        status = wifiPermissions.CheckStatusAsync();
        //        if (status.Result != PermissionStatus.Granted) {
        //            status = wifiPermissions.RequestAsync();
        //            if (status.Result != PermissionStatus.Granted) {
        //                this.OnErr("Network State permission required");
        //                return false;
        //            }
        //        }
        //        return true;



        //        //Task<PermissionStatus> status;
        //        //status = Permissions.CheckStatusAsync<Permissions.NetworkState>();
        //        //if (status.Result != PermissionStatus.Granted) {
        //        //    status = Permissions.RequestAsync<Permissions.NetworkState>();
        //        //    if (status.Result != PermissionStatus.Granted) {
        //        //        this.OnErr("Network State permission required");
        //        //        return false;
        //        //    }
        //        //}

        //        //status = Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        //        //if (status.Result != PermissionStatus.Granted) {
        //        //    status = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //        //    if (status.Result != PermissionStatus.Granted) {
        //        //        this.OnErr("Location permission required");
        //        //        return false;
        //        //    }
        //        //}


        //        //status = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //        //if (status.Result != PermissionStatus.Granted) {
        //        //    status = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //        //    if (status.Result != PermissionStatus.Granted) {
        //        //        this.OnErr("Location permission required");
        //        //        return false;
        //        //    }
        //        //}

        //        // Need access wifi state and change wifi state. Might be ok just declared in manifest


        //        return true;
        //    }
        //    catch (Exception e) {
        //        this.log.Exception(55555, "", e);
        //        this.OnErr(string.Format("Permissions error:{0}", e.Message));
        //        return false;
        //    }
        //}




        //protected async Task<bool> ChkWifiPermissions() {
        //    try {
        //        PermissionStatus status;
        //        status = await Permissions.CheckStatusAsync<Permissions.NetworkState>();
        //        if (status != PermissionStatus.Granted) {
        //            status = await Permissions.RequestAsync<Permissions.NetworkState>();
        //            if (status != PermissionStatus.Granted) {
        //                this.OnErr("Network State permission required");
        //                return false;
        //            }
        //        }

        //        status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
        //        if (status != PermissionStatus.Granted) {
        //            status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //            if (status != PermissionStatus.Granted) {
        //                this.OnErr("Location permission required");
        //                return false;
        //            }
        //        }


        //        //status = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //        //if (status.Result != PermissionStatus.Granted) {
        //        //    status = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
        //        //    if (status.Result != PermissionStatus.Granted) {
        //        //        this.OnErr("Location permission required");
        //        //        return false;
        //        //    }
        //        //}

        //        // Need access wifi state and change wifi state. Might be ok just declared in manifest


        //        return true;
        //    }
        //    catch (Exception e) {
        //        this.log.Exception(55555, "", e);
        //        this.OnErr(string.Format("Permissions error:{0}", e.Message));
        //        return false;
        //    }
        //}

        #endregion


    }
}