using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using StorageFactory.Net.interfaces;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    public class EthernetSelectResult {
        public IIndexItem<EthernetExtraInfo> Index { get; set; }
        public EthernetParams DataModel { get; set; }
    }
}
