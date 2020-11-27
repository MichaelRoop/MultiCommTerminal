using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommTerminal.XamarinForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WifiCommon.Net.DataModels;
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
            base.OnAppearing();
        }

        #region Controls events

        private void btnDiscover_Clicked(object sender, EventArgs e) {
            this.btnSelect.IsVisible = false;
            this.ResetWifiList(new List<WifiNetworkInfo>());
            this.IsBusy = true;
            this.viewModel.IsBusy = true;
            this.activity.IsRunning = true;
            App.Wrapper.WifiDiscoverAsync();
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

    }
}