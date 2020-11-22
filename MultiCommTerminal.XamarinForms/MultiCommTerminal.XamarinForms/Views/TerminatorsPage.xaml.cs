using LanguageFactory.Net.data;
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

        public TerminatorsPage() {
            InitializeComponent();
            App.Wrapper.LanguageChanged += this.OnLanguageChanged;
            this.UpdateLanguage();
        }

        private void OnLanguageChanged(object sender, LanguageFactory.Net.Messaging.SupportedLanguage e) {
            this.Title = App.GetText(MsgCode.Terminators);
        }

        private void OnCurrentTerminatorChanged(object sender, MultiCommData.Net.StorageDataModels.TerminatorDataModel e) {
            //throw new NotImplementedException();
        }

        protected override void OnAppearing() {
            this.btnAdd.IsEnabled = false;
            this.btnDelete.IsEnabled = false;
            this.btnEdit.IsEnabled = false;
            this.btnSelect.IsEnabled = false;

            //App.ShowError(this, "Terminators OnAppearing");

            App.Wrapper.GetTerminatorList((list) => {
                this.lstTerminators.SelectedItem = null;
                this.lstTerminators.ItemsSource = null;
                //this.items.Clear();
                this.items = list;
                this.lstTerminators.ItemsSource = this.items;
            }, (err) => App.ShowError(this, err));

            App.Wrapper.CurrentTerminatorChanged += this.OnCurrentTerminatorChanged;
        }


        protected override void OnDisappearing() {
            App.Wrapper.CurrentTerminatorChanged -= this.OnCurrentTerminatorChanged;
            base.OnDisappearing();
        }


        private void lstTerminators_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            // Enable the edit and delete

            this.btnAdd.IsEnabled = true;
            this.btnDelete.IsEnabled = this.items.Count > 1;
            this.btnEdit.IsEnabled = true;
            this.btnSelect.IsEnabled = true;

            // TODO background reverts to blue default on enable

        }


        private void UpdateLanguage() {
            this.Title = App.GetText(MsgCode.Terminators);
        }

        private void btnAdd_Clicked(object sender, EventArgs e) {

        }

        private void btnDelete_Clicked(object sender, EventArgs e) {

        }

        private void btnEdit_Clicked(object sender, EventArgs e) {

        }

        private void btnSelect_Clicked(object sender, EventArgs e) {

        }
    }
}