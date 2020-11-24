using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
//using MultiCommTerminal.XamarinForms.ViewModels;
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
//        private CommandEditViewModel viewModel;

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
//            this.BindingContext = this.viewModel = new CommandEditViewModel();
            this.interceptor = new NavigateBackInterceptor(this);
        }

        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            if (this.index != null) {
                App.Wrapper.RetrieveScriptData(
                    this.index, this.OnCommandLoaded, this.OnErr);
            }
            base.OnAppearing();
        }


        // Hardware back button
        protected override bool OnBackButtonPressed() {
            return this.interceptor.HardwareOnBackButtonQuestion(base.OnBackButtonPressed);
        }

        #endregion

        #region Button handlers

        private void btnCancel_Clicked(object sender, EventArgs e) {
            interceptor.MethodExitQuestion();
        }


        private void btnSave_Clicked(object sender, EventArgs e) {
            if (string.IsNullOrWhiteSpace(this.edName.Text) ||
                string.IsNullOrWhiteSpace(this.edCmd.Text)) {
                this.OnErr(App.GetText(MsgCode.EmptyName));
            }
            else {
                this.scriptItem.Display = this.edName.Text;
                this.scriptItem.Command = this.edCmd.Text;
                // do save
                App.Wrapper.SaveScript(this.index, this.scriptDataModel, () => {

                }, this.OnErr);
            }
        }


        private void edTextChanged(object sender, TextChangedEventArgs e) {
            this.interceptor.Changed = true;
        }

        #endregion

        #region Private

        private void UpdateLanguage(SupportedLanguage language) {
            this.lblCmd.Text = language.GetText(MsgCode.command);
            this.lblName.Text = language.GetText(MsgCode.Name);
        }


        private void OnCommandLoaded(ScriptDataModel dm) {
            this.scriptDataModel = dm;
            // This is the one created in CommandSetPage.
            // It is already in the script data model.Items
            this.scriptItem = App.Wrapper.GetScratch().ScriptCommand.Item;
            this.edName.Text = this.scriptItem.Display;
            this.edCmd.Text = this.scriptItem.Command;
            this.interceptor.Changed = false;
        }


        private void OnSaveOk() {
            this.interceptor.Changed = false;
            this.interceptor.MethodExitQuestion();
        }

        #endregion
    }
}