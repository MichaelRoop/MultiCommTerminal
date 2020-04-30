using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.DataModels {

    /// <summary>Cross platform data model for essential BLE Service info</summary>
    public class BLE_ServiceDataModel {

        public ushort AttributeHandle { get; set; }

        /// <summary>
        /// TODO Follow up: Gatt service instance path used to instantiate
        /// the GattDeviceService
        /// see: https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.genericattributeprofile.gattdeviceservice?view=winrt-18362
        /// </summary>
        public string DeviceId { get; set; }


        /// <summary>Get from the Uuid through the enumeration helpers</summary>
        public string DisplayName { get; set; }

        /// <summary>List of Gatt characteristics which include read/write sources</summary>
        public List<BLE_CharacteristicDataModel> Characteristics { get; set; }


        /// <summary>Gatt Service UUID</summary>
        public Guid Uuid { get; set; }

        // Other info from MS GattSession
        // BluetoothLEDevice - the parent object which has the services
        // Parent services (List)
        // DeviceAccessInformation
        //    DeviceAccessStatus unspecified (Unspecified, Allowed, DeniedByUser, DeniedBySystem)
        // GattSession
        // GattSharingMode (Unspecified, Exclusive, SharedReadOnly, SharedReadAndWrite)



    }
}
