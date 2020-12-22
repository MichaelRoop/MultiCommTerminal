using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.UIHelpers;
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
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            this.SubscribeToEvents();
            this.lstDevices.ItemSelected -= LstDevices_ItemSelectedHandler;
            // void the list, quick to reload
            this.lstDevices.SelectedItem = null;
            this.lstDevices.ItemsSource = null;
            this.devices.Clear();
            this.lstDevices.ItemsSource = this.devices;
            this.btnUnPair.IsVisible = false;
            this.btnSelect.IsVisible = false;
            this.lstDevices.ItemSelected += LstDevices_ItemSelectedHandler;
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            this.UnsubscribeFromEvents();
            base.OnDisappearing();
        }

        #endregion

        #region Wrapper event handlers

        private void LstDevices_ItemSelectedHandler(object sender, SelectedItemChangedEventArgs e) {
            this.btnSelect.IsVisible = true;
            this.btnUnPair.IsVisible = true;
        }


        private void DeviceDiscoveredHandler(object sender, BTDeviceInfo e) {
            Device.BeginInvokeOnMainThread(() => {
                this.lstDevices.ItemSelected -= LstDevices_ItemSelectedHandler;
                this.activity.IsRunning = false;
                this.lstDevices.ItemsSource = null;
                this.devices.Add(e);
                this.lstDevices.ItemsSource = this.devices;
                this.lstDevices.ItemSelected += LstDevices_ItemSelectedHandler;
            });
        }


        private void DiscoveryCompleteHandler(object sender, bool e) {
            Device.BeginInvokeOnMainThread(() => {
                this.activity.IsRunning = false;
            });
        }


        private void BT_UnPairStatusHandler(object sender, BTUnPairOperationStatus e) {
            Device.BeginInvokeOnMainThread(() => {
                if (e.IsSuccessful) {
                    this.btnDiscover_Clicked(null, null);
                }
                else {
                    this.OnErr(e.UnpairStatus.ToString());
                }
            });
        }

        #endregion

        #region Button events

        private void btnDiscover_Clicked(object sender, EventArgs args) {
            try {
                this.log.InfoEntry("btnDiscover_Clicked");
                this.lstDevices.ItemSelected -= this.LstDevices_ItemSelectedHandler;
                this.activity.IsRunning = true;
                this.lstDevices.ItemsSource = null;
                this.devices.Clear();
                this.lstDevices.ItemsSource = this.devices;
                this.lstDevices.ItemSelected += this.LstDevices_ItemSelectedHandler;
                App.Wrapper.BTClassicDiscoverAsync(true);
            }
            catch (Exception e) {
                App.ShowError(this, e.Message);
            }
        }


        private void btnPair_Clicked(object sender, EventArgs e) {
            this.viewModel.PairCommand.Execute(null);
        }


        private void btnUnPair_Clicked(object sender, EventArgs e) {
            BTDeviceInfo device = this.lstDevices.SelectedItem as BTDeviceInfo;
            if (device != null) {
                App.ShowYesNo(this, device.Name, MsgCode.Unpair, () => {
                    App.Wrapper.BTClassicUnPairAsync(device);
                });
            }
            else {
                this.OnErr(MsgCode.NothingSelected);
            }
        }


        private void btnSelect_Clicked(object sender, EventArgs e) {
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
            this.lbTitle.Text = language.GetText(MsgCode.PairedDevices);
            this.btnDiscover.SetScreenReader(MsgCode.discover);
            this.btnPair.SetScreenReader(MsgCode.Pair);
            this.btnUnPair.SetScreenReader(MsgCode.Unpair);
            this.btnSelect.SetScreenReader(MsgCode.select);
        }


        private void SubscribeToEvents() {
            App.Wrapper.BT_DeviceDiscovered += this.DeviceDiscoveredHandler;
            App.Wrapper.BT_DiscoveryComplete += this.DiscoveryCompleteHandler;
            App.Wrapper.BT_UnPairStatus += this.BT_UnPairStatusHandler;
            this.lstDevices.ItemSelected += LstDevices_ItemSelectedHandler;
        }


        private void UnsubscribeFromEvents() {
            App.Wrapper.BT_DeviceDiscovered -= this.DeviceDiscoveredHandler;
            App.Wrapper.BT_DiscoveryComplete -= this.DiscoveryCompleteHandler;
            App.Wrapper.BT_UnPairStatus -= this.BT_UnPairStatusHandler;
            this.lstDevices.ItemSelected -= LstDevices_ItemSelectedHandler;
        }


        #endregion

    }
}