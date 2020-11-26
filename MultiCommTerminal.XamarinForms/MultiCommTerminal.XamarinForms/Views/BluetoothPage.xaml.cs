using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.ViewModels;
using System;
using System.Collections.Generic;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage {

        #region Data

        /// <summary>The view model that handles page navigation</summary>
        private BluetoothViewModel viewModel;
        private List<BTDeviceInfo> devices = new List<BTDeviceInfo>();
        private ClassLog log = new ClassLog("BluetoothPage");

        #endregion

        #region Constructors and page overrides

        public BluetoothPage() {
            InitializeComponent();
            BindingContext = this.viewModel = new BluetoothViewModel();
            App.Wrapper.BT_DeviceDiscovered += this.DeviceDiscoveredHandler;
            App.Wrapper.BT_DiscoveryComplete += this.DiscoveryCompleteHandler;
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            base.OnAppearing();
        }

        #endregion

        #region Wrapper event handlers

        private void DeviceDiscoveredHandler(object sender, BTDeviceInfo e) {
            Device.BeginInvokeOnMainThread(() => {
                this.lstDevices.ItemsSource = null;
                this.devices.Add(e);
                this.lstDevices.ItemsSource = this.devices;
            });
        }


        private void DiscoveryCompleteHandler(object sender, bool e) {
            Device.BeginInvokeOnMainThread(() => { this.IsBusy = false; });
        }

        #endregion

        #region Button events

        private void btnDiscover_Clicked(object sender, EventArgs args) {
            try {
                this.log.InfoEntry("btnDiscover_Clicked");
                this.IsBusy = true;
                this.lstDevices.ItemsSource = null;
                this.devices.Clear();
                this.lstDevices.ItemsSource = this.devices;
                App.Wrapper.BTClassicDiscoverAsync(true);
            }
            catch (Exception e) {
                App.ShowError(this, e.Message);
            }
        }


        private void btnPair_Clicked(object sender, EventArgs e) {
            this.viewModel.PairCommand.Execute(null);
        }


        private void btnRun_Clicked(object sender, EventArgs e) {
            BTDeviceInfo info = (this.lstDevices.SelectedItem as BTDeviceInfo);
            if (info != null) {
                this.viewModel.RunCommand.Execute(this.lstDevices.SelectedItem as BTDeviceInfo);
            }
            else {
                App.ShowError(this, App.GetText(MsgCode.NothingSelected));
            }
        }

        #endregion

        #region Private

        private void UpdateLanguage(SupportedLanguage language) {
            //this.txtTitle.Text = language.GetText(MsgCode.PairedDevices);
            this.lbTitle.Text = "Bluetooth";

            //// buttons below. need to determine if we use icons or text
            //this.btnDiscover.Text = language.GetText(MsgCode.discover);
            //this.btnPair.Text = language.GetText(MsgCode.Pair);
            //this.btnRun.Text = language.GetText(MsgCode.connect);
        }

        #endregion

    }
}