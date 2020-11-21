using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [QueryProperty(nameof(BTDevice), nameof(BTDevice))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothRunPage : ContentPage {

        private BTDeviceInfo device;
        private ClassLog log = new ClassLog("BluetoothRunPage");

        private int okCount = 0;
        private int failCount = 0;
        private int connectTry = 0;

        public string BTDevice {
            get {
                return "";//

                

                //this.device;
            }
            set {
                // Set on page opening
                //this.device = value;
                this.device = JsonConvert.DeserializeObject< BTDeviceInfo>(Uri.UnescapeDataString(value ?? string.Empty));
                //this.DoConnection(this.device);
            }
        }

        #region Constructor and page overrides

        public BluetoothRunPage() {
            InitializeComponent();
            this.UpdateLanguage();

            App.Wrapper.LanguageChanged += OnLanguageChanged;
            App.Wrapper.BT_ConnectionCompleted += OnBT_ConnectionCompletedHandler;


        }

        protected override void OnAppearing() {
            // TODO move this to a view model to do a busy and finish?
            this.UpdateLanguage();
            this.okCount = 0;
            this.failCount = 0;
            this.connectTry = 0;
            this.DoConnection(this.device);
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            App.Wrapper.BTClassicDisconnect();
            base.OnDisappearing();
        }

        #endregion


        #region Wrapper event handlers

        private void OnBT_ConnectionCompletedHandler(object sender, bool ok) {
            this.IsBusy = false;
            if (ok) {
                this.okCount++;
                this.txtOk.Text = string.Format("Ok:{0}", okCount);

                // TODO - what to do
                //App.ShowError(this, "YAY CONNECTED");
            }
            else {
                //App.ShowError(this, App.GetText(MsgCode.ConnectionFailure));
                this.failCount++;
                this.txtFail.Text = string.Format("Failed:{0}", failCount);
            }
        }


        private void OnLanguageChanged(object sender, SupportedLanguage e) {
            this.UpdateLanguage();
        }

        #endregion


        /// <summary>Do the connection</summary>
        /// <param name="info">the device info for connection</param>
        private void DoConnection(BTDeviceInfo info) {
            if (info != null) {
                this.connectTry++;
                this.txtConnectTry.Text = this.connectTry.ToString();
                this.IsBusy = true;
                App.Wrapper.BTClassicConnectAsync(info);
            }
            else {
                //
            }
        }


        private void UpdateLanguage() {
            this.Title = App.GetText(LanguageFactory.Net.data.MsgCode.connect);
        }

        private void Refresh_Clicked(object sender, EventArgs e) {
            this.DoConnection(this.device);
        }
    }
}