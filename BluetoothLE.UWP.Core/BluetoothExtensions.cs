using BluetoothLE.Net.Enumerations;
using Common.Net.Network;
using LogUtils.Net;
using System.Collections.Generic;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace Bluetooth.UWP.Core {

    /// <summary>Windows specific extensions for helper</summary>
    public static class BluetoothExtensions {

        #region Data

        private static ClassLog log = new ClassLog("BLE_WinExtensions");

        #endregion

        static BluetoothExtensions() {
            NetPropertyHelpers.SetPropertyKeys(new Bluetooth_WinPropertyKeys());
        }


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



        private static bool HasProperty(this DeviceInformation info, string key) {
            if (info.Properties != null) {
                return info.Properties.ContainsKey(key);
            }
            return false;
        }


        public static Dictionary<string, NetPropertyDataModel> CreatePropertiesDictionary(this DeviceInformation info) {
            if (info != null) {
                return NetPropertyHelpers.CreatePropertiesDictionary(info.Properties);
            }
            return new Dictionary<string, NetPropertyDataModel>();
        }


        public static NetPropertiesUpdateDataModel CreatePropertiesUpdateData(this DeviceInformationUpdate updateInfo) {
            NetPropertiesUpdateDataModel dm = new NetPropertiesUpdateDataModel() {
                Id = updateInfo.Id,
                ServiceProperties = NetPropertyHelpers.CreatePropertiesDictionary(updateInfo.Properties),
            };
            return dm;
        }


        public static BLE_ConnectStatus Convert(this BluetoothConnectionStatus status) {
            switch (status) {
                case BluetoothConnectionStatus.Disconnected:
                    return BLE_ConnectStatus.Disconnected;
                case BluetoothConnectionStatus.Connected:
                    return BLE_ConnectStatus.Connected;
                default:
                    // Just for compiler. Only 2 enums
                    return BLE_ConnectStatus.Disconnected;
            }
        }



        #region Deprecated

        // TODO - never seen it so remove from variables. Can still access through Properties
        ///// <summary>Uses the Win BLE DeviceInformation Properties with Connectable key</summary>
        ///// <param name="info">The Win device information object</param>
        ///// <returns>true if connectable, otherwise false</returns>
        //public static bool IsConnectable(this DeviceInformation info) {
        //    if (info.HasProperty(KEYS.IsConnectable)) {
        //        Log.Error(5555, "+++++++ IS CONNECTABLE IS IN PROPERTIES +++++++");
        //
        //        return (bool?)info.Properties[KEYS.IsConnectable] == true;
        //    }
        //    Log.Error(9999, "*** IS CONNECTABLE NOT IN PROPERTIES ***");
        //    return false;
        //}


        /// <summary>Uses the Win BLE DeviceInformation Properties with Connected key</summary>
        /// <param name="info">The Win device information object</param>
        /// <returns>true if connected, otherwise false</returns>
        //public static bool IsConnected(this DeviceInformation info) {
        //    if (info.HasProperty(KEYS.IsConnected)) {
        //        Log.Error(5555, "+++++++ IS CONNECTED IS IN PROPERTIES +++++++");
        //        return (bool?)info.Properties[KEYS.IsConnected] == true;
        //    }
        //    Log.Error(9999, "*** IS CONNECTED NOT IN PROPERTIES ***");
        //    return false;
        //}

        #endregion

    }
}
