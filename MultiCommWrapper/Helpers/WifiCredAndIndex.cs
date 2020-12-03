using MultiCommData.Net.StorageDataModels;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;

namespace MultiCommWrapper.Net.Helpers {

    /// <summary>Group the WifiCredentialsDataModel storage data with its index</summary>
    public class WifiCredAndIndex {

        public IIndexItem<DefaultFileExtraInfo> Index { get; set; } = null;

        public WifiCredentialsDataModel Data { get; set; } = new WifiCredentialsDataModel();

        /// <summary>Indicates if the data model requires new values</summary>
        public bool RequiresUserData { get; set; } = false;


    }

}
