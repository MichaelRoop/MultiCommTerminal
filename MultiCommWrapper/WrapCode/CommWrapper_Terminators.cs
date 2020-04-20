using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public void GetCurrentTerminator(Action<TerminatorDataModel> onSuccess, Action<string> onError) {
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


        public void SetCurrentTerminators(TerminatorDataModel data, Action<string> onError) {
            this.GetSettings((settings) => {
                settings.CurrentTerminator = data;
                this.SaveSettings(settings, () => { }, onError);
            }, onError);
        }


        public void GetTerminatorList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.terminatorStorage.IndexedItems);
                });
                if (report.Code != 0) {
                    // TODO - language
                    onError.Invoke("Failed to retrieve terminator index");
                }
            });
        }




    }

}
