using MultiCommData.Net.StorageDataModels;
using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.Views {

    public class CommandSetsViewModel {

        #region Commands

        //Edit of create new command set

        /// <summary>Command to edit an existing command by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommandSet{get;}

        /// <summary>Command to add a new command to the command set</summary>
        public Command AddCommandSet;

        #endregion


        public CommandSetsViewModel() {
            this.AddCommandSet = new Command(this.OnAdd);
            this.EditCommandSet = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnEdit);
        }



        #region Commands

        private async void OnAdd() {
            //await Shell.Current.GoToAsync(nameof(CommandSetPage));
            await Shell.Current.GoToAsync($"{nameof(CommandSetPage)}?IndexItemAsString={""}");
        }


        private async void OnEdit(IIndexItem<DefaultFileExtraInfo> data) {
            if (data != null) {
                await Shell.Current.GoToAsync($"{nameof(CommandSetPage)}?IndexItemAsString={JsonConvert.SerializeObject(data)}");
            }
        }

        #endregion


    }
}
