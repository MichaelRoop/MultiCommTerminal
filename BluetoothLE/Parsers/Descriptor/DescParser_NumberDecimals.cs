using LogUtils.Net;
using System;

namespace BluetoothLE.Net.Parsers.Descriptor {

    public class DescParser_NumberDecimals : DescParser_Base {

        #region Data

        private readonly ClassLog log = new ClassLog("DescParser_NumberDecimals");
        private readonly int BYTE_LEN = 1;

        #endregion

        #region Constructors

        public DescParser_NumberDecimals() : base() { }
        public DescParser_NumberDecimals(byte[] data) : base(data) { }

        #endregion

        #region Properties

        public byte Number { get; set; }

        #endregion

        #region DescParser_Base overrides

        protected override string DoDisplayString() {
            return string.Format("Number of Digitals:{0}", this.Number);
        }


        protected override bool DoParse(byte[] data) {
            if (this.CopyToRawData(data, BYTE_LEN)) {
                this.Number = this.RawData[0];
                this.log.Info("DoParse", () => string.Format("Display:{0}", this.DisplayString()));
                return true;
            }
            return false;
        }


        protected override Type GetDerivedType() {
            return this.GetType();
        }


        protected override void ResetMembers() {
            this.Number = 0;
        }

        #endregion
    }
}
