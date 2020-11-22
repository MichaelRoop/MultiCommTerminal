using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothPage : ContentPage {

        /// <summary>The view model that handles commands and events</summary>
        BluetoothViewModel viewModel;
        List<BTDeviceInfo> devices = new List<BTDeviceInfo>();
        ClassLog log = new ClassLog("BluetoothPage");

        public BluetoothPage() {
            InitializeComponent();
            BindingContext = this.viewModel = new BluetoothViewModel();
            App.Wrapper.BT_DeviceDiscovered += this.DeviceDiscoveredHandler;
            App.Wrapper.BT_DiscoveryComplete += this.DiscoveryCompleteHandler;
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);

            // TODO - move away from view model. Buttons below
            this.viewModel.OnAppearing();
            //this.DoDiscovery();
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            this.viewModel.OnDisappearing();
            base.OnDisappearing();
        }


        #region Wrapper event handlers

        private void DeviceDiscoveredHandler(object sender, BTDeviceInfo e) {
            Device.BeginInvokeOnMainThread(() => {
                this.lstDevices.ItemsSource = null;
                this.devices.Add(e);
                this.lstDevices.ItemsSource = this.devices;
            });
        }


        private void DiscoveryCompleteHandler(object sender, bool e) {
            Device.BeginInvokeOnMainThread(() => {
                this.IsBusy = false;
                //App.ShowError(this, string.Format("Discovery complete:{0}", e));
            });
        }

        #endregion

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

        private void lstDevices_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            // so the view model can use it
            this.viewModel.SelectedInfo = (e.SelectedItem as BTDeviceInfo);
        }

        #region Private

        private void UpdateLanguage(SupportedLanguage language) {
            //this.Title = // Always Bluetooth
            this.tbTxtPair.Text = language.GetText(MsgCode.Pair);
            this.tbTxtRun.Text = language.GetText(MsgCode.connect);
            this.txtTitle.Text = language.GetText(MsgCode.PairedDevices);
            this.btnDiscover.Text = language.GetText(MsgCode.discover);
        }

        #endregion



    }
}