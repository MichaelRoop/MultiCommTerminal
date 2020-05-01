using LogUtils.Net;
using System;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>Client Characteristic Configuration Descriptor value parser</summary>
    /// <remarks>
    /// Uint16 data: Values 0-3
    ///   Bit 0 - Notifications disabled/enabled
    ///   Bit 1 - Indications disabled/enabled
    ///   Other bits reserved for future use
    /// </remarks>
    public class DescClientCharasteristicConfigParser : DescParser_Base {

        #region Data
        
        private readonly ClassLog log = new ClassLog("DescClientCharasteristicConfigParser");
        private readonly int RAW_DATA_LEN = 2;

        #endregion

        #region Properties

        public EnabledDisabled Notifications { get; set; } = EnabledDisabled.Disabled;
        public EnabledDisabled Indications { get; set; } = EnabledDisabled.Disabled;
        public ushort ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescClientCharasteristicConfigParser() : base() {
        }


        public DescClientCharasteristicConfigParser(byte[] data) : base(data) {
        }

        #endregion

        #region Overrides from DescParser_Base

        /// <summary>Assemble a string which displays the results of the parsed values</summary>
        /// <example>Return string = 'Notifications:Enabled, Indications:Disabled'</example>
        /// <returns>A display string</returns>
        protected override string DoDisplayString() {
            return string.Format("Notifications:{0}, Indications:{1}", this.Notifications.ToString(), this.Indications.ToString());
        }


        protected override bool DoParse(byte[] data) {
            this.log.InfoEntry("DoParse");
            if (this.CopyToRawData(data, RAW_DATA_LEN)) {
                this.ConvertedData = BitConverter.ToUInt16(this.RawData, 0);

                //   Bit 0 - Notifications, Bit 1 - Indications
                this.Notifications = (this.ConvertedData.IsBitSet(0)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                this.Indications = (this.ConvertedData.IsBitSet(1)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                this.log.Info("DoParse", () => string.Format("Display:{0}", this.DisplayString()));
                return true;
            }
            return false;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }


        protected override  void ResetMembers() {
            this.Notifications = EnabledDisabled.Disabled;
            this.Indications = EnabledDisabled.Disabled;
            this.ConvertedData = 0;
        }

        #endregion

    }
}
