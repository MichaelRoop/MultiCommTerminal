using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
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
        public event EventHandler<TerminatorDataModel> CurrentTerminatorChangedBT;
        public event EventHandler<TerminatorDataModel> CurrentTerminatorChangedBLE;
        public event EventHandler<TerminatorDataModel> CurrentTerminatorChangedUSB;
        public event EventHandler<TerminatorDataModel> CurrentTerminatorChangedWIFI;
        public event EventHandler<TerminatorDataModel> CurrentTerminatorChangedEthernet;


        public void BackCompatibilityInitializeExistingTerminatorNames() {
            try {
                this.GetTerminatorList(
                    (list) => {
                        foreach (var item in list) {
                            this.RetrieveTerminatorData(
                                item,
                                (tData) => {
                                    if (tData.Name.Length == 0) {
                                        tData.Name = item.Display;
                                        this.SaveTerminator(item, tData, () => { }, (err) => { });
                                    }
                                },
                                (e) => {
                                });
                        }
                    },
                    (err) => {
                    });

                // Check if current terminator selected
                this.GetCurrentTerminator(
                    (data) => {
                    // Nothing to do?
                },
                    (err) => {

                    });
            }
            catch(Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


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
                    // Force default creation
                    var x = this.terminatorStorage;
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();
                    items.CurrentTerminator = this.AssureTerminators(items.CurrentTerminator);
                    onSuccess(items.CurrentTerminator);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }



        public void GetCurrentTerminator(CommMedium medium, Action<TerminatorDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // Force default creation
                    var x = this.terminatorStorage;
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();

                    TerminatorDataModel dm = null;
                    switch (medium) {
                        case CommMedium.Bluetooth:
                            dm = items.CurrentTerminatorBT;
                            break;
                        case CommMedium.BluetoothLE:
                            dm = items.CurrentTerminatorBLE;
                            break;
                        case CommMedium.Ethernet:
                            dm = items.CurrentTerminatorEthernet;
                            break;
                        case CommMedium.Usb:
                            dm = items.CurrentTerminatorUSB;
                            break;
                        case CommMedium.Wifi:
                            dm = items.CurrentTerminatorWIFI;
                            break;
                        default:
                            dm = items.CurrentTerminator;
                            break;
                    }
                    if (dm == null) {
                        dm = items.CurrentTerminator;
                    }
                    dm = this.AssureTerminators(dm);
                    onSuccess(dm);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        /// <summary>Assure at least one terminator in case settings file erased</summary>
        /// <param name="dm"></param>
        /// <returns></returns>
        private TerminatorDataModel AssureTerminators(TerminatorDataModel dm) {
            if (string.IsNullOrWhiteSpace(dm.Name)) {
                dm.Name = "TMP terminators";
            }
            if (dm.TerminatorInfos.Count == 0) {
                List<TerminatorInfo> infos = new List<TerminatorInfo>();
                infos.Add(new TerminatorInfo(Terminator.LF));
                infos.Add(new TerminatorInfo(Terminator.CR));
                dm.Init(infos);
            }
            return dm;
        }




        public void SetCurrentTerminators(TerminatorDataModel data, CommMedium medium, OnErr onError) {
            this.GetSettings((settings) => {
                EventHandler<TerminatorDataModel> ev = null;
                switch (medium) {
                    case CommMedium.Bluetooth:
                        settings.CurrentTerminatorBT = data;
                        ev = this.CurrentTerminatorChangedBT;
                        break;
                    case CommMedium.BluetoothLE:
                        settings.CurrentTerminatorBLE = data;
                        ev = this.CurrentTerminatorChangedBLE;
                        break;
                    case CommMedium.Ethernet:
                        settings.CurrentTerminatorEthernet = data;
                        ev = this.CurrentTerminatorChangedEthernet;
                        break;
                    case CommMedium.Usb:
                        settings.CurrentTerminatorUSB = data;
                        ev = this.CurrentTerminatorChangedUSB;
                        break;
                    case CommMedium.Wifi:
                        settings.CurrentTerminatorWIFI = data;
                        ev = this.CurrentTerminatorChangedWIFI;
                        break;
                    default:
                        settings.CurrentTerminator = data;
                        ev = this.CurrentTerminatorChanged;
                        break;
                }

                this.SaveSettings(settings, () => { ev?.Invoke(this, data); }, onError);
            }, onError);
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
            if (index == null) {
                onError(this.GetText(MsgCode.NothingSelected));
            }
            else {
                this.RetrieveTerminatorData(
                    index,
                    (data) => {
                        this.SetCurrentTerminators(data, onError);
                        onSuccess.Invoke();
                    },
                    onError);
            }
        }


        public void SetCurrentTerminators(IIndexItem<DefaultFileExtraInfo> index, CommMedium medium, Action onSuccess, OnErr onError) {
            if (index == null) {
                onError(this.GetText(MsgCode.NothingSelected));
            }
            else {
                this.RetrieveTerminatorData(
                    index,
                    (data) => {
                        this.SetCurrentTerminators(data, medium, onError);
                        onSuccess.Invoke();
                    },
                    onError);
            }
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
                    if (index == null) {
                        onError.Invoke(this.GetText(MsgCode.NothingSelected));
                    }
                    else {
                        // TODO - check if exists
                        onSuccess.Invoke(this.terminatorStorage.Retrieve(index));
                    }
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


        public void CreateNewTerminator(string display, TerminatorDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError) {
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
                        data.Name = display;
                        this.SaveTerminator(idx, data, () => onSuccess.Invoke(idx), onError);
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
                    if (data.Name.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else if (idx.Display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        // Update the index display
                        idx.Display = data.Name;
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
                    if (index == null) {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                    else if (terminatorStorage.IndexedItems.Count < 2) {
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
