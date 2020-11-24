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
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        private const int TERMINATOR_MAX = 5;
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
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            App.Wrapper.RetrieveTerminatorData(this.index, this.OnTerminatorInfoLoad, this.OnErr);
            base.OnAppearing();
        }


        #endregion

        private void lstStoredTerminators_ItemSelected(object sender, SelectedItemChangedEventArgs e) {

        }

        private void btnAdd_Clicked(object sender, EventArgs e) {

        }

        private void btnCancel_Clicked(object sender, EventArgs e) {

        }

        private void btnSave_Clicked(object sender, EventArgs e) {

        }
        private void btnDelete_Clicked(object sender, EventArgs e) {

        }


        #region Delegates

        private void OnTerminatorLoadOk(List<TerminatorInfo> terminatorEntities) {
            this.lstStoredTerminators.ItemsSource = terminatorEntities;
        }


        private void UpdateLanguage(SupportedLanguage language) {
            //this.lbTitle.Text = language.GetText(MsgCode.Terminators);
            this.Title = language.GetText(MsgCode.Terminators);
        }

        private int currentPos = 0;


        private void OnTerminatorInfoLoad(TerminatorDataModel data) {
            this.dataModel = data;
            this.lbName.Text = this.dataModel.Name;

            this.log.Error(8888, () => string.Format("COUNT:{0}", this.dataModel.TerminatorInfos.Count));

            for (int i = 0; i< this.dataModel.TerminatorInfos.Count; i++) {
                if (i >= TERMINATOR_MAX) {
                    // Out of bounds
                    break;
                }
                this.currentPos = i;
                TerminatorInfo info = this.dataModel.TerminatorInfos[this.currentPos];

                this.log.Error(8888, () => string.Format("{0} : {1}", info.HexDisplay, info.Display));

                switch (i) {
                    case 0:
                        this.hex1.Text = info.HexDisplay;
                        this.name1.Text = info.Display;
                        break;
                    case 1:
                        this.hex2.Text = info.HexDisplay;
                        this.name2.Text = info.Display;
                        break;
                    case 2:
                        this.hex3.Text = info.HexDisplay;
                        this.name3.Text = info.Display;
                        break;
                    case 3:
                        this.hex4.Text = info.HexDisplay;
                        this.name4.Text = info.Display;
                        break;
                    case 4:
                        this.hex5.Text = info.HexDisplay;
                        this.name5.Text = info.Display;
                        break;
                }
            }
        }


        #endregion

    }
}