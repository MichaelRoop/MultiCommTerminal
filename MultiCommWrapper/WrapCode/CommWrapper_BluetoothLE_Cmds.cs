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


        public void GetFilteredBLECmdList(
            BLE_DataType dataType, 
            string characteristic,
            Action<List<IIndexItem<BLECmdIndexExtraInfo>>, List<IIndexItem<BLECmdIndexExtraInfo>>> onSuccess,
            OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "Failure on GetFilteredBLECmdList", () => {
                this.RetrieveIndex(
                    this.bleCmdStorage,
                    (idx) => {
                        List<IIndexItem<BLECmdIndexExtraInfo>> generalResult = new List<IIndexItem<BLECmdIndexExtraInfo>>();
                        List<IIndexItem<BLECmdIndexExtraInfo>> specificResult = new List<IIndexItem<BLECmdIndexExtraInfo>>();
                        if (idx.Count > 0) {
                            foreach (var item in idx) {
                                if (item.ExtraInfoObj.DataType == dataType) {
                                    generalResult.Add(item);
                                    if (!string.IsNullOrWhiteSpace(characteristic) && item.ExtraInfoObj.CharacteristicName == characteristic) {
                                        specificResult.Add(item);
                                    }
                                }
                            }
                        }
                        onSuccess.Invoke(generalResult, specificResult);
                    },
                    onError);
            });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => onError(this.GetText(MsgCode.UnknownError)));
            }
        }


        public void CreateBLECmdSet(BLECommandSetDataModel data, Action<IIndexItem<BLECmdIndexExtraInfo>> onSuccess, OnErr onError) {
            this.ValidateRanges(data, () => this.Create(
                data.Display, data, this.bleCmdStorage, onSuccess, onError, new BLECmdIndexExtraInfo(data)), onError);
        }


        public void CreateBLECmdSet(string display, BLECommandSetDataModel data, BLECmdIndexExtraInfo extraInfo, Action<IIndexItem<BLECmdIndexExtraInfo>> onSuccess, OnErr onError) {
            this.ValidateRanges(data, () => this.Create(display, data, this.bleCmdStorage, onSuccess, onError, extraInfo), onError);
        }


        public void RetrieveBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> index, Action<BLECommandSetDataModel> onSuccess, OnErr onError) {
            this.RetrieveItem(this.bleCmdStorage, index, onSuccess, onError);
        }


        public void SaveBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> idx, BLECommandSetDataModel data, Action onSuccess, OnErr onError) {
            this.ValidateRanges(data, 
                () => this.Save(this.bleCmdStorage, idx, data, (dm, index) => index.ExtraInfoObj.Update(dm), onSuccess, onError), onError);
        }


        public void DeleteBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            this.DeleteFromStorage(this.bleCmdStorage, index, onComplete, onError);
        }


        public void DeleteAllBLECmds(Action onSuccess, OnErr onError) {
            this.DeleteAllFromStorage(this.bleCmdStorage, onSuccess, onError);
        }


        public void DeleteAllBLECmdFiles(Action onSuccess, OnErr onError) {
            this.DeleteAllFilesFromStorage(this.bleCmdStorage, onSuccess, onError);
        }


        public void ValidateBLECmdItem(BLE_DataType dataType, ScriptItem item, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, () => {
                if (item.Display.Length > 0) {
                    this.ValidateBLEValue(dataType, item.Command, onSuccess, onError);
                }
                else {
                    onError.Invoke(this.GetText(MsgCode.EmptyName));
                }
            });
            if (report.Code != 0) {
                onError.Invoke(this.GetText(MsgCode.UnknownError));
            }
        }


        public void ValidateBLEValue(BLE_DataType dataType, string command, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, () => {
                this.log.Info("", () => string.Format("Validate:{0} '{1}'", dataType.ToStr(), command));


                RangeValidationResult result = this.bleRangeValidator.Validate(command, dataType);
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



        #region Create Command sets


        public void CreateBLEDemoCmdsBool(Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "Failure on CreateBLEBoolDemoCmds", () => {
                List<ScriptItem> items = new List<ScriptItem>();
                this.AddCmd(items, "ON", "1");
                this.AddCmd(items, "OFF", "0");
                this.CreateBLECmds("Demo Bool Commands", BLE_DataType.Bool, items, onSuccess, onError);
            });
        }


        public void CreateBLEDemoCmdsUint8(Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "Failure on CreateBLEUInt8DemoCmds", () => {
                List<ScriptItem> items = new List<ScriptItem>();
                this.AddCmd(items, "ALL OFF", "0");
                this.AddCmd(items, "ALL ON", 0xFF.ToString());
                this.AddCmd(items, "0,3,7 BITS ON", "137");
                this.CreateBLECmds("Demo Uint8 Commands", BLE_DataType.UInt_8bit, items, onSuccess, onError);
            });
        }


        public void CreateBLEDemoCmdsUint16(Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "Failure on CreateBLEUInt16DemoCmds", () => {
                List<ScriptItem> items = new List<ScriptItem>();
                this.AddCmd(items, "ALL OFF", "0");
                this.AddCmd(items, "ALL ON", 0xFFFF.ToString());
                this.AddCmd(items, "0,15 BITS ON", "32769");
                this.CreateBLECmds("Demo Uint16 Commands", BLE_DataType.UInt_16bit, items, onSuccess, onError);
            });
        }


        public void CreateBLEDemoCmdsUint32(Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "Failure on CreateBLEUInt32DemoCmds", () => {
                List<ScriptItem> items = new List<ScriptItem>();
                this.AddCmd(items, "ALL OFF", "0");
                this.AddCmd(items, "ALL ON", 0xFFFFFFFF.ToString());
                this.AddCmd(items, "First and last 8 bits ON", 0xFF0000FF.ToString());
                this.CreateBLECmds("Demo Uint32 Commands", BLE_DataType.UInt_32bit, items, onSuccess, onError);
            });
        }


        public void CreateBLECmds(string display, BLE_DataType dataType, List<ScriptItem> items, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "Failure on CreateBLEBoolDemoCmds", () => {
                BLECommandSetDataModel dm = new BLECommandSetDataModel(items, "", dataType, display);
                this.CreateBLECmdSet(
                    dm.Display, dm, new BLECmdIndexExtraInfo(dm), idx => onSuccess(), onError);
            });
        }

        #endregion

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
