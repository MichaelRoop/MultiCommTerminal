using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using System;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// Parse value from Characteristic Extentended Properties descriptor.
    /// (0x2900) Data type: uint16
    /// </summary>
    /// <remarks>
    /// https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.gatt.characteristic_extended_properties.xml
    /// </remarks>
    public class DescParser_CharacteristicExtendedProperties : DescParser_Base {

        #region Data

        private ClassLog log = new ClassLog("DescParser_CharacteristicExtendedProperties");

        #endregion

        #region Properties

        public EnabledDisabled ReliableWrite { get; set; } = EnabledDisabled.Disabled;
        public EnabledDisabled ReliableAuxiliary { get; set; } = EnabledDisabled.Disabled;
        public ushort ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescParser_CharacteristicExtendedProperties() : base() { }


        public DescParser_CharacteristicExtendedProperties(byte[] data) : base(data) { }

        #endregion

        #region Overrides from DescParser_Base

        /// <summary>
        /// Reset the object with values parsed from the 2 bytes of data retrieved from the Descriptor
        /// </summary>
        /// <param name="data">The 2 bytes of data returned from the OS descriptor</param>
        protected override bool DoParse(byte[] data) {
            if (this.CopyToRawData(data, UINT16_LEN)) {
                this.ConvertedData = BitConverter.ToUInt16(this.RawData, 0);

                //   Bit 0 - Reliable Write. Bit 1 Reliable Auxiliary. Others reserved
                this.ReliableWrite = (this.ConvertedData.IsBitSet(0)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                this.ReliableAuxiliary = (this.ConvertedData.IsBitSet(1)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                this.log.Info("Reset", () => string.Format("Display:{0}", this.DisplayString()));
                return true;
            }
            return false;
        }


        /// <summary>Assemble a string which displays the results of the parsed values</summary>
        /// <example>"Broadcasts:Enabled"</example>
        /// <returns>A display string</returns>
        protected override string DoDisplayString() {
            return string.Format("Reliable Write:{0} Reliable Auxiliary:{1}", 
                this.ReliableWrite.ToString(), this.ReliableAuxiliary.ToString());
        }


        protected override void  ResetMembers() {
            this.ReliableWrite = EnabledDisabled.Disabled;
            this.ReliableAuxiliary = EnabledDisabled.Disabled;
            this.ConvertedData = 0;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }

        #endregion

    }
}
