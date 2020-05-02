using LogUtils.Net;
using System;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// 
    /// Field 1: Name:Format. uint8 values 0-27. Enumeration  Others reserved
    /// Field 2: Name:Exponent. signed int8
    /// Field 3: Name:Unit. uint16 (Guid)
    /// Field 4: Name: Namespace. uint8. 0-1 Enumeration (0 not set, 1 Bluetooth SIG Assigned Numbers)
    /// Field 5: Name: Description. uint16. Enumeration
    ///             https://www.bluetooth.com/specifications/assigned-numbers/gatt-namespace-descriptors/
    ///             // Way to many to put in an enum
    /// (0x2904)
    /// </summary>
    public class DescParser_PresentationFormat : DescParser_Base {

        #region Data

        private ClassLog log = new ClassLog("DescParser_PresentationFormat");

        #endregion

        #region Properties

        /// <summary>
        /// The data format as enumeration:
        /// https://www.bluetooth.com/specifications/assigned-numbers/format-types/
        /// </summary>
        public DataFormatEnum Format { get; set; }

        /// <summary>The exponent</summary>
        public sbyte Exponent { get; set; }

        /// <summary>
        /// International units of measurement in a BLE SIG enumeration:
        /// https://www.bluetooth.com/specifications/assigned-numbers/units/
        /// </summary>
        public ushort Unit { get; set; }

        /// <summary>
        /// Currently only 1 namespace which is Bluetooth SIG
        /// https://www.bluetooth.com/specifications/assigned-numbers/gatt-namespace-descriptors/
        /// </summary>
        public byte Namespace { get; set; }

        /// <summary>
        /// The BLE SIG description enumeration
        /// https://www.bluetooth.com/specifications/assigned-numbers/gatt-namespace-descriptors/
        /// </summary>
        public ushort Description { get; set; }

        #endregion

        #region Constructors

        public DescParser_PresentationFormat() : base() { }
        public DescParser_PresentationFormat(byte[] data) : base(data) { }

        #endregion

        #region DescParser_Base overrides

        protected override string DoDisplayString() {
            return string.Format(
                "Format:{0} Exponent:{1} Unit:{2} Namespace:{3} Description Enum:{4}",
                this.Format, 
                this.Exponent, 
                this.Unit, 
                this.Namespace == 1 ? "Bluetooth SIG (1)" : this.Namespace.ToString(), 
                this.Description);
        }


        protected override bool DoParse(byte[] data) {
            // Length of 5 fields
            int len = 
                BYTE_LEN +      // Format
                BYTE_LEN +      // Exponent
                UINT16_LEN +    // Unit
                BYTE_LEN +      // Namespace
                UINT16_LEN;     // Description
            if (this.CopyToRawData(data, len)) {
                this.Format = this.GetFormat(this.RawData[0]);
                this.Exponent = (sbyte)this.RawData[1];
                this.Unit = BitConverter.ToUInt16(this.RawData, 2);
                this.Namespace = this.RawData[4];
                this.Description = BitConverter.ToUInt16(this.RawData, 5);
                return true;
            }
            return false;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }


        protected override void ResetMembers() {
            this.Format = DataFormatEnum.Reserved;
            this.Exponent = 0;
            this.Unit = 0;
            this.Namespace = 0;
            this.Description = 0;
        }

        #endregion

        #region Private

        private DataFormatEnum GetFormat(byte value) {
            if (value > (byte)DataFormatEnum.OpaqueStructure) {
                this.log.Error(9999, "GetFormat", () => string.Format("Format:{0} not handled", value));
                return DataFormatEnum.Unhandled;
            }
            return (DataFormatEnum)this.RawData[0];
        }

        #endregion

    }
}
