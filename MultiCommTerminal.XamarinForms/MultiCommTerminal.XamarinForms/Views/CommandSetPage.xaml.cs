using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommTerminal.XamarinForms.ViewModels;
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

    [QueryProperty(nameof(IndexAsString), nameof(IndexAsString))]

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommandSetPage : ContentPage {

        #region Data

        private IIndexItem<DefaultFileExtraInfo> index = null;
        private CommandSetViewModel viewModel;
        private NavigateBackInterceptor interceptor;
        private NavigateBackInterceptor.ChangedIndicator changed = new NavigateBackInterceptor.ChangedIndicator();

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
                    index = null;
                }
            }
        }

        #endregion

        #region Constructors and page overrides

        public CommandSetPage() {
            InitializeComponent();
            this.BindingContext = this.viewModel = new CommandSetViewModel();
            this.interceptor = new NavigateBackInterceptor(this, this.changed);
        }


        // Hardware back button
        protected override bool OnBackButtonPressed() {
            return this.interceptor.HardwareOnBackButtonQuestion(base.OnBackButtonPressed);
        }


        protected override void OnAppearing() {
            this.scriptDataModel = null;
            this.lstCmds.ItemsSource = null;
            this.lstCmds.SelectedItem = null;
            this.changed.IsChanged = false;

            if (index != null) {
                App.Wrapper.RetrieveScriptData(
                    index, this.LoadExistingHandler, this.OnErr);
            }
            else {
                List<ScriptItem> items = new List<ScriptItem>();
                items.Add(new ScriptItem("CmdName1", "Cmd1"));
                items.Add(new ScriptItem("CmdName2", "Cmd2"));
                this.scriptDataModel = new ScriptDataModel(items);
                this.lstCmds.ItemsSource = this.scriptDataModel.Items;
            }

            // TEMP TEST HACK - only set if data changes
            this.changed.IsChanged = true;

            base.OnAppearing();
        }

        private ScriptDataModel scriptDataModel = null;


        private void LoadExistingHandler(ScriptDataModel dataModel) {
            this.scriptDataModel = dataModel;
            this.lstCmds.ItemsSource = this.scriptDataModel.Items;
        }



        #endregion

        private void btnAdd_Clicked(object sender, EventArgs e) {

        }

        private void btnDelete_Clicked(object sender, EventArgs e) {

        }

        private void btnEdit_Clicked(object sender, EventArgs e) {

        }

        private void btnCancel_Clicked(object sender, EventArgs e) {
            //this.changed.IsChanged = false;

            // Here you would need to ask the question
            this.interceptor.MethodExitQuestion();
        }

        private void btnSave_Clicked(object sender, EventArgs e) {
            // TODO save


            this.changed.IsChanged = false;
            

        }
    }
}