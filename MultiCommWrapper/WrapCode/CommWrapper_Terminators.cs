using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        /// <summary>When the current terminator is changed</summary>
        public event EventHandler<TerminatorDataModel> CurrentTerminatorChanged;


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
                this.SaveSettings(settings, () => {
                    if (this.CurrentTerminatorChanged != null) {
                        this.CurrentTerminatorChanged(this, data);
                    }
                }, onError);
            }, onError);
        }


        public void SetCurrentTerminators(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, Action<string> onError) {
            this.RetrieveTerminatorData(
                index, 
                (data) => {
                    this.SetCurrentTerminators(data, onError);
                    onSuccess.Invoke();
                }, 
                onError);
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



        public void RetrieveTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<TerminatorDataModel> onSuccess,Action<string> onError) {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // TODO - check if exists
                    onSuccess.Invoke(this.terminatorStorage.Retrieve(index));
                });
                if (report.Code != 0) {
                    // TODO - language
                    onError.Invoke("Failed to retrieve terminator data");
                }
            });
        }


        public void CreateNewTerminator(string display, TerminatorDataModel data, Action onSuccess, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(data.UId) {
                        Display = display,
                    };
                    this.SaveTerminator(idx, data, onSuccess, onError);
                });
                if (report.Code != 0) {
                    onError.Invoke("Failed to create terminator data");
                }
            });
        }


        public void SaveTerminator(IIndexItem<DefaultFileExtraInfo> idx, TerminatorDataModel data, Action onSuccess, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    this.terminatorStorage.Store(data, idx);
                    onSuccess.Invoke();
                });
                if (report.Code != 0) {
                    onError.Invoke("Failed to save terminator data");
                }
            });
        }


        public void DeleteTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (terminatorStorage.IndexedItems.Count < 2) {
                        onError("Cannot delete last terminator");
                    }
                    else {
                        bool ok = this.terminatorStorage.DeleteFile(index);
                        this.GetCurrentTerminator(
                            (data) => {
                                if (data.UId == index.UId_Object) {
                                    this.RetrieveTerminatorData(
                                        this.terminatorStorage.IndexedItems[0],
                                        (newData) => {
                                            this.SetCurrentTerminators(newData, (err) => { });
                                        },
                                        (err) => { });
                                }
                            },
                            (err) => { });
                        onComplete(ok);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke("Failed to retrieve terminator data");
                }
            });
        }



    }

}
