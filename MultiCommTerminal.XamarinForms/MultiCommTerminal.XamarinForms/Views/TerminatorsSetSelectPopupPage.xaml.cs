using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.XamarinForms.UIHelpers;
using Rg.Plugins.Popup.Services;
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
    public partial class TerminatorsSetSelectPopupPage : Rg.Plugins.Popup.Pages.PopupPage {

        #region Constructors and overrides

        public TerminatorsSetSelectPopupPage() {
            InitializeComponent();
            this.CloseWhenBackgroundIsClicked = false;
        }


        protected override void OnAppearing() {
            this.btnSelect.IsVisible = false;
            this.lbTerminatorsText.Text = "";
            App.Wrapper.GetTerminatorList(this.PopulateTerminators, this.OnErr);
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            this.lstTerminatorSets.ItemSelected += this.LstTerminatorSets_ItemSelected;
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            this.lstTerminatorSets.ItemSelected -= this.LstTerminatorSets_ItemSelected;
            base.OnDisappearing();
        }

        protected override bool OnBackButtonPressed() {
            return true;
        }

        #endregion

        #region Controls events

        private void LstTerminatorSets_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            this.btnSelect.IsVisible = true;
            var item = e.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            this.lbTerminatorsText.Text = "";
            App.Wrapper.RetrieveTerminatorData(item,
                (data) => {
                    StringBuilder tmp = new StringBuilder();
                    for (int i = 0; i < data.TerminatorInfos.Count; i++) {
                        if (i > 0) {
                            tmp.Append(",");
                        }
                        tmp.Append(data.TerminatorInfos[i].Display);
                    }
                    this.lbTerminatorsText.Text = tmp.ToString();
                }, this.OnErr);
        }

        private void btnCancel_Clicked(object sender, EventArgs e) {
            this.Exit();
        }


        private void btnSelect_Clicked(object sender, EventArgs e) {
            IIndexItem<DefaultFileExtraInfo> ndx = this.lstTerminatorSets.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            App.Wrapper.SetCurrentTerminators(ndx, this.Exit, this.OnErr);
        }

        #endregion

        #region Private

        private void PopulateTerminators(List<IIndexItem<DefaultFileExtraInfo>> indexes) {
            this.lstTerminatorSets.ItemsSource = indexes;
        }


        private void UpdateLanguage(SupportedLanguage l) {
            this.lbTitle.Text = l.GetText(MsgCode.Terminators);
            this.btnCancel.SetScreenReader(l.GetText(MsgCode.cancel));
            this.btnSelect.SetScreenReader(l.GetText(MsgCode.select));
        }


        private void Exit() {
            PopupNavigation.Instance.PopAsync(true);
        }

        #endregion

    }
}