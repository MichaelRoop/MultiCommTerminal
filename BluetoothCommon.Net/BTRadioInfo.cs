
using System.Collections.Generic;

namespace BluetoothCommon.Net {

    /// <summary>To get information about the BT Classic radio</summary>
    public class BTRadioInfo {
        public string Manufacturer { get; set; } = "N/A";
        public string LinkManagerProtocol { get; set; } = "N/A";
        public List<string> Features { get; set; } = new List<string>();

    }
}

