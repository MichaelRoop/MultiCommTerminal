using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using IconFactory.data;
using MultiCommWrapper.Net.interfaces;
using System;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public void IconInfo(UIIcon code, Action<IconDataModel> onSuccess) {
            this.IconInfo(code, onSuccess, (msg) => { });
        }


        public void IconInfo(UIIcon code, Action<IconDataModel> onSuccess, Action<string> onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999,
                () => string.Format("No icon for {0}", code),
                () => {
                    onSuccess.Invoke(this.iconFactory.GetIcon(code));
                });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => {
                    onError(string.Format("Unhandled error retrieving icon {0}", code));
                });
            }
        }

        public string IconSource(UIIcon code) {
            ErrReport report;
            string source = WrapErr.ToErrReport(out report, 9999,
                () => string.Format(""),
                () => {
                    string tmp = "";
                    this.IconInfo(code, (info) => {
                        tmp = info.IconSource as string;
                        WrapErr.ChkVar(tmp, 9999, () => string.Format("No source string for {0}", code));
                        WrapErr.ChkTrue(tmp.Length > 0, 9999, () => string.Format("0 length source string for {0}", code));
                    }, (msg) => {
                        tmp = "";
                    });
                    return tmp;
                });
            return report.Code == 0 ? source : "";
        }



    }
}
