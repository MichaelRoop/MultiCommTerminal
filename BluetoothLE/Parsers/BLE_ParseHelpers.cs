using BluetoothLE.Net.Parsers.Descriptor;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
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

        private static DescParser_CharacteristicAggregateFormat agragateFormatValueParser = new DescParser_CharacteristicAggregateFormat();
        private static DescParser_ClientCharacteristicConfig clientCharacteristicConfigParser = new  DescParser_ClientCharacteristicConfig();
        private static DescParser_ServerCharacteristicConfig serverCharacteristicConfigParser = new DescParser_ServerCharacteristicConfig();
        private static DescParser_ValidRange validRangeParser = new DescParser_ValidRange();
        private static DescParser_ReportReference reportReferenceValueParser = new DescParser_ReportReference();
        private static DescParser_CharacteristicExtendedProperties charExtendedPropertiesParser = new DescParser_CharacteristicExtendedProperties();

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
            string result = "";
            ErrReport report;
            result = WrapErr.ToErrReport(out report, 9999, () => string.Format("Failed to parse descriptor value"), () => {
                if (IsSigDefinedUuid(descriptorUuid)) {
                    GattNativeDescriptorUuid descriptorEnum;
                    if (Enum.TryParse(descriptorUuid.ToShortId().ToString(), out descriptorEnum)) {
                        //return descriptorName.ToString().CamelCaseToSpaces();
                        switch (descriptorEnum) {
                            case GattNativeDescriptorUuid.CharacteristicAggregateFormat:
                                return agragateFormatValueParser.Parse(value);
                            case GattNativeDescriptorUuid.CharacteristicExtendedProperties:
                                return charExtendedPropertiesParser.Parse(value);
                            case GattNativeDescriptorUuid.CharacteristicPresentationFormat:
                                // byte 0-27 enumeration - data type of characteristic value  
                                return Byte.Parse(Encoding.ASCII.GetString(value)).ToString();
                            case GattNativeDescriptorUuid.CharacteristicUserDescription:
                                return Encoding.UTF8.GetString(value);
                            case GattNativeDescriptorUuid.ClientCharacteristicConfiguration:
                                return clientCharacteristicConfigParser.Parse(value);
                            case GattNativeDescriptorUuid.EnvironmentalSensingConfiguration:
                                // ?
                                break;
                            case GattNativeDescriptorUuid.EnvironmentalSensingMeasurement:
                                // ?
                                break;
                            case GattNativeDescriptorUuid.EnvironmentalSensingTriggerSetting:
                                // ?
                                break;
                            case GattNativeDescriptorUuid.ExternalReportReference:
                                // Gatt UUID
                                break;
                            case GattNativeDescriptorUuid.ServerCharacteristicConfiguration:
                                return serverCharacteristicConfigParser.Parse(value);
                            case GattNativeDescriptorUuid.TimeTriggerSetting:
                                // uint8 0-4
                                return Byte.Parse(Encoding.ASCII.GetString(value)).ToString();
                            case GattNativeDescriptorUuid.ValidRange:
                                return validRangeParser.Parse(value);
                            case GattNativeDescriptorUuid.ValueTriggerSetting:
                                // uint8 -  0-8
                                return Byte.Parse(Encoding.ASCII.GetString(value)).ToString();
                            case GattNativeDescriptorUuid.ReportReference:
                                return reportReferenceValueParser.Parse(value);
                            case GattNativeDescriptorUuid.NumberOfDigitals:
                                // uint8 Number of digitals in a characteristic
                                return string.Format("Number of Digitals:{0}", value[0]);
                        }
                    }
                }
                return "";
            });
            return report.Code == 0 ? result: "*FAILED*";
        }


        /// <summary>
        ///     The SIG has a standard base value for Assigned UUIDs. In order to determine if a UUID is SIG defined,
        ///     zero out the unique section and compare the base sections.
        /// </summary>
        /// <param name="uuid">The UUID to determine if SIG assigned</param>
        /// <returns></returns>
        private static bool IsSigDefinedUuid(Guid uuid) {
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


        // This is a problem. Uses Windows propriatary IBuffer. Need to move out to Win implementation

        ///// <summary>Convert from buffer to properly sized byte array</summary>
        ///// <param name="buffer">The buffer containing data</param>
        ///// <returns>Byte array</returns>
        //public static byte[] FromBufferToBytes(this IBuffer buffer) {
        //    uint dataLength = buffer.Length;
        //    byte[] data = new byte[dataLength];
        //    using (DataReader reader = DataReader.FromBuffer(buffer)) {
        //        reader.ReadBytes(data);
        //    }
        //    return data;
        //}

    }



}
