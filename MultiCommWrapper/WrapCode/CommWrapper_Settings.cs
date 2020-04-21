using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using System;

namespace MultiCommWrapper.Net.WrapCode {
    
    partial class CommWrapper : ICommWrapper {

        #region Public

        public void GetSettings(Action<SettingItems> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.settings.ReadObjectFromDefaultFile());
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void SaveSettings(SettingItems settings, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (this.settings.WriteObjectToDefaultFile(settings)) {
                        onSuccess.Invoke();
                    }
                    else {
                        onError.Invoke(this.GetText(MsgCode.SaveFailed));
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }

        #endregion

    }
}
