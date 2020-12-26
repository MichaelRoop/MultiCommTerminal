using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Newtonsoft.Json;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [QueryProperty(nameof(BTDevice), nameof(BTDevice))]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothRunPage : ContentPage {

        #region Data

        private BTDeviceInfo device;
        private ClassLog log = new ClassLog("BluetoothRunPage");
        private List<ScriptItem> cmds = new List<ScriptItem>();
        private List<string> responses = new List<string>();

        #endregion

        #region Properties

        public string BTDevice {
            set {
                // Set on page opening with routing. Can only pass strings as parameters
                this.device = JsonConvert.DeserializeObject<BTDeviceInfo>(Uri.UnescapeDataString(value ?? string.Empty));
            }
        }

        #endregion

        #region Constructor and page overrides

        public BluetoothRunPage() {
            InitializeComponent();
            this.lstCmds.HeightRequest = 1000;
            this.lstResponses.HeightRequest = 1000;
            this.lstCmds.ItemsSource = this.cmds;
            this.lstResponses.ItemsSource = this.responses;
        }


        protected override void OnAppearing() {
            this.SubscribeToEvents();
            this.cmds.Clear();
            this.responses.Clear();
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            App.Wrapper.GetCurrentScript(this.PopulateScriptList, this.OnErr);
            App.Wrapper.GetCurrentTerminator(this.PopulateTerminators, this.OnErr);
            this.SetConnectedLight(false);
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            this.UnsubscribeFromEvents();
            App.Wrapper.BTClassicDisconnect();
            this.SetConnectedLight(false);
            base.OnDisappearing();
        }

        #endregion

        #region Controls event handlers

        private void btnConnect_Clicked(object sender, EventArgs e) {
            if (this.device != null) {
                this.SetConnectedLight(false);
                this.activity.IsRunning = true;
                App.Wrapper.BTClassicConnectAsync(this.device);
            }
        }


        private void btnDisconnect_Clicked(object sender, EventArgs e) {
            if (this.device != null) {
                this.SetConnectedLight(false);
                App.Wrapper.BTClassicDisconnect();
            }
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


        private void lstCmds_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            ScriptItem item = e.SelectedItem as ScriptItem;
            if (item != null) {
                this.entryCmd.Text = item.Command;
            }
        }


        private void btnSend_Clicked(object sender, EventArgs e) {
            App.Wrapper.BTClassicSend(this.entryCmd.Text);
        }

        #endregion

        #region Wrapper event handlers

        private void OnBT_BytesReceivedHandler(object sender, string e) {
            Device.BeginInvokeOnMainThread(() => {
                this.lstResponses.ItemsSource = null;
                this.responses.Add(e);
                if (this.responses.Count > 100) {
                    this.responses.RemoveAt(0);
                }
                this.lstResponses.ItemsSource = this.responses;
            });
        }


        private void OnBT_ConnectionCompletedHandler(object sender, bool ok) {
            Device.BeginInvokeOnMainThread(() => {
                activity.IsRunning = false;
                if (ok) {
                    this.SetConnectedLight(true);
                }
            });
        }

        #endregion

        #region Delegates

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
            for (int i = 0; i < data.TerminatorInfos.Count; i++) {
                if (i > 0) {
                    tmp.Append(",");
                }
                tmp.Append(data.TerminatorInfos[i].Display);
            }
            this.lbTerminatorsText.Text = tmp.ToString();
        }


        private void UpdateLanguage(SupportedLanguage l) {
            this.lbTitle.Text = App.GetText(MsgCode.Run);
            //this.lblCmds.Text = App.GetText(MsgCode.command);
            this.lblResponses.Text = App.GetText(MsgCode.response);
            this.btnSend.Text = App.GetText(MsgCode.send);
            this.btnConnect.Text = App.GetText(MsgCode.connect);
            this.btnDisconnect.Text = l.GetText(MsgCode.Disconnect);
        }


        private void SetConnectedLight(bool isOn) {
            this.onLight.IsVisible = isOn;
            this.offLight.IsVisible = !isOn;
        }


        private void SubscribeToEvents() {
            App.Wrapper.BT_ConnectionCompleted += this.OnBT_ConnectionCompletedHandler;
            App.Wrapper.BT_BytesReceived += this.OnBT_BytesReceivedHandler;
            App.Wrapper.CurrentScriptChanged += CurrentScriptChangedHandler;
            App.Wrapper.CurrentTerminatorChanged += this.CurrentTerminatorChangedHandler;
        }


        private void CurrentTerminatorChangedHandler(object sender, TerminatorDataModel e) {
            this.PopulateTerminators(e);
        }


        private void CurrentScriptChangedHandler(object sender, ScriptDataModel dataModel) {
            this.PopulateScriptList(dataModel);
        }


        private void UnsubscribeFromEvents() {
            App.Wrapper.BT_ConnectionCompleted -= this.OnBT_ConnectionCompletedHandler;
            App.Wrapper.BT_BytesReceived -= this.OnBT_BytesReceivedHandler;
            App.Wrapper.CurrentTerminatorChanged -= this.CurrentTerminatorChangedHandler;
        }


        #endregion

    }
}