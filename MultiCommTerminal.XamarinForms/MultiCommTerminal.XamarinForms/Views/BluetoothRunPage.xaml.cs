using BluetoothCommon.Net;
using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
        private ObservableCollection<ScriptItem> cmds = new ObservableCollection<ScriptItem>();
        private ObservableCollection<string> responses = new ObservableCollection<string>();

        private int okCount = 0;
        private int failCount = 0;
        private int connectTry = 0;

        public string BTDevice {
            set {
                // Set on page opening with routing. Can only pass strings as parameters
                this.device = JsonConvert.DeserializeObject<BTDeviceInfo>(Uri.UnescapeDataString(value ?? string.Empty));
            }
        }

        #region Constructor and page overrides

        public BluetoothRunPage() {
            InitializeComponent();
            this.UpdateLanguage();

            App.Wrapper.LanguageChanged += this.OnLanguageChanged;
            App.Wrapper.BT_ConnectionCompleted += this.OnBT_ConnectionCompletedHandler;
            App.Wrapper.BT_BytesReceived += this.OnBT_BytesReceivedHandler;
            this.lstCmds.ItemsSource = this.cmds;
            this.lstResponses.ItemsSource = this.responses;
        }


        protected override void OnAppearing() {
            // TODO move this to a view model to do a busy and finish?
            this.UpdateLanguage();
            this.okCount = 0;
            this.failCount = 0;
            this.connectTry = 0;
            this.cmds.Clear();
            this.responses.Clear();

            App.Wrapper.GetCurrentScript(
                this.PopulateScriptList, 
                (err) => App.ShowError(this, err));

            this.DoConnection(this.device);
            base.OnAppearing();
        }


        private void PopulateScriptList(ScriptDataModel scripts) {
            this.lstCmds.ItemsSource = null;
            this.cmds.Clear();
            foreach (var script in scripts.Items) {
                this.cmds.Add(script);
            }
            this.lstCmds.ItemsSource = this.cmds;
        }



        protected override void OnDisappearing() {
            App.Wrapper.BTClassicDisconnect();
            base.OnDisappearing();
        }

        #endregion


        #region Wrapper event handlers

        private void OnBT_ConnectionCompletedHandler(object sender, bool ok) {
            Device.BeginInvokeOnMainThread(() => {
                this.IsBusy = false;
                if (ok) {
                    this.okCount++;
                    //this.txtOk.Text = string.Format("Ok:{0}", okCount);
                }
                else {
                    this.failCount++;
                    //this.txtFail.Text = string.Format("Failed:{0}", failCount);
                }
            });
        }


        private void OnLanguageChanged(object sender, SupportedLanguage e) {
            Device.BeginInvokeOnMainThread(() => {
                this.UpdateLanguage();
            });
        }

        #endregion


        /// <summary>Do the connection</summary>
        /// <param name="info">the device info for connection</param>
        private void DoConnection(BTDeviceInfo info) {
            if (info != null) {
                this.connectTry++;
                //this.txtConnectTry.Text = this.connectTry.ToString();
                this.IsBusy = true;
                App.Wrapper.BTClassicConnectAsync(info);
            }
            else {
                // TODO - backwards
            }
        }


        private void UpdateLanguage() {
            this.Title = App.GetText(MsgCode.connect);
            this.lblCmds.Text = App.GetText(MsgCode.command);
            this.lblResponses.Text = App.GetText(MsgCode.response);
            this.btnSend.Text = App.GetText(MsgCode.send);
            this.btnRefresh.Text = App.GetText(MsgCode.connect);
        }


        private void Refresh_Clicked(object sender, EventArgs e) {
            this.DoConnection(this.device);
        }

        private void lstCmds_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            ScriptItem item = e.SelectedItem as ScriptItem;
            if (item != null) {
                this.entryCmd.Text = item.Command;
            }
        }



        private void btnSend_Clicked(object sender, EventArgs e) {
            // ----------------------------------------------------------------------
            // TODO REMOVE HACK
            // TO TEST NEED CR LN terminators
            List<TerminatorInfo> infos = new List<TerminatorInfo>();
            infos.Add(new TerminatorInfo(Terminator.CR));
            infos.Add(new TerminatorInfo(Terminator.LF));
            TerminatorDataModel dm = new TerminatorDataModel(infos);
            App.Wrapper.SetCurrentTerminators(dm, (err)=> App.ShowError(this, err));
            // ----------------------------------------------------------------------

            App.Wrapper.BTClassicSend(this.entryCmd.Text);
        }


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



    }
}