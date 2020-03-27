using System;

namespace BluetoothCommon.Net.interfaces {

    /// <summary>Interface for Bluetooth LE devices</summary>
    public interface IBLETInterface {

        /// <summary>Event raised when a device is dropped from OS</summary>
        event EventHandler<string> DeviceRemoved;


        /// <summary>Event raised when a device is discovered</summary>
        event EventHandler<BluetoothLEDeviceInfo> DeviceDiscovered;


        /// <summary>Start or restart the device discovery</summary>
        void DiscoverDevices();

        void Connect(BluetoothLEDeviceInfo deviceInfo);

    }
}
