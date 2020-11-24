using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
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

        /// <summary>Command to edit an existing command by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommand { get; }

        /// <summary>Command to add a new command to the command set</summary>
        //public Command AddCommand { get; }


        public CommandSetViewModel() {
            //this.AddCommand = new Command(this.OnAdd);
            this.EditCommand = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnEdit);
        }



        private async void OnEdit(IIndexItem<DefaultFileExtraInfo> data) {
            // Set a property to indicate Add
            //await Shell.Current.GoToAsync(nameof(CommandEditPage));
            if (data != null) {
                await Shell.Current.GoToAsync(
                    $"{nameof(CommandEditPage)}?CommandEditPage.IndexAsString={JsonConvert.SerializeObject(data)}");
            }
        }


        //private async void OnAdd() {
        //    // Set a property to indicate Add
        //    await Shell.Current.GoToAsync(nameof(CommandEditPage));
        //}


    }
}
