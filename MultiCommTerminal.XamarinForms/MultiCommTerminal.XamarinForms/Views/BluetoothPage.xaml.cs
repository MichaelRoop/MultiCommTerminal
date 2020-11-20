using BluetoothCommon.Net;
using LanguageFactory.Net.data;
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


        public BluetoothPage() {
            InitializeComponent();
            BindingContext = this.viewModel = new BluetoothViewModel();

            this.UpdateLanguage();
            App.Wrapper.LanguageChanged += this.OnLanguageChangedHandler;
            App.Wrapper.BT_DeviceDiscovered += OnBT_DeviceDiscoveredHandler;
            App.Wrapper.BT_DiscoveryComplete += OnBT_DiscoveryCompleteHandler;
        }


        protected override void OnAppearing() {
            this.viewModel.OnAppearing();
            this.DoDiscovery();
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            this.viewModel.OnDisappearing();
            base.OnDisappearing();
        }


        private void OnBT_DeviceDiscoveredHandler(object sender, BTDeviceInfo e) {
            //App.ShowError(this, string.Format("Adding device:{0}", e.Name));
            this.lstDevices.ItemsSource = null;
            this.devices.Add(e);
            this.lstDevices.ItemsSource = this.devices;
        }


        private void OnBT_DiscoveryCompleteHandler(object sender, bool e) {
            this.IsBusy = false;
            //App.ShowError(this, string.Format("Discovery complete:{0}", e));
        }


        private void OnLanguageChangedHandler(object sender, LanguageFactory.Net.Messaging.SupportedLanguage e) {
            this.UpdateLanguage();
        }


        private void UpdateLanguage() {
            this.tbTxtPair.Text = App.GetText(MsgCode.Pair);
            this.tbTxtRun.Text = App.GetText(MsgCode.connect);
            this.txtTitle.Text = App.GetText(MsgCode.PairedDevices);
        }


        private void btnRefresh_Clicked(object sender, EventArgs e) {
            this.DoDiscovery();

        }

        private void lstDevices_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            // Not sure what to do here
        }


        private void DoDiscovery() {
            try {
                //App.ShowError(this, string.Format("Do Discovery"));

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


    }
}