using LogUtils.Net;
using System;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>Parse out the values from the Report Reference Descriptor</summary>
    public class DescParser_ReportReference {

        #region Data

        private ClassLog log = new ClassLog("DescReportReferenceValueParser");
        private static int RAW_DATA_LEN = 1;

        #endregion

        #region Properties

        public ReportType TypeOfReport { get; set; } = ReportType.Input;

        public byte[] RawData { get; set; } = new byte[RAW_DATA_LEN];
        public byte ConvertedData { get; set; }

        #endregion

        #region Constructors

        public DescParser_ReportReference() {
            this.ResetMembers();
        }


        public DescParser_ReportReference(byte[] data) {
            this.Parse(data);
        }

        #endregion

        #region Public

        public string Parse(byte[] data) {
            this.ResetMembers();
            if (data != null) {
                if (data.Length >= RAW_DATA_LEN) {
                    Array.Copy(data, this.RawData, RAW_DATA_LEN);
                    this.ConvertedData = this.RawData[0];
                    this.TypeOfReport = this.GetReportType(this.ConvertedData);
                    this.log.Info("Parse", () => string.Format("Data:{0}", this.RawData.ToAsciiString()));
                    this.log.Info("Parse", () => string.Format("Display:{0}", this.DisplayString()));
                }
                else {
                    this.log.Error(9999, "Parse", () => string.Format("byte[] length {0} is less than 2", data.Length));
                }
            }
            else {
                this.log.Error(9999, "Parse", "Raw byte[] is null");
            }

            return this.DisplayString();
        }

        public string DisplayString() {
            return this.TypeOfReport.ToString();
        }

        #endregion

        #region Private

        private void ResetMembers() {
            this.RawData = new byte[RAW_DATA_LEN];
            this.ConvertedData = 0;
            this.TypeOfReport = ReportType.Input;
        }

        private ReportType GetReportType(byte value) {
            // Report ID   uint8  0-255 - report ID and Type
            // Report Type uint8 1-3 (Input Report=1, Output Report=2, Feature Report=3
            switch (value) {
                case 1: return ReportType.Input;
                case 2: return ReportType.Output;
                case 3: return ReportType.Feature;
                default: return ReportType.Undefined;
            }
        }

        #endregion

    }
}
