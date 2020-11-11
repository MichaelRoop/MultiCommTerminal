using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.DataModels;
using Ethernet.Common.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Ethernet events

        public event EventHandler<string> Ethernet_BytesReceived;
        public event EventHandler<EthernetParams> EthernetParamsRequestedEvent;
        public event EventHandler<MsgPumpResults> OnEthernetConnectionAttemptCompleted;
        public event EventHandler<MsgPumpResults> OnEthernetError;
        public event EventHandler<List<IIndexItem<DefaultFileExtraInfo>>> OnEthernetListChange;

        #endregion

        #region ICommWrapper methods

        public void EthernetConnect(EthernetParams dataModel) {
            // TODO have an OnError and wrap this in try catch
            this.log.Info("EthernetConnect", "Connecting to ethernet");
            this.ethernet.ConnectAsync(dataModel);
        }


        public void EthernetDisconnect() {
            this.ethernet.Disconnect();
        }


        public void EthernetSend(string msg) {
            this.GetCurrentTerminator(
                (data) => {
                    this.ethernetStack.InTerminators = data.TerminatorBlock;
                    this.ethernetStack.OutTerminators = data.TerminatorBlock;

                }, (err) => { });

            this.ethernetStack.SendToComm(msg);
        }

        #endregion


        private void EthernetTeardown() {
            this.ethernetStack.MsgReceived -= this.EthernetStack_MsgReceivedHandler;
            this.ethernet.ParamsRequestedEvent -= Ethernet_ParamsRequestedEventHandler;
            this.ethernet.OnError -= this.Ethernet_OnErrorHandler;
            this.ethernet.OnEthernetConnectionAttemptCompleted -= this.Ethernet_OnEthernetConnectionAttemptCompletedHandler;
        }


        private void EthernetInit() {
            this.ethernetStack.SetCommChannel(this.ethernet);
            this.ethernetStack.InTerminators = "\r\n".ToAsciiByteArray();
            this.ethernetStack.OutTerminators = "\r\n".ToAsciiByteArray();
            this.ethernetStack.MsgReceived += this.EthernetStack_MsgReceivedHandler;  

            this.ethernet.ParamsRequestedEvent += Ethernet_ParamsRequestedEventHandler;
            this.ethernet.OnError += this.Ethernet_OnErrorHandler;
            this.ethernet.OnEthernetConnectionAttemptCompleted += this.Ethernet_OnEthernetConnectionAttemptCompletedHandler;
        }

        #region Storage

        public void GetEthernetDataList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.ethernetStorage.IndexedItems);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void CreateNewEthernetData(string display, EthernetParams data, Action onSuccess, OnErr onError) {
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
                        this.SaveEthernetData(idx, data, onSuccess, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        public void CreateNewEthernetData(EthernetParams data, Action onSuccess, OnErr onError) {
            this.CreateNewEthernetData(data.DisplayString, data, onSuccess, onError);
        }


        public void RetrieveEthernetData(IIndexItem<DefaultFileExtraInfo> index, Action<EthernetParams> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // TODO - check if exists
                    onSuccess.Invoke(this.ethernetStorage.Retrieve(index));
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void SaveEthernetData(IIndexItem<DefaultFileExtraInfo> idx, EthernetParams data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (idx.Display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        // This one a bit different since we create the display from the field values
                        idx.Display = data.DisplayString;
                        this.ethernetStorage.Store(data, idx);
                        this.GetEthernetDataList(
                            (list) => {
                                this.OnEthernetListChange?.Invoke(this, list);
                                onSuccess.Invoke();
                            }, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        public void DeleteEthernetData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    bool ok = this.ethernetStorage.DeleteFile(index);
                    this.GetEthernetDataList(
                        (list) => {
                            this.OnEthernetListChange?.Invoke(this, list);
                            onComplete(ok);
                        }, onError);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.WriteFailue));
                }
            });
        }


        public void DeleteEthernetData(object index, Action<bool> onComplete, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    IIndexItem<DefaultFileExtraInfo> idx = index as IIndexItem<DefaultFileExtraInfo>;
                    if (idx == null) {
                        bool ok = this.ethernetStorage.DeleteFile(idx);
                        this.GetEthernetDataList(
                            (list) => {
                                this.OnEthernetListChange?.Invoke(this, list);
                                onComplete(ok);
                            }, onError);
                    }
                    else {
                        onError(this.GetText(MsgCode.NothingSelected));
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.WriteFailue));
                }
            });
        }

        #endregion

        #region Event handlers

        private void Ethernet_OnEthernetConnectionAttemptCompletedHandler(object sender, MsgPumpResults e) {

            // Can do something here to save any pending IP entered
            this.log.Info("Ethernet_OnEthernetConnectionAttemptCompletedHandler", 
                () => string.Format("Connect attempt returned:{0}", e.Code));

            this.OnEthernetConnectionAttemptCompleted?.Invoke(sender, e);
        }

        private void Ethernet_OnErrorHandler(object sender, MsgPumpResults e) {
            this.OnEthernetError?.Invoke(sender, e);
        }

        private void EthernetStack_MsgReceivedHandler(object sender, byte[] e) {
            string msg = Encoding.ASCII.GetString(e, 0, e.Length);
            this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
            this.Ethernet_BytesReceived?.Invoke(sender, msg);
        }


        private void Ethernet_ParamsRequestedEventHandler(object sender, EthernetParams e) {
            this.EthernetParamsRequestedEvent?.Invoke(sender, e);
        }

        #endregion

    }

}
