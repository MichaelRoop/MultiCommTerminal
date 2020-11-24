﻿using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.UIHelpers;
using MultiCommTerminal.XamarinForms.ViewModels;
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

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class TerminatorsPage : ContentPage {

        private List<IIndexItem<DefaultFileExtraInfo>> items = new List<IIndexItem<DefaultFileExtraInfo>>();
        private TerminatorsViewModel viewer;
        private ClassLog log = new ClassLog("TerminatorsPage");


        public TerminatorsPage() {
            InitializeComponent();
            this.BindingContext = this.viewer = new TerminatorsViewModel();
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            this.ReloadList(true);
        }


        protected override void OnDisappearing() {
            base.OnDisappearing();
        }

        #region Button events

        private void btnAdd_Clicked(object sender, EventArgs e) {
            this.log.InfoEntry("btnAdd_Clicked");
            App.Wrapper.CreateNewTerminator("NewSet", new TerminatorDataModel(),
                (ndx) => {
                    this.viewer.EditTerminatorSet.Execute(ndx);
                }, 
                this.OnErr);
        }


        private void btnDelete_Clicked(object sender, EventArgs e) {
            this.log.InfoEntry("btnDelete_Clicked");
            IIndexItem<DefaultFileExtraInfo> item = this.lstTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            App.Wrapper.DeleteTerminatorData(item, this.ReloadList, (err) => App.ShowError(this, err));
        }


        private void btnEdit_Clicked(object sender, EventArgs e) {
            this.log.InfoEntry("btnEdit_Clicked");
            var index = this.lstTerminators.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            App.Wrapper.RetrieveTerminatorData(
                index,
                (dataModel) => {
                    // Not actually going to store the info here. Just called 
                    // to validate the index
                    this.viewer.EditTerminatorSet.Execute(index);

                }, this.OnErr);
        }

        #endregion

        #region Private

        private void UpdateLanguage(SupportedLanguage language) {
            this.Title = language.GetText(MsgCode.Terminators);
        }


        private void ReloadList(bool success) {
            // The bool just tells you if the delete from file was successful
            App.Wrapper.GetTerminatorList((list) => {
                this.lstTerminators.SelectedItem = null;
                this.lstTerminators.ItemsSource = null;
                //this.items.Clear();
                this.items = list;
                this.lstTerminators.ItemsSource = this.items;
            }, (err) => App.ShowError(this, err));
        }

        #endregion

    }
}