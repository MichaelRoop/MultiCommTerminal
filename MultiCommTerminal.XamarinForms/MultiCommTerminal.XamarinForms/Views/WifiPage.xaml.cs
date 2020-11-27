using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.interfaces;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommTerminal.XamarinForms.ViewModels;
using System;
using System.Collections.Generic;
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
        private bool permissionsGranted = false;

        #endregion

        #region Constructor and overrides

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
            base.OnAppearing();
        }

        #endregion

        #region Controls events

        private void btnDiscover_Clicked(object sender, EventArgs e) {
            Device.BeginInvokeOnMainThread(async () => {
                if (await this.ChkWifiPermissions()) {
                    this.btnSelect.IsVisible = false;
                    this.ResetWifiList(new List<WifiNetworkInfo>());
                    this.activity.IsRunning = true;
                    App.Wrapper.WifiDiscoverAsync();
                }
                else {
                    this.OnErr("Insufficient permissions to continue");
                }
            });
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


        private async Task SetAreGranted(bool granted) {
            await Task.Run(() => this.permissionsGranted = granted);
        }

        public async Task<bool> GetIsGranted() {
            return await Task<bool>.Run(() => { return this.permissionsGranted; });
        }


        /// <summary>Checks and asks user for sufficient permissions for WIFI</summary>
        /// <returns>true if ok to continue, otherwise false</returns>
        public async Task<bool> ChkWifiPermissions() {
            try {
                await this.SetAreGranted(false);
                ILocationWhileInUsePermission wifiPermissions = 
                    DependencyService.Get<ILocationWhileInUsePermission>();
                PermissionStatus status = await wifiPermissions.CheckStatusAsync();
                if (status != PermissionStatus.Granted) {
                    status = await wifiPermissions.RequestAsync();
                    if (status != PermissionStatus.Granted) {
                        return await this.GetIsGranted();
                    }
                }
                await this.SetAreGranted(true);
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                await this.SetAreGranted(false);
            }
            return await this.GetIsGranted();
        }

        #endregion

    }
}