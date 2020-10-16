using LogUtils.Net;
using System;
using System.Collections.Generic;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;

namespace Wifi.UWP.Core {

    public partial class WifiImpleUwp : IWifiInterface {

        #region Data

        ClassLog log = new ClassLog("WifiImpleUwp");

        #endregion


        public event EventHandler<List<WifiAdapterInfo>> DiscoveredAdapters;

    }
}
