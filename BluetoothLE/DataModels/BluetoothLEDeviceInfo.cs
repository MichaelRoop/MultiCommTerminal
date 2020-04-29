using System;
using System.Collections.Generic;

namespace BluetoothLE.Net.DataModels {

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

        public string Kind { get; set; }

        public List<Tuple<string,string>> LEProperties { get; set; }


        /// <summary>OS 
        /// specific implementations require a specific object 
        /// returned from discovery to make a connection
        /// </summary>
        public object OSSpecificObj { get; set; }  


        public BluetoothLEDeviceInfo() {
            this.LEProperties = new List<Tuple<string,string>>();
        }


    }
}
