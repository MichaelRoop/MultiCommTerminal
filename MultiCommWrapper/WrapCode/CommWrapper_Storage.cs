﻿using BluetoothLE.Net.Enumerations;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;

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
        private readonly string BLE_CMD_DIR = "BleCommands";
        private readonly string BLE_CMD_INDEX_FILE = "BleCommandsIndex.txt";
        private readonly string WIFI_CRED_DIR = "WifiCredentials";
        private readonly string WIFI_CRED_INDEX_FILE = "WifiCredIndex.txt";
        private readonly string ETHERNET_DATA_DIR = "EthernetCfgData";
        private readonly string ETHERNET_DATA_INDEX_FILE = "EthernetDataIndex.txt";
        private readonly string SERIAL_CFG_DIR = "SerialConfigurations";
        private readonly string SERIAL_CFG_INDEX_FILE = "SerialCfgIndex.txt";


        // UWP path and file name for document
        private readonly string UWP_ORIGINE_USER_MANUAL_PATH_AND_FILE = string.Format("{0}{1}",             
            AppDomain.CurrentDomain.BaseDirectory, PDF_USER_MANUAL_DIR_AND_FILE);

        // Windows path and file name for document
        private readonly string WIN_ORIGINE_USER_MANUAL_PATH_AND_FILE =
            string.Format(@"{0}\{1}", 
                Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), 
                PDF_USER_MANUAL_DIR_AND_FILE);

        // Container created on demand
        private IStorageManagerFactory _storageFactory = null;

        // Storage members
        private IStorageManager<SettingItems> _settings = null;
        private IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> _terminatorStorage = null;
        private IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> _scriptStorage = null;
        private IIndexedStorageManager<BLECommandSetDataModel, BLECmdIndexExtraInfo> _bleCmdStorage = null;
        private IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> _wifiCredStorage = null;
        private IIndexedStorageManager<EthernetParams, EthernetExtraInfo> _ethernetStorage = null;
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

        private IIndexedStorageManager<BLECommandSetDataModel, BLECmdIndexExtraInfo> bleCmdStorage {
            get {
                if (this._bleCmdStorage == null) {
                    this._bleCmdStorage =
                        this.storageFactory.GetIndexedManager<BLECommandSetDataModel, BLECmdIndexExtraInfo>(this.Dir(BLE_CMD_DIR), BLE_CMD_INDEX_FILE);
                    this.AssureBLECmdsDefault(this._bleCmdStorage);
                }
                return this._bleCmdStorage;
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


        private IIndexedStorageManager<EthernetParams, EthernetExtraInfo> ethernetStorage {
            get {
                if (this._ethernetStorage == null) {
                    this._ethernetStorage =
                        this.storageFactory.GetIndexedManager<EthernetParams, EthernetExtraInfo>(this.Dir(ETHERNET_DATA_DIR), ETHERNET_DATA_INDEX_FILE);
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
                        this.storageFactory.GetIndexedManager<EthernetParams, EthernetExtraInfo>(this.Dir(ETHERNET_DATA_DIR), ETHERNET_DATA_INDEX_FILE);
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

                if (this._bleCmdStorage == null) {
                    this._bleCmdStorage =
                        this.storageFactory.GetIndexedManager<BLECommandSetDataModel, BLECmdIndexExtraInfo> (this.Dir(BLE_CMD_DIR), BLE_CMD_INDEX_FILE);
                }
                this._bleCmdStorage.DeleteStorageDirectory();
                this._bleCmdStorage = null;

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
                var cmd = this.bleCmdStorage;
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
                    Display = "Demo terminator set \\n\\r"
                };

                IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(dm.UId) {
                    Display = dm.Display
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


        private void AssureBLECmdsDefault(IIndexedStorageManager<BLECommandSetDataModel, BLECmdIndexExtraInfo> manager) {
            List<IIndexItem<BLECmdIndexExtraInfo>> index = manager.IndexedItems;
            if (index.Count == 0) {
                this.CreateBLEDemoCmdsBool(() => { }, err => { });
                this.CreateBLEDemoCmdsUint8(() => { }, err => { });
                this.CreateBLEDemoCmdsUint16(() => { }, err => { });
                this.CreateBLEDemoCmdsUint32(() => { }, err => { });
            }
        }

        #endregion

        #region Generic Delete

        private void DeleteFromStorage<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, IIndexItem<TExtraInfo> indexItem, Action<bool> onComplete, OnErr onError)
            where TSToreObject : class where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (indexItem == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else {
                        bool ok = manager.DeleteFile(indexItem);
                        onComplete(ok);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.DeleteFailure));
                }
            });
        }


        private void DeleteFromStorage<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, 
            IIndexItem<TExtraInfo> indexItem,
            string msg, 
            Func<string, bool> areYouSure,
            Action<bool> onComplete, 
            OnErr onError)
            where TSToreObject : class where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (indexItem == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else {
                        if (areYouSure(msg)) {
                            bool ok = manager.DeleteFile(indexItem);
                            onComplete(ok);
                        }
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.DeleteFailure));
                }
            });
        }


        private void DeleteFromStorage<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            TSToreObject data,
            Func<string, bool> areYouSure,
            Action<bool> onComplete,
            OnErr onError)
            where TSToreObject : class, IDisplayableData, IIndexible  where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    this.RetrievelIndexedItem(manager, data,
                        (ndx) => {
                            if (areYouSure(data.Display)) {
                                bool ok = manager.DeleteFile(ndx);
                                onComplete(ok);
                            }
                        }, onError);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.DeleteFailure));
                }
            });
        }


        private void DeleteFromStorageNotLast<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, IIndexItem<TExtraInfo> indexItem, Action<bool> onComplete, OnErr onError)
            where TSToreObject : class where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (indexItem == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else if (manager.IndexedItems.Count < 2) {
                        onError(this.GetText(MsgCode.CannotDeleteLast));
                    }
                    else {
                        bool ok = manager.DeleteFile(indexItem);
                        onComplete(ok);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.DeleteFailure));
                }
            });
        }


        private void DeleteFromStorageNotLast<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, 
            IIndexItem<TExtraInfo> indexItem,
            string msg, 
            Func<string, bool> areYouSure,
            Action<bool> onComplete, 
            OnErr onError)
            where TSToreObject : class where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (indexItem == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else if (manager.IndexedItems.Count < 2) {
                        onError(this.GetText(MsgCode.CannotDeleteLast));
                    }
                    else {
                        if (areYouSure(msg)) {
                            bool ok = manager.DeleteFile(indexItem);
                            onComplete(ok);
                        }
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.DeleteFailure));
                }
            });
        }


        private void DeleteAllFromStorage<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, Action onSuccess, OnErr onError)
            where TSToreObject : class where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (manager.DeleteStorageDirectory()) {
                        onSuccess.Invoke();
                    }
                    else {
                        onError.Invoke(this.GetText(MsgCode.DeleteFailure));
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.UnknownError));
                }
            });
        }


        private void DeleteAllFilesFromStorage<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, Action onSuccess, OnErr onError)
            where TSToreObject : class where TExtraInfo : class {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (manager.DeleteAllFiles()) {
                        onSuccess.Invoke();
                    }
                    else {
                        onError.Invoke(this.GetText(MsgCode.DeleteFailure));
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.UnknownError));
                }
            });
        }




        #endregion

        #region Generic Retrieve

        private void RetrieveIndex<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager, 
            Action<List<IIndexItem<TExtraInfo>>> onSuccess, OnErr onError)
            where TSToreObject : class where TExtraInfo : class {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(manager.IndexedItems);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }



        private void RetrieveItem<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            IIndexItem<TExtraInfo> index,
            Action<TSToreObject> onSuccess, 
            OnErr onError)
            where TSToreObject : class where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (index == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else {
                        if (manager.FileExists(index)) {
                            TSToreObject item = manager.Retrieve(index);
                            if (item != null) {
                                onSuccess.Invoke(item);
                            }
                            else {
                                onError.Invoke(this.GetText(MsgCode.NotFound));
                            }
                        }
                        else {
                            onError.Invoke(this.GetText(MsgCode.NotFound));
                        }
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        #endregion

        #region Generic Create

        private void Create<TSToreObject, TExtraInfo>(
            string display,
            TSToreObject data,
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            Action<IIndexItem<TExtraInfo>> onSuccess,
            OnErr onError, TExtraInfo extraInfo = null)
            where TSToreObject : class, IDisplayableData, IIndexible where TExtraInfo : class, new() {

            this.Create(display, data, manager, onSuccess, (d) => { }, onError, extraInfo);
        }


        private void Create<TSToreObject, TExtraInfo>(
            string display,
            TSToreObject data,
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            Action<IIndexItem<TExtraInfo>> onSuccess,
            Action<TSToreObject> onChange,
            OnErr onError, TExtraInfo extraInfo = null)
            where TSToreObject : class, IDisplayableData, IIndexible where TExtraInfo : class, new() {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        IIndexItem<TExtraInfo> idx = (extraInfo == null)
                            ? new IndexItem<TExtraInfo>(data.UId)
                            : new IndexItem<TExtraInfo>(data.UId, extraInfo);
                        idx.Display = display;
                        this.Save(manager, idx, data, (obj, idx) => { }, () => onSuccess(idx), onChange, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.UnknownError));
                }
            });
        }

        #endregion

        #region Generic Save

        private void Save<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            IIndexItem<TExtraInfo> idx,
            TSToreObject data,
            Action<TSToreObject, IIndexItem<TExtraInfo>> preSaveIndexUpdate,
            Action onSuccess,
            OnErr onError) 
            where TSToreObject : class, IDisplayableData where TExtraInfo : class {

            this.Save(manager, idx, data, preSaveIndexUpdate, onSuccess, (d) => { }, onError);
        }


        private void Save<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            IIndexItem<TExtraInfo> idx,
            TSToreObject data,
            Action<TSToreObject, IIndexItem<TExtraInfo>> preSaveIndexUpdate,
            Action onSuccess, 
            Action<TSToreObject> onChange,
            OnErr onError) 
            where TSToreObject : class, IDisplayableData where TExtraInfo : class {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (idx == null) {
                        onError.Invoke(this.GetText(MsgCode.NothingSelected));
                    }
                    else if (string.IsNullOrWhiteSpace(data.Display)) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        // Transfer display name
                        idx.Display = data.Display;
                        // update index
                        preSaveIndexUpdate(data, idx);
                        manager.Store(data, idx);
                        onSuccess.Invoke();
                        onChange.Invoke(data);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }

        #endregion

        #region Generic Save or Create

        private void SaveOrCreate<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            string display,
            TSToreObject data,
            Action<TSToreObject, IIndexItem<TExtraInfo>> preSaveIndexUpdate,
            Action<IIndexItem<TExtraInfo>> onSuccess,
            OnErr onError,
            TExtraInfo extraInfo = null)
            where TSToreObject : class, IDisplayableData, IIndexible where TExtraInfo : class, new() {

            this.SaveOrCreate(manager, display, data, preSaveIndexUpdate, onSuccess, (d) => { }, onError, extraInfo);
        }



        private void SaveOrCreate<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            string display,
            TSToreObject data,
            Action<TSToreObject, IIndexItem<TExtraInfo>> preSaveIndexUpdate,
            Action<IIndexItem<TExtraInfo>> onSuccess,
            Action<TSToreObject> onChange,
            OnErr onError, 
            TExtraInfo extraInfo = null)
            where TSToreObject : class, IDisplayableData, IIndexible where TExtraInfo : class, new() {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    this.RetrievelIndexedItem(manager, data,
                        (idx) => {
                            // Found. Save
                            this.Save(manager, idx, data, preSaveIndexUpdate, () => onSuccess(idx), onChange, onError);
                        },
                        () => {
                            // Not found. Create
                            this.Create(display, data, manager, onSuccess, onChange, onError, extraInfo);
                        }, onError);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        private void RetrievelIndexedItem<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            TSToreObject inObject,
            Action<IIndexItem<TExtraInfo>> found,
            Action notFound,
            OnErr onError)
            where TSToreObject : class, IDisplayableData, IIndexible where TExtraInfo : class {

            this.RetrieveIndex(manager,
                (idx) => {
                    foreach (IIndexItem<TExtraInfo> item in idx) {
                        if (item.UId_Object == inObject.UId) {
                            found.Invoke(item);
                            this.log.Info("RetrievelIndexedItem", "Found the object index");
                            return;
                        }
                    }
                    notFound.Invoke();
                }, onError);
        }


        /// <summary>Get the index with no event if not found</summary>
        /// <typeparam name="TSToreObject"></typeparam>
        /// <typeparam name="TExtraInfo"></typeparam>
        /// <param name="manager"></param>
        /// <param name="inObject"></param>
        /// <param name="found"></param>
        /// <param name="onError"></param>
        private void RetrievelIndexedItem<TSToreObject, TExtraInfo>(
            IIndexedStorageManager<TSToreObject, TExtraInfo> manager,
            TSToreObject inObject,
            Action<IIndexItem<TExtraInfo>> found,
            OnErr onError)
            where TSToreObject : class, IDisplayableData, IIndexible where TExtraInfo : class {
            this.RetrievelIndexedItem(manager, inObject, found, () => { }, onError);
        }




        #endregion

    }
}
