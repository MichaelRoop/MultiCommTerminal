using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.Views;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {


    // TODO - property to accept the index item of the set to accept the 
    // index to load
    public class CommandSetViewModel {

        // Already in Page
        //private IIndexItem<DefaultFileExtraInfo> itemIndex = null;
        //public string IndexItemAsString { get; set; } = string.Empty;


        #region Commands

        //Edit of create new command set

        /// <summary>Command to edit an existing command by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommand { get; }

        /// <summary>Command to add a new command to the command set</summary>
        public Command<ScriptDataModel> AddCommand { get; }

        public Command GoBackCommand { get; }


        #endregion


        public CommandSetViewModel() {
            this.GoBackCommand = new Command(this.OnGoBack);
        }



        private async void OnGoBack() {
            await Shell.Current.GoToAsync(nameof(CommandSetsPage));
        }


    }
}
