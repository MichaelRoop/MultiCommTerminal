using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class WifiCredentialsModalEditPage : ContentPage {

        #region Data

        private ClassLog log = new ClassLog("WifiCredentialsModalEditPage");
        private WifiCredentialsDataModel dataModel;
        private IIndexItem<DefaultFileExtraInfo> index = null;
        private NavigateBackInterceptor interceptor;
        private Command PopBack;


        #endregion

        #region Properties

        public string IndexAsString {
            set {
                if (!string.IsNullOrWhiteSpace(value)) {
                    try {
                        // Must not be interface for deserialize to work
                        this.index = JsonConvert.DeserializeObject<IndexItem<DefaultFileExtraInfo>>(
                            Uri.UnescapeDataString(value));
                    }
                    catch (Exception e) {
                        Log.Exception(9999, "IndexAsString", "", e);
                        // TODO - error message and pop back?
                        this.OnErr(e.Message);
                    }
                }
                else {
                    this.index = null;
                }
            }
        }

        #endregion

        #region Constructor and overrides

        public WifiCredentialsModalEditPage() {
            InitializeComponent();
            this.PopBack = new Command(this.PopBackRoute);
            this.interceptor = new NavigateBackInterceptor(this);
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            this.interceptor.Changed = false;
            if (this.index != null) {
                // Need to load from memory. Only way to know which item of list to modify
                this.dataModel = App.Wrapper.GetScratch().WifiCred.WifiCredentials;
            }
            else {
                //create a new Data model for create
                this.dataModel = new WifiCredentialsDataModel() {
                    // Differentiate between networks with same SSID
                    // TODO - check if only for UWP
                    Id = Guid.NewGuid(),
                };
            }

            this.edSsid.Text = this.dataModel.SSID;
            this.edHost.Text = this.dataModel.RemoteHostName;
            this.edPort.Text = this.dataModel.RemoteServiceName;
            this.edPwd.Text = this.dataModel.WifiPassword;
            base.OnAppearing();
        }

        /// <summary>Disable back button</summary>
        /// <returns>false always to prevent movement</returns>
        protected override bool OnBackButtonPressed() {
            return true;
        }


        #endregion

        private void btnSave_Clicked(object sender, EventArgs e) {
            this.PopBack.Execute(null);

        }

        private void btnDelete_Clicked(object sender, EventArgs e) {
            this.PopBack.Execute(null);

        }

        private void btnCancel_Clicked(object sender, EventArgs e) {
            this.PopBack.Execute(null);
        }


        private void LanguageUpdate(SupportedLanguage l) {
            // TODO - any title

            this.lbSsid.Text = l.GetText(MsgCode.Name);
            this.lbPwd.Text = l.GetText(MsgCode.NetworkSecurityKey);
            this.lbHost.Text = l.GetText(MsgCode.HostName);
            this.lbPort.Text = l.GetText(MsgCode.Port);
        }


        private async void PopBackRoute() {
            await Shell.Current.GoToAsync("..");
        }


    }
}