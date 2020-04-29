using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothLE.Win32 {

    /// <summary>Windows specific extensions for helper</summary>
    public static class BLE_WinExtensions {

        private static string IS_CONNECTED_KEY = "System.Devices.Aep.IsConnected";
        private static string IS_CONNECTABLE_KEY = "System.Devices.Aep.IsConnectable";


        /// <summary>Convert from Windows IBuffer to properly sized byte array</summary>
        /// <param name="buffer">The buffer containing data</param>
        /// <returns>Byte array</returns>
        public static byte[] FromBufferToBytes(this IBuffer buffer) {
            uint dataLength = buffer.Length;
            byte[] data = new byte[dataLength];
            using (DataReader reader = DataReader.FromBuffer(buffer)) {
                reader.ReadBytes(data);
            }
            return data;
        }


        /// <summary>Uses the Win BLE DeviceInformation Properties with Connectable key</summary>
        /// <param name="info">The Win device information object</param>
        /// <returns>true if connectable, otherwise false</returns>
        public static bool IsConnectable(this DeviceInformation info) {
            if (info.HasProperty(IS_CONNECTABLE_KEY)) {
                return (bool?)info.Properties[IS_CONNECTABLE_KEY] == true;
            }
            return false;
        }


        /// <summary>Uses the Win BLE DeviceInformation Properties with Connected key</summary>
        /// <param name="info">The Win device information object</param>
        /// <returns>true if connected, otherwise false</returns>
        public static bool IsConnected(this DeviceInformation info) {
            if (info.HasProperty(IS_CONNECTED_KEY)) {
                return (bool?)info.Properties[IS_CONNECTED_KEY] == true;
            }
            return false;
        }


        private static bool HasProperty(this DeviceInformation info, string key) {
            if (info.Properties != null) {
                return info.Properties.ContainsKey(key);
            }
            return false;
        }




    }
}
