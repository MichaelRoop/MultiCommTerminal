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

        #endregion

        #region Private

        private void InitStorage() {
            this.settings = this.storageFactory.GetManager<SettingItems>(this.Dir(this.SETTINGS_DIR), this.SETTINGS_FILE);
            this.AssureSettingsDefault();

            this.terminatorStorage = 
                this.storageFactory.GetIndexedManager<TerminatorDataModel, DefaultFileExtraInfo>(this.Dir(TERMINATOR_DIR), TERMINATOR_INDEX_FILE);
            this.AssureTerminatorsDefault();
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

        #endregion

        #endregion

    }
}
