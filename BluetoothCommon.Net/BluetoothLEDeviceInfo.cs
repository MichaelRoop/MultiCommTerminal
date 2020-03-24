using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothCommon.Net {

    /// <summary>Information particular to Bluetooth LE devices</summary>
    public class BluetoothLEDeviceInfo {

        /// <summary>Name</summary>
        public string Name { get; set; }

        /// <summary>Id or Address</summary>
        public string Id { get; set; }

        public bool IsDefault { get; set; }

        public bool IsEnabled { get; set; }

        public bool CanPair { get; set; }

        public bool IsPaired { get; set; }

    }
}
