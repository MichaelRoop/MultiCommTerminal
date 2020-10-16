using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;
using WifiCommon.Net.Enumerations;
using Windows.Devices.WiFi;

namespace Wifi.UWP.Core.Helpers {

    public static class EnumerationHelpers {

        public static NetworkKind Convert(this WiFiNetworkKind kind) {
            return EnumHelpers.ToEnum<NetworkKind>(
                EnumHelpers.ToInt(kind));
        }

    }
}
