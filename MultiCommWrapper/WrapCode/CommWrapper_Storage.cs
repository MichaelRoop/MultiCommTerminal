using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Collections.Generic;
using System.IO;

namespace MultiCommWrapper.Net.WrapCode {
    public partial class CommWrapper : ICommWrapper {

        #region Data

        private readonly string APP_DIR = "MultiCommSerialTerminal";
        private readonly string SETTINGS_DIR = "Settings";
        private readonly string SETTINGS_FILE = "MultiCommSettings.txt";
        private readonly string TERMINATOR_DIR = "Terminators";
        private readonly string TERMINATOR_INDEX_FILE = "TerminatorsIndex.txt";
        private readonly string SCRIPTS_DIR = "Scripts";
        private readonly string SCRIPTS_INDEX_FILE = "ScriptsIndex.txt";
        private readonly string WIFI_CRED_DIR = "WifiCredentials";
        private readonly string WIFI_CRED_INDEX_FILE = "WifiCredIndex.txt";

        #endregion

        #region Private

        private void InitStorage() {
            this.settings = this.storageFactory.GetManager<SettingItems>(this.Dir(this.SETTINGS_DIR), this.SETTINGS_FILE);
            this.AssureSettingsDefault();

            this.terminatorStorage = 
                this.storageFactory.GetIndexedManager<TerminatorDataModel, DefaultFileExtraInfo>(this.Dir(TERMINATOR_DIR), TERMINATOR_INDEX_FILE);
            this.AssureTerminatorsDefault();

            this.scriptStorage =
                this.storageFactory.GetIndexedManager<ScriptDataModel, DefaultFileExtraInfo>(this.Dir(SCRIPTS_DIR), SCRIPTS_INDEX_FILE);
            this.AssureScriptDefault();

            this.wifiCredStorage =
                this.storageFactory.GetIndexedManager<WifiCredentialsDataModel, DefaultFileExtraInfo>(this.Dir(WIFI_CRED_DIR), WIFI_CRED_INDEX_FILE);
            // We do not have a dummy entry
        }


        private string Dir(string subDir) {
            return Path.Combine(APP_DIR, subDir);
        }

        #region Assure default methods

        /// <summary>Create default settings if it does not exist</summary>
        private void AssureSettingsDefault() {
            if (!this.settings.DefaultFileExists()) {
                this.settings.WriteObjectToDefaultFile(new SettingItems());
            }
        }


        /// <summary>
        /// If no terminators defined, create default, store and set as default in settings
        /// </summary>
        private void AssureTerminatorsDefault() {
            // If nothing exists create default
            List<IIndexItem<DefaultFileExtraInfo>> index = this.terminatorStorage.IndexedItems;
            if (index.Count == 0) {
                // For a new one. Different when updating. Do not need to create new index
                List<TerminatorInfo> infos = new List<TerminatorInfo>();
                infos.Add(new TerminatorInfo(Terminator.LF));
                infos.Add(new TerminatorInfo(Terminator.CR));
                TerminatorDataModel dm = new TerminatorDataModel(infos);

                IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(dm.UId) {
                    Display = "Default terminator \\n\\r"
                };
                this.terminatorStorage.Store(dm, idx);

                this.GetSettings(
                    (settings) => {
                        settings.CurrentTerminator = dm;
                        this.SaveSettings(settings, () => { }, (err) => { });
                    },
                    (err) => { });
            }
        }


        private void AssureScriptDefault() {
            List<IIndexItem<DefaultFileExtraInfo>> index = this.scriptStorage.IndexedItems;
            if (index.Count == 0) {
                List<ScriptItem> items = new List<ScriptItem>();
                items.Add(new ScriptItem() { Display = "Command 1", Command = "This is first command" });
                items.Add(new ScriptItem() { Display = "Command 2", Command = "This is second command" });
                items.Add(new ScriptItem() { Display = "Command 3", Command = "This is third command" });
                items.Add(new ScriptItem() { Display = "Command 4", Command = "This is fourth command" });
                ScriptDataModel dm = new ScriptDataModel(items) { 
                    Display = "Default script"
                };
                IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(dm.UId) {
                    Display = "Default script",
                };
                this.scriptStorage.Store(dm, idx);

                this.GetSettings(
                    (settings) => {
                        settings.CurrentScript = dm;
                        this.SaveSettings(settings, () => { }, (err) => { });
                    },
                    (err) => { });
            }
        }

        #endregion

        #endregion

    }
}
