using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommWrapper.Net.interfaces;
using System;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<ErrReport> UnexpectedExceptionEvent;


        private void RaiseIfException(ErrReport report) {
            if (report.Code != 0) {
                WrapErr.ToErrReport(9999, "Error raising unexpected exception event", () => {
                    this.UnexpectedExceptionEvent?.Invoke(this, report);
                });
            }
        }


    }
}
