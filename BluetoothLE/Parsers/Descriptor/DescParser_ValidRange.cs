using LogUtils.Net;
using System;

namespace BluetoothLE.Net.Parsers.Descriptor {


    /// <summary>
    /// Parse value returned from Valid Range Descriptor
    /// (0x2906) Data type: uint16 or uint32
    /// </summary>
    /// <remarks>
    ///  Hex values 2 or 4 bytes
    /// ex: 0x020x0D == 2-13
    /// ex: 0x58 0x02 0x20 0x1C == 600 - 7,200 seconds
    /// see: https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.valid_range.xml
    /// 
    /// 
    /// </remarks>
    public class DescParser_ValidRange : DescParser_Base {

        #region Data

        private ClassLog log = new ClassLog("DescParser_ValidRange");

        #endregion

        #region Properties

        public ushort Min { get; set; } = 0;
        public ushort Max { get; set; } = 0;
        public uint ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescParser_ValidRange() {
            this.ResetMembers();
        }


        public DescParser_ValidRange(byte[] data) {
            this.Parse(data);
        }

        #endregion

        #region Overrides from DescParser_Base

        protected override bool DoParse(byte[] data) {
            this.ResetMembers();

            // TODO - revisit this. Seems that each of the two values are based on the kind of 
            // Characteristic it is attached to
            // Hex values 2 or 4 bytes
            // ex: 0x020x0D == 2-13
            // ex: 0x58 0x02 0x20 0x1C == 600 - 7,200 seconds
            // see: https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.valid_range.xml
            int count = (data.Length >= UINT32_LEN) ? UINT32_LEN : UINT16_LEN;
            if (this.CopyToRawData(data, count)) {
                if (count == UINT32_LEN) {
                    this.ConvertedData = BitConverter.ToUInt32(this.RawData, 0);
                    // TODO convert from hex
                    this.Min = BitConverter.ToUInt16(this.RawData, 0);
                    this.Max = BitConverter.ToUInt16(this.RawData, 2);
                    // TODO - lot more work to do. Exponents lookup etc. see spec
                    this.log.Info("DoParse", () => string.Format("Display:{0}", this.DisplayString()));
                }
                else {
                    // Two 1 byte numbers
                    this.ConvertedData = BitConverter.ToUInt16(this.RawData, 0);
                    this.Min = this.RawData[0];
                    this.Max = this.RawData[1];
                }
                return true;
            }
            return false;
        }


        protected override string DoDisplayString() {
            return string.Format("Min:{0} Max:{1}", this.Min, this.Max);
        }


        protected override void ResetMembers() {
            this.Min = 0;
            this.Max = 0;
            this.ConvertedData = 0;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }

        #endregion

    }
}
