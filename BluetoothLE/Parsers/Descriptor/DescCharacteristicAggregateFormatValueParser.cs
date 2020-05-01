using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothLE.Net.Parsers.Descriptor {

    public class DescCharacteristicAggregateFormatValueParser {

        #region Data

        private ClassLog log = new ClassLog("DescCharacteristicAggregateFormatValueParser");
        private static int RAW_DATA_UNIT_LEN = 2;

        #endregion

        #region Properties

        /// <summary>List of attribute handles</summary>
        public List<ushort> AttributeHandles { get; set; } = new List<ushort>();

        public byte[] RawData { get; set; } = new byte[RAW_DATA_UNIT_LEN];

        #endregion

        #region Constructors

        public DescCharacteristicAggregateFormatValueParser() {
            this.ResetMembers();
        }


        public DescCharacteristicAggregateFormatValueParser(byte[] data) {
            this.Parse(data);
        }

        #endregion

        #region Public 

        /// <summary>
        /// Reset with values parsed from the bytes retrieved from the Descriptor
        /// </summary>
        /// <param name="data">The bytes of data returned from the OS descriptor</param>
        public string Parse(byte[] data) {
            this.ResetMembers();
            if (data != null) {
                if (data.Length >= RAW_DATA_UNIT_LEN) {
                    int count = data.Length / 2;
                    this.RawData = new byte[count * 2];
                    Array.Copy(data, this.RawData, this.RawData.Length);
                    for (int i = 0; i < count; i++) {
                        this.AttributeHandles.Add(BitConverter.ToUInt16(data, i * RAW_DATA_UNIT_LEN));
                    }
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


        /// <summary>Assemble a string which displays the results of the parsed values</summary>
        /// <example>"21,455,22"</example>
        /// <returns>A display string</returns>
        public string DisplayString() {
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

        #endregion

        private void ResetMembers() {
            this.AttributeHandles = new List<ushort>();
            this.RawData = new byte[RAW_DATA_UNIT_LEN];
        }


    }
}
