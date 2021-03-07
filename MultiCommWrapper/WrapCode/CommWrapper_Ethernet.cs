using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Ethernet events

        public event EventHandler<string> Ethernet_BytesReceived;
        public event EventHandler<EthernetParams> EthernetParamsRequestedEvent;
        public event EventHandler<MsgPumpResults> OnEthernetConnectionAttemptCompleted;
        public event EventHandler<MsgPumpResults> OnEthernetError;
        public event EventHandler<List<IIndexItem<EthernetExtraInfo>>> OnEthernetListChange;

        #endregion

        #region ICommWrapper methods

        public void EthernetConnectAsync(EthernetParams dataModel) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200100, "Failure on EthernetConnectAsync", () => {
                this.log.Info("EthernetConnect", "Connecting to ethernet");
                this.ethernet.ConnectAsync(dataModel);
            });
            this.RaiseIfException(report);
        }


        public void EthernetDisconnect() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200101, "Failure on EthernetDisconnect", () => {
                this.ethernet.Disconnect();
            });
            this.RaiseIfException(report);
        }


        public void EthernetSend(string msg) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200102, "Failure on ", () => {
                this.GetCurrentTerminator(
                    CommMedium.Ethernet,
                    (data) => {
                        this.ethernetStack.InTerminators = data.TerminatorBlock;
                        this.ethernetStack.OutTerminators = data.TerminatorBlock;

                    }, (err) => { });

                this.ethernetStack.SendToComm(msg);
            });
            this.RaiseIfException(report);
        }

        #endregion


        private void EthernetTeardown() {
            this.ethernetStack.MsgReceived -= this.EthernetStack_MsgReceivedHandler;
            this.ethernet.ParamsRequestedEvent -= Ethernet_ParamsRequestedEventHandler;
            this.ethernet.OnError -= this.Ethernet_OnErrorHandler;
            this.ethernet.OnEthernetConnectionAttemptCompleted -= this.Ethernet_OnEthernetConnectionAttemptCompletedHandler;
        }

        #region Storage

        public void GetEthernetDataList(Action<List<IIndexItem<EthernetExtraInfo>>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // TODO - they would have a blank extra info. Need to fill

                    onSuccess.Invoke(this.ethernetStorage.IndexedItems);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }

        ///// <summary>Split the display into data model fields for column display</summary>
        ///// <param name="onSuccess">Invoked on success</param>
        ///// <param name="onError">Invoked on error</param>
        //public void GetEthernetDataList(Action<List<EthernetDisplayDataModel>> onSuccess, OnErr onError) {
        //    WrapErr.ToErrReport(9999, () => {
        //        ErrReport report;
        //        WrapErr.ToErrReport(out report, 9999, () => {
        //            List<EthernetDisplayDataModel> list = new List<EthernetDisplayDataModel>();
        //            foreach(IIndexItem<EthernetExtraInfo> index in this.ethernetStorage.IndexedItems) {
        //                EthernetDisplayDataModel dm = new EthernetDisplayDataModel() {
        //                    Index = index,
        //                };
        //                this.log.Info("**********", index.Display);
        //                string[] parts = index.Display.Split(':');
        //                if (parts != null && parts.Count() == 3) {
        //                    dm.Name = parts[0];
        //                    dm.Address = parts[1];
        //                    dm.Port = parts[2];
        //                }
        //                list.Add(dm);
        //            }
        //            onSuccess?.Invoke(list);
        //        });
        //        if (report.Code != 0) {
        //            onError.Invoke(this.GetText(MsgCode.LoadFailed));
        //        }
        //    });

        //}


        //public void CreateNewEthernetData(string display, EthernetParams data, Action onSuccess, OnErr onError) {
        //    WrapErr.ToErrReport(9999, () => {
        //        ErrReport report;
        //        WrapErr.ToErrReport(out report, 9999, () => {
        //            if (display.Length == 0) {
        //                onError.Invoke(this.GetText(MsgCode.EmptyName));
        //            }
        //            else {
        //                // For now
        //                IIndexItem<EthernetExtraInfo> idx = new IndexItem<EthernetExtraInfo>(data.UId, new EthernetExtraInfo(data)) {
        //                    Display = display,
        //                };
        //                this.SaveEthernetData(idx, data, onSuccess, onError);
        //            }
        //        });
        //        if (report.Code != 0) {
        //            onError.Invoke(this.GetText(MsgCode.SaveFailed));
        //        }
        //    });
        //}


        public void CreateNewEthernetData(EthernetParams data, Action onSuccess, OnErr onError) {
            this.Create(data.Display, data, this.ethernetStorage,
                (idx) => onSuccess(),
                (d) => {
                    this.RaiseEthernetListChange(() => { }, onError);
                    //this.GetEthernetDataList(
                    //    (list) => {
                    //        this.OnEthernetListChange?.Invoke(this, list);
                    //        //onSuccess.Invoke();
                    //    }, onError);
                }, onError, new EthernetExtraInfo(data));
            //ErrReport report;
            //WrapErr.ToErrReport(out report, 200103, "Failure on CreateNewEthernetData", () => {
            //    this.CreateNewEthernetData(data.DisplayString, data, onSuccess, onError);
            //});
            //this.RaiseIfException(report);
        }


        ///// <summary>As of March 2021 I ported the storage to generic which lost the old ethernet Name field
        ///// 
        ///// </summary>
        ///// <param name="display"></param>
        ///// <returns></returns>
        //private string PortEhernetName(string newDisplay, string oldName) {
        //    try {
        //        // Porting issue in case saved under the old 'Name' field, move index display to item Display
        //        if (newDisplay.Length == 0) {
        //            // The old index string has concatenated values
        //            string[] parts = oldName.Split(':');
        //            if (parts != null && parts.Count() > 0) {
        //                return parts[0];
        //            }
        //        }
        //        return newDisplay;
        //    }
        //    catch (Exception) {
        //        return newDisplay;
        //    }
        //}


        public void RetrieveEthernetData(IIndexItem<EthernetExtraInfo> index, Action<EthernetParams> onSuccess, OnErr onError) {
            this.RetrieveItem(this.ethernetStorage, index, onSuccess, onError);

            //this.RetrieveItem(this.ethernetStorage, index,
            //    (data) => {
            //        data.Display = this.PortEhernetName(data.Display, index.Display);
            //        onSuccess(data);
            //    }, onError);
            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        // TODO - check if exists
            //        onSuccess.Invoke(this.ethernetStorage.Retrieve(index));
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.LoadFailed));
            //    }
            //});
        }


        public void SaveEthernetData(IIndexItem<EthernetExtraInfo> idx, EthernetParams data, Action onSuccess, OnErr onError) {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    idx.ExtraInfoObj.Update(data);
                    this.Save(this.ethernetStorage, idx, data, onSuccess,
                        (d) => {
                            this.RaiseEthernetListChange(() => { }, onError);
                            //this.GetEthernetDataList(
                            //    (list) => {
                            //        this.OnEthernetListChange?.Invoke(this, list);
                            //        //onSuccess.Invoke();
                            //    }, onError);
                        }, onError);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        if (idx.Display.Length == 0) {
            //            onError.Invoke(this.GetText(MsgCode.EmptyName));
            //        }
            //        else {
            //            // This one a bit different since we create the display from the field values
            //            idx.Display = data.DisplayString;
            //            this.ethernetStorage.Store(data, idx);
            //            this.GetEthernetDataList(
            //                (list) => {
            //                    this.OnEthernetListChange?.Invoke(this, list);
            //                    onSuccess.Invoke();
            //                }, onError);
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.SaveFailed));
            //    }
            //});
        }


        private void RaiseEthernetListChange(Action onSuccess, OnErr onError) {
            this.GetEthernetDataList(
                (list) => {
                    this.OnEthernetListChange?.Invoke(this, list);
                    onSuccess();
                }, onError);


        }


        public void DeleteEthernetData(IIndexItem<EthernetExtraInfo> index, string name, Func<string, bool> areYouSure, Action<bool> onComplete, OnErr onError) {
            this.DeleteFromStorage(
                this.ethernetStorage, index, name, areYouSure,
                (ok) => {
                    this.RaiseEthernetListChange(() => onComplete(ok), onError);
                }, onError);
            
            
            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        if (areYouSure(name)) {
            //            this.DeleteFromStorage(this.ethernetStorage, index,
            //                (ok) => {
            //                    this.GetEthernetDataList(
            //                        (list) => {
            //                            this.OnEthernetListChange?.Invoke(this, list);
            //                            onComplete(ok);
            //                        }, onError);
            //                }, onError);


            //            //bool ok = this.ethernetStorage.DeleteFile(index);
            //            //this.GetEthernetDataList(
            //            //    (list) => {
            //            //        this.OnEthernetListChange?.Invoke(this, list);
            //            //        onComplete(ok);
            //            //    }, onError);
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.DeleteFailure));
            //    }
            //});
        }

        public void DeleteAllEthernetData(Action onSuccess, OnErr onError) {
            this.DeleteAllFromStorage(this.ethernetStorage, onSuccess, onError);
        }


        //public void DeleteEthernetData(IIndexItem<EthernetExtraInfo> index, Action<bool> onComplete, OnErr onError) {
        //    this.DeleteFromStorage(this.ethernetStorage, index,
        //        (ok) => {

        //        }, onError);


        //    WrapErr.ToErrReport(9999, () => {
        //        ErrReport report;
        //        WrapErr.ToErrReport(out report, 9999, () => {
        //            IIndexItem<EthernetExtraInfo> idx = index as IIndexItem<EthernetExtraInfo>;
        //            if (idx == null) {
        //                bool ok = this.ethernetStorage.DeleteFile(idx);
        //                this.GetEthernetDataList(
        //                    (list) => {
        //                        this.OnEthernetListChange?.Invoke(this, list);
        //                        onComplete(ok);
        //                    }, onError);
        //            }
        //            else {
        //                onError(this.GetText(MsgCode.NothingSelected));
        //            }
        //        });
        //        if (report.Code != 0) {
        //            onError.Invoke(this.GetText(MsgCode.WriteFailue));
        //        }
        //    });
        //}

        #endregion

        #region Event handlers

        private void Ethernet_OnEthernetConnectionAttemptCompletedHandler(object sender, MsgPumpResults e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200104, "Failure on Ethernet_OnEthernetConnectionAttemptCompletedHandler", () => {
                // Can do something here to save any pending IP entered
                this.log.Info("Ethernet_OnEthernetConnectionAttemptCompletedHandler", 
                    () => string.Format("Connect attempt returned:{0}", e.Code));

                this.OnEthernetConnectionAttemptCompleted?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }

        private void Ethernet_OnErrorHandler(object sender, MsgPumpResults e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200105, "Failure on Ethernet_OnErrorHandler", () => {
                this.OnEthernetError?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }

        private void EthernetStack_MsgReceivedHandler(object sender, byte[] e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200106, "Failure on EthernetStack_MsgReceivedHandler", () => {
                string msg = Encoding.ASCII.GetString(e, 0, e.Length);
                this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
                this.Ethernet_BytesReceived?.Invoke(sender, msg);
            });
            this.RaiseIfException(report);
        }


        private void Ethernet_ParamsRequestedEventHandler(object sender, EthernetParams e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200107, "Failure on Ethernet_ParamsRequestedEventHandler", () => {
                this.EthernetParamsRequestedEvent?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }

        #endregion

    }

}
