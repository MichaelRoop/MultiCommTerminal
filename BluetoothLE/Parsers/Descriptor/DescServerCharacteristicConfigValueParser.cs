using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// Parse the 16bit value from the Server Characteristic Config Descriptor
    /// </summary>
    public class DescServerCharacteristicConfigValueParser {

        #region Data

        private ClassLog log = new ClassLog("DescServerCharacteristicConfigValueParser");
        private static int RAW_DATA_LEN = 2;

        #endregion

        #region Properties

        public EnabledDisabled Broadcasts { get; set; } = EnabledDisabled.Disabled;
        public byte[] RawData { get; set; } = new byte[RAW_DATA_LEN];
        public ushort ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescServerCharacteristicConfigValueParser() {
            this.ResetMembers();
        }

        public DescServerCharacteristicConfigValueParser(byte[] data) {
            this.Parse(data);
        }

        #endregion

        #region Public

        /// <summary>
        /// Reset the object with values parsed from the 2 bytes of data retrieved from the Descriptor
        /// </summary>
        /// <param name="data">The 2 bytes of data returned from the OS descriptor</param>
        public string Parse(byte[] data) {
            this.ResetMembers();
            if (data != null) {
                if (data.Length >= RAW_DATA_LEN) {
                    Array.Copy(data, this.RawData, RAW_DATA_LEN);
                    this.ConvertedData = BitConverter.ToUInt16(this.RawData, 0);

                    //   Bit 0 - Broadcasts. Others reserved
                    this.Broadcasts = (this.ConvertedData.IsBitSet(0)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                    this.log.Info("Reset", () => string.Format("Data:{0}", this.RawData.ToAsciiString()));
                    this.log.Info("Reset", () => string.Format("Display:{0}", this.DisplayString()));
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
        /// <example>"Broadcasts:Enabled"</example>
        /// <returns>A display string</returns>
        public string DisplayString() {
            return string.Format("Broadcasts:{0}", this.Broadcasts.ToString());
        }

        #endregion

        private void ResetMembers() {
            this.Broadcasts = EnabledDisabled.Disabled;
            this.RawData = new byte[RAW_DATA_LEN];
            this.ConvertedData = 0;
        }
    }
}
