﻿using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public void WifiDiscoverAsync() {
            this.wifi.DiscoverWifiAdaptersAsync();
        }

    }
}
