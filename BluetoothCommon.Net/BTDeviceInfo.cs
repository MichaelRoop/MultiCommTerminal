using System;

namespace BluetoothCommon.Net {

    /// <summary>Has information on one Bluetooth device</summary>
    public class BTDeviceInfo {

        /// <summary>Authenticated state</summary>
        public bool Authenticated { get; set; }


        /// <summary>Connected state</summary>
        public bool Connected { get; set; }

        // Remembered
        // Installed services List<Guid>
        // Service records
        // Service records unparsed

        /// <summary>CLS compliant class</summary>
        public uint DeviceClassInt { get; set; }

        public string DeviceClassName { get; set; }

        /// <summary>Address</summary>
        public string Address { get; set; }

        /// <summary>Name</summary>
        public string Name { get; set; }

        /// <summary>RSSI signal strength</summary>
        public int Strength { get; set; }

        public int ServiceClassInt { get; set; }
        public string ServiceClassName { get; set; }

        public DateTime LastSeen { get; set; }
        public DateTime LastUsed { get; set; }

        public BTRadioInfo Radio { get; set; }


        public BTDeviceInfo() {
            this.Authenticated = false;
            this.Connected = false;
            this.DeviceClassInt = 0;
            this.DeviceClassName = "Unknown";
            this.Address = "Unknown";
            this.Name = "Unknown";
            this.Strength = 0;
            this.ServiceClassInt = 0;
            this.ServiceClassName = "Unknown";
            this.LastSeen = DateTime.Now;
            this.LastUsed = DateTime.Now;
            this.Radio = new BTRadioInfo();

        }

    }


}

