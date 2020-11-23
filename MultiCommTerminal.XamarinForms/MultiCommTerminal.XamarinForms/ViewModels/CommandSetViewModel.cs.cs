using MultiCommData.Net.StorageDataModels;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    public class CommandSetViewModel {

        #region Commands

        //Edit of create new command set

        /// <summary>Command to edit an existing command by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommand { get; }

        /// <summary>Command to add a new command to the command set</summary>
        public Command<ScriptDataModel> AddCommand;

        #endregion




    }
}
