using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {


    /// <summary>
    /// 
    /// </summary>
    /// <remarks>
    ///  Hex values 2 or 4 bytes
    /// ex: 0x020x0D == 2-13
    /// ex: 0x58 0x02 0x20 0x1C == 600 - 7,200 seconds
    /// see: https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.valid_range.xml
    /// 
    /// 
    /// </remarks>
    public class DescParser_ValidRange {

        #region Data

        private ClassLog log = new ClassLog("DescValidRangeValueParser");
        private static int RAW_DATA_MIN_LEN = 2;
        private static int RAW_DATA_MAX_LEN = 4;

        #endregion

        #region Properties

        public ushort Min { get; set; } = 0;
        public ushort Max { get; set; } = 0;

        public byte[] RawData { get; set; } = new byte[RAW_DATA_MAX_LEN];
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

        public string Parse(byte[] data) {
            this.ResetMembers();

            // TODO - revisit this. Seems that each of the two values are based on the kind of 
            // Characteristic it is attached to
            // Hex values 2 or 4 bytes
            // ex: 0x020x0D == 2-13
            // ex: 0x58 0x02 0x20 0x1C == 600 - 7,200 seconds
            // see: https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.valid_range.xml

            if (data != null) {
                if (data.Length >= RAW_DATA_MAX_LEN) {
                    this.RawData = new byte[RAW_DATA_MAX_LEN];
                    Array.Copy(data, this.RawData, this.RawData.Length);
                    this.ConvertedData = BitConverter.ToUInt32(this.RawData, 0);
                    // TODO convert from hex
                    this.Min = BitConverter.ToUInt16(this.RawData, 0);
                    this.Max = BitConverter.ToUInt16(this.RawData, 2);
                    // TODO - lot more work to do. Exponents lookup etc. see spec
                    this.log.Info("Parse", () => string.Format("Data:{0}", this.RawData.ToAsciiString()));
                    this.log.Info("Parse", () => string.Format("Display:{0}", this.DisplayString()));
                }
                else if (data.Length >= RAW_DATA_MIN_LEN) {
                    // Two 1 byte numbers
                    this.RawData = new byte[RAW_DATA_MIN_LEN];
                    Array.Copy(data, this.RawData, this.RawData.Length);
                    this.ConvertedData = BitConverter.ToUInt16(this.RawData, 0);
                    this.Min = this.RawData[0];
                    this.Max = this.RawData[1];
                }
                else {
                    this.log.Error(9999, "Reset", () => string.Format("byte[] length {0} is less than 2", data.Length));
                }
            }
            else {
                this.log.Error(9999, "Reset", "Raw byte[] is null");
            }
            return this.DisplayString();
        }


        public string DisplayString() {
            return string.Format("Min:{0} Max:{1}", this.Min, this.Max);
        }



        private void ResetMembers() {
            this.Min = 0;
            this.Max = 0;
            this.RawData = new byte[RAW_DATA_MAX_LEN];
            this.ConvertedData = 0;
        }


    }
}
