using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// List of attribute handles
    /// (0x2905) Data type: List<uint16></uint16>
    /// </summary>
    public class DescParser_CharacteristicAggregateFormat :DescParser_Base {

        #region Data

        private ClassLog log = new ClassLog("DescParser_CharacteristicAggregateFormat");

        #endregion

        #region Properties

        /// <summary>List of attribute handles</summary>
        public List<ushort> AttributeHandles { get; set; } = new List<ushort>();

        #endregion

        #region Constructors

        public DescParser_CharacteristicAggregateFormat() :base() {
        }


        public DescParser_CharacteristicAggregateFormat(byte[] data) : base(data) {
        }

        #endregion

        #region Overrides from DescParser_Base 

        /// <summary>
        /// Reset with values parsed from the bytes retrieved from the Descriptor
        /// </summary>
        /// <param name="data">The bytes of data returned from the OS descriptor</param>
        protected override bool DoParse(byte[] data) {
            int count = data.Length / UINT16_LEN;
            if (this.CopyToRawData(data, (count * UINT16_LEN))) {
                for (int i = 0; i < count; i++) {
                    this.AttributeHandles.Add(
                        BitConverter.ToUInt16(this.RawData, i * UINT16_LEN));
                }
                return true;
            }
            return false;
        }


        /// <summary>Assemble a string which displays the results of the parsed values</summary>
        /// <example>"21,455,22"</example>
        /// <returns>A display string</returns>
        protected override string DoDisplayString() {
            StringBuilder sb = new StringBuilder();
            bool first = true;
            foreach(ushort val in this.AttributeHandles) {
                if (first) {
                    sb.Append(",");
                    first = false;
                }
                sb.Append(val);
            }
            return sb.ToString();
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }


        protected override void ResetMembers() {
            this.AttributeHandles = new List<ushort>();
        }

        #endregion

    }
}
