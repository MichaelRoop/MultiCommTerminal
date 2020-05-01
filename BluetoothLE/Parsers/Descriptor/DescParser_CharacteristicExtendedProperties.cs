using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>Parse value from Characteristic Extentended Properties descriptor</summary>
    /// <remarks>
    /// https://www.bluetooth.com/xml-viewer/?src=https://www.bluetooth.com/wp-content/uploads/Sitecore-Media-Library/Gatt/Xml/Descriptors/org.bluetooth.descriptor.gatt.characteristic_extended_properties.xml
    /// </remarks>
    public class DescParser_CharacteristicExtendedProperties {

        #region Data

        private ClassLog log = new ClassLog("DescParser_CharacteristicExtendedProperties");
        private static int RAW_DATA_LEN = 2;

        #endregion

        #region Properties

        public EnabledDisabled ReliableWrite { get; set; } = EnabledDisabled.Disabled;
        public EnabledDisabled ReliableAuxiliary { get; set; } = EnabledDisabled.Disabled;
        public byte[] RawData { get; set; } = new byte[RAW_DATA_LEN];
        public ushort ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescParser_CharacteristicExtendedProperties() {
            this.ResetMembers();
        }


        public DescParser_CharacteristicExtendedProperties(byte[] data) {
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

                    //   Bit 0 - Reliable Write. Bit 1 Reliable Auxiliary. Others reserved
                    this.ReliableWrite = (this.ConvertedData.IsBitSet(0)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
                    this.ReliableAuxiliary = (this.ConvertedData.IsBitSet(1)) ? EnabledDisabled.Enabled : EnabledDisabled.Disabled;
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
            return string.Format("Reliable Write:{0} Reliable Auxiliary:{1}", 
                this.ReliableWrite.ToString(), this.ReliableAuxiliary.ToString());
        }

        #endregion

        #region Private

        private void ResetMembers() {
            this.ReliableWrite = EnabledDisabled.Disabled;
            this.ReliableAuxiliary = EnabledDisabled.Disabled;
            this.RawData = new byte[RAW_DATA_LEN];
            this.ConvertedData = 0;
        }

        #endregion

    }
}
