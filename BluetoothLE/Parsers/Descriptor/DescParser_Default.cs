using LogUtils.Net;
using System;
using VariousUtils;

namespace BluetoothLE.Net.Parsers.Descriptor {

    /// <summary>For descriptors not yet specifically implemented to return byte string</summary>
    public class DescParser_Default : DescParser_Base {

        #region Data

        private readonly ClassLog log = new ClassLog("DescParser_Default");

        #endregion

        #region Constructors

        public DescParser_Default() : base() { }
        public DescParser_Default(byte[] data) : base(data) { }

        #endregion

        #region Properties

        public string ByteString { get; set; } = "";

        #endregion

        #region DescParser_Base overrides

        protected override string DoDisplayString() {
            return string.Format("NOT IMPLEMENTED:{0}", this.ByteString);
        }


        protected override bool DoParse(byte[] data) {
            if (this.CopyToRawData(data, data.Length)) {
                this.ByteString = this.RawData.ToFormatedByteString();
                this.log.Info("DoParse", () => string.Format("Display:{0}", this.DisplayString()));
                return true;
            }
            return false;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }


        protected override void ResetMembers() {
            this.ByteString = "";
        }

        #endregion

    }
}
