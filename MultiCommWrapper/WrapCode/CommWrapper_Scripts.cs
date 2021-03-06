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
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000250, "Failure on SetCurrentScript", () => {
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
            });
            this.RaiseIfException(report);
        }


        public void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000251, "Failure on SetCurrentScript", () => {
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
            });
            this.RaiseIfException(report);
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
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000252, "Failure on SetCurrentScript", () => {
                this.GetSettings((settings) => {
                    EventHandler<ScriptDataModel> ev = null;
                    switch (medium) {
                        case CommMedium.Bluetooth:
                            settings.CurrentScriptBT = data;
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
            });
            this.RaiseIfException(report);
        }



        public void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, CommMedium medium, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000253, "Failure on SetCurrentScript", () => {
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
            });
            this.RaiseIfException(report);
        }



        public void RetrieveScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<ScriptDataModel> onSuccess, OnErr onError) {
            this.RetrieveItem(this.scriptStorage, index, onSuccess, onError);
            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        if (index == null) {
            //            onError(this.GetText(MsgCode.NothingSelected));
            //        }
            //        else { 
            //            // TODO - check if exists
            //            onSuccess.Invoke(this.scriptStorage.Retrieve(index));
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.LoadFailed));
            //    }
            //});
        }


        public void CreateNewScript(string display, ScriptDataModel data, Action onSuccess, OnErr onError) {
            this.Create(display, data, this.scriptStorage, 
                (ndx) => {
                    this.CurrentScriptChanged?.Invoke(this, data);
                    onSuccess(); 
                }, onError);




            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        if (display.Length == 0) {
            //            onError.Invoke(this.GetText(MsgCode.EmptyName));
            //        }
            //        else {
            //            IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(data.UId) {
            //                Display = display,
            //            };
            //            this.SaveScript(idx, data, onSuccess, onError);
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.SaveFailed));
            //    }
            //});
        }


        public void CreateNewScript(string display, ScriptDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError) {
            this.Create(display, data, this.scriptStorage, 
                (ndx) => {
                    this.CurrentScriptChanged?.Invoke(this, data);
                    onSuccess(ndx);
                }, onError);

            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        if (display.Length == 0) {
            //            onError.Invoke(this.GetText(MsgCode.EmptyName));
            //        }
            //        else {
            //            IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(data.UId) {
            //                Display = display,
            //            };
            //            this.SaveScript(idx, data, () => onSuccess.Invoke(idx), onError);
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.SaveFailed));
            //    }
            //});
        }

        public void GetScriptList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError) {
            this.RetrieveIndex(this.scriptStorage, onSuccess, onError);
            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        onSuccess.Invoke(this.scriptStorage.IndexedItems);
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.LoadFailed));
            //    }
            //});
        }


        public void SaveScript(IIndexItem<DefaultFileExtraInfo> idx, ScriptDataModel data, Action onSuccess, OnErr onError) {
            this.Save(this.scriptStorage, idx, data, 
                ()=> {
                    this.CurrentScriptChanged?.Invoke(this, data);
                    onSuccess.Invoke();
                }, onError);


            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        if (idx.Display.Length == 0) {
            //            onError.Invoke(this.GetText(MsgCode.EmptyName));
            //        }
            //        else if (string.IsNullOrWhiteSpace(data.Display)) {
            //            onError.Invoke(this.GetText(MsgCode.EmptyName));
            //        }
            //        else {
            //            // Transfer display name
            //            idx.Display = data.Display;
            //            this.scriptStorage.Store(data, idx);
            //            this.CurrentScriptChanged?.Invoke(this, data);
            //            onSuccess.Invoke();
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.SaveFailed));
            //    }
            //});
        }



        public void DeleteScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    //if (index == null) {
                    //    onError(this.GetText(MsgCode.NothingSelected));
                    //}
                    if (this.scriptStorage.IndexedItems.Count < 2) {
                        onError(this.GetText(MsgCode.CannotDeleteLast));
                    }
                    else {
                        this.DeleteFromStorage(this.scriptStorage, index,
                            (ok) => {
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
                            }, onError);

                        //bool ok = this.scriptStorage.DeleteFile(index);
                        //this.GetCurrentTerminator(
                        //    (data) => {
                        //        // If deleted script was the current, set current to first in list
                        //        if (data.UId == index.UId_Object) {
                        //            this.RetrieveScriptData(
                        //                this.scriptStorage.IndexedItems[0],
                        //                (newData) => {
                        //                    this.SetCurrentScript(newData, (err) => { });
                        //                },
                        //                (err) => { });
                        //       }
                        //    },
                        //    (err) => { });
                        //onComplete(ok);
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


        public void CreateHC05AtCmds(Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000254, "Failure on CreateHC05AtCmds", () => {
                List<ScriptItem> items = new List<ScriptItem>();
                this.AddCmd(items, "AT", "AT");
                this.AddCmd(items, "AT+ADDR?", "AT+ADDR?");
                this.AddCmd(items, "AT+NAME?", "AT+NAME?");
                this.AddCmd(items, "AT+NAME=<Param>", "AT+NAME=<Param>");
                this.AddCmd(items, "AT+VERSION", "AT+VERSION?");
                this.AddCmd(items, "AT+UART?", "AT+UART?");
                this.AddCmd(items, "AT+UART=<Param>,<Param2>,<Param3>", "AT+UART=<Param>,<Param2>,<Param3>");
                this.AddCmd(items, "AT+RESET", "AT+RESET");
                this.AddCmd(items, "AT+ORGL", "AT+ORGL");
                this.AddCmd(items, "AT+RNAME?<Param>", "AT+RNAME?<Param1>");
                this.AddCmd(items, "AT+ROLE?", "AT+ROLE?");
                this.AddCmd(items, "AT+ROLE=<Param>", "AT+ROLE=<Param>");
                this.AddCmd(items, "AT+CLASS", "AT+CLASS?");
                this.AddCmd(items, "AT+CLASS=<Param>", "AT+CLASS=<Param>");
                this.AddCmd(items, "AT+IAC?", "AT+IAC?");
                this.AddCmd(items, "AT+IAC=<Param>", "AT+IAC=<Param>");
                this.AddCmd(items, "AT+INQM?", "AT+INQM?");
                this.AddCmd(items, "AT+INQM=<Param>,<Param2>,<Param3>", "AT+INQM=<Param>,<Param2>, <Param3>");
                this.AddCmd(items, "AT+PSW?", "AT+PSWD?");
                this.AddCmd(items, "AT+PSW=<Param>", "AT+PSWD=<Param>");
                this.AddCmd(items, "AT+CMODE?", "AT+CMODE?");
                this.AddCmd(items, "AT+CMODE=<Param>", "AT+CMODE=<Param>");
                this.AddCmd(items, "AT+BIND?", "AT+BIND?");
                this.AddCmd(items, "AT+BIND=<Param>", "AT+BIND=<Param>");
                this.AddCmd(items, "AT+POLAR?", "AT+POLAR?");
                this.AddCmd(items, "AT+POLAR=<Param>,<Param2>", "AT+POLAR=<Param1,<Param2>");
                this.AddCmd(items, "AT+PIO", "AT+PIO=<Param1>,<Param2>");
                this.AddCmd(items, "AT+IPSCAN?", "AT+IPSCAN?");
                this.AddCmd(items, "AT+IPSCAN=<Param>,<Param2>,<Param3>,<Param4>", "AT+IPSCAN=<Param1>,<Param2>,<Param3>,<Param4>");
                this.AddCmd(items, "AT+SNIFF", "AT+SNIFF?");
                this.AddCmd(items, "AT+SNIFF=<Param>,<Param2>,<Param3>,<Param4>", "AT+SNIFF=<Param1>,<Param2>,<Param3>,<Param4>");
                this.AddCmd(items, "AT+SENM?", "AT+SENM?");
                this.AddCmd(items, "AT+SENM=<Param1>,<Param2>", "AT+SENM=<Param1>,<Param2>");
                this.AddCmd(items, "AT+PMSAD=<Param1>", "AT+PMSAD=<Param>");
                this.AddCmd(items, "AT+RMAAD", "AT+RMAAD");
                this.AddCmd(items, "AT+FSAD=<Param1>", "AT+FSAD=<Param>");
                this.AddCmd(items, "AT+ADCN?", "AT+ADCN?");
                this.AddCmd(items, "AT+STATE?", "AT+STATE?");
                this.AddCmd(items, "AT+INIT", "AT+INIT");
                this.AddCmd(items, "AT+INQ", "AT+INQ");
                this.AddCmd(items, "AT+INQC", "AT+INQC");
                this.AddCmd(items, "AT+PAIR=<Param1>,<Param2>", "AT+PAIR=<Param1>,<Param2>");
                this.AddCmd(items, "AT+LINK=<Param>", "AT+LINK=<Param>");
                this.AddCmd(items, "AT+DISC", "AT+DISC");
                this.AddCmd(items, "AT+EXSNIFF=<Param>", "AT+EXSNIFF=<Param>");
                this.SaveCmdSet("HC-05", items, onSuccess, onError);
            });
        }

        private void AddCmd(List<ScriptItem> list, string name, string cmd) {
            list.AddNew(name, cmd);
        }


        private void SaveCmdSet(string name, List<ScriptItem> items, Action onSuccess, OnErr onError) {
            ScriptDataModel dm = new ScriptDataModel(items) { 
                Display = name
            };
            this.CreateNewScript(name, dm, onSuccess, onError);
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
