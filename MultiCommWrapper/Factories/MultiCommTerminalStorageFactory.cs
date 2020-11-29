using ChkUtils.Net;
using Ethernet.Common.Net.DataModels;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using StorageFactory.Net.interfaces;

namespace MultiCommWrapper.Net.Factories {

    /// <summary>Store data in files</summary>
    public class MultiCommTerminalStorageFactory : IStorageManagerFactory {

        private IStorageManagerSet set = null;


        public MultiCommTerminalStorageFactory(IStorageManagerSet managers) {
            this.set = managers;
        }


        public IStorageManager<T> GetManager<T>() where T : class {
            if (typeof(T).Name == typeof(SettingItems).Name) {
                return this.set.Settings as IStorageManager<T>;
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
                return this.set.Terminators as IIndexedStorageManager<TData,TIndexExtraInfo>;
            }
            else if (typeof(TData).Name == typeof(ScriptDataModel).Name) {
                return this.set.Scripts as IIndexedStorageManager<TData, TIndexExtraInfo>;
            }
            else if (typeof(TData).Name == typeof(WifiCredentialsDataModel).Name) {
                return this.set.WifiCred as IIndexedStorageManager<TData, TIndexExtraInfo>;
            }
            else if (typeof(TData).Name == typeof(SerialDeviceInfo).Name) {
                return this.set.Serial as IIndexedStorageManager<TData, TIndexExtraInfo>;
            }
            else if (typeof(TData).Name == typeof(EthernetParams).Name) {
                return this.set.Ethernet as IIndexedStorageManager<TData, TIndexExtraInfo>;
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
