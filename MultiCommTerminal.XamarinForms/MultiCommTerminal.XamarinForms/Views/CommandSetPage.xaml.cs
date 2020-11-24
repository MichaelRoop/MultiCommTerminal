using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommTerminal.XamarinForms.ViewModels;
using MultiCommWrapper.Net.Helpers;
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

    //[QueryProperty(nameof(CommandSetPage.IndexAsString), nameof(CommandSetPage.IndexAsString))]
    [QueryProperty(nameof(IndexAsString), "CommandSetPage.IndexAsString")]

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommandSetPage : ContentPage {

        #region Data

        private IIndexItem<DefaultFileExtraInfo> index = null;
        private CommandSetViewModel viewModel;
        private NavigateBackInterceptor interceptor;
        private ClassLog log = new ClassLog("CommandSetPage");

        #endregion

        #region Properties

        /// <summary>To receive the index of the set if editing. Otherise empty</summary>
        public string IndexAsString { set {
                if (!string.IsNullOrWhiteSpace(value)) {
                    try {
                        // Must not be interface for deserialize to work
                        this.index = JsonConvert.DeserializeObject<IndexItem<DefaultFileExtraInfo>>(
                            Uri.UnescapeDataString(value));
                    }
                    catch(Exception e) {
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

        #region Constructors and page overrides

        public CommandSetPage() {
            InitializeComponent();
            this.BindingContext = this.viewModel = new CommandSetViewModel();
            this.interceptor = new NavigateBackInterceptor(this);
            this.btnSave.IsVisible = false;
            this.edName.TextChanged += this.edName_TextChanged;
        }


        // Hardware back button
        protected override bool OnBackButtonPressed() {
            return this.interceptor.HardwareOnBackButtonQuestion(base.OnBackButtonPressed);
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            this.scriptDataModel = null;
            this.lstCmds.ItemsSource = null;
            this.lstCmds.SelectedItem = null;
            this.interceptor.Changed = false;

            if (this.index != null) {
                App.Wrapper.RetrieveScriptData(index, this.LoadExistingHandler, this.OnErr);
            }
            base.OnAppearing();
        }

        private ScriptDataModel scriptDataModel = null;


        private void LoadExistingHandler(ScriptDataModel dataModel) {
            this.edName.TextChanged -= this.edName_TextChanged;
            this.edName.Text = dataModel.Display;
            this.edName.TextChanged += this.edName_TextChanged;

            //this.setMode = ScratchMode.Edit;
            this.scriptDataModel = dataModel;
            this.lstCmds.ItemsSource = this.scriptDataModel.Items;
        }

        #endregion


        private void SetCommandItemScratch(ScriptItem item) {
            ScratchSet scratch = App.Wrapper.GetScratch();
            scratch.ScriptCommand.Mode = ScratchMode.Edit;
            scratch.ScriptCommand.Item = item;
            scratch.ScriptCommandSet.ScriptSet = this.scriptDataModel;
        }

        // TODO Where to save the name?

        private void btnAdd_Clicked(object sender, EventArgs e) {
            ScriptItem si = new ScriptItem();
            this.SetCommandItemScratch(si);
            this.scriptDataModel.Items.Add(si);
            // TODO - check the length
            this.scriptDataModel.Display = this.edName.Text;
            App.Wrapper.SaveScript(
                this.index, this.scriptDataModel, 
                () => {
                    this.viewModel.EditCommand.Execute(this.index);
                }, 
                this.OnErr);
        }


        private void btnDelete_Clicked(object sender, EventArgs e) {
            ScriptItem item = this.lstCmds.SelectedItem as ScriptItem;
            if (item != null) {
                bool success = this.scriptDataModel.Items.Remove(item);
                App.Wrapper.SaveScript(
                    this.index, 
                    this.scriptDataModel, () => {
                        App.Wrapper.RetrieveScriptData(index, this.LoadExistingHandler, this.OnErr);
                    }, 
                    this.OnErr);
            }
            else {
                this.OnErr(App.GetText(MsgCode.NothingSelected));
            }
        }


        private void btnEdit_Clicked(object sender, EventArgs e) {
            ScriptItem item = this.lstCmds.SelectedItem as ScriptItem;
            if (item != null) {
                // TODO - check the length
                this.scriptDataModel.Display = this.edName.Text;
                this.SetCommandItemScratch(item);
                this.viewModel.EditCommand.Execute(this.index);
            }
            else {
                this.OnErr(App.GetText(MsgCode.NothingSelected));
            }
        }


        private void btnCancel_Clicked(object sender, EventArgs e) {

            // Here you would need to ask the question
            this.interceptor.MethodExitQuestion();
        }

        private void btnSave_Clicked(object sender, EventArgs e) {
            // TODO save
            // TODO - check text length
            this.scriptDataModel.Display = this.edName.Text;
            App.Wrapper.SaveScript(
                this.index, 
                this.scriptDataModel, () => { 
                    this.interceptor.Changed = false;
                    this.interceptor.MethodExitQuestion();
                }, 
                this.OnErr);


        }

        private void edName_TextChanged(object sender, TextChangedEventArgs e) {
            this.interceptor.Changed = true;
            this.btnSave.IsVisible = true;
        }


        private void UpdateLanguage(SupportedLanguage language) {
            this.lbName.Text = language.GetText(MsgCode.Name);
        }


    }
}