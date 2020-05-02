using BluetoothLE.Net.interfaces;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LogUtils.Net;
using System;

namespace BluetoothLE.Net.Parsers.Descriptor {

    public class DescParserFactory : IDescParserFactory {

        #region Data

        private ClassLog log = new ClassLog("DescParserFactory");
        private DescParser_CharacteristicAggregateFormat aggregateFormat = new DescParser_CharacteristicAggregateFormat();
        private DescParser_CharacteristicExtendedProperties extendedProperties = new DescParser_CharacteristicExtendedProperties();
        private DescParser_ClientCharacteristicConfig clientConfig = new DescParser_ClientCharacteristicConfig();
        private DescParser_Default defaultParser = new DescParser_Default();
        private DescParser_NumberDigitals numberDecimals = new DescParser_NumberDigitals();
        private DescParser_ReportReference reportReference = new DescParser_ReportReference();
        private DescParser_ServerCharacteristicConfig serverConfig = new DescParser_ServerCharacteristicConfig();
        private DescParser_UserDescription userDescription = new DescParser_UserDescription();
        private DescParser_ValidRange validRange = new DescParser_ValidRange();
        private DescParser_TimeTriggerSetting timeTriggerSettings = new DescParser_TimeTriggerSetting();
        private DescParser_PresentationFormat presentationFormat = new DescParser_PresentationFormat();

        #endregion


        public string GetParsedValueAsString(Guid descriptorUuid, byte[] value) {
            IDescParser parser = this.GetParser(descriptorUuid);
            if (parser == null) {
                return "* Failed to retrieve parser *";
            }
            try {
                return parser.Parse(value);
            }
            catch(Exception e) {
                this.log.Exception(9999, "GetParsedValueAsString", "", e);
                return "* FAILED ON DESCRIPTOR VALUE PARSE *";
            }
        }

        public IDescParser GetParser(Guid descriptorUuid) {
            IDescParser parser = null;
            ErrReport report;
            parser = WrapErr.ToErrReport<IDescParser>(out report, 9999, 
                () => string.Format("Failed to find descriptor parser"), 
                () => {
                    if (BLE_ParseHelpers.IsSigDefinedUuid(descriptorUuid)) {
                        GattNativeDescriptorUuid descriptorEnum;
                        if (Enum.TryParse(descriptorUuid.ToShortId().ToString(), out descriptorEnum)) {
                            switch (descriptorEnum) {
                                case GattNativeDescriptorUuid.CharacteristicAggregateFormat:
                                    return this.aggregateFormat;
                                case GattNativeDescriptorUuid.CharacteristicExtendedProperties:
                                    return this.extendedProperties;
                                case GattNativeDescriptorUuid.CharacteristicUserDescription:
                                    return this.userDescription;
                                case GattNativeDescriptorUuid.ClientCharacteristicConfiguration:
                                    return this.clientConfig;
                                case GattNativeDescriptorUuid.EnvironmentalSensingConfiguration:
                                case GattNativeDescriptorUuid.EnvironmentalSensingMeasurement:
                                case GattNativeDescriptorUuid.EnvironmentalSensingTriggerSetting:
                                case GattNativeDescriptorUuid.ExternalReportReference:
                                case GattNativeDescriptorUuid.ValueTriggerSetting:
                                    return this.defaultParser;
                                    // TODO Make parsers for above
                                case GattNativeDescriptorUuid.CharacteristicPresentationFormat:
                                    return this.presentationFormat;
                                case GattNativeDescriptorUuid.ServerCharacteristicConfiguration:
                                    return this.serverConfig;
                                case GattNativeDescriptorUuid.TimeTriggerSetting:
                                    return this.timeTriggerSettings;
                                case GattNativeDescriptorUuid.ValidRange:
                                    return this.validRange;
                                case GattNativeDescriptorUuid.ReportReference:
                                    return this.reportReference;
                                case GattNativeDescriptorUuid.NumberOfDigitals:
                                    return this.numberDecimals;
                                default:
                                    return this.defaultParser;
                            }
                        }
                        else {
                            this.log.Error(9999, "GetParser", () => 
                                string.Format("Failed to parse out Guid:{0}", descriptorUuid.ToString()));
                            return this.defaultParser;
                        }
                    }
                    else {
                        this.log.Error(9999, "GetParser", () =>
                            string.Format("Sig not defined:{0}", descriptorUuid.ToString()));
                        return this.defaultParser;
                    }
            });
            return report.Code == 0 ? parser : null;
        }
    }
}
