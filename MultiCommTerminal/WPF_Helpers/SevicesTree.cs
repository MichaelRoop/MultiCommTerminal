using BluetoothLE.Net.DataModels;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.WPF_Helpers {

    /// <summary>
    /// Provides a data type to use in XAML. Just pass in the Device Services Dictionary.Values</summary>
    public class ServicesTree : List<BLE_ServiceDataModel> {
    }


    /// <summary>Temporary to simulate the Device Services dictionary</summary>
    public class ServiceTreeDict : Dictionary<string, BLE_ServiceDataModel> { }


}
