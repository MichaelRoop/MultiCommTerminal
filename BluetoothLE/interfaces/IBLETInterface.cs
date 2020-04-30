using BluetoothLE.Net.DataModels;
using CommunicationStack.Net.interfaces;
using System;

namespace BluetoothLE.Net.interfaces {

    /// <summary>Interface for Bluetooth LE devices</summary>
    public interface IBLETInterface : ICommStackChannel {

        /// <summary>Event raised when a device is dropped from OS</summary>
        event EventHandler<string> DeviceRemoved;

        /// <summary>Event raised when a device is discovered</summary>
        event EventHandler<BluetoothLEDeviceInfo> DeviceDiscovered;

        /// <summary>Will post this at end of enumeration since early adds are not necessarly complete</summary>
        event EventHandler<bool> DeviceDiscoveryCompleted;

        /// <summary>Raised when BLE updates a device properties</summary>
        event EventHandler<BLE_PropertiesUpdateDataModel> DeviceUpdated;

        /// <summary>Start or restart the device discovery</summary>
        void DiscoverDevices();

        void Connect(BluetoothLEDeviceInfo deviceInfo);


        void Disconnect();

    }
}
