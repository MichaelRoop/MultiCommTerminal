using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.DataModels {

    public class BLE_PropertiesUpdateDataModel {

        public string Id { get; set; } = "";
        public Dictionary<string, BLE_PropertyDataModel> ServiceProperties { get; set; } = new Dictionary<string, BLE_PropertyDataModel>();
        public string Kind { get; set; } = "";





    }
}
