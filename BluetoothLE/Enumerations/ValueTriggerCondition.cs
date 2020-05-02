
namespace BluetoothLE.Net.Enumerations {

    /// <summary>Used for the Value Trigger Descriptor first field for condition</summary>
    /// <remarks>TODO Need to study each one more</remarks>
    public enum ValueTriggerCondition {

        /// <summary>
        /// The state is changed if the characteristic value is changed.
        /// (valid for: Digital, Analog)
        /// </summary>
        None = 0,
        /// <summary>
        /// Crossed a boundary. The state is changed if the value of 
        /// the analog characteristic changes from less than to greater 
        /// than a settable Analog value, or from greater than to less 
        /// than a settable Analog value (valid for: Analog)" requires="C2"
        /// </summary>
        AnalogCrossedBoundry = 1,
        /// <summary>
        /// On the boundary. The state is changed if the value of an analog 
        /// characteristic changes from less than to equal to a settable 
        /// Analog value, or from greater than to equal to a settable 
        /// Analog value, or from equal to to less than or greater than a 
        /// settable Analog value (valid for: Analog)" requires="C2"
        /// </summary>
        AnalogReturnedToBoundry = 2,
        /// <summary>
        /// The state is changed if the value of the analog characteristic 
        /// is changed more than a settable Analog value (valid for: Analog)" 
        /// requires="C2"
        /// </summary>
        AnalogStateChanged = 3,
        /// <summary>
        /// Mask then compare (logical-and of the Digital Input and the Bit Mask, 
        /// condition is true if the result of this is different from the last stet) 
        /// (valid for: Digital)" requires="C3"
        /// </summary>
        BitMask = 4,
        /// <summary>
        /// Inside or outside the boundaries. The state is changed if the value of 
        /// the analog characteristic changes from less than a settable Analog One 
        /// value and greater than a settable Analog Two value to greater than a 
        /// settable Analog One value or less than a settable Analog Two value 
        /// (valid for: Analog)" requires="C4"
        /// </summary>
        AnalogIntervalInsideOutsideBoundries = 5,
        /// <summary>
        /// On the boundaries. The state is changed if the value of the analog 
        /// characteristic changes from equal to a settable Analog One value or 
        /// settable Analog Two value to any other value (valid for: Analog)" 
        /// requires="C4"
        /// </summary>
        AnalogIntervalOnBoundry = 6,
        /// <summary>
        /// No value trigger. This condition causes no state change regardless 
        /// if the characteristic value changes. It can be used for example when 
        /// the value of one or more characteristic should not cause indication or 
        /// notification of the Aggregate characteristic (valid for: Digital, Analog)"
        /// </summary>
        NoneNoValueTriggerEvenIfChanged = 7,


    }
}
