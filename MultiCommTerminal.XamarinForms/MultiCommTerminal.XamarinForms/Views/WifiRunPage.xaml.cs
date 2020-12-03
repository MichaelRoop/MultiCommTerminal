using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommTerminal.XamarinForms.ViewModels;
using MultiCommWrapper.Net.Helpers;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using WifiCommon.Net.DataModels;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [QueryProperty(nameof(WifiInfoAsString), "WifiRunPage.WifiInfoAsString")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class WifiRunPage : ContentPage {

        #region Data

        private List<ScriptItem> cmds = new List<ScriptItem>();
        private List<string> responses = new List<string>();
        private WifiRunViewModel viewModel;
        WifiNetworkInfo networkInfo;
        ClassLog log = new ClassLog("WifiRunPage");

        #endregion

        #region Properties

        public string WifiInfoAsString {
            set {
                string s = value;
                this.networkInfo = JsonConvert.DeserializeObject<WifiNetworkInfo>(Uri.UnescapeDataString(value ?? string.Empty));
                if (this.networkInfo == null) {
                    this.OnErr("FAILED THE PROPERTY INITIALISED");

                }
                else {
                    //this.OnErr("OK GOT THE PROPERTY INITIALISED");
                }
            }
        }

        #endregion

        #region Constructors and overrides

        public WifiRunPage() {
            InitializeComponent();
            this.BindingContext = this.viewModel = new WifiRunViewModel();
            this.lstCmds.ItemsSource = this.cmds;
            this.lstResponses.ItemsSource = this.responses;
        }


        private void OnWifiErrorHandler(object sender, WifiError e) {
            this.viewModel.IsBusy = false;
            this.activity.IsRunning = false;
            this.OnErr(e.ExtraInfo.Length > 0 ? e.ExtraInfo : e.Code.ToString());
        }


        private void OnWifiConnectionAttemptCompletedHandler(object sender, CommunicationStack.Net.DataModels.MsgPumpResults e) {
            Device.BeginInvokeOnMainThread(() => {
                this.viewModel.IsBusy = false;
                this.activity.IsRunning = false;
                if (e.Code == MsgPumpResultCode.Connected) {
                    this.SetConnectedLight(true);
                }
                else {
                    this.OnErr(string.Format("Connect Attempt Result:{0} - {1}", e.Code, e.ErrorString));
                }
            });
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.OnLanguageUpdate);
            App.Wrapper.GetCurrentScript(this.PopulateScriptList, this.OnErr);
            this.SetConnectedLight(false);
            this.SubscribeToEvents();
            base.OnAppearing();
        }

        protected override void OnDisappearing() {
            this.UnsubscribeToEvents();
            App.Wrapper.WifiDisconect();
            this.SetConnectedLight(false);
            base.OnDisappearing();
        }


        private void Wifi_BytesReceivedHandler(object sender, string e) {
            Device.BeginInvokeOnMainThread(() => {
                this.lstResponses.ItemsSource = null;
                this.responses.Add(e);
                if (this.responses.Count > 100) {
                    this.responses.RemoveAt(0);
                }
                this.lstResponses.ItemsSource = this.responses;
            });
        }

        #endregion


        private void lstCmds_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            ScriptItem cmd = e.SelectedItem as ScriptItem;
            if (cmd != null) {
                this.entryCmd.Text = cmd.Command;
            }
        }

        private void btnSend_Clicked(object sender, EventArgs e) {
            App.Wrapper.WifiSend(this.entryCmd.Text);
        }

        private void btnConnect_Clicked(object sender, EventArgs e) {
            Device.BeginInvokeOnMainThread(async () => {
                this.SetConnectedLight(false);
                this.log.Info("********************************", "WifiRunPage - Connect - before validate");
                WifiCredAndIndex cred = App.Wrapper.ValidateCredentials(this.networkInfo, this.OnErr);
                this.log.Info("********************************", "WifiRunPage - Connect - after validate");
                if (cred.RequiresUserData) {
                    this.log.Info("********************************", "WifiRunPage - Connect - before popup");
                    await PopupNavigation.Instance.PushAsync(
                        new WifiCredRequestPopUpPage(cred, this.networkInfo, this.DelegateRunConnection));
                    this.log.Info("********************************", "WifiRunPage - Connect - after popup");
                    //App.Wrapper.WifiConnectAsync(this.networkInfo);
                }
                else {
                    this.log.Info("********************************", "WifiRunPage - Connect - no popup");
                    this.DelegateRunConnection(this.networkInfo);
                }
            });
        }


        private void DelegateRunConnection(WifiNetworkInfo info) {
            Task.Run(() => {
                this.log.Info("********************************", "DelegateRunConnection - Connect - no popup");
                Device.BeginInvokeOnMainThread(() => { this.activity.IsRunning = true; });
                App.Wrapper.WifiConnectAsync(this.networkInfo);
            });
        }



        private void btnDisconnect_Clicked(object sender, EventArgs e) {
            this.SetConnectedLight(false);
            App.Wrapper.WifiDisconect();
        }


        #region Delegates

        private void OnLanguageUpdate(SupportedLanguage language) {
            this.lbTitle.Text = App.GetText(MsgCode.Run);
            this.btnConnect.Text = language.GetText(MsgCode.connect);
            this.btnDisconnect.Text = language.GetText(MsgCode.Disconnect);
            this.btnSend.Text = language.GetText(MsgCode.send);
            this.lblCmds.Text = language.GetText(MsgCode.commands);
            this.lblResponses.Text = language.GetText(MsgCode.response);
        }


        private void PopulateScriptList(ScriptDataModel scripts) {
            this.lstCmds.ItemsSource = null;
            this.cmds.Clear();
            foreach (var script in scripts.Items) {
                this.cmds.Add(script);
            }
            this.lstCmds.ItemsSource = this.cmds;
        }


        private void SetConnectedLight(bool isOn) {
            this.onLight.IsVisible = isOn;
            this.offLight.IsVisible = !isOn;
        }


        private void CredentialsRequestedEventHandler(object sender, WifiCredentials cred) {

            this.log.Info("********************************", "WifiRunPage - Credentials request");

            // TODO - this does not work - may have to set it here before call down to wrapper

            //cred.CompletedEvent.Reset();
            //this.viewModel.PopUpCred.Execute(cred);

            //if (!cred.CompletedEvent.WaitOne(60000)) {
            //    App.ShowError(this, MsgCode.Timeout);
            //}


            // THis just holds up the main UI thread so that the popup does not display until after timeout
            // Hack
            //AutoResetEvent done = new AutoResetEvent(false);
            //Device.BeginInvokeOnMainThread(async () => {
            //    await PopupNavigation.Instance.PushAsync(new WifiCredRequestPopUpPage(cred));
            //    done.Set();
            //});
            //if (!done.WaitOne(60000)) {
            //    this.OnErr(MsgCode.Timeout);
            //}




        }


        private void SubscribeToEvents() {
            App.Wrapper.Wifi_BytesReceived += this.Wifi_BytesReceivedHandler;
            App.Wrapper.OnWifiConnectionAttemptCompleted += this.OnWifiConnectionAttemptCompletedHandler;
            App.Wrapper.OnWifiError += this.OnWifiErrorHandler;
            App.Wrapper.CredentialsRequestedEvent += this.CredentialsRequestedEventHandler;
        }


        private void UnsubscribeToEvents() {
            App.Wrapper.Wifi_BytesReceived -= this.Wifi_BytesReceivedHandler;
            App.Wrapper.OnWifiConnectionAttemptCompleted -= this.OnWifiConnectionAttemptCompletedHandler;
            App.Wrapper.OnWifiError -= this.OnWifiErrorHandler;
            App.Wrapper.CredentialsRequestedEvent -= this.CredentialsRequestedEventHandler;
        }

        #endregion

    }
}