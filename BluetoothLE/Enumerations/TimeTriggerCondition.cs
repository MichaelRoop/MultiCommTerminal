
namespace BluetoothLE.Net.Enumerations {

    /// <summary>
    /// Condition for descriptor parser Time Trigger 0x290E (
    /// </summary>
    public enum TimeTriggerCondition : byte {

        /// <summary>
        /// No time-based triggering used (valid for: Digital, Analog)" requires="C1"
        /// </summary>
        None = 0,
        /// <summary>
        /// Indicates or notifies unconditionally after a settable time. This condition 
        /// will cause server to periodically send notification or indication for the 
        /// corresponding characteristic regardless of the Value Trigger state (valid 
        /// for: Digital, Analog)" requires="C2"
        /// </summary>
        TimeIntervalContinuousAfterSettableTime = 1,
        /// <summary>
        /// Not indicated or notified more often than a settable time. After a successful 
        /// indication or notification, the next indication or notification shall not be 
        /// sent for the Time Interval time. When the Time Interval expires, the 
        /// characteristic is indicated or notified If the corresponding Value Trigger has 
        /// a different state than at the time of the last successful indication or 
        /// notification (valid for: Digital, Analog)" requires="C2"
        /// </summary>
        TimeIntervalOnTimeExpiredOrDifferentState = 2,
        /// <summary>
        /// Changed more often than. This condition will cause server to count number of 
        /// times the Value Trigger has changed its state and send the notification or 
        /// indication for the corresponding characteristic on the “count” occurrence of 
        /// the state change (valid for: Digital, Analog)" requires="C3"
        /// </summary>
        Count = 3,

        // rest are reserved

    }
}
