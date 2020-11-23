using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.XamarinForms.UIHelpers;
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
    public partial class CommandSetsPage : ContentPage {


        private List<IIndexItem<DefaultFileExtraInfo>> scriptSets = new List<IIndexItem<DefaultFileExtraInfo>>();
        private CommandSetsViewModel viewModel;

        #region Constructors and page events

        public CommandSetsPage() {
            InitializeComponent();
            BindingContext = this.viewModel = new CommandSetsViewModel();
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.LanguageUpdate);
            this.UpdateList();
            base.OnAppearing();
        }

        #endregion

        private void btnAdd_Clicked(object sender, EventArgs e) {
            this.viewModel.AddCommandSet.Execute(null);
        }

        private void btnDelete_Clicked(object sender, EventArgs e) {
            IIndexItem<DefaultFileExtraInfo> item = this.lstCmdSets.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            App.Wrapper.DeleteScriptData(item, this.OnDelete, this.OnErr);
        }


        private void btnEdit_Clicked(object sender, EventArgs e) {
            // TODO - transition to CommandSetPage
            IIndexItem<DefaultFileExtraInfo> item = (this.lstCmdSets.SelectedItem as IIndexItem<DefaultFileExtraInfo>);
            if (item != null) {
                this.viewModel.EditCommandSet.Execute(item);
            }
            else {
                this.OnErr(App.GetText(MsgCode.NothingSelected));
            }
        }


        private void btnSelect_Clicked(object sender, EventArgs e) {
            // TODO
        }


        private void LanguageUpdate(SupportedLanguage language) {
            // TODO
        }


        private void OnDelete(bool ok) {
            this.UpdateList();
        }


        private void Load(List<IIndexItem<DefaultFileExtraInfo>> data) {
            this.lstCmdSets.ItemsSource = null;
            this.lstCmdSets.SelectedItem = null;
            this.scriptSets = data;
            this.lstCmdSets.ItemsSource = this.scriptSets;
        }


        private void UpdateList() {
            App.Wrapper.GetScriptList(this.Load, this.OnErr);
        }

    }
}