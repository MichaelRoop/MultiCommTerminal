using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using System.IO;

namespace MultiCommWrapper.Net.WrapCode {
    public partial class CommWrapper : ICommWrapper {

        #region Data

        private readonly string APP_DIR = "MultiCommSerialTerminal";
        private readonly string SETTINGS_DIR = "Settings";
        private readonly string SETTINGS_FILE = "MultiCommSettings.txt";

        #endregion

        #region Private

        private void InitStorage() {
            this.settings = this.storageFactory.GetManager<SettingItems>(this.Dir(this.SETTINGS_DIR), this.SETTINGS_FILE);
            if (!this.settings.DefaultFileExists()) {
                this.settings.WriteObjectToDefaultFile(new SettingItems());
            }
        }


        private string Dir(string subDir) {
            return Path.Combine(APP_DIR, subDir);
        }

        #endregion

    }
}
