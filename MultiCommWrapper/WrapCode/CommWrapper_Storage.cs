using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.IO;

namespace MultiCommWrapper.Net.WrapCode {
    public partial class CommWrapper : ICommWrapper {

        #region Data

        private readonly string APP_DIR = "MultiCommSerialTerminal";
        private readonly string SETTINGS_DIR = "Settings";
        private readonly string TERMINATOR_DIR = "Terminators";
        private readonly string SETTINGS_FILE = "MultiCommSettings.txt";

        #endregion


        #region Private

        private void InitStorage() {
            // Settings
            this.settings = this.storageFactory.GetManager<SettingItems>(this.Dir(this.SETTINGS_DIR), this.SETTINGS_FILE);
            if (!this.settings.DefaultFileExists()) {
                this.settings.WriteObjectToDefaultFile(new SettingItems());
            }

            // Terminator indexed storage
            this.terminatorStorage = 
                this.storageFactory.GetIndexedManager<TerminatorData, DefaultFileExtraInfo>(this.Dir(TERMINATOR_DIR));
        }


        private string Dir(string subDir) {
            return Path.Combine(APP_DIR, subDir);
        }

        #endregion

    }
}
