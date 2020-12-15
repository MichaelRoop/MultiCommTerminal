using Ethernet.Common.Net.DataModels;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    public class EthernetSelectResult {
        public IIndexItem<DefaultFileExtraInfo> Index { get; set; }
        public EthernetParams DataModel { get; set; }
    }
}
