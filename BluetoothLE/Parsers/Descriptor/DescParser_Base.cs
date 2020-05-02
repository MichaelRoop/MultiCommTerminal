using BluetoothLE.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {
    public abstract class DescParser_Base : IDescParser {

        #region Data

        private ClassLog baseLog = new ClassLog("DescParser_Base");

        /// <summary>Number of bytes in byte field</summary>
        protected readonly int BYTE_LEN = 1;
        /// <summary>Number of bytes for uint16 field</summary>
        protected readonly int UINT16_LEN = sizeof(ushort);
        /// <summary>Number of bytes for uint32 field</summary>
        protected readonly int UINT32_LEN = sizeof(uint);
        /// <summary>Number of bytes for time second field</summary>
        protected readonly int TIMESECOND_LEN = 3; // 3 bytes, 24bit


        #endregion

        #region IDescParser Propertis and methods

        /// <summary>Raw bytes as returned from Descriptor retrieval</summary>
        public byte[] RawData { get; private set; } = new byte[0];

        /// <summary>Type of derived class to determine cast for specific Property with data</summary>
        public Type ImplementationType { get; private set; } = typeof(DescParser_Base);


        /// <summary>Provides a string of the parsed data for display</summary>
        /// <returns>A display string</returns>
        public string DisplayString() {
            try {
                return this.DoDisplayString();
            }
            catch(Exception e) {
                this.baseLog.Exception(9999, "", "", e);
                return "* FAILED *";
            }
        }


        /// <summary>Parse out the bytes returned from querying Descriptor</summary>
        /// <remarks>All is wrapped in try/catch so derived do not have to</remarks>
        /// <param name="data">The bytes to parse</param>
        /// <returns>A display string or "* N/A *" on failure</returns>
        public string Parse(byte[] data) {
            try {
                // Make sure zero out raw value. 
                this.RawData = new byte[0];
                // Do not need to reset type. Done on construction
                this.ResetMembers();
                if (data != null) {
                    if (data.Length >= 0) {
                        if (this.DoParse(data)) {
                            return this.DisplayString();
                        }
                    }
                    else {
                        this.baseLog.Error(9999, "Parse", "byte[] is zero length");
                    }
                }
                else {
                    this.baseLog.Error(9999, "Parse", "Raw byte[] is null");
                }
            }
            catch (Exception e) {
                this.baseLog.Exception(9999, "Parse", "", e);
            }
            return "* N/A *";
        }

        #endregion

        #region Constructors

        public DescParser_Base() {
            this.ImplementationType = this.GetDerivedType();
            this.RawData = new byte[0];
            this.ResetMembers();
        }

        public DescParser_Base(byte[] data) {
            this.ImplementationType = this.GetDerivedType();
            this.Parse(data);
        }

        #endregion


        /// <summary>
        /// Creates a new sized Raw buffer array and copies bytes to it. CALL IN DoParse method
        /// </summary>
        /// <param name="data">The data to copy</param>
        /// <param name="length">The length of data to copy</param>
        /// <returns>true on success, otherwise false on exception or if data null or smaller than length</returns>
        protected bool CopyToRawData(byte[] data, int length) {
            try {
                if (data != null) {
                    if (data.Length >= length) {
                        this.RawData = new byte[length];
                        Array.Copy(data, this.RawData, this.RawData.Length);
                        this.baseLog.Info("CopyToRawData", () => string.Format("Data:{0}", this.RawData.ToHexByteString()));
                        return true;
                    }
                }
                else {
                    this.baseLog.Error(9999, "CopyToRawData", "Raw byte[] is null");
                }
            }
            catch (Exception e) {
                this.baseLog.Exception(9999, "CopyToRawData", "", e);
            }
            return false;
        }


        #region Abstract methods

        /// <summary>Parse data according to derived. Null and zero length data checked</summary>
        /// <param name="data">The data to parse</param>
        /// <returns>true on success, otherwise false</returns>
        protected abstract bool DoParse(byte[] data);


        /// <summary>
        /// Derived to reset their specific data propertis before parse. 
        /// Base sets its own. NOTE: need to set the base RawData with 
        /// CopyToRawData when length is known in the DoParse method
        /// </summary>
        protected abstract void ResetMembers();


        /// <summary>Override to provide type for future cast of specific data fields</summary>
        /// <returns>The type of the derived class</returns>
        protected abstract Type GetDerivedType();


        /// <summary>Provides a string of the derived class parsed data for display</summary>
        /// <returns>A display string</returns>
        protected abstract string DoDisplayString();


        #endregion

    }
}
