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

    [QueryProperty(nameof(IndexAsString), "CommandEditPage.IndexAsString")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommandEditPage : ContentPage {

        #region Data

        private ClassLog log = new ClassLog("CommandEditPage");
        private ScriptItem scriptItem;
        private ScriptDataModel scriptDataModel;
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

        #region Contructor and page overrides

        public CommandEditPage() {
            InitializeComponent();
            this.interceptor = new NavigateBackInterceptor(this);
        }

        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            if (this.index != null) {
                // Need to load from memory. Only way to know which item of list to modify
                this.scriptDataModel = App.Wrapper.GetScratch().ScriptCommandSet.ScriptSet;
                this.scriptItem = App.Wrapper.GetScratch().ScriptCommand.Item;
                this.edName.Text = this.scriptItem.Display;
                this.edCmd.Text = this.scriptItem.Command;
                this.interceptor.Changed = false;
            }
            base.OnAppearing();
        }


        protected override bool OnBackButtonPressed() {
            return this.interceptor.HardwareOnBackButtonQuestion(base.OnBackButtonPressed);
        }

        #endregion

        #region Button handlers

        private void btnCancel_Clicked(object sender, EventArgs e) {
            interceptor.MethodExitQuestion();
        }


        private void btnSave_Clicked(object sender, EventArgs e) {
            ScriptItem tmp = new ScriptItem(this.edName.Text, this.edCmd.Text);
            App.Wrapper.ValidateScriptItem(
                tmp,
                () => {
                    // Iinitialise this object since it is in data model's item list 
                    this.scriptItem.Display = tmp.Display;
                    this.scriptItem.Command = tmp.Command;
                    App.Wrapper.SaveScript(
                        this.index, this.scriptDataModel, this.OnSaveOk, this.OnErr);
                }, this.OnErr);
        }


        private void edTextChanged(object sender, TextChangedEventArgs e) {
            this.interceptor.Changed = true;
        }

        #endregion

        #region Private 

        /// <summary>Delegate to handle updated language selection</summary>
        /// <param name="language">The current language</param>
        private void UpdateLanguage(SupportedLanguage language) {
            this.lbTitle.Text = language.GetText(MsgCode.Edit);
            this.lblCmd.Text = language.GetText(MsgCode.command);
            this.lblName.Text = language.GetText(MsgCode.Name);
            this.btnCancel.SetScreenReader(MsgCode.cancel);
            this.btnSave.SetScreenReader(MsgCode.save);
        }


        /// <summary>Delegate to fire on save OK to close the page</summary>
        private void OnSaveOk() {
            this.interceptor.Changed = false;
            this.interceptor.MethodExitQuestion();
        }

        #endregion
    }
}