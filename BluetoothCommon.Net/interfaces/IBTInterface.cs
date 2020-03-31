using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothCommon.Net.interfaces {
    public interface IBTInterface {

        event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        event EventHandler<bool> DiscoveryComplete;
        event EventHandler<bool> ConnectionCompleted;
        event EventHandler<byte[]> BytesReceived;


        /// <summary>Get a list of Bluetooth devices</summary>
        /// <returns>A list of info on the devices discovered</returns>
        List<BTDeviceInfo> DiscoverDevices();

        void Connect(BTDeviceInfo device);
        void Disconnect();
    }
}
