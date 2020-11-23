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

    [QueryProperty(nameof(IndexAsString), nameof(IndexAsString))]

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class CommandSetPage : ContentPage {

        
        private IIndexItem<DefaultFileExtraInfo> index = null;

        #region Properties

        /// <summary>To receive the index of the set if editing. Otherise empty</summary>
        public string IndexAsString { set {
                if (!string.IsNullOrWhiteSpace(value)) {
                    this.index = JsonConvert.DeserializeObject<IIndexItem<DefaultFileExtraInfo>>(
                        Uri.UnescapeDataString(value));
                }
                else {
                    index = null;
                }
            }
        }

        #endregion

        #region Constructors and page overrides

        public CommandSetPage() {
            InitializeComponent();
        }

        protected override void OnAppearing() {
            if (index != null) {
                // Load the command set into list view from wrapper
            }
            else {
                // Create new command set and post empty list to list view
            }
            base.OnAppearing();
        }

        #endregion






    }
}