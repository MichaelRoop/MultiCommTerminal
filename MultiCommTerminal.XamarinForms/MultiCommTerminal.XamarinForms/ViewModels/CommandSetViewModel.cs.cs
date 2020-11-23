using MultiCommData.Net.StorageDataModels;
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

        private IIndexItem<DefaultFileExtraInfo> itemIndex = null;


        public string IndexItemAsString { get; set; } = string.Empty;


        #region Commands

        //Edit of create new command set

        /// <summary>Command to edit an existing command by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommand { get; }

        /// <summary>Command to add a new command to the command set</summary>
        public Command<ScriptDataModel> AddCommand;

        #endregion




    }
}
