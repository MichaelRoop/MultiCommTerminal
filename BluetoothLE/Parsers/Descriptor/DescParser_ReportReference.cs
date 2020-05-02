using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using System;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// Parse out the values from the Report Reference Descriptor
    /// (0x2908) Data type: uint8, uint8 ? 
    /// TODO - I only have uint  8
    /// </summary>
    public class DescParser_ReportReference : DescParser_Base {

        #region Data

        private ClassLog log = new ClassLog("DescParser_ReportReference");

        #endregion

        #region Properties

        public ReportType TypeOfReport { get; set; } = ReportType.Input;
        public byte ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescParser_ReportReference() : base() { }


        public DescParser_ReportReference(byte[] data) : base(data) { }

        #endregion

        #region Overrides from DescParser_Base

        protected override bool DoParse(byte[] data) {
            if (this.CopyToRawData(data, BYTE_LEN)) {
                this.ConvertedData = this.RawData[0];
                this.TypeOfReport = this.GetReportType(this.ConvertedData);
                this.log.Info("Parse", () => string.Format("Display:{0}", this.DisplayString()));
                return true;
            }
            return false;
        }

        protected override string DoDisplayString() {
            return this.TypeOfReport.ToString();
        }


        protected override void ResetMembers() {
            this.ConvertedData = 0;
            this.TypeOfReport = ReportType.Input;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }

        #endregion

        #region Private

        private ReportType GetReportType(byte value) {
            // Report ID   uint8  0-255 - report ID and Type
            // Report Type uint8 1-3 (Input Report=1, Output Report=2, Feature Report=3
            switch (value) {
                case 1:
                    return ReportType.Input;
                case 2:
                    return ReportType.Output;
                case 3:
                    return ReportType.Feature;
                default:
                    return ReportType.Undefined;
            }
        }

        #endregion

    }
}
