using LogUtils.Net;
using System;
using System.Text;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>
    /// Parses User Description Descriptor data
    /// (0x2901) Data type: string
    /// </summary>
    public class DescParser_UserDescription : DescParser_Base {

        #region Data

        private readonly ClassLog log = new ClassLog("DescParser_UserDescription");

        #endregion

        #region Properties

        public string Description { get; set; } = "";

        #endregion

        #region Constructors

        public DescParser_UserDescription() : base() { }
        public DescParser_UserDescription(byte[] data) : base(data) { }


        #endregion

        #region DescParser_Base overrides

        protected override string DoDisplayString() {
            return this.Description;
        }


        protected override bool DoParse(byte[] data) {
            if (this.CopyToRawData(data, data.Length)) {
                this.Description = Encoding.UTF8.GetString(this.RawData);
                return true;
            }
            return false;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }


        protected override void ResetMembers() {
            this.Description = "";
        }

        #endregion

    }
}
