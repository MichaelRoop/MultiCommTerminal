using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.Parsers;
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
                    return this.GetText(MsgCode.DataTypeUnhandled);
                case BLE_DataValidationStatus.UnhandledError:
                    return this.GetText(MsgCode.UnhandledError);
                default:
                    return "ERR";
            }
        }


        private void Translate(BLE_ServiceDataModel dataModel) {
            dataModel.DisplayHeader = this.GetText(MsgCode.Service);
            dataModel.DisplayName = this.Translate(dataModel.ServiceTypeEnum, dataModel.DisplayName);
            foreach (BLE_CharacteristicDataModel d in dataModel.Characteristics) {
                this.Translate(d);
            }
        }


        public void Translate(BLE_CharacteristicDataModel dataModel) {
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
                dataModel.CharValue = this.Translate(dataModel.Parser as CharParser_Appearance);
            }
            this.TranslateIfBool(dataModel);

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
            return desc.Translate(() => {
                return string.Format(
                    "{0} : {1}, {2} : {3}",
                    this.GetText(MsgCode.Notifications),
                    this.Translate(desc.Notifications),
                    "Indications",
                    this.Translate(desc.Indications));
            });
        }


        #region Translate BLE units and results

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


        private void TranslateIfBool(BLE_CharacteristicDataModel dm) {
            if (dm.Parser.DataType == BLE_DataType.Bool) {
                dm.CharValue = dm.Parser.BoolValue 
                    ? this.GetText(MsgCode.True) 
                    : this.GetText(MsgCode.False);
            }
        }

        private string Translate(GattNativeServiceUuid id, string current) {
            switch (id) {
                case GattNativeServiceUuid.None:
                    return current;
                case GattNativeServiceUuid.GenericAccess:
                case GattNativeServiceUuid.GenericAttribute:
                case GattNativeServiceUuid.ImmediateAlert:
                case GattNativeServiceUuid.LinkLoss:
                case GattNativeServiceUuid.TxPower:
                case GattNativeServiceUuid.CurrentTimeService:
                case GattNativeServiceUuid.ReferenceTimeUpdateService:
                case GattNativeServiceUuid.NextDSTChange:
                case GattNativeServiceUuid.Glucose:
                case GattNativeServiceUuid.HealthThermometer:
                case GattNativeServiceUuid.DeviceInformation:
                case GattNativeServiceUuid.HeartRate:
                case GattNativeServiceUuid.PhoneAlertStatus:
                case GattNativeServiceUuid.Battery:
                case GattNativeServiceUuid.BloodPressure:
                case GattNativeServiceUuid.AlertNotification:
                case GattNativeServiceUuid.ScanParameters:
                case GattNativeServiceUuid.HumanInterfaceDevice:
                case GattNativeServiceUuid.RunningSpeedandCadence:
                case GattNativeServiceUuid.AutomationIO:
                case GattNativeServiceUuid.CyclingSpeedandCadence:
                case GattNativeServiceUuid.CyclingPower:
                case GattNativeServiceUuid.LocationAndNavigation:
                case GattNativeServiceUuid.EnvironmentalSensing:
                case GattNativeServiceUuid.BodyComposition:
                case GattNativeServiceUuid.UserData:
                case GattNativeServiceUuid.WeightScale:
                case GattNativeServiceUuid.BondManagement:
                case GattNativeServiceUuid.ContinuousGlucoseMonitoring:
                case GattNativeServiceUuid.InternetProtocolSupport:
                case GattNativeServiceUuid.IndoorPositioning:
                case GattNativeServiceUuid.PulseOximeter:
                case GattNativeServiceUuid.HTTPProxy:
                case GattNativeServiceUuid.TransportDiscovery:
                case GattNativeServiceUuid.ObjectTransfer:
                case GattNativeServiceUuid.FitnessMachine:
                case GattNativeServiceUuid.MeshProvisioning:
                case GattNativeServiceUuid.MeshProxy:
                case GattNativeServiceUuid.ReconnectionConfiguration:
                case GattNativeServiceUuid.InsulinDelivery:
                case GattNativeServiceUuid.BinarySensor:
                case GattNativeServiceUuid.EmergencyConfiguration:
                case GattNativeServiceUuid.PhysicalActivityMonitor:
                case GattNativeServiceUuid.AudioInputControl:
                case GattNativeServiceUuid.VolumeControl:
                case GattNativeServiceUuid.VolumeOffsetControl:
                case GattNativeServiceUuid.DeviceTime:
                case GattNativeServiceUuid.ConstantToneExtension:
                case GattNativeServiceUuid.SimpleKeyService:
                    return id.ToString().CamelCaseToSpaces();
                default:
                    return this.GetText(MsgCode.NotFound);
            }
        }


        #endregion

        #region Appearance translation

        private string Translate(CharParser_Appearance p) {
            return p.Translate(() => {
                return this.TranslateAppearance(
                    p.Category, p.SubCategory, p.DisplayString);
            });
        }

        private string TranslateAppearance(uint category, uint sub, string existing) {
            // Some categories only have one entry
            // when we find it we can process and return
            BLE_AppearanceCategory cat = category.FirstOrDefault(BLE_AppearanceCategory.Unknown);
            switch (cat) {
                case BLE_AppearanceCategory.Unknown:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());

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
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceCategory.Watch:
                    return this.TranslateWatchCategory(cat, sub);
                case BLE_AppearanceCategory.Heart_Rate_Sensor:
                    return this.TranslateHeartRateCategory(cat, sub);
                case BLE_AppearanceCategory.Blood_Pressure:
                    return this.TranslateHeartBloodPressureCategory(cat, sub);
                case BLE_AppearanceCategory.Human_Interface_Device:
                    return this.TranslateHIDCategory(cat, sub);
                case BLE_AppearanceCategory.Run_Walk_Sensor:
                    return this.TranslateRunWalkCategory(cat, sub);
                case BLE_AppearanceCategory.Cycling:
                    return this.TranslateCyclingCategory(cat, sub);
                case BLE_AppearanceCategory.Control_Device:
                    return this.TranslateControlDeviceCategory(cat, sub);
                case BLE_AppearanceCategory.Sensor:
                    return this.TranslateSensprDeviceCategory(cat, sub);
                case BLE_AppearanceCategory.Light_Fixture:
                    return this.TranslateLightFixtureCategory(cat, sub);
                case BLE_AppearanceCategory.Fan:
                    return this.TranslateFanCategory(cat, sub);
                case BLE_AppearanceCategory.HVAC:
                    return this.TranslateHVACCategory(cat, sub);
                case BLE_AppearanceCategory.Heating:
                    return this.TranslateHeatingCategory(cat, sub);
                case BLE_AppearanceCategory.Access_Control:
                    return this.TranslateAccessControlCategory(cat, sub);
                case BLE_AppearanceCategory.Motorized_Device:
                    return this.TranslateMotorizedDeviceCategory(cat, sub);
                case BLE_AppearanceCategory.Power_Device:
                    return this.TranslatePowerDeviceCategory(cat, sub);
                case BLE_AppearanceCategory.Light_Source:
                    return this.TranslateLightSourceCategory(cat, sub);
                case BLE_AppearanceCategory.Oximeter:
                    return this.TranslateOximeterCategory(cat, sub);
                case BLE_AppearanceCategory.Outdoor_Sports_Activity:
                    return this.TranslateOutdoorsSportsActivityCategory(cat, sub);
                default:
                    return existing;
            }




        }

        private string TranslateWatchCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceWatch s = sub.FirstOrDefault(BLE_AppearanceWatch.Not_Handled);
            switch (s) {
                case BLE_AppearanceWatch.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceWatch.Sports_Watch:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceWatch.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateHeartRateCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceHeartRate s = sub.FirstOrDefault(BLE_AppearanceHeartRate.Not_Handled);
            switch (s) {
                case BLE_AppearanceHeartRate.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceHeartRate.On_Belt_Heart_Rate_Sensor:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceHeartRate.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateHeartBloodPressureCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceBloodPressure s = sub.FirstOrDefault(BLE_AppearanceBloodPressure.Not_Handled);
            switch (s) {
                case BLE_AppearanceBloodPressure.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceBloodPressure.Blood_Pressure_on_Arm:
                case BLE_AppearanceBloodPressure.Blood_Pressure_on_Wrist:
                case BLE_AppearanceBloodPressure.Not_Handled:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());

                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateHIDCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceHID s = sub.FirstOrDefault(BLE_AppearanceHID.Not_Handled);
            switch (s) {
                case BLE_AppearanceHID.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceHID.Keyboard:
                case BLE_AppearanceHID.Mouse:
                case BLE_AppearanceHID.Joystick:
                case BLE_AppearanceHID.Gamepad:
                case BLE_AppearanceHID.Digitizer:
                case BLE_AppearanceHID.Card_Reader:
                case BLE_AppearanceHID.Digital_Pen:
                case BLE_AppearanceHID.Barcode_Scanner:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceHID.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateRunWalkCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceRunWalkSensor s = sub.FirstOrDefault(BLE_AppearanceRunWalkSensor.Not_Handled);
            switch (s) {
                case BLE_AppearanceRunWalkSensor.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceRunWalkSensor.In_Shoe_Run_Walk_Sensor:
                case BLE_AppearanceRunWalkSensor.On_Shoe_Run_Walk_Sensor:
                case BLE_AppearanceRunWalkSensor.On_Hip_Run_Walk_Sensor:
                case BLE_AppearanceRunWalkSensor.Not_Handled:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateCyclingCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceCycling s = sub.FirstOrDefault(BLE_AppearanceCycling.Not_Handled);
            switch (s) {
                case BLE_AppearanceCycling.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceCycling.Cycling_Computer:
                case BLE_AppearanceCycling.Cycling_Speed_Sensor:
                case BLE_AppearanceCycling.Cycling_Cadence_Sensor:
                case BLE_AppearanceCycling.Cycling_Power_Sensor:
                case BLE_AppearanceCycling.Cycling_Speed_and_Cadence_Sensor:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceCycling.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateControlDeviceCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceControlDevice s = sub.FirstOrDefault(BLE_AppearanceControlDevice.Not_Handled);
            switch (s) {
                case BLE_AppearanceControlDevice.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceControlDevice.Switch:
                case BLE_AppearanceControlDevice.Multi_Switch:
                case BLE_AppearanceControlDevice.Button:
                case BLE_AppearanceControlDevice.Slider:
                case BLE_AppearanceControlDevice.Rotary:
                case BLE_AppearanceControlDevice.Touch_Panel:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceControlDevice.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateSensprDeviceCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceSensor s = sub.FirstOrDefault(BLE_AppearanceSensor.Not_Handled);
            switch (s) {
                case BLE_AppearanceSensor.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceSensor.Motion_Sensor:
                case BLE_AppearanceSensor.Air_Quality_Sensor:
                case BLE_AppearanceSensor.Temperature_Sensor:
                case BLE_AppearanceSensor.Humidity_Sensor:
                case BLE_AppearanceSensor.Leak_Sensor:
                case BLE_AppearanceSensor.Smoke_Sensor:
                case BLE_AppearanceSensor.Occupancy_Sensor:
                case BLE_AppearanceSensor.Contact_Sensor:
                case BLE_AppearanceSensor.Carbon_Monoxide_Sensor:
                case BLE_AppearanceSensor.Carbon_Dioxide_Sensor:
                case BLE_AppearanceSensor.Ambient_Light_Sensor:
                case BLE_AppearanceSensor.Energy_Sensor:
                case BLE_AppearanceSensor.Color_Light_Sensor:
                case BLE_AppearanceSensor.Rain_Sensor:
                case BLE_AppearanceSensor.Fire_Sensor:
                case BLE_AppearanceSensor.Wind_Sensor:
                case BLE_AppearanceSensor.Proximity_Sensor:
                case BLE_AppearanceSensor.Multi_Sensor:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceSensor.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateLightFixtureCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceLightFixture s = sub.FirstOrDefault(BLE_AppearanceLightFixture.Not_Handled);
            switch (s) {
                case BLE_AppearanceLightFixture.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceLightFixture.Wall_Light:
                case BLE_AppearanceLightFixture.Ceiling_Light:
                case BLE_AppearanceLightFixture.Floor_Light:
                case BLE_AppearanceLightFixture.Cabinet_Light:
                case BLE_AppearanceLightFixture.Desk_Light:
                case BLE_AppearanceLightFixture.Troffer_Light:
                case BLE_AppearanceLightFixture.Pendant_Light:
                case BLE_AppearanceLightFixture.In_Ground_Light:
                case BLE_AppearanceLightFixture.Flood_Light:
                case BLE_AppearanceLightFixture.Underwater_Light:
                case BLE_AppearanceLightFixture.Bollard_Light:
                case BLE_AppearanceLightFixture.Pathway_Light:
                case BLE_AppearanceLightFixture.Garden_Light:
                case BLE_AppearanceLightFixture.Pole_Top_Light:
                case BLE_AppearanceLightFixture.Spot_Light:
                case BLE_AppearanceLightFixture.Linear_Light:
                case BLE_AppearanceLightFixture.Street_Light:
                case BLE_AppearanceLightFixture.Shelves_Light:
                case BLE_AppearanceLightFixture.High_Bay_Low_Bay_Light:
                case BLE_AppearanceLightFixture.Emergency_Exit_Light:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceLightFixture.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateFanCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceFan s = sub.FirstOrDefault(BLE_AppearanceFan.Not_Handled);
            switch (s) {
                case BLE_AppearanceFan.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceFan.Ceiling_Fan:
                case BLE_AppearanceFan.Axial_Fan:
                case BLE_AppearanceFan.Exhaust_Fan:
                case BLE_AppearanceFan.Pedestal_Fan:
                case BLE_AppearanceFan.Desk_Fan:
                case BLE_AppearanceFan.Wall_Fan:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceFan.Not_Handled:
                    default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateHVACCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceHVAC s = sub.FirstOrDefault(BLE_AppearanceHVAC.Not_Handled);
            switch (s) {
                case BLE_AppearanceHVAC.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceHVAC.Thermostat:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceHVAC.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateHeatingCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceHeating s = sub.FirstOrDefault(BLE_AppearanceHeating.Not_Handled);
            switch (s) {
                case BLE_AppearanceHeating.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceHeating.Radiator:
                case BLE_AppearanceHeating.Boiler:
                case BLE_AppearanceHeating.Heat_Pump:
                case BLE_AppearanceHeating.Infrared:
                case BLE_AppearanceHeating.Radiant_Panel:
                case BLE_AppearanceHeating.Heating_Fan:
                case BLE_AppearanceHeating.Heating_Air_Curtain:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceHeating.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }

        private string TranslateAccessControlCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceAccessControl s = sub.FirstOrDefault(BLE_AppearanceAccessControl.Not_Handled);
            switch (s) {
                case BLE_AppearanceAccessControl.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceAccessControl.Access_Door:
                case BLE_AppearanceAccessControl.Garage_Door:
                case BLE_AppearanceAccessControl.Emergency_Exit_Door:
                case BLE_AppearanceAccessControl.Access_Lock:
                case BLE_AppearanceAccessControl.Elevator:
                case BLE_AppearanceAccessControl.Window:
                case BLE_AppearanceAccessControl.Entrance_Gate:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceAccessControl.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateMotorizedDeviceCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceMotorizedDevice s = sub.FirstOrDefault(BLE_AppearanceMotorizedDevice.Not_Handled);
            switch (s) {
                case BLE_AppearanceMotorizedDevice.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceMotorizedDevice.Motorized_Gate:
                case BLE_AppearanceMotorizedDevice.Motorized_Awning:
                case BLE_AppearanceMotorizedDevice.Motorized_Blinds_or_Shades:
                case BLE_AppearanceMotorizedDevice.Motorized_Curtains:
                case BLE_AppearanceMotorizedDevice.Motorized_Screen:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceMotorizedDevice.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslatePowerDeviceCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearancePowerDevice s = sub.FirstOrDefault(BLE_AppearancePowerDevice.Not_Handled);
            switch (s) {
                case BLE_AppearancePowerDevice.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearancePowerDevice.Power_Outlet:
                case BLE_AppearancePowerDevice.Power_Strip:
                case BLE_AppearancePowerDevice.Plug:
                case BLE_AppearancePowerDevice.Power_Supply:
                case BLE_AppearancePowerDevice.LED_Driver:
                case BLE_AppearancePowerDevice.Fluorescent_Lamp_Gear:
                case BLE_AppearancePowerDevice.HID_Lamp_Gear:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearancePowerDevice.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateLightSourceCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceLightSource s = sub.FirstOrDefault(BLE_AppearanceLightSource.Not_Handled);
            switch (s) {
                case BLE_AppearanceLightSource.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceLightSource.Incandescent_Light_Bulb:
                case BLE_AppearanceLightSource.LED_Bulb:
                case BLE_AppearanceLightSource.HID_Lamp:
                case BLE_AppearanceLightSource.Fluorescent_Lamp:
                case BLE_AppearanceLightSource.LED_Array:
                case BLE_AppearanceLightSource.Multi_Color_LED_Array:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceLightSource.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateOximeterCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceOximeter s = sub.FirstOrDefault(BLE_AppearanceOximeter.Not_Handled);
            switch (s) {
                case BLE_AppearanceOximeter.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceOximeter.Fingertip_Oximeter:
                case BLE_AppearanceOximeter.Wrist_Worn_Oximeter:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceOximeter.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }


        private string TranslateOutdoorsSportsActivityCategory(BLE_AppearanceCategory cat, uint sub) {
            BLE_AppearanceOutdoorSportActivity s = sub.FirstOrDefault(BLE_AppearanceOutdoorSportActivity.Not_Handled);
            switch (s) {
                case BLE_AppearanceOutdoorSportActivity.Generic:
                    return string.Format("{0}", cat.ToString().UnderlineToSpaces());
                case BLE_AppearanceOutdoorSportActivity.Sports_Location_Display_Device:
                case BLE_AppearanceOutdoorSportActivity.Sports_Location_and_Navigation_Display_Device:
                case BLE_AppearanceOutdoorSportActivity.Sports_Location_Pod:
                case BLE_AppearanceOutdoorSportActivity.Sports_Location_and_Navigation_Pod:
                    return string.Format("{0}", s.ToString().UnderlineToSpaces());
                case BLE_AppearanceOutdoorSportActivity.Not_Handled:
                default:
                    return string.Format("{0}:{1}", cat.ToString().UnderlineToSpaces(), s.ToString().UnderlineToSpaces());
            }
        }

        #endregion

    }



}
