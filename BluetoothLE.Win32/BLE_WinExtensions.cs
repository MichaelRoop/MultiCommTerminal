using Windows.Storage.Streams;

namespace BluetoothLE.Win32 {

    /// <summary>Windows specific extensions for helper</summary>
    public static class BLE_WinExtensions {

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

    }
}
