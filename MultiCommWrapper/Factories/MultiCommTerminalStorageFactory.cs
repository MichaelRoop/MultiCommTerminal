using ChkUtils.Net;
using MultiCommData.Net.StorageDataModels;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.Serializers;
using StorageFactory.Net.StorageManagers;

namespace MultiCommWrapper.Net.Factories {

    public class MultiCommTerminalStorageFactory : IStorageManagerFactory {

        /// <summary>Singleton of settings manager</summary>
        IStorageManager<SettingItems> settingStorage = 
            new SimpleStorageManger<SettingItems>(new JsonReadWriteSerializerIndented<SettingItems>());


        public IStorageManager<T> GetManager<T>() where T : class {
            if (typeof(T).Name == typeof(SettingItems).Name) {
                return this.settingStorage as IStorageManager<T>;
            }
            //else if() {

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

    }
}
