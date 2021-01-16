using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using BluetoothLE.Net.Parsers;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        private async Task BuildCharacteristicDataModel(GattCharacteristic ch, BLE_ServiceDataModel service) {
            try {
                this.log.InfoEntry("BuildCharacteristicDataModel");
                BLE_CharacteristicDataModel characteristic = new BLE_CharacteristicDataModel();
                characteristic.Uuid = ch.Uuid;
                characteristic.GattType = BLE_DisplayHelpers.GetCharacteristicEnum(ch);
                characteristic.UserDescription = ch.UserDescription;
                characteristic.AttributeHandle = ch.AttributeHandle;
                characteristic.Service = service;
                characteristic.CharName = BLE_DisplayHelpers.GetCharacteristicName(ch);
                characteristic.PropertiesFlags = ch.CharacteristicProperties.ToUInt().ToEnum<CharacteristicProperties>();
                characteristic.ProtectionLevel = (BLE_ProtectionLevel)ch.ProtectionLevel;
                characteristic.PresentationFormats = this.BuildPresentationFormats(ch);
                characteristic.CharValue = await this.ReadValue(ch);

                await this.BuildDescriptors(ch, characteristic);

                // Associate the UWP and data model characteristic for 2 way events to set and get
                this.characteristicBinders.Add(new BLE_CharacteristicBinder(ch, characteristic));

                service.Characteristics.Add(characteristic.Uuid.ToString(), characteristic);

                // This also sends and receives dummy data to Arduino
                //await this.DumpCharacteristic(ch);

            }
            catch (Exception e) {
                this.log.Exception(9999, "Failed during build of characteristic", e);
            }
        }


        private async Task<string> ReadValue(GattCharacteristic ch) {
            try {
                if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read)) {
                    GattReadResult readResult = await ch.ReadValueAsync();
                    if (readResult.Status == GattCommunicationStatus.Success) {
                        byte[] data = readResult.Value.FromBufferToBytes();
                        return BLE_ParseHelpers.GetCharacteristicValueAsString(ch.Uuid, data);
                    }
                    else {
                        this.log.Error(9999, "ReadValue", () => string.Format("Failed read:{0}", readResult.Status));
                    }
                }
                else {
                    this.log.Info("ReadValue", "No READ property");
                }
            }
            catch(Exception e) {
                this.log.Exception(9999, "ReadValue", "", e);
            }
            return "";
        }



        private List<BLE_PresentationFormat> BuildPresentationFormats(GattCharacteristic ch) {
            this.log.InfoEntry("BuildPresentationFormats");
            List<BLE_PresentationFormat> formats = new List<BLE_PresentationFormat>();
            foreach (GattPresentationFormat pf in ch.PresentationFormats) {
                BLE_PresentationFormat format = new BLE_PresentationFormat() {
                    Description = pf.Description,
                    Exponent = pf.Exponent,
                    Format = (DataFormatEnum)pf.FormatType,
                    Units = (UnitsOfMeasurement)pf.Unit,
                    Namespace = pf.Namespace,
                };
                formats.Add(format);
                this.DebugDumpPresentationFormats(ch, format);
            }
            return formats;
        }


        private void DebugDumpPresentationFormats(GattCharacteristic ch, BLE_PresentationFormat f) {
            this.log.Info("DebugDumpPresentationFormats", () => string.Format("Characteristic : {0}", BLE_DisplayHelpers.GetCharacteristicName(ch)));
            this.log.Info("DebugDumpPresentationFormats", () => string.Format("    Description: {0}", f.Description));
            this.log.Info("DebugDumpPresentationFormats", () => string.Format("       Exponent: {0}", f.Exponent));
            this.log.Info("DebugDumpPresentationFormats", () => string.Format("         Format: {0}", f.Format));
            this.log.Info("DebugDumpPresentationFormats", () => string.Format("      Namespace: {0}", f.Namespace));
            this.log.Info("DebugDumpPresentationFormats", () => string.Format("          Units: {0}", f.Units));
        }



        private async Task DumpCharacteristic(GattCharacteristic ch) {
            try {
                if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read)) {
                    GattReadResult readResult = await ch.ReadValueAsync();
                    if (readResult.Status == GattCommunicationStatus.Success) {
                        //if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read)) {
                        byte[] b = readResult.Value.FromBufferToBytes();
                        switch (BLE_DisplayHelpers.GetCharacteristicEnum(ch)) {
                            case GattNativeCharacteristicUuid.String:
                            case GattNativeCharacteristicUuid.DeviceName:
                            case GattNativeCharacteristicUuid.ManufacturerNameString: 
                                string strVal = Encoding.ASCII.GetString(b, 0, (int)readResult.Value.Length);
                                this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1} Handle:{2}",
                                    BLE_DisplayHelpers.GetCharacteristicName(ch), strVal, ch.AttributeHandle));
                                break;

                            case GattNativeCharacteristicUuid.BatteryLevel:
                                #region Battery Level
                                // Hardcoded experiment with Arduino

                                // This works until we get another add existing again
                                // Setting it to notify so I can pick up the event
                                var c = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                                ch.ValueChanged += Ch_BatteryLevelValueChanged;

                                // Will be length 1- value is Hex
                                byte uint8Data = b[0];

                                // My Arduino maps it 0-100
                                // TODO - must be hex between 0x00 - 0x64
                                //int level = Convert.ToInt32(uint8Data.ToString(), 16);
                                int level = Convert.ToInt32(uint8Data.ToString());

                                this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:0x{1} - {2}%  Handle:{3}",
                                    BLE_DisplayHelpers.GetCharacteristicName(ch), uint8Data, level, ch.AttributeHandle));
                                #endregion
                                break;
                            case GattNativeCharacteristicUuid.PnPID:
                                #region PPnPID
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
                                this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}  Handle:{2}",
                                    BLE_DisplayHelpers.GetCharacteristicName(ch), sb.ToString(), ch.AttributeHandle));
                                #endregion
                                break;
                            case GattNativeCharacteristicUuid.Appearance:
                                #region Appearance
                                this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}  Handle:{2}",
                                    BLE_DisplayHelpers.GetCharacteristicName(ch), b.ToGattAppearanceEnum().ToString().CamelCaseToSpaces(), ch.AttributeHandle));
                                #endregion
                                break;
                            case GattNativeCharacteristicUuid.PeripheralPreferredConnectionParameters:
                                #region 
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
                                this.log.Info("DumpCharacteristic", () => string.Format("    Characteristic:{0}  Value:{1}  Handle:{2}",
                                    BLE_DisplayHelpers.GetCharacteristicName(ch), sb2.ToString(), ch.AttributeHandle));
                                #endregion
                                break;
                            default:
                                byte[] data = new byte[readResult.Value.Length];
                                Array.Copy(b, data, data.Length);
                                // TODO - this is old hardcoded connection to serial Arduino in out. Replace
                                if (BLE_DisplayHelpers.GetCharacteristicName(ch) == "39320") {
                                    var xx = await ch.WriteClientCharacteristicConfigurationDescriptorAsync(GattClientCharacteristicConfigurationDescriptorValue.Notify);
                                    ch.ValueChanged += Ch_ValueChangedSerialReturn;
                                }
                                this.log.Info("DumpCharacteristic", () => string.Format(
                                    "    Characteristic: ** NOT ENUMERATED **  {0}  Value:{1}  Length:{2}  Handle:{3}",
                                    BLE_DisplayHelpers.GetCharacteristicName(ch), data.ToFormatedByteString(), data.Length, ch.AttributeHandle));
                                break;
                        }
                    }
                    else {
                        this.log.Info("ConnectToDevice", () => string.Format("    Characteristic:{0}  Read FAILED result:{1} Enum:{2}",
                            BLE_DisplayHelpers.GetCharacteristicName(ch), readResult.Status, BLE_DisplayHelpers.GetCharacteristicEnum(ch)));
                    }
                }
                else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) {
                    if (BLE_DisplayHelpers.GetCharacteristicName(ch) == "39319") {
                        try {

                            // TODO - old hardcoded to write values to an Arduino characteristic


                            //WrapErr.ToErrReport(9999, () => {
                            this.log.Info("LKDJFKLJSDFLK:JSDKLF", "GOT 39319 (output to device)");
                            // Test message                                    
                            //using (var ms = new DataWriter()) {
                            //    // do it this way when we have a multi byte block
                            //    ms.WriteBytes(Encoding.ASCII.GetBytes("Blipo message\n\r"));
                            //    var result = await ch.WriteValueAsync(ms.DetachBuffer());
                            //}

                            for (int ii = 0; ii < 5; ii++) {

                                //byte[] bytes = Encoding.ASCII.GetBytes("Blipo message\n\r");
                                byte[] bytes = Encoding.ASCII.GetBytes("Blipo message being somewhat long and convaluted just to test the return of the entire thing\n\r");
                                int blockLimit = 20;
                                int count = bytes.Length / blockLimit;
                                int rest = (bytes.Length % blockLimit);
                                int lastIndex = 0;
                                for (int i = 0; i < count; i++) {
                                    lastIndex = i * blockLimit;
                                    using (var ms = new DataWriter()) {
                                        byte[] part = new byte[blockLimit];
                                        Array.Copy(bytes, lastIndex, part, 0, part.Length);
                                        this.log.Error(9191, part.ToFormatedByteString());
                                        ms.WriteBytes(part);
                                        var result = await ch.WriteValueAsync(ms.DetachBuffer());
                                    }
                                }

                                if (lastIndex > 0) {
                                    if (lastIndex > 0) {
                                        lastIndex += blockLimit;
                                    }
                                    using (var ms = new DataWriter()) {
                                        byte[] part = new byte[rest];
                                        Array.Copy(bytes, lastIndex, part, 0, part.Length);
                                        this.log.Error(9192, part.ToFormatedByteString());
                                        ms.WriteBytes(part);
                                        var result = await ch.WriteValueAsync(ms.DetachBuffer());
                                    }
                                }

                            }



                            //for (int i = 0; i < bytes.Length; i++) {
                            //    using (var ms = new DataWriter()) {
                            //        ms.WriteByte(bytes[i]);
                            //        var result = await ch.WriteValueAsync(ms.DetachBuffer());
                            //    }
                            //}
                            //});
                        }
                        catch (Exception e) {
                            this.log.Exception(9999, "Fail on write to descriptor", e);
                        }
                    }

                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "Failed during dump info", e);
            }
        }

        private void Ch_ValueChangedSerialReturn(GattCharacteristic sender, GattValueChangedEventArgs args) {
            //throw new NotImplementedException();

            this.log.Info("==============================================================================================================", () => string.Format("NUMBER OF INCOMING BYTES {0}", args.CharacteristicValue.Length));
            byte[] b = args.CharacteristicValue.FromBufferToBytes();
            this.log.Info("Ch_ValueChangedSerialReturn", 
                () => string.Format("** ++ ** ++ ** RETURN VALUE '{0}'", Encoding.ASCII.GetString(b)));
            this.log.Info("==============================================================================================================", "");

        }


        private void Ch_BatteryLevelValueChanged(GattCharacteristic sender, GattValueChangedEventArgs args) {
            // TODO temp to read the changed value in the characteristic

            //args.CharacteristicValue.Length;
            byte[] b = args.CharacteristicValue.FromBufferToBytes();
            // Will be length 1- value is Hex
            byte uint8Data = b[0];

            // My Arduino maps it 0-100
            // TODO - must be hex between 0x00 - 0x64
            //int level = Convert.ToInt32(uint8Data.ToString(), 16);
            int level = Convert.ToInt32(uint8Data.ToString());
            this.log.Info("Ch_ValueChanged", () => string.Format("    Characteristic:{0}  Value:0x{1} - {2}%  Handle:{3}",
                BLE_DisplayHelpers.GetCharacteristicName(sender), 
                uint8Data, level, 
                sender.AttributeHandle));
        }



    }
}
