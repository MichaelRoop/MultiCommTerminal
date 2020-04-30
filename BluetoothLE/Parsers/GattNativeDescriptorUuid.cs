namespace BluetoothLE.Net.Parsers {

    /// <summary>
    ///     This enum assists in finding a string representation of a BT SIG assigned value for Descriptor UUIDs
    ///     Reference: https://developer.bluetooth.org/gatt/descriptors/Pages/DescriptorsHomePage.aspx
    ///     Reference: https://www.bluetooth.com/specifications/gatt/descriptors/
    /// </summary>
    public enum GattNativeDescriptorUuid : ushort {
        CharacteristicExtendedProperties = 0x2900,
        CharacteristicUserDescription = 0x2901,
        ClientCharacteristicConfiguration = 0x2902,
        ServerCharacteristicConfiguration = 0x2903,
        CharacteristicPresentationFormat = 0x2904,
        CharacteristicAggregateFormat = 0x2905,
        ValidRange = 0x2906,
        ExternalReportReference = 0x2907,
        ReportReference = 0x2908,
        // NEW
        NumberOfDigitals = 0X2909,
        ValueTriggerSetting = 0X290A,
        EnvironmentalSensingConfiguration = 0X290B,
        EnvironmentalSensingMeasurement = 0X290C,
        EnvironmentalSensingTriggerSetting = 0X290D,
        TimeTriggerSetting = 0X290E,
    }

}
