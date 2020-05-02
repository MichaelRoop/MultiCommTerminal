using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using System;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// Parse the 16bit value from the Server Characteristic Config Descriptor
    /// (0x2903) Data type: uint16
    /// </summary>
    public class DescParser_ServerCharacteristicConfig : DescParser_Base {

        #region Data

        private ClassLog log = new ClassLog("DescParser_ServerCharacteristicConfig");

        #endregion

        #region Properties

        public EnabledDisabled Broadcasts { get; set; } = EnabledDisabled.Disabled;
        public ushort ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescParser_ServerCharacteristicConfig() : base() { }

        public DescParser_ServerCharacteristicConfig(byte[] data) : base(data) { }

        #endregion

        #region Overrides from DescParser_Base

        /// <summary>
        /// Reset the object with values parsed from the 2 bytes of data retrieved from the Descriptor
        /// </summary>
        /// <param name="data">The 2 bytes of data returned from the OS descriptor</param>
        protected override bool DoParse(byte[] data) {
            if (this.CopyToRawData(data, UINT16_LEN)) {
                this.ConvertedData = BitConverter.ToUInt16(this.RawData, 0);
                //   Bit 0 - Broadcasts. Others reserved
                this.Broadcasts = (this.ConvertedData.IsBitSet(0)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                this.log.Info("Reset", () => string.Format("Display:{0}", this.DisplayString()));
                return true;
            }
            return false;
        }


        /// <summary>Assemble a string which displays the results of the parsed values</summary>
        /// <example>"Broadcasts:Enabled"</example>
        /// <returns>A display string</returns>
        protected override string DoDisplayString() {
            return string.Format("Broadcasts:{0}", this.Broadcasts.ToString());
        }


        protected override void ResetMembers() {
            this.Broadcasts = EnabledDisabled.Disabled;
            this.ConvertedData = 0;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }

        #endregion
    }
}
