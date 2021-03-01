using Ethernet.Common.Net.DataModels;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    public class EthernetSelectResult {
        public IIndexItem<DefaultFileExtraInfo> Index { get; set; }
        public EthernetParams DataModel { get; set; }
    }
}
