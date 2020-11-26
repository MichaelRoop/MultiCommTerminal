using CommunicationStack.Net.Enumerations;
using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
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
            App.Wrapper.Wifi_BytesReceived += Wifi_BytesReceivedHandler;
            App.Wrapper.OnWifiConnectionAttemptCompleted += this.OnWifiConnectionAttemptCompletedHandler;
            this.lstCmds.ItemsSource = this.cmds;
            this.lstResponses.ItemsSource = this.responses;
        }


        private void OnWifiConnectionAttemptCompletedHandler(object sender, CommunicationStack.Net.DataModels.MsgPumpResults e) {
            if (e.Code != MsgPumpResultCode.Connected) {
                this.OnErr(string.Format("Connect Attempt Result:{0} - {1}", e.Code, e.ErrorString));
            }
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.OnLanguageUpdate);
            App.Wrapper.GetCurrentScript(this.PopulateScriptList, this.OnErr);
            base.OnAppearing();
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
            App.Wrapper.WifiConnectAsync(this.networkInfo);
        }

        private void btnDisconnect_Clicked(object sender, EventArgs e) {
            App.Wrapper.WifiDisconect();
        }


        #region Delegates

        private void OnLanguageUpdate(SupportedLanguage language) {
            this.btnConnect.Text = language.GetText(MsgCode.connect);
            this.btnDisconnect.Text = language.GetText(MsgCode.Disconnect);
            this.btnSend.Text = language.GetText(MsgCode.send);
        }


        private void PopulateScriptList(ScriptDataModel scripts) {
            this.lstCmds.ItemsSource = null;
            this.cmds.Clear();
            foreach (var script in scripts.Items) {
                this.cmds.Add(script);
            }
            this.lstCmds.ItemsSource = this.cmds;
        }

        #endregion

    }
}