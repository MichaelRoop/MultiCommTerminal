using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public void GetCurrentTerminator(Action<TerminatorData> onSuccess, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();
                    onSuccess(items.CurrentTerminator);
                });
                if (report.Code != 0) {
                    // TODO - language
                    onError.Invoke("Failed to load settings");
                }
            });
        }


        public void SetCurrentTerminators(TerminatorData data, Action<string> onError) {
            this.GetSettings((settings) => {
                settings.CurrentTerminator = data;
                this.SaveSettings(settings, () => { }, onError);
            }, onError);
        }




    }

}
