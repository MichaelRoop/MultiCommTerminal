using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.Stacks;
using Ethernet.Common.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {
    public partial class CommWrapper : ICommWrapper {

        #region Data

        private static string PDF_USER_MANUAL_DIR_AND_FILE = "Documents/MultiCommTerminalUserDocRelease.pdf";

        private readonly string APP_DIR = "MultiCommSerialTerminal";
        private readonly string SETTINGS_DIR = "Settings";
        private readonly string SETTINGS_FILE = "MultiCommSettings.txt";
        private readonly string TERMINATOR_DIR = "Terminators";
        private readonly string TERMINATOR_INDEX_FILE = "TerminatorsIndex.txt";
        private readonly string SCRIPTS_DIR = "Scripts";
        private readonly string SCRIPTS_INDEX_FILE = "ScriptsIndex.txt";
        private readonly string WIFI_CRED_DIR = "WifiCredentials";
        private readonly string WIFI_CRED_INDEX_FILE = "WifiCredIndex.txt";
        private readonly string ETHERNET_DATA_DIR = "EthernetData";
        private readonly string ETHERNET_DATA_INDEX_FILE = "EthernetDataIndex.txt";
        private readonly string SERIAL_CFG_DIR = "SerialConfigurations";
        private readonly string SERIAL_CFG_INDEX_FILE = "SerialCfgIndex.txt";

        private readonly string USER_MANUAL_DIR = "Documents";


        // UWP path and file name for document
        private readonly string UWP_ORIGINE_USER_MANUAL_PATH_AND_FILE = string.Format("{0}{1}",             
            AppDomain.CurrentDomain.BaseDirectory, PDF_USER_MANUAL_DIR_AND_FILE);

        // Windows path and file name for document
        private readonly string WIN_ORIGINE_USER_MANUAL_PATH_AND_FILE =
            string.Format(@"/{0}", 
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                PDF_USER_MANUAL_DIR_AND_FILE);

        // Container created on demand
        private IStorageManagerFactory _storageFactory = null;

        // Storage members
        private IStorageManager<SettingItems> _settings = null;
        private IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> _terminatorStorage = null;
        private IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> _scriptStorage = null;
        private IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> _wifiCredStorage = null;
        private IIndexedStorageManager<EthernetParams, DefaultFileExtraInfo> _ethernetStorage = null;
        private IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> _serialStorage = null;

        #endregion

        #region Just in Time Storage Creation Properties

        private IStorageManagerFactory storageFactory {
            get {
                if (this._storageFactory == null) {
                    this._storageFactory = this.container.GetObjSingleton<IStorageManagerFactory>();
                }
                return this._storageFactory;
            }
        }


        private IStorageManager<SettingItems> settings {
            get {
                if (this._settings == null) {
                    this._settings =
                        this.storageFactory.GetManager<SettingItems>(this.Dir(this.SETTINGS_DIR), this.SETTINGS_FILE);
                    this.AssureSettingsDefault(this._settings);
                }
                return this._settings;
            }
        }


        private IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> terminatorStorage {
            get {
                if (this._terminatorStorage == null) {
                    this._terminatorStorage =
                        this.storageFactory.GetIndexedManager<TerminatorDataModel, DefaultFileExtraInfo>(this.Dir(TERMINATOR_DIR), TERMINATOR_INDEX_FILE);
                    this.AssureTerminatorsDefault(this._terminatorStorage);
                }
                return this._terminatorStorage;
            }
        }

        private IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> scriptStorage {
            get {
                if (this._scriptStorage == null) {
                    this._scriptStorage =
                        this.storageFactory.GetIndexedManager<ScriptDataModel, DefaultFileExtraInfo>(this.Dir(SCRIPTS_DIR), SCRIPTS_INDEX_FILE);
                    this.AssureScriptDefault(this._scriptStorage);
                }
                return this._scriptStorage;
            }
        }


        private IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> wifiCredStorage {
            get {
                if (this._wifiCredStorage == null) {
                    this._wifiCredStorage =
                        this.storageFactory.GetIndexedManager<WifiCredentialsDataModel, DefaultFileExtraInfo>(this.Dir(WIFI_CRED_DIR), WIFI_CRED_INDEX_FILE);
                    //this.AssureWifiCredDefault(this._wifiCredStorage);
                }
                return this._wifiCredStorage;
            }
        }


        private IIndexedStorageManager<EthernetParams, DefaultFileExtraInfo> ethernetStorage {
            get {
                if (this._ethernetStorage == null) {
                    this._ethernetStorage =
                        this.storageFactory.GetIndexedManager<EthernetParams, DefaultFileExtraInfo>(this.Dir(ETHERNET_DATA_DIR), ETHERNET_DATA_INDEX_FILE);
                }
                return this._ethernetStorage;
            }
        }


        private IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> serialStorage {
            get {
                if (this._serialStorage == null) {
                    this._serialStorage =
                        this.storageFactory.GetIndexedManager<SerialDeviceInfo, SerialIndexExtraInfo>(
                            this.Dir(SERIAL_CFG_DIR), SERIAL_CFG_INDEX_FILE);
                }
                return this._serialStorage;
            }
        }

        #endregion

        #region Public


        public void RebuildAllData() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000301, "Failure on RebuidAllData", () => {
                // If we call the property it will recreate it

                if (this._serialStorage == null) {
                    this._serialStorage =
                        this.storageFactory.GetIndexedManager<SerialDeviceInfo, SerialIndexExtraInfo>(
                            this.Dir(SERIAL_CFG_DIR), SERIAL_CFG_INDEX_FILE);
                }
                this._serialStorage.DeleteStorageDirectory();
                this._serialStorage = null;

                if (this._ethernetStorage == null) {
                    this._ethernetStorage =
                        this.storageFactory.GetIndexedManager<EthernetParams, DefaultFileExtraInfo>(this.Dir(ETHERNET_DATA_DIR), ETHERNET_DATA_INDEX_FILE);
                }
                this._ethernetStorage.DeleteStorageDirectory();
                this._ethernetStorage = null;

                if (this._wifiCredStorage == null) {
                    this._wifiCredStorage =
                        this.storageFactory.GetIndexedManager<WifiCredentialsDataModel, DefaultFileExtraInfo>(this.Dir(WIFI_CRED_DIR), WIFI_CRED_INDEX_FILE);
                }
                this._wifiCredStorage.DeleteStorageDirectory();
                this._wifiCredStorage = null;

                if (this._scriptStorage == null) {
                    this._scriptStorage =
                        this.storageFactory.GetIndexedManager<ScriptDataModel, DefaultFileExtraInfo>(this.Dir(SCRIPTS_DIR), SCRIPTS_INDEX_FILE);
                }
                this._scriptStorage.DeleteStorageDirectory();
                this._scriptStorage = null;

                if (this._terminatorStorage == null) {
                    this._terminatorStorage =
                        this.storageFactory.GetIndexedManager<TerminatorDataModel, DefaultFileExtraInfo>(this.Dir(TERMINATOR_DIR), TERMINATOR_INDEX_FILE);
                }
                this._terminatorStorage.DeleteStorageDirectory();
                this._terminatorStorage = null;

                if (this._settings == null) {
                    this._settings =
                        this.storageFactory.GetManager<SettingItems>(this.Dir(this.SETTINGS_DIR), this.SETTINGS_FILE);
                }
                this._settings.DeleteStorageDirectory();
                this._settings = null;



                // Calling the just in time properties will rebuild the data
                var set = this.settings;
                var ser = this.serialStorage;
                var eth = this.ethernetStorage;
                var wi = this.wifiCredStorage;
                var sc = this.scriptStorage;
                var tem = this.terminatorStorage;
            });
            this.RaiseIfException(report);

        }


        public string GetDataFilesPath() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200030, "Failure on ", () => {
            });
            this.RaiseIfException(report);

            return Path.Combine(this.settings.StorageRootDir, APP_DIR);
        }


        public string UserManualFullFileName { get {
                // Check WIN first
                if (File.Exists(this.WIN_ORIGINE_USER_MANUAL_PATH_AND_FILE)) {
                    this.log.Info("UserManualFullFileName", () => string.Format("Using WIN Path:{0}",
                        this.WIN_ORIGINE_USER_MANUAL_PATH_AND_FILE));
        

                    return this.WIN_ORIGINE_USER_MANUAL_PATH_AND_FILE;
                }
                // UWP path
                this.log.Info("UserManualFullFileName", () => string.Format("Using UWP Path:{0}",
                    this.UWP_ORIGINE_USER_MANUAL_PATH_AND_FILE));
                return this.UWP_ORIGINE_USER_MANUAL_PATH_AND_FILE;
            }
        }

        #endregion

        #region Private

        private string Dir(string subDir) {
            return Path.Combine(APP_DIR, subDir);
        }

        private string FullStorageDirectory(string subDir) {
            // Just use one of the roots
            return Path.Combine(this.scriptStorage.StorageRootDir, this.Dir(subDir));
        }

        #endregion

        #region Assure default methods

        /// <summary>Create default settings if it does not exist</summary>
        private void AssureSettingsDefault(IStorageManager<SettingItems> data) {
            if (!data.DefaultFileExists()) {
                data.WriteObjectToDefaultFile(new SettingItems());
            }
        }


        /// <summary>
        /// If no terminators defined, create default, store and set as default in settings
        /// </summary>
        private void AssureTerminatorsDefault(IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> data) {
            // If nothing exists create default
            List<IIndexItem<DefaultFileExtraInfo>> index = data.IndexedItems;
            if (index.Count == 0) {
                // For a new one. Different when updating. Do not need to create new index
                List<TerminatorInfo> infos = new List<TerminatorInfo>();
                infos.Add(new TerminatorInfo(Terminator.LF));
                infos.Add(new TerminatorInfo(Terminator.CR));
                TerminatorDataModel dm = new TerminatorDataModel(infos) {
                    Name = "Demo terminator set \\n\\r"
                };

                IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(dm.UId) {
                    Display = dm.Name
                };
                data.Store(dm, idx);

                this.GetSettings(
                    (settings) => {
                        settings.CurrentTerminator = dm;
                        settings.CurrentTerminatorBLE = dm;
                        settings.CurrentTerminatorBT = dm;
                        settings.CurrentTerminatorEthernet = dm;
                        settings.CurrentTerminatorUSB = dm;
                        settings.CurrentTerminatorWIFI = dm;
                        this.SaveSettings(settings, () => { }, (err) => { });
                    },
                    (err) => { });

                this.CreateArduinoTerminators(() => { }, (err) => { });

            }
            else {
                // back compatible to add the display name into the data object
                this.BackCompatibilityInitializeExistingTerminatorNames();
            }
        }


        private void AssureScriptDefault(IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> data) {
            List<IIndexItem<DefaultFileExtraInfo>> index = this.scriptStorage.IndexedItems;
            if (index.Count == 0) {
                List<ScriptItem> items = new List<ScriptItem>();
                items.Add(new ScriptItem() { Display = "Open door cmd", Command = "OpenDoor" });
                items.Add(new ScriptItem() { Display = "Close door cmd", Command = "CloseDoor" });
                ScriptDataModel dm = new ScriptDataModel(items) { 
                    Display = "Demo open close commands"
                };
                IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(dm.UId) {
                    Display = "Demo script",
                };
                data.Store(dm, idx);

                this.GetSettings(
                    (settings) => {
                        settings.CurrentScript = dm;
                        settings.CurrentScriptBLE = dm;
                        settings.CurrentScriptBT = dm;
                        settings.CurrentScriptEthernet = dm;
                        settings.CurrentScriptUSB = dm;
                        settings.CurrentScriptWIFI = dm;
                        this.SaveSettings(settings, () => { }, (err) => { });
                    },
                    (err) => { });

                this.CreateHC05AtCmds(() => { }, (err) => { });

            }
        }

        #endregion

        #region Storage generics

        private void DeleteFromStorage<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, IIndexItem<TExtraInfo> indexItem, Action<bool> onComplete, OnErr onError)
            where TSToreObject : class where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    bool ok = manager.DeleteFile(indexItem);
                    onComplete(ok);
                });
                if (report.Code != 0) {
                    // TODO - add language - delete failed
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        #endregion

    }
}
