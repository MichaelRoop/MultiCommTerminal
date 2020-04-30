using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.DataModels {
    
    /// <summary>Cross platform holder of BLE Characteristic information</summary>
    public class BLE_CharacteristicDataModel {

        /// <summary>Unique identifier given by device</summary>
        public ushort AttributeHandle { get; set; }

        /// <summary>Unique identifier</summary>
        public Guid Uuid { get; set; }

        /// <summary>User friendly description or empty</summary>
        public string UserDescription { get; set; }




        /// <summary>The service that holds this characteristic</summary>
        public BLE_ServiceDataModel Service { get; set; }







    }
}
