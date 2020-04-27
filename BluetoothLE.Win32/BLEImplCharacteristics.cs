using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VariousUtils;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothLE.Win32 {

    public partial class BluetoothLEImplWin32 : IBLETInterface {



        private async Task DumpCharacteristic(GattCharacteristic ch) {
            GattReadResult readResult = await ch.ReadValueAsync();
            if (readResult.Status == GattCommunicationStatus.Success) {
                if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read)) {
                    byte[] b = readResult.Value.FromBufferToBytes();
                    switch (BLE_DisplayHelpers.GetCharacteristicEnum(ch)) {
                        case GattNativeCharacteristicUuid.String:
                        case GattNativeCharacteristicUuid.DeviceName:
                        case GattNativeCharacteristicUuid.ManufacturerNameString:
                            string strVal = Encoding.ASCII.GetString(b, 0, (int)readResult.Value.Length);
                            this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}",
                                BLE_DisplayHelpers.GetCharacteristicName(ch), strVal));
                            break;

                        case GattNativeCharacteristicUuid.BatteryLevel:

                            // Will be length 1- value is Hex
                            byte uint8Data = b[0];
                            // TODO - must be hex between 0x00 - 0x64
                            int level = Convert.ToInt32(uint8Data.ToString(), 16);
                            this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:0x{1} - {2}%",
                                BLE_DisplayHelpers.GetCharacteristicName(ch), uint8Data, level));
                            break;
                        case GattNativeCharacteristicUuid.PnPID:
                            // 7 bytes
                            //0x02,0x5E,0x04,0x17,0x08,0x31,0x01
                            // field1 - 1 byte uint8 source of Vendor ID
                            // field2 - 2 byte Uint16 - product vendor namespace
                            // field3 - 2 byte Uint16 - manufacturer ID
                            // field4 - 2 byte Uint16 - manufacturer version of product
                            StringBuilder sb = new StringBuilder();
                            sb
                                .Append("Vendor ID:").Append(b[0]).Append(",")
                                .Append("Vendor Namespace:").Append(BitConverter.ToInt16(b, 1)).Append(",")
                                .Append("Manufacturer ID:").Append(BitConverter.ToInt16(b, 3)).Append(",")
                                .Append("Manufacturer Namespace:").Append(BitConverter.ToInt16(b, 5));
                            this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}",
                                BLE_DisplayHelpers.GetCharacteristicName(ch), sb.ToString()));
                            break;
                        case GattNativeCharacteristicUuid.Appearance:
                            this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}",
                                BLE_DisplayHelpers.GetCharacteristicName(ch),  b.ToGattAppearanceEnum().ToString().CamelCaseToSpaces()));
                            break;
                        case GattNativeCharacteristicUuid.PeripheralPreferredConnectionParameters:
                            //Peripheral Preferred Connection Parameters  Value:0x14,0x00,0x24,0x00,0x04,0x00,0xC8,0x00 - 8 bytes
                            // 8 bytes in 4 uint16 fields
                            // 1. Minimum Connect interval 6-3200
                            // 2. Maximum Connect interval 6-3200
                            // 3. Slave latency 0-1000
                            // 4. Connection Supervisor Timeout Multiplier 10-3200
                            StringBuilder sb2 = new StringBuilder();
                            sb2
                                .Append("Min Connect Interval:").Append(BitConverter.ToInt16(b, 0)).Append(",")
                                .Append("Max Connect Interval:").Append(BitConverter.ToInt16(b, 2)).Append(",")
                                .Append("Slave Latency:").Append(BitConverter.ToInt16(b, 4)).Append(",")
                                .Append("Connect Supervisor Timout multiplier:").Append(BitConverter.ToInt16(b, 6));
                            this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}",
                                BLE_DisplayHelpers.GetCharacteristicName(ch), sb2.ToString()));
                            break;
                        default:
                            byte[] data = new byte[readResult.Value.Length];
                            Array.Copy(b, data, data.Length);
                            this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}  Length:{2}",
                                BLE_DisplayHelpers.GetCharacteristicName(ch), data.ToFormatedByteString(), data.Length));
                            break;
                    }
                }
                else {
                    this.log.Info("ConnectToDevice", () => string.Format("     Characteristic:{0}  Not Read:{1} Enum:{2}",
                        BLE_DisplayHelpers.GetCharacteristicName(ch), readResult.Status, BLE_DisplayHelpers.GetCharacteristicEnum(ch)));
                }
            }
            else {
                this.log.Info("ConnectToDevice", () => string.Format("    ----- Characteristic:{0}  Read FAILED result:{1} Enum:{2}",
                    BLE_DisplayHelpers.GetCharacteristicName(ch), readResult.Status, BLE_DisplayHelpers.GetCharacteristicEnum(ch)));
            }
        }

    }
}
