using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.Parsers.Characteristics;
using BluetoothLE.Net.Parsers.Descriptor;
using BluetoothLE.Net.Tools;
using LanguageFactory.Net.data;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Text;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public void Translate(BluetoothLEDeviceInfo device) {
            try {
                if (device != null) {
                    foreach (var s in device.Services) {
                        this.Translate(s);
                    }
                }
            }
            catch (Exception e) {
                this.log.Exception(9000, "Translate(device)", "", e);
            }
        }


        private string Translate(RangeValidationResult result) {
            switch (result.Status) {
                case BLE_DataValidationStatus.Success:
                    return this.GetText(MsgCode.Ok);
                case BLE_DataValidationStatus.OutOfRange:
                    return this.GetText(MsgCode.OutOfRange);
                case BLE_DataValidationStatus.StringConversionFailed:
                    // translation easier than parse fail
                    return this.GetText(MsgCode.InvalidInput);
                case BLE_DataValidationStatus.Empty:
                    return this.GetText(MsgCode.EmptyParameter);
                case BLE_DataValidationStatus.InvalidInput:
                    return this.GetText(MsgCode.InvalidInput);
                case BLE_DataValidationStatus.NotHandled:
                    return this.GetText(MsgCode.UnhandledError);
                case BLE_DataValidationStatus.UnhandledError:
                    return this.GetText(MsgCode.UnhandledError);
                default:
                    return "ERR";
            }
        }


        private void Translate(BLE_ServiceDataModel dataModel) {
            dataModel.DisplayHeader = this.GetText(MsgCode.Service);
            foreach (BLE_CharacteristicDataModel d in dataModel.Characteristics) {
                this.Translate(d);
                this.TranslateBool(d);
            }
        }


        private void Translate(BLE_CharacteristicDataModel dataModel) {
            dataModel.DisplayHeader = this.GetText(MsgCode.Characteristic);
            dataModel.DisplayReadWrite = string.Empty;
            if (dataModel.IsReadable || dataModel.IsWritable) {
                StringBuilder sb = new StringBuilder();
                if (dataModel.IsReadable) {
                    sb.Append(this.GetText(MsgCode.Read));
                }
                if (dataModel.IsWritable) {
                    if (sb.Length > 0) {
                        sb.Append(", ");
                    }
                    sb.Append(this.GetText(MsgCode.Write));
                }
                if (sb.Length > 0) {
                    dataModel.DisplayReadWrite = string.Format("({0})", sb.ToString());
                }
            }

            if (dataModel.Parser is CharParser_Appearance) {

                //dataModel.DisplayHeader = 
                dataModel.CharValue = this.Translate(dataModel.Parser as CharParser_Appearance);
            }

            foreach (BLE_DescriptorDataModel d in dataModel.Descriptors) {
                this.Translate(d);
            }
        }


        private void Translate(BLE_DescriptorDataModel desc) {
            desc.DisplayHeader = this.GetText(MsgCode.Descriptor);
            if (desc.Parser is DescParser_PresentationFormat) {
                desc.DisplayName = this.Translate(desc.Parser as DescParser_PresentationFormat);
            }
            else if (desc.Parser is DescParser_ClientCharacteristicConfig) {
                desc.DisplayName = this.Translate(
                    desc.Parser as DescParser_ClientCharacteristicConfig);
            }
        }


        private string Translate(DescParser_PresentationFormat desc) {
            return desc.TranslateDisplayString(
                this.GetText(MsgCode.DataType),
                this.GetText(MsgCode.Unit),
                this.Translate(desc.MeasurementUnitsEnum),
                this.GetText(MsgCode.Exponent),
                this.GetText(MsgCode.Description));
        }


        private string Translate(DescParser_ClientCharacteristicConfig desc) {
            return desc.TranslateDisplayString(
                this.GetText(MsgCode.Notifications),
                this.Translate(desc.Notifications),
                "Indications",
                this.Translate(desc.Indications));
        }


        private string Translate(EnabledDisabled state) {
            if (state == EnabledDisabled.Enabled) {
                return this.GetText(MsgCode.Enabled);
            }
            return this.GetText(MsgCode.Disabled);
        }


        private string Translate(BLEOperationStatus status) {
            switch (status) {
                case BLEOperationStatus.Success:
                    return this.GetText(MsgCode.Ok);
                case BLEOperationStatus.NotFound:
                    return this.GetText(MsgCode.NotFound);
                case BLEOperationStatus.NoServices:
                    return this.GetText(MsgCode.NoServices);
                case BLEOperationStatus.GetServicesFailed:
                    return this.GetText(MsgCode.ServicesFailure);
                case BLEOperationStatus.Failed:
                    return this.GetText(MsgCode.UnknownError);
                case BLEOperationStatus.UnhandledError:
                    return this.GetText(MsgCode.UnhandledError);
                case BLEOperationStatus.UnknownError:
                    return this.GetText(MsgCode.UnknownError);
                default:
                    return status.ToString().CamelCaseToSpaces();
            }
        }


        private string Translate(CharParser_Appearance p) {
            p.Translate(() => {
                return this.TranslateAppearance(
                    p.Category, p.SubCategory, p.DisplayString);
            });
            return p.DisplayString;
        }


        private string Translate(UnitsOfMeasurement unit) {
            // First check for string returned if they are handled lower
            string value = unit.ToStr();
            if (value.Length > 0) {
                return value;
            }

            // Others which do not return cross language string either because there
            // it has not been implemented or a cross language string impractical
            switch (unit) {
                // Special case where user sets at zero. Non in spec
                case UnitsOfMeasurement.Unknown:
                case UnitsOfMeasurement.Unitless:
                    return this.GetText(MsgCode.None);
                case UnitsOfMeasurement.NOT_HANDLED:
                    return this.GetText(MsgCode.NotFound);
                default:
                    return unit.ToString().CamelCaseToSpaces();
            }
        }


        private void TranslateBool(BLE_CharacteristicDataModel dm) {
            if (dm.Parser.DataType == BLE_DataType.Bool) {
                // TODO put in true false translation
                if (dm.CharValue.ToLower() == "true") {
                    dm.CharValue = this.GetText(MsgCode.True);
                }
                else if (dm.CharValue.ToLower() == "false") {
                    dm.CharValue = this.GetText(MsgCode.False);
                }
            }
        }


        private string TranslateAppearance(uint category, uint sub, string existing) {
            BLE_AppearanceCategory cat = category.FirstOrDefault(BLE_AppearanceCategory.Unknown);
            if (cat == BLE_AppearanceCategory.Unknown) {
                return existing;
            }

            // Some categories only have one entry
            // when we find it we can process and return
            switch (cat) {
                case BLE_AppearanceCategory.Unknown:
                    return existing;

                // Entries with no sub categories
                case BLE_AppearanceCategory.Phone:
                case BLE_AppearanceCategory.Computer:
                case BLE_AppearanceCategory.Clock:
                case BLE_AppearanceCategory.Display:
                case BLE_AppearanceCategory.Remote_Control:
                case BLE_AppearanceCategory.Eye_Glasses:
                case BLE_AppearanceCategory.Tag:
                case BLE_AppearanceCategory.Key_Ring:
                case BLE_AppearanceCategory.Media_Player:
                case BLE_AppearanceCategory.Barcode_Scanner:
                case BLE_AppearanceCategory.Thermometer:
                case BLE_AppearanceCategory.Glucose_Meter:
                case BLE_AppearanceCategory.Network_Device:
                case BLE_AppearanceCategory.Air_Conditioning:
                case BLE_AppearanceCategory.Humidifier:
                case BLE_AppearanceCategory.Weight_Scale:
                    return string.Format("{0} {1}", existing, cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Watch:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceHeartRate.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Heart_Rate:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceWatch.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Blood_Pressure:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceBloodPressure.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Human_Interface_Device:
                    return string.Format("{0} {1}",
                        existing,
                        //cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceHID.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Run_Walk_Sensor:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceRunWalkSensor.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Cycling:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceCycling.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Control_Device:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceControlDevice.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Sensor:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceSensor.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Light_Fixture:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceLightFixture.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Fan:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceFan.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.HVAC:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceHVAC.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Heating:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceHeating.Generic).ToString().UnderlineToSpaces());
                
                case BLE_AppearanceCategory.Access_Control:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceAccessControl.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Motorized_Device:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceMotorizedDevice.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Power_Device:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearancePowerDevice.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Light_Source:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceLightSource.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Oximeter:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceOximeter.Generic).ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Outdoor_Sports_Activity:
                    return string.Format("{0} {1} : {2}",
                        existing,
                        cat.ToString().UnderlineToSpaces(),
                        sub.FirstOrDefault(BLE_AppearanceOutdoorSportActivity.Generic).ToString().UnderlineToSpaces());
                default:
                    return existing;
            }
        }

    }

}
