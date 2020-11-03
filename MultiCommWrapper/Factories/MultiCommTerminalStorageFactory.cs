using ChkUtils.Net;
using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.Serializers;
using StorageFactory.Net.StorageManagers;

namespace MultiCommWrapper.Net.Factories {

    public class MultiCommTerminalStorageFactory : IStorageManagerFactory {

        /// <summary>Singleton of settings manager</summary>
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

        /// <summary>Encrypted storage for the WIFI credentials</summary>
        private IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> wifiCredStorage =
            new IndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo>(
                new EncryptingReadWriteSerializer<WifiCredentialsDataModel>(),
                new EncryptingReadWriteSerializer<IIndexGroup<DefaultFileExtraInfo>>());

        private IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> serialStorage =
            new IndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo>(
                new JsonReadWriteSerializerIndented<SerialDeviceInfo>(),
                new JsonReadWriteSerializerIndented<IIndexGroup<SerialIndexExtraInfo>>());


        public MultiCommTerminalStorageFactory() { }


        public IStorageManager<T> GetManager<T>() where T : class {
            if (typeof(T).Name == typeof(SettingItems).Name) {
                return this.settingStorage as IStorageManager<T>;
            }
            //else if(typeof(T).Name == typeof(TerminatorData).Name) {
            //}

            WrapErr.ChkTrue(false, 9999, () => string.Format("No storage manager for type {0}", typeof(T).Name));
            return null;
        }


        public IStorageManager<T> GetManager<T>(string subDirectory) where T : class {
            IStorageManager<T> manager = this.GetManager<T>();
            manager.StorageSubDir = subDirectory;
            return manager;
        }

        public IStorageManager<T> GetManager<T>(string subDirectory, string defaultFileName) where T : class {
            IStorageManager<T> manager = this.GetManager<T>(subDirectory);
            manager.DefaultFileName = defaultFileName;
            return manager;
        }



        /// <summary>
        /// Retrieved the storage manager for a class TData object which has an 
        /// index that contains a TIndexExtraInfo in the index object 
        /// </summary>
        /// <typeparam name="TData">The type to store and retrieve</typeparam>
        /// <typeparam name="TIndexExtraInfo">The extra info in the index</typeparam>
        /// <param name="subDirectory">The subdirectory off the root</param>
        /// <returns>The indexed storage manager for the type TData class with TIndexExtraInfo index extra info</returns>
        public IIndexedStorageManager<TData, TIndexExtraInfo> GetIndexedManager<TData, TIndexExtraInfo>()
            where TData : class where TIndexExtraInfo : class {

            if (typeof(TData).Name == typeof(TerminatorDataModel).Name) {
                return this.terminatorStorage as IIndexedStorageManager<TData,TIndexExtraInfo>;
            }
            else if (typeof(TData).Name == typeof(ScriptDataModel).Name) {
                return this.scriptStorage as IIndexedStorageManager<TData, TIndexExtraInfo>;
            }
            else if (typeof(TData).Name == typeof(WifiCredentialsDataModel).Name) {
                return this.wifiCredStorage as IIndexedStorageManager<TData, TIndexExtraInfo>;
            }
            else if (typeof(TData).Name == typeof(SerialDeviceInfo).Name) {
                return this.serialStorage as IIndexedStorageManager<TData, TIndexExtraInfo>;
            }
            // Add others

            return null;
        }


        /// <summary>
        /// Retrieved the storage manager for a class TData object which has an 
        /// index that contains a TIndexExtraInfo in the index object 
        /// </summary>
        /// <typeparam name="TData">The type to store and retrieve</typeparam>
        /// <typeparam name="TIndexExtraInfo">The extra info in the index</typeparam>
        /// <param name="subDirectory">The subdirectory off the root</param>
        /// <returns>The indexed storage manager for the type TData class with TIndexExtraInfo index extra info</returns>
        public IIndexedStorageManager<TData, TIndexExtraInfo> GetIndexedManager<TData, TIndexExtraInfo>(string subDirectory) 
            where TData : class where TIndexExtraInfo : class {
            IIndexedStorageManager<TData, TIndexExtraInfo> manager = this.GetIndexedManager<TData, TIndexExtraInfo>();
            manager.StorageSubDir = subDirectory;
            return manager;
        }


        public IIndexedStorageManager<TData, TIndexExtraInfo> GetIndexedManager<TData, TIndexExtraInfo>(string subDirectory, string indexName)
            where TData : class where TIndexExtraInfo : class {
            IIndexedStorageManager<TData, TIndexExtraInfo> manager = this.GetIndexedManager<TData, TIndexExtraInfo>(subDirectory);
            manager.IndexFileName = indexName;
            return manager;
        }



    }
}
