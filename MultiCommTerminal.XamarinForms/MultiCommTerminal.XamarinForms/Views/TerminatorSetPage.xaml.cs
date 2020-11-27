using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [QueryProperty(nameof(IndexAsString), "TerminatorSetPage.IndexAsString")]
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TerminatorSetPage : ContentPage {

        #region Data

        private IIndexItem<DefaultFileExtraInfo> index = null;
        private NavigateBackInterceptor interceptor;
        private TerminatorDataModel dataModel;
        private TerminatorSetDisplay terminatorDisplay = new TerminatorSetDisplay();
        private ClassLog log = new ClassLog("TerminatorSetPage");

        #endregion

        #region Properties

        /// <summary>To receive the index of the set if editing. Otherise empty</summary>
        public string IndexAsString {
            set {
                if (!string.IsNullOrWhiteSpace(value)) {
                    try {
                        // Must not be interface for deserialize to work
                        this.index = JsonConvert.DeserializeObject<IndexItem<DefaultFileExtraInfo>>(
                            Uri.UnescapeDataString(value));
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "IndexAsString", "", e);
                        // TODO - error message and pop back?
                    }
                }
                else {
                    // TODO - major error
                    this.log.Error(9999, "IndexAsString", "Empty index string");
                    this.index = null;
                }
            }
        }

        #endregion

        #region Constructor and overrides

        public TerminatorSetPage() {
            InitializeComponent();
            this.interceptor = new NavigateBackInterceptor(this);

            // Only have to load the terminator list once
            App.Wrapper.GetTerminatorEntitiesList(this.OnTerminatorLoadOk, this.OnErr);

            this.terminatorDisplay.CreateLabelSet(this.name1, this.hex1);
            this.terminatorDisplay.CreateLabelSet(this.name2, this.hex2);
            this.terminatorDisplay.CreateLabelSet(this.name3, this.hex3);
            this.terminatorDisplay.CreateLabelSet(this.name4, this.hex4);
            this.terminatorDisplay.CreateLabelSet(this.name5, this.hex5);
            this.lstStoredTerminators.ItemSelected += this.lstStoredTerminators_ItemSelected;
            this.edName.TextChanged += this.edName_TextChanged;
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            App.Wrapper.RetrieveTerminatorData(this.index, this.OnTerminatorInfoLoad, this.OnErr);
            base.OnAppearing();
        }

        protected override bool OnBackButtonPressed() {
            this.DetermineIfChanged();
            return this.interceptor.HardwareOnBackButtonQuestion(base.OnBackButtonPressed);
        }

        #endregion

        #region Controls events

        private void lstStoredTerminators_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            this.lstStoredTerminators.ItemSelected -= this.lstStoredTerminators_ItemSelected;
            TerminatorInfo item = this.lstStoredTerminators.SelectedItem as TerminatorInfo;
            if (item != null) {
                this.btnSave.IsVisible = true;
                this.terminatorDisplay.AddEntry(item);
                this.DetermineIfChanged();
                this.lstStoredTerminators.SelectedItem = null;
            }
            this.lstStoredTerminators.ItemSelected += this.lstStoredTerminators_ItemSelected;
        }


        private void btnCancel_Clicked(object sender, EventArgs e) {
            this.interceptor.MethodExitQuestion();    
        }


        private void btnSave_Clicked(object sender, EventArgs e) {
            this.dataModel.Init(this.terminatorDisplay.InfoList);
            this.dataModel.Name = this.edName.Text;
            App.Wrapper.SaveTerminator(this.index, this.dataModel, this.OnSaveOk, this.OnErr);
        }

        private void btnDelete_Clicked(object sender, EventArgs e) {
            this.btnSave.IsVisible = true;
            this.terminatorDisplay.RemoveEntry();
            this.DetermineIfChanged();
        }

        #endregion

        #region Delegates

        private void OnTerminatorLoadOk(List<TerminatorInfo> terminatorEntities) {
            this.lstStoredTerminators.ItemsSource = terminatorEntities;
        }


        private void UpdateLanguage(SupportedLanguage language) {
            this.lbTitle.Text = language.GetText(MsgCode.Edit);
        }


        private void OnTerminatorInfoLoad(TerminatorDataModel data) {
            this.edName.TextChanged -= this.edName_TextChanged;
            this.dataModel = data;
            this.edName.Text = this.dataModel.Name;
            this.terminatorDisplay.Reset();
            data.TerminatorInfos.ForEach(this.terminatorDisplay.AddEntry);
            this.edName.TextChanged += this.edName_TextChanged;
            this.interceptor.Changed = false;
        }


        private void OnSaveOk() {
            this.interceptor.Changed = false;
            this.interceptor.MethodExitQuestion();
        }


        private void DetermineIfChanged() {
            if (!this.interceptor.Changed) {
                this.interceptor.Changed = this.terminatorDisplay.IsChanged;
            }
        }

        #endregion

        private void edName_TextChanged(object sender, TextChangedEventArgs e) {
            this.interceptor.Changed = true;
            this.btnSave.IsVisible = true;
        }
    }
}