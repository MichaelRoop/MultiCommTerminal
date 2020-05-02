using System;

namespace BluetoothLE.Net.Enumerations {

    /// <summary>From the SIG to get readable Appearance from the BLE Apppearance Characteristic</summary>
    /// <remarks>
    /// https://www.bluetooth.com/specifications/gatt/characteristics/
    /// </remarks>
    public enum GattAppearanceEnum : UInt16 {
        // None
        Unknown = 0,
        GenericPhone = 64,
        GenericComputer = 128,
        GenericWatch = 192,
        SportsWatch = 193,
        GenericClock = 256,
        GenericDisplay = 320,
        GenericRemoteControl = 384,
        GenericEyeGlasses = 448,
        GenericTag = 512,
        GenericKeyring = 576,
        GenericMediaPlayer = 640,
        GenericBarcodeScanner = 704,
        GenericThermometer = 768,
        EarThermometer = 769,
        GenericHeartRateSensor = 832,
        HeartRateSensorHeartRateBelt = 883,
        GenericBloodPressure = 896,
        BloodPressureArm = 897,
        BloodPressureWrist = 898,
        HumanInterfaceDeviceHid = 960,
        Keyboard = 961,
        Mouse = 962,
        Joystick = 963,
        Gamepad = 964,
        DigitizerTablet = 965,
        CardReader = 966,
        DigitalPen = 967,
        BarcodeScanner = 968,
        GenericGlucoseMeter = 1024,
        GenericRunningWalkingSensor = 1088,
        RunningWalkingSensorInShoe = 1089,
        RunningWalkingSensorOnShoe = 1090,
        RunningWalkingSensorOnHip = 1091,
        GenericCycling = 1152,
        CyclingCyclingComputer = 1153,
        CyclingSpeedSensor = 1154,
        CyclingCadenceSensor = 1155,
        CyclingPowerSensor = 1156,
        CyclingSpeedAndCadenceSensor = 1157,
        GenericPulseOximeter = 3136,
        GenericPulseFingertip = 3137,
        GenericPulseWristWorn = 3138,
        GenericWeightScale = 3200,
        GenericPersonalMobilityDevice = 3264,
        PoweredWheelchair = 3265,
        MobilityScooter = 3266,
        GenericContinuousGlucoseMonitor = 3328,
        GenericInsulinPump = 3392,
        InsulinPumpDurablePump = 3393,
        InsulinPumpPatchPump = 3396,
        InsulinPen = 3400,
        GenericMedicationDelivery = 3456,
        GenericOutdoorSportsActivity = 5184,
        LocationDisplayDevice = 5185,
        LocationAndNavigationDisplayDevice = 5186,
        LocationPod = 5187,
        LocationAndNavigationPod = 5188,
    }


}
