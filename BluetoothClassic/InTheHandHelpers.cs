using InTheHand.Net.Bluetooth;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothClassic.Net {
    public static class InTheHandHelpers {

        public static string LmpVerToString(this LmpVersion version) {
            switch (version) {
                case LmpVersion.v1_0_b: return "Bluetooth Core Specification 1.0b";
                case LmpVersion.v1_1: return "Bluetooth Core Specification 1.1";
                case LmpVersion.v1_2: return "Bluetooth Core Specification 1.2";
                case LmpVersion.v2_0wEdr: return "Bluetooth Core Specification 2.0 + EDR";
                case LmpVersion.v2_1wEdr: return "Bluetooth Core Specification 2.1 + EDR";
                case LmpVersion.v3_0wHS: return "Bluetooth Core Specification 3.0 + HS";
                case LmpVersion.v4_0: return "Bluetooth Core Specification 4.0";
                case LmpVersion.Unknown:
                default:
                    return "Version Unknown";
            }
        }

    }
}
