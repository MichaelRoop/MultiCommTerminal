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

        
        private IIndexItem<DefaultFileExtraInfo> index = null;
        private CommandSetViewModel viewModel;

        //private bool isChanged = true;
        private NavigateBackInterceptor exitQuestion;
        private NavigateBackInterceptor.ChangedIndicator changed = new NavigateBackInterceptor.ChangedIndicator();


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
            this.exitQuestion = new NavigateBackInterceptor(this, this.changed);


            //// This handles the navigation bar back button
            //Shell.SetBackButtonBehavior(this, new BackButtonBehavior {
            //    Command = new Command(async () => {
            //        if (!this.isChanged) {
            //            await Shell.Current.Navigation.PopAsync();
            //        }
            //    })
            //});
        }

        // Hardware back button
        protected override bool OnBackButtonPressed() {
            return this.exitQuestion.HardwareOnBackButtonQuestion(base.OnBackButtonPressed);

            //Device.BeginInvokeOnMainThread(async () => {
            //    if (await DisplayAlert("Exit?", "Are you sure you want to exit from this page?", "Yes", "No")) {
            //        base.OnBackButtonPressed();
            //        await Shell.Current.Navigation.PopAsync();
            //    }
            //});
            //return true;
        }



        protected override void OnAppearing() {
            this.scriptDataModel = null;
            this.lstCmds.ItemsSource = null;
            this.lstCmds.SelectedItem = null;
            this.changed.IsChanged = false;

            if (index != null) {
                App.Wrapper.RetrieveScriptData(
                    index,
                    this.LoadExistingHandler,
                    (err) => App.ShowError(this, err));
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
            //Navigation.PopAsync(true);
            //this.isChanged = false;

            this.changed.IsChanged = false;

        }

        private void btnSave_Clicked(object sender, EventArgs e) {
            // TODO save

            this.changed.IsChanged = false;

            //Navigation.PopAsync(true);

        }
    }
}