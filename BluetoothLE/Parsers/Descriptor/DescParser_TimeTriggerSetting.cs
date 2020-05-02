using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using System;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// TODO - implement
    /// 
    /// (0x290E) Data type:
    ///     1. Condition: uint8
    ///     2. Value
    ///         Condition: 0 (None) - no comparison required (Digital or Analog)
    ///             uint8
    ///         Condition: 1,2 (TimeIntervalContinuousAfterSettableTime, TimeIntervalOnTimeExpiredOrDifferentState) 
    ///             uint24 (3 bytes) (org.bluetooth.unit.time.second)
    ///         Condition: 3 (Count) 
    ///             uint16
    /// </summary>
    public class DescParser_TimeTriggerSetting : DescParser_Base {

        #region Data

        private ClassLog log = new ClassLog("DescParser_TimeTriggerSetting");

        #endregion

        #region Properties

        public TimeTriggerCondition Condition { get; set; } = TimeTriggerCondition.None;
        public ushort Count { get; set; }
        public byte NoneValue { get; set; }

        public uint TimeInterval { get; set; }

        public bool IsValid { get; set; } = false;

        #endregion

        #region Constructors

        public DescParser_TimeTriggerSetting() : base() { }
        public DescParser_TimeTriggerSetting(byte[] data) : base(data) { }

        #endregion

        protected override string DoDisplayString() {
            if (this.IsValid) {
                switch (this.Condition) {
                    case TimeTriggerCondition.None:
                        return string.Format("Time Trigger Condition:{0} Value:-", this.Condition.ToString());
                    case TimeTriggerCondition.Count:
                        return string.Format("Time Trigger Condition:{0} Value:{1}", this.Condition.ToString(), this.Count);
                    case TimeTriggerCondition.TimeIntervalContinuousAfterSettableTime:
                    case TimeTriggerCondition.TimeIntervalOnTimeExpiredOrDifferentState:
                        return string.Format("Time Trigger Condition:{0} Value:{1}", this.Condition.ToString(), this.TimeInterval);
                    default:
                        return string.Format("Time Trigger Condition:{0} UNHANDLED", this.Condition.ToString());
                }
            }
            else {
                return "* N/A *";
            }
        }

        protected override bool DoParse(byte[] data) {
            // first need to read the condition to determine the values size
            if (this.CopyToRawData(data, BYTE_LEN)) {
                int readSize = BYTE_LEN;
                if (this.RawData[0] <= (byte)TimeTriggerCondition.Count) {
                    this.Condition = (TimeTriggerCondition)this.RawData[0];
                    readSize += this.GetValueSize(this.Condition);
                    if (this.CopyToRawData(data, readSize)) {
                        this.SetValue(this.Condition, data);
                        this.IsValid = true;
                        return true;
                    }
                }
                else {
                    this.log.Error(9999, "", () => string.Format("Condition {0} not handled", this.RawData[0]));
                }
            }
            return false;
        }

        protected override Type GetDerivedType() {
            return this.GetType();
        }

        protected override void ResetMembers() {
            this.Condition = TimeTriggerCondition.None;
            this.Count = 0;
            this.TimeInterval = 0;
            this.NoneValue = 0;
            this.IsValid = false;
        }


        private int GetValueSize(TimeTriggerCondition triggerType) {
            switch (triggerType) {
                case TimeTriggerCondition.Count:
                    return UINT16_LEN;
                case TimeTriggerCondition.None:
                    return BYTE_LEN;
                case TimeTriggerCondition.TimeIntervalContinuousAfterSettableTime:
                case TimeTriggerCondition.TimeIntervalOnTimeExpiredOrDifferentState:
                    return TIMESECOND_LEN;
                default:
                    // should never happen. just to satisfy compiler
                    return BYTE_LEN;
            }
        }


        private void SetValue(TimeTriggerCondition triggerType, byte[] data) {
            switch (triggerType) {
                case TimeTriggerCondition.Count:
                    this.Count = BitConverter.ToUInt16(data, 1);
                    break;
                case TimeTriggerCondition.None:
                    this.NoneValue = data[1];
                    break;
                case TimeTriggerCondition.TimeIntervalContinuousAfterSettableTime:
                case TimeTriggerCondition.TimeIntervalOnTimeExpiredOrDifferentState:
                    // TODO How to copy the 3 bytes for the BLE seconds time data
                    this.TimeInterval = 9999;
                    break;
                default:
                    // should never happen. just to satisfy compiler
                    break;
            }
        }



    }
}
