using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.interfaces;
using MultiCommTerminal.XamarinForms.UIHelpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPairPage : ContentPage {

        #region Data

        private List<BTDeviceInfo> devices = new List<BTDeviceInfo>();
        private ClassLog log = new ClassLog("BluetoothPairPage");
        
        #endregion

        public BluetoothPairPage() {
            InitializeComponent();
        }


        protected override void OnAppearing() {
            this.SubscribeToEvents();
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            this.btnPair.IsVisible = false;
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            this.UnsubscribeFromEvents();
            this.btnPair.IsVisible = false;
            base.OnDisappearing();
        }


        private void btnDiscover_Clicked(object sender, EventArgs args) {
            Device.BeginInvokeOnMainThread(async () => {
                if (await this.ChkBTPermissions()) {
                    this.log.InfoEntry("btnDiscover_Clicked");
                    this.activity.IsRunning = true;
                    this.lstDevices.ItemsSource = null;
                    this.devices.Clear();
                    this.lstDevices.ItemsSource = this.devices;
                    App.Wrapper.BTClassicDiscoverAsync(false);
                }
                else {
                    this.OnErr(MsgCode.InsufficienPermissions);
                }
            });
        }


        private void btnPair_Clicked(object sender, EventArgs e) {
            BTDeviceInfo info = this.lstDevices.SelectedItem as BTDeviceInfo;
            if (info != null) {
                App.Wrapper.BTClassicPairAsync(info);
            }
            else {
                this.OnErr(MsgCode.NothingSelected);
            }
        }


        private void UpdateLanguage(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.PairBluetooth);
        }


        private void BT_DiscoveryCompleteHandler(object sender, bool e) {
            Device.BeginInvokeOnMainThread(() => {
                this.activity.IsRunning = false;
                if (devices.Count == 0) {
                    this.OnErr("", App.GetText(MsgCode.NotFound));
                }
         });
        }


        private void BT_DeviceDiscoveredHandler(object sender, BluetoothCommon.Net.BTDeviceInfo e) {
            Device.BeginInvokeOnMainThread(() => {
                this.btnPair.IsVisible = true;
                this.activity.IsRunning = false;
                this.lstDevices.ItemsSource = null;
                this.devices.Add(e);
                this.lstDevices.ItemsSource = this.devices;
            });
        }

        private void BT_PairStatusHandler(object sender, BTPairOperationStatus e) {
            Device.BeginInvokeOnMainThread(() => {
                if (e.IsSuccessful) {
                    this.lstDevices.ItemsSource = null;
                    var d = this.devices.FirstOrDefault(x => x.Name == e.Name);
                    if (d != null) {
                        this.devices.Remove(d);
                    }
                    this.lstDevices.ItemsSource = this.devices;
                }
                else {
                    this.OnErr(e.PairStatus.ToString());
                }
            });
        }

        private bool permissionsGranted = false;


        private async Task SetAreGranted(bool granted) {
            await Task.Run(() => this.permissionsGranted = granted);
        }

        public async Task<bool> GetIsGranted() {
            return await Task<bool>.Run(() => { return this.permissionsGranted; });
        }


        /// <summary>Checks and asks user for sufficient permissions for WIFI</summary>
        /// <returns>true if ok to continue, otherwise false</returns>
        public async Task<bool> ChkBTPermissions() {
            try {
                await this.SetAreGranted(false);
                IBluetoothPermissions btPermissions =
                    DependencyService.Get<IBluetoothPermissions>();
                PermissionStatus status = await btPermissions.CheckStatusAsync();
                if (status != PermissionStatus.Granted) {
                    status = await btPermissions.RequestAsync();
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


        private void SubscribeToEvents() {
            App.Wrapper.BT_DeviceDiscovered += this.BT_DeviceDiscoveredHandler;
            App.Wrapper.BT_DiscoveryComplete += BT_DiscoveryCompleteHandler;
            App.Wrapper.BT_PairStatus += this.BT_PairStatusHandler;
        }

        private void UnsubscribeFromEvents() {
            App.Wrapper.BT_DeviceDiscovered -= this.BT_DeviceDiscoveredHandler;
            App.Wrapper.BT_DiscoveryComplete -= BT_DiscoveryCompleteHandler;
            App.Wrapper.BT_PairStatus -= this.BT_PairStatusHandler;
        }

    }
}