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
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommandSet { get; }

        #endregion


        public CommandSetsViewModel() {
            this.EditCommandSet = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnEdit);
        }


        #region Commands

        private async void OnEdit(IIndexItem<DefaultFileExtraInfo> data) {
            if (data != null) {
                await Shell.Current.GoToAsync($"{nameof(CommandSetPage)}?CommandSetPage.IndexAsString={JsonConvert.SerializeObject(data)}");
            }
        }

        #endregion


    }
}
