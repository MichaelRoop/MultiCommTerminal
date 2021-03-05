using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.Tools;
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

        BLERangeValidator bleRangeValidator = new BLERangeValidator();

        public void GetBLECmdList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError) {
            this.RetrieveIndex(this.bleCmdStorage, onSuccess, onError);

            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        onSuccess.Invoke(this.bleCmdStorage.IndexedItems);
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.LoadFailed));
            //    }
            //});
        }


        public void CreateBLECmdSet(string display, BLECommandSetDataModel data, Action onSuccess, OnErr onError) {
            this.Create(display, data, this.bleCmdStorage, onSuccess, onError);
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
            //            this.SaveBLECmdSet(idx, data, onSuccess, onError);
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.SaveFailed));
            //    }
            //});
        }


        public void CreateBLECmdSet(string display, BLECommandSetDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError) {
            this.Create(display, data, this.bleCmdStorage, onSuccess, onError);

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
            //            this.SaveBLECmdSet(idx, data, () => onSuccess.Invoke(idx), onError);
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.SaveFailed));
            //    }
            //});
        }


        public void RetrieveBLECmdSet(IIndexItem<DefaultFileExtraInfo> index, Action<BLECommandSetDataModel> onSuccess, OnErr onError) {
            this.RetrieveItem(this.bleCmdStorage, index, onSuccess, onError);


            //WrapErr.ToErrReport(9999, () => {
            //    ErrReport report;
            //    WrapErr.ToErrReport(out report, 9999, () => {
            //        if (index == null) {
            //            onError(this.GetText(MsgCode.NothingSelected));
            //        }
            //        else {
            //            // TODO - check if exists
            //            onSuccess.Invoke(this.bleCmdStorage.Retrieve(index));
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.LoadFailed));
            //    }
            //});

        }


        // TODO - refactor with option to invoke current changed event
        public void SaveBLECmdSet(IIndexItem<DefaultFileExtraInfo> idx, BLECommandSetDataModel data, Action onSuccess, OnErr onError) {
            this.Save(this.bleCmdStorage, idx, data, onSuccess, onError);


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
            //            this.bleCmdStorage.Store(data, idx);
            //            onSuccess.Invoke();
            //        }
            //    });
            //    if (report.Code != 0) {
            //        onError.Invoke(this.GetText(MsgCode.SaveFailed));
            //    }
            //});
        }


        public void DeleteBLECmdSet(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            this.DeleteFromStorage(this.bleCmdStorage, index, onComplete, onError);
        }


        public void ValidateBLECmdItem(BLE_DataType dataType, ScriptItem item, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, () => {
                RangeValidationResult result = this.bleRangeValidator.Validate(item.Command, dataType);
                if (result.Status == BLE_DataValidationStatus.Success) {
                    onSuccess.Invoke();
                }
                else {
                    onError.Invoke(this.Translate(result));
                }
            });
            if (report.Code != 0) {
                onError.Invoke(this.GetText(MsgCode.UnknownError));
            }
        }


    }

}
