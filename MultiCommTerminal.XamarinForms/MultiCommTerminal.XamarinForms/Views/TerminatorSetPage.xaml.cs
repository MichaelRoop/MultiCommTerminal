﻿using CommunicationStack.Net.Stacks;
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
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            App.Wrapper.RetrieveTerminatorData(this.index, this.OnTerminatorInfoLoad, this.OnErr);
            base.OnAppearing();
        }

        #endregion

        #region Controls events

        private void lstStoredTerminators_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            this.lstStoredTerminators.ItemSelected -= this.lstStoredTerminators_ItemSelected;
            TerminatorInfo item = this.lstStoredTerminators.SelectedItem as TerminatorInfo;
            if (item != null) {
                this.btnSave.IsVisible = true;
                this.terminatorDisplay.AddEntry(item);
                this.lstStoredTerminators.SelectedItem = null;
            }
            this.lstStoredTerminators.ItemSelected += this.lstStoredTerminators_ItemSelected;
        }


        private void btnCancel_Clicked(object sender, EventArgs e) {
            // exit - check for changes
        }

        private void btnSave_Clicked(object sender, EventArgs e) {

        }

        private void btnDelete_Clicked(object sender, EventArgs e) {
            this.btnSave.IsVisible = true;
            this.terminatorDisplay.RemoveEntry();
        }

        #endregion

        #region Delegates

        private void OnTerminatorLoadOk(List<TerminatorInfo> terminatorEntities) {
            this.lstStoredTerminators.ItemsSource = terminatorEntities;
        }


        private void UpdateLanguage(SupportedLanguage language) {
            this.Title = language.GetText(MsgCode.Terminators);
        }


        private void OnTerminatorInfoLoad(TerminatorDataModel data) {
            this.dataModel = data;
            this.lbName.Text = this.dataModel.Name;
            this.terminatorDisplay.Reset();
            data.TerminatorInfos.ForEach(this.terminatorDisplay.AddEntry);
        }

        #endregion

    }
}