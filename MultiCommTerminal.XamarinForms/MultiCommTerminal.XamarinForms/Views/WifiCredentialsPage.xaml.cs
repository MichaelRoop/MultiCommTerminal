using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
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

    public partial class WifiCredentialsPage : ContentPage {

        #region Data

        WifiCredentialsViewModel viewModel;

        #endregion

        #region Constructors and overrides

        public WifiCredentialsPage() {
            InitializeComponent();
            this.BindingContext = this.viewModel = new WifiCredentialsViewModel();
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            App.Wrapper.GetWifiCredList(this.ReloadList, this.OnErr);
            base.OnAppearing();
        }

        #endregion


        #region Button event handlers

        private void btnAdd_Clicked(object sender, EventArgs e) {
            this.viewModel.GoToCredEdit.Execute(null);
        }

        private void btnEdit_Clicked(object sender, EventArgs e) {
            this.viewModel.GoToCredEdit.Execute(null);
        }

        private void btnDelete_Clicked(object sender, EventArgs e) {

        }

        #endregion

        #region Event handlers

        private void LanguageUpdate(SupportedLanguage language) {
            this.lbTitle.Text = language.GetText(MsgCode.Credentials);
        }

        #endregion


        private void ReloadList(List<IIndexItem<DefaultFileExtraInfo>> items) {
            this.lstCreds.ItemsSource = null;
            this.lstCreds.SelectedItem = null;
            this.lstCreds.ItemsSource = items;
        }

    }
}