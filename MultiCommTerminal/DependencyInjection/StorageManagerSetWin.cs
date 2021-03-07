using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.Serializers;
using StorageFactory.Net.StorageManagers;

namespace MultiCommTerminal.NetCore.DependencyInjection {

    public class StorageManagerSetWin : IStorageManagerSet {

        #region Data

        private IStorageManager<SettingItems> settingStorage =
            new SimpleStorageManger<SettingItems>(new JsonReadWriteSerializerIndented<SettingItems>());


        /// <summary>Singleton terminator indexed storage</summary>
        private IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> terminatorStorage =
            new IndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo>(
                new JsonReadWriteSerializerIndented<TerminatorDataModel>(),
                new JsonReadWriteSerializerIndented<IIndexGroup<DefaultFileExtraInfo>>());

        private IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> scriptStorage =
            new IndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo>(
                new JsonReadWriteSerializerIndented<ScriptDataModel>(),
                new JsonReadWriteSerializerIndented<IIndexGroup<DefaultFileExtraInfo>>());

        private IIndexedStorageManager<BLECommandSetDataModel, BLECmdIndexExtraInfo> bleCommandsStorage =
            new IndexedStorageManager<BLECommandSetDataModel, BLECmdIndexExtraInfo>(
                new JsonReadWriteSerializerIndented<BLECommandSetDataModel>(),
                new JsonReadWriteSerializerIndented<IIndexGroup<BLECmdIndexExtraInfo>>());


        /// <summary>Encrypted storage for the WIFI credentials</summary>
        private IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> wifiCredStorage =
            new IndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo>(
                new EncryptingReadWriteSerializer<WifiCredentialsDataModel>(),
                new EncryptingReadWriteSerializer<IIndexGroup<DefaultFileExtraInfo>>());


        private IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> serialStorage =
            new IndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo>(
                new JsonReadWriteSerializerIndented<SerialDeviceInfo>(),
                new JsonReadWriteSerializerIndented<IIndexGroup<SerialIndexExtraInfo>>());

        private IIndexedStorageManager<EthernetParams, EthernetExtraInfo> ethernetStorage =
            new IndexedStorageManager<EthernetParams, EthernetExtraInfo>(
                new JsonReadWriteSerializerIndented<EthernetParams>(),
                new JsonReadWriteSerializerIndented<IIndexGroup<EthernetExtraInfo>>());


        #endregion

        #region Properties

        public IStorageManager<SettingItems> Settings { 
            get { return this.settingStorage; } 
        }

        public IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> Terminators { 
            get { return this.terminatorStorage; } 
        }

        public IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> Scripts {
            get { return this.scriptStorage; }
        }

        public IIndexedStorageManager<BLECommandSetDataModel, BLECmdIndexExtraInfo> BLECommands {
            get { return this.bleCommandsStorage; } 
        }

        public IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> WifiCred { 
            get { return this.wifiCredStorage; } 
        }

        public IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> Serial { 
            get { return this.serialStorage; } 
        }

        public IIndexedStorageManager<EthernetParams, EthernetExtraInfo> Ethernet { 
            get { return this.ethernetStorage; } 
        }

        #endregion

    }
}
