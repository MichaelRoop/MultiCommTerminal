using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public void GetCurrentScript(Action<ScriptIndexDataModel> onSuccess, OnErr onError) {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();
                    onSuccess(items.CurrentScript);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


    }
}
