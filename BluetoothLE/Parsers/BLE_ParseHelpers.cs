using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using BluetoothLE.Net.Parsers.Descriptor;
using System;
using VariousUtils;

namespace BluetoothLE.Net.Parsers {


    /// <summary>
    /// Parse out BLE information for display. Starting point was MS example 
    /// DisplayHelpers. Made OS independent
    /// </summary>
    /// <remarks>
    /// Link to the spec: https://www.bluetooth.com/specifications/gatt/descriptors/
    /// </remarks>
    public static class BLE_ParseHelpers {

        #region Data

        private static IDescParserFactory factory = new DescParserFactory();

        #endregion


        public static string GetServiceName(Guid serviceUuid) {
            if (IsSigDefinedUuid(serviceUuid)) {
                GattNativeServiceUuid serviceName;
                if (Enum.TryParse(serviceUuid.ToShortId().ToString(), out serviceName)) {
                    return serviceName.ToString().CamelCaseToSpaces();
                }
            }
            return "Custom Service: " + serviceUuid;
        }


        public static string GetCharacteristicName(Guid characteristicUuid, string userDescription) {
            if (IsSigDefinedUuid(characteristicUuid)) {
                GattNativeCharacteristicUuid characteristicName;
                if (Enum.TryParse(characteristicUuid.ToShortId().ToString(), out characteristicName)) {
                    return characteristicName.ToString().CamelCaseToSpaces();
                }
            }

            if (!string.IsNullOrEmpty(userDescription)) {
                return userDescription;
            }

            else {
                return "Custom Characteristic: " + characteristicUuid;
            }
        }


        public static GattNativeCharacteristicUuid GetCharacteristicEnum(Guid characteristicUuid, string userDescription) {
            if (IsSigDefinedUuid(characteristicUuid)) {
                GattNativeCharacteristicUuid characteristicName;
                if (Enum.TryParse(characteristicUuid.ToShortId().ToString(), out characteristicName)) {
                    return characteristicName;
                }
            }

            if (!string.IsNullOrEmpty(userDescription)) {
                return GattNativeCharacteristicUuid.None;
            }
            return GattNativeCharacteristicUuid.None;
        }


        public static string GetDescriptorName(Guid descriptorUuid) {
            if (IsSigDefinedUuid(descriptorUuid)) {
                GattNativeDescriptorUuid descriptorName;
                if (Enum.TryParse(descriptorUuid.ToShortId().ToString(), out descriptorName)) {
                    return descriptorName.ToString().CamelCaseToSpaces();
                }
            }
            return "Custom Descriptor: " + descriptorUuid;
        }


        public static string GetDescriptorValueAsString(Guid descriptorUuid, byte[] value) {
            return factory.GetParsedValueAsString(descriptorUuid, value);
        }        


        /// <summary>
        ///     The SIG has a standard base value for Assigned UUIDs. In order to determine if a UUID is SIG defined,
        ///     zero out the unique section and compare the base sections.
        /// </summary>
        /// <param name="uuid">The UUID to determine if SIG assigned</param>
        /// <returns></returns>
        public static bool IsSigDefinedUuid(Guid uuid) {
            var bluetoothBaseUuid = new Guid("00000000-0000-1000-8000-00805F9B34FB");

            var bytes = uuid.ToByteArray();
            // Zero out the first and second bytes
            // Note how each byte gets flipped in a section - 1234 becomes 34 12
            // Example Guid: 35918bc9-1234-40ea-9779-889d79b753f0
            //                   ^^^^
            // bytes output = C9 8B 91 35 34 12 EA 40 97 79 88 9D 79 B7 53 F0
            //                ^^ ^^
            bytes[0] = 0;
            bytes[1] = 0;
            var baseUuid = new Guid(bytes);
            return baseUuid == bluetoothBaseUuid;
        }
    }



    /// <summary>Extensions for helper</summary>
    public static class BLE_ParseHelperExtensions {

        public static GattAppearanceEnum ToGattAppearanceEnum(this byte[] bytes) {
            // Must be 2 bytes
            if (bytes.Length != 2) {
                return GattAppearanceEnum.Unknown;
            }

            UInt16 val = BitConverter.ToUInt16(bytes, 0);
            foreach (GattAppearanceEnum a in EnumHelpers.GetEnumList<GattAppearanceEnum>()) {
                if (((UInt16)a) == val) {
                    return a;
                }
            }
            return GattAppearanceEnum.Unknown;
        }


        /// <summary>
        /// Convert from standard 128bit UUID to assigned 32bit UUIDs. Makes it easy to compare services
        /// that devices expose to the standard list.
        /// </summary>
        /// <param name="uuid">UUID to convert to 32 bit</param>
        /// <returns>The unsigned short ID</returns>
        public static ushort ToShortId(this Guid uuid) {
            // Get the short Uuid
            byte[] bytes = uuid.ToByteArray();
            ushort shortUuid = (ushort)(bytes[0] | (bytes[1] << 8));
            return shortUuid;
        }

    }

}
