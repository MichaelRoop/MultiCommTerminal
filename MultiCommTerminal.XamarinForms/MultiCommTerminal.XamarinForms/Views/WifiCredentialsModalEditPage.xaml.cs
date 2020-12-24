using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [QueryProperty(nameof(IndexAsString), "WifiCredentialsModalEditPage.IndexAsString")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiCredentialsModalEditPage : ContentPage {

        #region Data

        private ClassLog log = new ClassLog("WifiCredentialsModalEditPage");
        private WifiCredentialsDataModel dataModel;
        private IIndexItem<DefaultFileExtraInfo> index = null;
        private NavigateBackInterceptor interceptor;

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
            this.interceptor = new NavigateBackInterceptor(this);
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            this.interceptor.Changed = false;
            if (this.index != null) {
                // Can load this one from storage since not yet loaded
                App.Wrapper.RetrieveWifiCredData(
                    this.index, this.OnLoadSuccess, this.ErrAndCLose);

                if (this.dataModel == null) {
                    this.OnErr(string.Format("Data model for '{0}' is NULL", this.index.Display));
                }
            }
            else {
                //create a new Data model for create
                this.dataModel = new WifiCredentialsDataModel();
            }

            if (this.dataModel != null) {
                this.edSsid.Text = this.dataModel.SSID;
                this.edHost.Text = this.dataModel.RemoteHostName;
                this.edPort.Text = this.dataModel.RemoteServiceName;
                this.edPwd.Text = this.dataModel.WifiPassword;
            }
            base.OnAppearing();
        }


        /// <summary>Disable back button</summary>
        /// <returns>false always to prevent movement</returns>
        protected override bool OnBackButtonPressed() {
            return this.interceptor.HardwareOnBackButtonQuestion(base.OnBackButtonPressed);
        }

        #endregion

        #region Button handlers

        private void btnSave_Clicked(object sender, EventArgs e) {
            if (this.dataModel != null) {
                // Think validation done in wrapper
                this.dataModel.SSID = this.edSsid.Text;
                this.dataModel.WifiPassword = this.edPwd.Text;
                this.dataModel.RemoteHostName = this.edHost.Text;
                this.dataModel.RemoteServiceName = this.edPort.Text;

                if (this.index == null) {
                    App.Wrapper.CreateNewWifiCred(
                        this.dataModel.SSID, this.dataModel, this.OnCredSaveOk, this.OnErr); ;
                }
                else {
                    this.index.Display = this.dataModel.SSID;
                    App.Wrapper.SaveWifiCred(
                        this.index, this.dataModel, this.OnCredSaveOk, this.OnErr);
                }
            }
            else {
                this.log.Error(1111, () => string.Format("Data model is NULL"));
                this.ErrAndCLose("NULL DATA MODEL");
            }
        }


        private void btnCancel_Clicked(object sender, EventArgs e) {
            this.interceptor.MethodExitQuestion();
        }


        private void ed_TextChanged(object sender, TextChangedEventArgs e) {
            this.interceptor.Changed = true;
        }

        #endregion

        #region Private

        private void LanguageUpdate(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.Edit);
            this.lbSsid.Text = l.GetText(MsgCode.Name);
            this.lbPwd.Text = l.GetText(MsgCode.NetworkSecurityKey);
            this.lbHost.Text = l.GetText(MsgCode.HostName);
            this.lbPort.Text = l.GetText(MsgCode.Port);

            this.btnCancel.SetScreenReader(l.GetText(MsgCode.cancel));
            this.btnSave.SetScreenReader(l.GetText(MsgCode.save));
        }



        private void OnLoadSuccess(WifiCredentialsDataModel dataModel) {
            this.dataModel = dataModel;
        }


        private void ErrAndCLose(string err) {
            this.OnErr(err);
            this.interceptor.Changed = false;
            this.interceptor.MethodExitQuestion();
        }


        private void OnCredSaveOk() {
            this.interceptor.Changed = false;
            this.interceptor.MethodExitQuestion();
        }

        #endregion

    }
}