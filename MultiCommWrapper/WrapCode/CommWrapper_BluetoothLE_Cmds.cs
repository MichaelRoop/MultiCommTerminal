using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.Tools;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        BLERangeValidator bleRangeValidator = new BLERangeValidator();

        public void GetBLECmdList(Action<List<IIndexItem<BLECmdIndexExtraInfo>>> onSuccess, OnErr onError) {
            this.RetrieveIndex(this.bleCmdStorage, onSuccess, onError);
        }

        public void CreateBLECmdSet(string display, BLECommandSetDataModel data, BLECmdIndexExtraInfo extraInfo, Action<IIndexItem<BLECmdIndexExtraInfo>> onSuccess, OnErr onError) {
            this.ValidateRanges(data,
                () => {
                    this.Create(display, data, extraInfo, this.bleCmdStorage, onSuccess, onError);
                }, onError);
        }


        public void RetrieveBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> index, Action<BLECommandSetDataModel> onSuccess, OnErr onError) {
            this.RetrieveItem(this.bleCmdStorage, index, onSuccess, onError);
        }


        public void SaveBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> idx, BLECommandSetDataModel data, Action onSuccess, OnErr onError) {
            this.ValidateRanges(data,
                () => {
                    idx.ExtraInfoObj.CharacteristicName = data.CharacteristicName;
                    idx.ExtraInfoObj.DataType = data.DataType;
                    idx.ExtraInfoObj.DataTypeDisplay = data.DataType.ToStr();
                    this.Save(this.bleCmdStorage, idx, data, onSuccess, onError);
                }, onError);
        }


        public void DeleteBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            this.DeleteFromStorage(this.bleCmdStorage, index, onComplete, onError);
        }


        public void DeleteAllBLECmds(Action onSuccess, OnErr onError) {
            this.bleCmdStorage.DeleteStorageDirectory();
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


        #region Private

        private void ValidateRanges(BLECommandSetDataModel data, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, () => {
                string error = string.Empty;
                foreach (ScriptItem item in data.Items) {
                    this.ValidateBLECmdItem(data.DataType, item, () => { }, (err) => {
                        if (error == string.Empty) {
                            error = err;
                        }
                    });
                }
                if (error == string.Empty) {
                    onSuccess();
                }
                else {
                    onError(error);
                }
            });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => onError(report.Msg));
            }
        }

        #endregion

    }

}
