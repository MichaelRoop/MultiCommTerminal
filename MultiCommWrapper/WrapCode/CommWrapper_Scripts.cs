using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<ScriptIndexDataModel> CurrentScriptChanged;

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


        public void SetCurrentScript(ScriptIndexDataModel data, OnErr onError) {
            this.GetSettings((settings) => {
                settings.CurrentScript = data;
                this.SaveSettings(settings, () => {
                    if (this.CurrentTerminatorChanged != null) {
                        this.CurrentScriptChanged(this, data);
                    }
                }, onError);
            }, onError);
        }


        public void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError) {
            this.RetrieveScriptData(
                index,
                (data) => {
                    this.SetCurrentScript(data, onError);
                    onSuccess.Invoke();
                },
                onError);
        }

        public void RetrieveScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<ScriptIndexDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // TODO - check if exists
                    onSuccess.Invoke(this.scriptStorage.Retrieve(index));
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void GetScriptList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.scriptStorage.IndexedItems);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void SaveScript(IIndexItem<DefaultFileExtraInfo> idx, ScriptIndexDataModel data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (idx.Display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        this.scriptStorage.Store(data, idx);
                        this.CurrentScriptChanged?.Invoke(this, data);
                        onSuccess.Invoke();
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });

        }



        public void DeleteScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (this.scriptStorage.IndexedItems.Count < 2) {
                        onError(this.GetText(MsgCode.CannotDeleteLast));
                    }
                    else {
                        bool ok = this.scriptStorage.DeleteFile(index);
                        this.GetCurrentTerminator(
                            (data) => {
                                // If deleted script was the current, set current to first in list
                                if (data.UId == index.UId_Object) {
                                    this.RetrieveScriptData(
                                        this.scriptStorage.IndexedItems[0],
                                        (newData) => {
                                            this.SetCurrentScript(newData, (err) => { });
                                        },
                                        (err) => { });
                               }
                            },
                            (err) => { });
                        onComplete(ok);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });

        }

    }
}
