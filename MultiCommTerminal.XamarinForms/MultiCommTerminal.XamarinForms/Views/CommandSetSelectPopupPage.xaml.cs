using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Rg.Plugins.Popup.Services;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommandSetSelectPopupPage : Rg.Plugins.Popup.Pages.PopupPage {

        public CommandSetSelectPopupPage() {
            InitializeComponent();
            App.Wrapper.GetScriptList(this.UpdateCommandList, this.OnErr);
            this.CloseWhenBackgroundIsClicked = false;
        }

        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            this.btnSelect.IsVisible = false;
            this.lstCmdSets.ItemSelected += this.LstCmdSets_ItemSelected;
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            this.lstCmdSets.ItemSelected -= this.LstCmdSets_ItemSelected;
            base.OnDisappearing();
        }


        protected override bool OnBackButtonPressed() {
            return true;
            //return base.OnBackButtonPressed();
        }


        private void LstCmdSets_ItemSelected(object sender, Xamarin.Forms.SelectedItemChangedEventArgs e) {
            this.btnSelect.IsVisible = true;
        }


        private void btnCancel_Clicked(object sender, EventArgs e) {
            this.Exit();
        }


        private void btnSelect_Clicked(object sender, EventArgs e) {
            IIndexItem<DefaultFileExtraInfo> ndx = this.lstCmdSets.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            App.Wrapper.SetCurrentScript(ndx, this.Exit, this.OnErr);
        }

        #region Delegates

        private void UpdateCommandList(List<IIndexItem<DefaultFileExtraInfo>> scriptSets) {
            this.lstCmdSets.ItemsSource = scriptSets;
        }


        private void UpdateLanguage(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.commands);
            this.btnSelect.SetScreenReader(l.GetText(MsgCode.select));
            this.btnCancel.SetScreenReader(l.GetText(MsgCode.cancel));
        }


        private void Exit() {
            PopupNavigation.Instance.PopAsync(true);
        }

        #endregion

    }
}