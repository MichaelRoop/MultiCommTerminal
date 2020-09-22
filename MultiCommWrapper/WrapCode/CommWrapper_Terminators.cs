using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        private TerminatorFactory terminatorEntityFactory = new TerminatorFactory();


        public event EventHandler<TerminatorDataModel> CurrentTerminatorChanged;


        public void GetTerminatorEntitiesList(Action<List<TerminatorInfo>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.terminatorEntityFactory.Items);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void GetCurrentTerminator(Action<TerminatorDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();
                    onSuccess(items.CurrentTerminator);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void SetCurrentTerminators(TerminatorDataModel data, OnErr onError) {
            this.GetSettings((settings) => {
                settings.CurrentTerminator = data;
                this.SaveSettings(settings, () => {
                    if (this.CurrentTerminatorChanged != null) {
                        this.CurrentTerminatorChanged(this, data);
                    }
                }, onError);
            }, onError);
        }


        public void SetCurrentTerminators(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError) {
            this.RetrieveTerminatorData(
                index, 
                (data) => {
                    this.SetCurrentTerminators(data, onError);
                    onSuccess.Invoke();
                }, 
                onError);
        }


        public void GetTerminatorList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.terminatorStorage.IndexedItems);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }



        public void RetrieveTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<TerminatorDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // TODO - check if exists
                    onSuccess.Invoke(this.terminatorStorage.Retrieve(index));
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void CreateNewTerminator(string display, TerminatorDataModel data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(data.UId) {
                            Display = display,
                        };
                        this.SaveTerminator(idx, data, onSuccess, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        public void SaveTerminator(IIndexItem<DefaultFileExtraInfo> idx, TerminatorDataModel data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (idx.Display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        this.terminatorStorage.Store(data, idx);
                        onSuccess.Invoke();
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        public void DeleteTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (terminatorStorage.IndexedItems.Count < 2) {
                        onError(this.GetText(MsgCode.CannotDeleteLast));
                    }
                    else {
                        bool ok = this.terminatorStorage.DeleteFile(index);
                        // If deleted terminator was the current, update current to first in list
                        this.GetCurrentTerminator(
                            (data) => {
                                if (data.UId == index.UId_Object) {
                                    this.RetrieveTerminatorData(
                                        this.terminatorStorage.IndexedItems[0],
                                        (newData) => {
                                            // TODO error handling if we fail to set current
                                            this.SetCurrentTerminators(newData, (err) => { });
                                        },
                                        // TODO error handling if we fail to retrieve current terminator
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
