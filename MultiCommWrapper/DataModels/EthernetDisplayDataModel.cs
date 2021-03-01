using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;

namespace MultiCommWrapper.Net.DataModels {

    /// <summary>
    /// Assemble info to display Ethernet params in list without loading each one
    /// </summary>
    public class EthernetDisplayDataModel {

        /// <summary>Storage index</summary>
        public IIndexItem<DefaultFileExtraInfo> Index { get; set; } = null;

        public string Name { get; set; } = string.Empty;

        /// <summary>Socket host name or IP</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Socket port such as 80 for HTTP</summary>
        public string Port { get; set; } = string.Empty;

    }

}
