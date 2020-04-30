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
    public class DescClientCharasteristicConfigParser {

        #region Data
        
        private ClassLog log = new ClassLog("DescValueClientCharasteristicConfig");
        private static int RAW_DATA_LEN = 2;

        #endregion

        #region Properties

        public EnabledDisabled Notifications { get; set; } = EnabledDisabled.Disabled;
        public EnabledDisabled Indications { get; set; } = EnabledDisabled.Disabled;
        public byte[] RawData { get; set; } = new byte[RAW_DATA_LEN];
        public ushort ConvertedData { get; set; }

        #endregion

        public DescClientCharasteristicConfigParser() {
            this.ResetMembers();
        }


        public DescClientCharasteristicConfigParser(byte[] data) {
            this.Reset(data);
        }


        /// <summary>
        /// Reset the object with values parsed from the 2 bytes of data retrieved from the Descriptor
        /// </summary>
        /// <param name="data">The 2 bytes of data returned from the OS descriptor</param>
        public void Reset(byte[] data) {
            this.ResetMembers();
            if (data != null) {
                if (data.Length >= RAW_DATA_LEN) {
                    Array.Copy(data, this.RawData, RAW_DATA_LEN);
                    this.ConvertedData = BitConverter.ToUInt16(this.RawData, 0);

                    //   Bit 0 - Notifications, Bit 1 - Indications
                    this.Notifications = (this.ConvertedData.IsBitSet(0)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                    this.Indications = (this.ConvertedData.IsBitSet(1)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
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
        }


        /// <summary>Assemble a string which displays the results of the parsed values</summary>
        /// <example>Return string = 'Notifications:Enabled, Indications:Disabled'</example>
        /// <returns>A display string</returns>
        public string DisplayString() {
            return string.Format("Notifications:{0}, Indications:{1}", this.Notifications.ToString(), this.Indications.ToString());
        }


        private void ResetMembers() {
            this.Notifications = EnabledDisabled.Disabled;
            this.Indications = EnabledDisabled.Disabled;
            this.RawData = new byte[RAW_DATA_LEN];
            this.ConvertedData = 0;
        }


    }
}
