using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;
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


        public void CreateNewEthernetData(EthernetParams data, Action onSuccess, OnErr onError) {
            this.CreateNewEthernetData(data, (idx) => { onSuccess(); }, onError);
        }


        public void CreateNewEthernetData(EthernetParams data, Action<IIndexItem<EthernetExtraInfo>> onSuccess, OnErr onError) {
            this.Create(data.Display, data, this.ethernetStorage, onSuccess,
                (ethernetParam) => {
                    this.RaiseEthernetListChange(() => { }, onError);
                }, onError, new EthernetExtraInfo(data));
        }


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
        }


        public void DeleteAllEthernetData(Action onSuccess, OnErr onError) {
            this.DeleteAllFromStorage(this.ethernetStorage, onSuccess, onError);
        }

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
