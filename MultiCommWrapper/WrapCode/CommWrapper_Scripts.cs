using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public event EventHandler<ScriptDataModel> CurrentScriptChanged;
        public event EventHandler<ScriptDataModel> CurrentScriptChangedBT;
        public event EventHandler<ScriptDataModel> CurrentScriptChangedBLE;
        public event EventHandler<ScriptDataModel> CurrentScriptChangedUSB;
        public event EventHandler<ScriptDataModel> CurrentScriptChangedWIFI;
        public event EventHandler<ScriptDataModel> CurrentScriptChangedEthernet;


        public void GetCurrentScript(Action<ScriptDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // Force creations if not yet created
                    var x = this.scriptStorage;
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();
                    onSuccess(items.CurrentScript);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void SetCurrentScript(ScriptDataModel data, OnErr onError) {
            if (data == null) {
                onError(this.GetText(MsgCode.SaveFailed));
            }
            else {
                this.GetSettings((settings) => {
                    settings.CurrentScript = data;
                    this.SaveSettings(settings, () => {
                        if (this.CurrentScriptChanged != null) {
                            this.CurrentScriptChanged(this, data);
                        }
                    }, onError);
                }, onError);
            }
        }


        public void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError) {
            if (index == null) {
                onError(this.GetText(MsgCode.NothingSelected));
            }
            else {
                this.RetrieveScriptData(
                    index,
                    (data) => {
                        this.SetCurrentScript(data, onError);
                        onSuccess.Invoke();
                    },
                    onError);
            }
        }


        public void GetCurrentScript(CommMedium medium, Action<ScriptDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // Force default creation
                    var x = this.scriptStorage;
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();

                    ScriptDataModel dm = null;
                    switch (medium) {
                        case CommMedium.Bluetooth:
                            dm = items.CurrentScriptBT;
                            break;
                        case CommMedium.BluetoothLE:
                            dm = items.CurrentScriptBLE;
                            break;
                        case CommMedium.Ethernet:
                            dm = items.CurrentScriptEthernet;
                            break;
                        case CommMedium.Usb:
                            dm = items.CurrentScriptUSB;
                            break;
                        case CommMedium.Wifi:
                            dm = items.CurrentScriptWIFI;
                            break;
                        default:
                            dm = items.CurrentScript;
                            break;
                    }
                    if (dm == null) {
                        dm = items.CurrentScript;
                    }
                    dm = this.AssureScript(dm);

                    onSuccess(dm);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void SetCurrentScript(ScriptDataModel data, CommMedium medium, OnErr onError) {
            this.GetSettings((settings) => {
                EventHandler<ScriptDataModel> ev = null;
                switch (medium) {
                    case CommMedium.Bluetooth:
                        settings.CurrentScriptBT= data;
                        ev = this.CurrentScriptChangedBT;
                        break;
                    case CommMedium.BluetoothLE:
                        settings.CurrentScriptBLE = data;
                        ev = this.CurrentScriptChangedBLE;
                        break;
                    case CommMedium.Ethernet:
                        settings.CurrentScriptEthernet = data;
                        ev = this.CurrentScriptChangedEthernet;
                        break;
                    case CommMedium.Usb:
                        settings.CurrentScriptUSB = data;
                        ev = this.CurrentScriptChangedUSB;
                        break;
                    case CommMedium.Wifi:
                        settings.CurrentScriptWIFI = data;
                        ev = this.CurrentScriptChangedWIFI;
                        break;
                    default:
                        settings.CurrentScript = data;
                        ev = this.CurrentScriptChanged;
                        break;
                }

                this.SaveSettings(settings, () => { ev?.Invoke(this, data); }, onError);
            }, onError);
        }



        public void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, CommMedium medium, Action onSuccess, OnErr onError) {
            if (index == null) {
                onError(this.GetText(MsgCode.NothingSelected));
            }
            else {
                this.RetrieveScriptData(
                    index,
                    (data) => {
                        this.SetCurrentScript(data, medium, onError);
                        onSuccess.Invoke();
                    },
                    onError);
            }
        }



        public void RetrieveScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<ScriptDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (index == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else { 
                        // TODO - check if exists
                        onSuccess.Invoke(this.scriptStorage.Retrieve(index));
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void CreateNewScript(string display, ScriptDataModel data, Action onSuccess, OnErr onError) {
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
                        this.SaveScript(idx, data, onSuccess, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        public void CreateNewScript(string display, ScriptDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError) {
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
                        this.SaveScript(idx, data, () => onSuccess.Invoke(idx), onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
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


        public void SaveScript(IIndexItem<DefaultFileExtraInfo> idx, ScriptDataModel data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (idx.Display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else if (string.IsNullOrWhiteSpace(data.Display)) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        // Transfer display name
                        idx.Display = data.Display;
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
                    if (index == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else if (this.scriptStorage.IndexedItems.Count < 2) {
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


        public void ValidateScriptItem(ScriptItem item, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (string.IsNullOrWhiteSpace(item.Display)) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else if (string.IsNullOrWhiteSpace(item.Command)) {
                        onError.Invoke(
                            string.Format("{0} ({1})", 
                                this.GetText(MsgCode.EmptyParameter),
                                this.GetText(MsgCode.command)));
                    }
                    else {
                        onSuccess.Invoke();
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.UnknownError));
                }
            });
        }



        private ScriptDataModel AssureScript(ScriptDataModel dataModel) {
            // TODO - create a temp 
            if (string.IsNullOrWhiteSpace(dataModel.Display)) {
                dataModel.Display = "TMP Script";
            }
            if (dataModel.Items.Count == 0) {
                dataModel.Items.Add(new ScriptItem() {
                    Display = "TEMP CMD1",
                    Command = "CMD1",
                });
                dataModel.Items.Add(new ScriptItem() {
                    Display = "TEMP CMD2",
                    Command = "CMD2",
                });
            }
            return dataModel;
        }


    }
}
