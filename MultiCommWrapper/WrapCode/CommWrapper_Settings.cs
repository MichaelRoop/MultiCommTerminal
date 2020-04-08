using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using System;

namespace MultiCommWrapper.Net.WrapCode {
    
    partial class CommWrapper : ICommWrapper {

        #region Public

        public void GetSettings(Action<SettingItems> onSuccess, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.settings.ReadObjectFromDefaultFile());
                });
                if (report.Code != 0) {
                    // TODO - language
                    onError.Invoke("Failed to load settings");
                }
            });
        }


        public void SaveSettings(SettingItems settings, Action onSuccess, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    //onSuccess.Invoke(this.settings.ReadObjectFromDefaultFile());
                    if (this.settings.WriteObjectToDefaultFile(settings)) {
                        onSuccess.Invoke();
                    }
                    else {
                        // TODO error msg Language
                        onError.Invoke("Failed to write settings");
                    }
                });
                if (report.Code != 0) {
                    // TODO - language
                    onError.Invoke("Failed to write settings");
                }
            });
        }

        #endregion

    }
}
