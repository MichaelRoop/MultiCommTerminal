using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

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
            App.Wrapper.BT_ConnectionCompleted += this.OnBT_ConnectionCompletedHandler;
            App.Wrapper.BT_BytesReceived += this.OnBT_BytesReceivedHandler;
            this.lstCmds.ItemsSource = this.cmds;
            this.lstResponses.ItemsSource = this.responses;
        }


        protected override void OnAppearing() {
            this.cmds.Clear();
            this.responses.Clear();
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            App.Wrapper.GetCurrentScript(this.PopulateScriptList, this.OnErr);
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            App.Wrapper.BTClassicDisconnect();
            base.OnDisappearing();
        }

        #endregion

        #region Controls event handlers

        private void btnConnect_Clicked(object sender, EventArgs e) {
            if (this.device != null) {
                this.activity.IsRunning = true;
                App.Wrapper.BTClassicConnectAsync(this.device);
            }
        }


        private void btnDisconnect_Clicked(object sender, EventArgs e) {
            if (this.device != null) {
                App.Wrapper.BTClassicDisconnect();
            }
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
                if (!ok) {
                    //this.txtFail.Text = string.Format("Failed:{0}", failCount);
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
        }


        private void UpdateLanguage(SupportedLanguage language) {
            this.lbTitle.Text = App.GetText(MsgCode.Run);
            this.lblCmds.Text = App.GetText(MsgCode.command);
            this.lblResponses.Text = App.GetText(MsgCode.response);
            this.btnSend.Text = App.GetText(MsgCode.send);
            this.btnConnect.Text = App.GetText(MsgCode.connect);
        }

        #endregion

    }
}