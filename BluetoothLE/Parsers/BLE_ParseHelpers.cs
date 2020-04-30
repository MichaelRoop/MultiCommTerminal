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
    public static class BLE_ParseHelpers {
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
                                #region 
                                //List of uint16 Atribute Handles
                                StringBuilder sb = new StringBuilder();
                                sb.Append("Attribute Handles: ");
                                int count = value.Length / 2;
                                for (int i = 0; i < count; i++) {
                                    if (i > 0) {
                                        sb.Append(",");
                                    }
                                    sb.Append(BitConverter.ToUInt16(value, i * 2).ToString());
                                }
                                #endregion
                                return sb.ToString();
                            case GattNativeDescriptorUuid.CharacteristicExtendedProperties:
                                // 16bit - 0-3 Reliable write and writable auxilliaries
                                // https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.gatt.characteristic_extended_properties.xml
                                return UInt16.Parse(Encoding.ASCII.GetString(value)).ToString();
                            case GattNativeDescriptorUuid.CharacteristicPresentationFormat:
                                // byte 0-27 enumeration - data type of characteristic value  
                                return Byte.Parse(Encoding.ASCII.GetString(value)).ToString();
                            case GattNativeDescriptorUuid.CharacteristicUserDescription:
                                return Encoding.UTF8.GetString(value);
                            case GattNativeDescriptorUuid.ClientCharacteristicConfiguration:
                                return new DescClientCharasteristicConfigParser(value).DisplayString();
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
                                // uint16 - 0-1
                                ushort serverConfig = UInt16.Parse(Encoding.ASCII.GetString(value));
                                switch (serverConfig) {
                                    case 0:
                                        return "Broadcast Disabled";
                                    case 1:
                                        return "Broadcast Enabled";
                                    default:
                                        Log.Error(999, "BLE_ParseHelpers", "", () => string.Format("{0} not handled", serverConfig));
                                        return serverConfig.ToString();
                                }
                            case GattNativeDescriptorUuid.TimeTriggerSetting:
                                // uint8 0-4
                                return Byte.Parse(Encoding.ASCII.GetString(value)).ToString();
                            case GattNativeDescriptorUuid.ValidRange:
                                // Hex values 2 or 4 bytes
                                // ex: 0x020x0D == 2-13
                                // ex: 0x58 0x02 0x20 0x1C == 600 - 7,200 seconds
                                // see: https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.valid_range.xml
                                //StringBuilder sb = new StringBuilder();
                                switch (value.Length) {
                                    case 2:
                                        // TODO conversions from hex
                                        return String.Format("0x{0} - 0x{1}", value[0], value[1]);
                                    case 4:
                                        return String.Format(
                                            "0x{0} - 0x{1}", 
                                            BitConverter.ToUInt16(value, 0),
                                            BitConverter.ToUInt16(value, 2));
                                    default:
                                        return "INVALID RANGE";
                                }
                            case GattNativeDescriptorUuid.ValueTriggerSetting:
                                // uint8 -  0-8
                                return Byte.Parse(Encoding.ASCII.GetString(value)).ToString();
                            case GattNativeDescriptorUuid.ReportReference:
                                // Report ID   uint8  0-255 - report ID and Type
                                // Report Type uint8 1-3 (Input Report=1, Output Report=2, Feature Report=3
                                UInt16 reportType = BitConverter.ToUInt16(value, 1);
                                string str = "INVALID";
                                switch (reportType) {
                                    case 1:
                                        str = "Input";
                                        break;
                                    case 2:
                                        str = "Output";
                                        break;
                                    case 3:
                                        str = "Feature";
                                        break;
                                }
                                return string.Format("Report Id:{0} Type:{1}", value[0], str);
                            case GattNativeDescriptorUuid.NumberOfDigitals:
                                // uint8 Number of digitals in a characteristic
                                return Byte.Parse(Encoding.ASCII.GetString(value)).ToString();
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
