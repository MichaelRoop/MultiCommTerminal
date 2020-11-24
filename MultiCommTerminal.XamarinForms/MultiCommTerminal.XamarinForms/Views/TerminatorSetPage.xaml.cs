using LogUtils.Net;
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

        }

        #endregion


    }
}