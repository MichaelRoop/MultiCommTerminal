using CommunicationStack.Net.Enumerations;
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
using System.Text;
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
            this.lstCmds.HeightRequest = 1000;
            this.lstResponses.HeightRequest = 1000;
            this.lstCmds.ItemsSource = this.cmds;
            this.lstResponses.ItemsSource = this.responses;
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.OnLanguageUpdate);
            App.Wrapper.GetCurrentScript(this.PopulateScriptList, this.OnErr);
            App.Wrapper.GetCurrentTerminator(this.PopulateTerminators, this.OnErr);
            this.SetConnectedLight(false);
            this.SubscribeToEvents();
            base.OnAppearing();
        }

        protected override void OnDisappearing() {
            this.UnsubscribeFromEvents();
            App.Wrapper.WifiDisconect();
            this.SetConnectedLight(false);
            base.OnDisappearing();
        }

        #endregion

        #region Event handlers

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
            this.SetConnectedLight(false);
            Task.Run(() => {
                WifiCredAndIndex cred = App.Wrapper.ValidateCredentials(this.networkInfo, this.OnErr);
                if (cred.RequiresUserData) {
                    Device.BeginInvokeOnMainThread(() =>
                    PopupNavigation.Instance.PushAsync(
                        new WifiCredRequestPopUpPage(cred, this.networkInfo, this.DelegateRunConnection)));
                }
                else {
                    this.DelegateRunConnection(this.networkInfo);
                }
            });


            /*
            Device.BeginInvokeOnMainThread(async () => {
                this.SetConnectedLight(false);
                WifiCredAndIndex cred = App.Wrapper.ValidateCredentials(this.networkInfo, this.OnErr);
                if (cred.RequiresUserData) {
                    await PopupNavigation.Instance.PushAsync(
                        new WifiCredRequestPopUpPage(cred, this.networkInfo, this.DelegateRunConnection));
                }
                else {
                    this.DelegateRunConnection(this.networkInfo);
                }
            });
            */


        }


        private void btnDisconnect_Clicked(object sender, EventArgs e) {
            this.SetConnectedLight(false);
            App.Wrapper.WifiDisconect();
        }


        private void ClearTapGestureRecognizer_Tapped(object sender, EventArgs e) {
            this.lstResponses.ItemsSource = null;
            this.responses.Clear();
            this.lstResponses.ItemsSource = this.responses;
        }


        private void CommandsTapGestureRecognizer_Tapped(object sender, EventArgs e) {
            Device.BeginInvokeOnMainThread(async () => {
                await PopupNavigation.Instance.PushAsync(new CommandSetSelectPopupPage());
            });
        }


        private void TerminatorsTapGestureRecognizer_Tapped(object sender, EventArgs e) {
            Device.BeginInvokeOnMainThread(async () => {
                // Temp.  add terminators popup
                await PopupNavigation.Instance.PushAsync(new TerminatorsSetSelectPopupPage());
            });
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

        #endregion

        #region Delegates

        private void DelegateRunConnection(WifiNetworkInfo info) {
            Device.BeginInvokeOnMainThread(() => { this.activity.IsRunning = true; });
            Task.Run(() => {
                App.Wrapper.WifiConnectPreValidatedAsync(this.networkInfo);
            });
        }


        private void OnLanguageUpdate(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.Run);
            this.btnConnect.Text = l.GetText(MsgCode.connect);
            this.btnDisconnect.Text = l.GetText(MsgCode.Disconnect);
            this.btnSend.Text = l.GetText(MsgCode.send);
            //this.lblCmds.Text = l.GetText(MsgCode.commands);
            this.lblResponses.Text = l.GetText(MsgCode.response);
        }


        private void PopulateScriptList(ScriptDataModel scripts) {
            this.lstCmds.ItemsSource = null;
            this.cmds.Clear();
            foreach (var script in scripts.Items) {
                this.cmds.Add(script);
            }
            this.lstCmds.ItemsSource = this.cmds;
            this.lblCmds.Text = scripts.Display;
        }


        private void PopulateTerminators(TerminatorDataModel data) {
            this.lbTerminatorsName.Text = data.Name;
            this.lbTerminatorsText.Text = "";
            StringBuilder tmp = new StringBuilder();
            for(int i = 0; i < data.TerminatorInfos.Count; i++) {
                if (i > 0) {
                    tmp.Append(",");
                }
                tmp.Append(data.TerminatorInfos[i].Display);
            }
            this.lbTerminatorsText.Text = tmp.ToString();
        }


        private void SetConnectedLight(bool isOn) {
            this.onLight.IsVisible = isOn;
            this.offLight.IsVisible = !isOn;
        }


        private void SubscribeToEvents() {
            App.Wrapper.Wifi_BytesReceived += this.Wifi_BytesReceivedHandler;
            App.Wrapper.OnWifiConnectionAttemptCompleted += this.OnWifiConnectionAttemptCompletedHandler;
            App.Wrapper.OnWifiError += this.OnWifiErrorHandler;
            App.Wrapper.CurrentScriptChanged += this.CurrentScriptChangedHandler;
            App.Wrapper.CurrentTerminatorChanged += this.CurrentTerminatorChangedHandler;
        }


        private void CurrentTerminatorChangedHandler(object sender, TerminatorDataModel e) {
            this.PopulateTerminators(e);
        }


        private void CurrentScriptChangedHandler(object sender, ScriptDataModel dataModel) {
            this.PopulateScriptList(dataModel);
        }


        private void UnsubscribeFromEvents() {
            App.Wrapper.Wifi_BytesReceived -= this.Wifi_BytesReceivedHandler;
            App.Wrapper.OnWifiConnectionAttemptCompleted -= this.OnWifiConnectionAttemptCompletedHandler;
            App.Wrapper.OnWifiError -= this.OnWifiErrorHandler;
            App.Wrapper.CurrentScriptChanged -= this.CurrentScriptChangedHandler;
            App.Wrapper.CurrentTerminatorChanged -= this.CurrentTerminatorChangedHandler;
        }

        #endregion

    }
}