using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.Enumerations;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {

        #region ICommWrapper events

        public event EventHandler<List<SerialDeviceInfo>> SerialDiscoveredDevices;
        public event EventHandler<SerialUsbError> SerialOnError;
        public event EventHandler<string> Serial_BytesReceived;
        public event EventHandler<SerialDeviceInfo> OnSerialConfigRequest;
        public event EventHandler<MsgPumpResults> OnSerialConnectionAttemptCompleted;
 
        #endregion

        #region ICommWrapper methods

        public void SerialUsbDisconnect() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003001, "Failure on SerialUsbDisconnect", () => {
                this.serial.Disconnect();
            });
            this.RaiseIfException(report);
        }


        public void SerialUsbDiscoverAsync() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003002, "Failure on SerialUsbDiscoverAsync", () => {
                this.serial.DiscoverSerialDevicesAsync();
            });
            this.RaiseIfException(report);
        }


        public void SerialUsbConnect(SerialDeviceInfo dataModel, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003003, "Failure on SerialUsbConnect", () => {
                this.InitSerialDeviceInfoConfigFields(
                    dataModel, () => this.serial.ConnectAsync(dataModel), onError);
            });
            this.RaiseIfException(report);
        }


        public void SerialUsbSend(string msg) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003004, "Failure on SerialUsbSend", () => {
                this.GetCurrentTerminator(
                    CommMedium.Usb,
                    (data) => {
                        this.serialStack.InTerminators = data.TerminatorBlock;
                        this.serialStack.OutTerminators = data.TerminatorBlock;

                    }, (err) => { });
                this.serialStack.SendToComm(msg);
            });
            this.RaiseIfException(report);
        }

        #region Display data retrieval

        public List<KeyValuePropertyDisplay> Serial_GetDeviceInfoForDisplay(SerialDeviceInfo info) {
            List<KeyValuePropertyDisplay> list = new List<KeyValuePropertyDisplay>();
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Name), info.Name));
            // Primary values
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Port), info.PortName));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.BaudRate), info.Baud));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.DataBits), info.DataBits));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.StopBits), info.StopBits.Display()));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Parity), this.Translate(info.Parity)));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.FlowControl), this.Translate(info.FlowHandshake)));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.ReadTimeout), info.ReadTimeout.TotalMilliseconds.ToString()));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.WriteTimeout), info.WriteTimeout.TotalMilliseconds.ToString()));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Vendor), info.USB_VendorIdDisplay));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Product), info.USB_ProductIdDisplay));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Default), this.Translate(info.IsDefault)));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Enabled), this.Translate(info.IsEnabled)));

            // TODO Add others

            //list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Name), info.Name));


            return list;
        }


        public void Serial_GetStopBitsForDisplay(SerialDeviceInfo info, Action<List<StopBitDisplayClass>,int> onSuccess) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003005, "Failure on Serial_GetStopBitsForDisplay", () => {
                List<StopBitDisplayClass> stopBits = new List<StopBitDisplayClass>();
                stopBits.Add(new StopBitDisplayClass(SerialStopBits.One));
                stopBits.Add(new StopBitDisplayClass(SerialStopBits.OnePointFive));
                stopBits.Add(new StopBitDisplayClass(SerialStopBits.Two));
                int ndx = stopBits.FindIndex(x => x.StopBits == info.StopBits);
                onSuccess?.Invoke(stopBits, (ndx == -1) ? 0 : ndx);
            });
            this.RaiseIfException(report);
        }


        public void Serial_GetParitysForDisplay(SerialDeviceInfo info, Action<List<SerialParityDisplayClass>, int> onSuccess) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003006, "Failure on Serial_GetParitysForDisplay", () => {
                List<SerialParityDisplayClass> paritys = new List<SerialParityDisplayClass>();
                paritys.Add(new SerialParityDisplayClass(SerialParityType.None, this.Translate));
                paritys.Add(new SerialParityDisplayClass(SerialParityType.Even, this.Translate));
                paritys.Add(new SerialParityDisplayClass(SerialParityType.Odd, this.Translate));
                paritys.Add(new SerialParityDisplayClass(SerialParityType.Mark, this.Translate));
                paritys.Add(new SerialParityDisplayClass(SerialParityType.Space, this.Translate));
                int ndx = paritys.FindIndex(x => x.ParityType == info.Parity);
                onSuccess?.Invoke(paritys, (ndx == -1) ? 0 : ndx);
            });
            this.RaiseIfException(report);
        }


        public void Serial_GetBaudsForDisplay(SerialDeviceInfo info, Action<List<uint>, int> onSuccess) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003007, "Failure on Serial_GetBaudsForDisplay", () => {
                List<uint> bauds = new List<uint>();
                bauds.Add(300);
                bauds.Add(600);
                bauds.Add(1200);
                bauds.Add(2400);
                bauds.Add(4800);
                bauds.Add(9600);
                bauds.Add(14400);
                bauds.Add(19200);
                bauds.Add(28800);
                bauds.Add(31250);
                bauds.Add(38400);
                bauds.Add(57600);
                bauds.Add(115200);
                int ndx = bauds.FindIndex(x => x == info.Baud);
                onSuccess?.Invoke(bauds, (ndx == -1) ? 0 : ndx);
            });
            this.RaiseIfException(report);
        }


        public void Serial_GetDataBitsForDisplay(SerialDeviceInfo info, Action<List<ushort>, int> onSuccess) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003008, "Failure on Serial_GetDataBitsForDisplay", () => {
                List<ushort> dataBits = new List<ushort>();
                dataBits.Add(5);
                dataBits.Add(6);
                dataBits.Add(7);
                dataBits.Add(8);
                int ndx = dataBits.FindIndex(x => x == info.DataBits);
                onSuccess?.Invoke(dataBits, (ndx == -1) ? 0 : ndx);
            });
            this.RaiseIfException(report);
        }


        public void Serial_FlowControlForDisplay(SerialDeviceInfo info, Action<List<FlowControlDisplay>, int> onSuccess) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003009, "Failure on Serial_FlowControlForDisplay", () => {
                List<FlowControlDisplay> fc = new List<FlowControlDisplay>();
                fc.Add(new FlowControlDisplay(SerialFlowControlHandshake.None, this.Translate));
                fc.Add(new FlowControlDisplay(SerialFlowControlHandshake.RequestToSend, this.Translate));
                fc.Add(new FlowControlDisplay(SerialFlowControlHandshake.XonXoff, this.Translate));
                fc.Add(new FlowControlDisplay(SerialFlowControlHandshake.RequestToSendXonXoff, this.Translate));
                int ndx = fc.FindIndex(x => x.FlowControl == info.FlowHandshake);
                onSuccess?.Invoke(fc, (ndx == -1) ? 0 : ndx);
            });
            this.RaiseIfException(report);
        }

        #endregion

        #region Storage


        public void GetSerialCfgList(Action<List<IIndexItem<SerialIndexExtraInfo>>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.serialStorage.IndexedItems);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void CreateNewSerialCfg(string display, SerialDeviceInfo data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        this.Validate5msReadWrite(data, data);
                        // Initialise extra index fields object
                        SerialIndexExtraInfo extraInfo = new SerialIndexExtraInfo() {
                            PortName = data.PortName,
                            USBVendorId = data.USB_VendorId,
                            USBVendor = data.USB_VendorIdDisplay,
                            USBProductId = data.USB_ProductId,
                            USBProduct = data.USB_ProductIdDisplay
                        };

                        IIndexItem<SerialIndexExtraInfo> idx = new IndexItem<SerialIndexExtraInfo>(
                            data.StorageUid, extraInfo) {
                            Display = display,
                        };
                        this.SaveSerialCfg(idx, data, onSuccess, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        
        public void CreateOrSaveSerialCfg(string display, SerialDeviceInfo data, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003010, "Failure on CreateOrSaveSerialCfg", () => {
                // Only have access to the object and not its index object
                this.RetrieveSerialIndexItem(data,
                    (indexItem) => {
                        this.SaveSerialCfg(indexItem, data, onSuccess, onError);
                    },
                    () => {
                        this.CreateNewSerialCfg(display, data, onSuccess, onError);
                    }, onError);
            });
            this.RaiseIfException(report);
        }


        public void RetrieveSerialCfg(IIndexItem<SerialIndexExtraInfo> index, Action<SerialDeviceInfo> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    var info = this.serialStorage.Retrieve(index);
                    if (info == null) {
                        onError.Invoke(this.GetText(MsgCode.NotFound));
                    }
                    else {
                        this.Validate5msReadWrite(info, info);
                        onSuccess.Invoke(info);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void InitSerialDeviceInfoConfigFields(SerialDeviceInfo info, Action onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, () => {
                this.RetrieveSerialCfg(info, (storedInfo) => {
                    // ** Found in storage. Initialise passed in object with configured fields
                    info.Baud = storedInfo.Baud;
                    info.DataBits = storedInfo.DataBits;
                    info.StopBits = storedInfo.StopBits;
                    info.Parity = storedInfo.Parity;
                    info.FlowHandshake = storedInfo.FlowHandshake;
                    this.Validate5msReadWrite(info, storedInfo);
                    onSuccess();
                },
                () => {
                    // ** Not found. User opens dialog on event, adds config values and decides on storage
                    OnSerialConfigRequest?.Invoke(this, info);
                    onSuccess();
                },
                onError);
            });
        }


        public void SaveSerialCfg(IIndexItem<SerialIndexExtraInfo> idx, SerialDeviceInfo data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (idx.Display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        this.Validate5msReadWrite(data, data);
                        this.serialStorage.Store(data, idx);
                        onSuccess.Invoke();
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        public void DeleteSerialCfg(IIndexItem<SerialIndexExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003011, "Failure on DeleteSerialCfg", () => {
                this.DeleteFromStorage(this.serialStorage, index, onComplete, onError);
            });
            this.RaiseIfException(report);
        }


        public void DeleteSerialCfg(SerialDeviceInfo device, Func<string, bool> areYouSure, Action<bool> onComplete, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003011, "Failure on DeleteSerialCfg", () => {
                if (device == null) {
                    onError?.Invoke(this.GetText(MsgCode.NothingSelected));
                }
                else {
                    this.GetSerialIndex(device, (ndx) => {
                        if (areYouSure(device.PortName)) {
                            this.DeleteSerialCfg(ndx, onComplete, onError);
                        }
                    }, onError);
                }
            });
            this.RaiseIfException(report);
        }



        private void RetrieveSerialCfg(SerialDeviceInfo inObject, Action<SerialDeviceInfo> found, Action notFound, OnErr onError) {
            this.GetSerialCfgList((items) => {
                // Iterate through index and compare fields
                foreach (IIndexItem<SerialIndexExtraInfo> item in items) {
                    if (item.ExtraInfoObj.PortName == inObject.PortName &&
                        item.ExtraInfoObj.USBVendorId == inObject.USB_VendorId &&
                        item.ExtraInfoObj.USBProductId == inObject.USB_ProductId) {
                        this.RetrieveSerialCfg(item, found, onError);
                        return;
                    }
                }
                notFound.Invoke();
            }, onError);
        }


        private void RetrieveSerialIndexItem(SerialDeviceInfo inObject, Action<IIndexItem<SerialIndexExtraInfo>> found, Action notFound, OnErr onError) {
            this.GetSerialCfgList((items) => {
                // Iterate through index and compare fields
                foreach (IIndexItem<SerialIndexExtraInfo> item in items) {
                    if (item.ExtraInfoObj.PortName == inObject.PortName &&
                        item.ExtraInfoObj.USBVendorId == inObject.USB_VendorId &&
                        item.ExtraInfoObj.USBProductId == inObject.USB_ProductId) {
                        found.Invoke(item);
                        this.log.Info("RetrieveSerialIndexItem", "Found the serial index");
                        return;
                    }
                }
                notFound.Invoke();
            }, onError);


        }


        private TimeSpan ForceTo5msIf0(TimeSpan inValue) {
            if (inValue.TotalMilliseconds == 0) {
                return TimeSpan.FromMilliseconds(5);
            }
            return inValue;
        }


        private void Validate5msReadWrite(SerialDeviceInfo target, SerialDeviceInfo source) {
            target.ReadTimeout = this.ForceTo5msIf0(source.ReadTimeout);
            target.WriteTimeout = this.ForceTo5msIf0(source.WriteTimeout);
        }

        public string Translate(SerialParityType parityType) {
            switch (parityType) {
                case SerialParityType.None:
                    return this.GetText(MsgCode.None);
                case SerialParityType.Odd:
                    return this.GetText(MsgCode.Odd);
                case SerialParityType.Even:
                    return this.GetText(MsgCode.Even);
                case SerialParityType.Mark:
                    return this.GetText(MsgCode.Mark);
                case SerialParityType.Space:
                    return this.GetText(MsgCode.Space);
                default:
                    return parityType.ToString();
            }
        }


        public string Translate(SerialFlowControlHandshake controlFlow) {
            switch (controlFlow) {
                case SerialFlowControlHandshake.None:
                    return this.GetText(MsgCode.None);
                case SerialFlowControlHandshake.RequestToSend:
                    return "RTS";
                case SerialFlowControlHandshake.XonXoff:
                    return "Xon/Xoff";
                case SerialFlowControlHandshake.RequestToSendXonXoff:
                    return "RTS/Xon/Xoff";
                default:
                    return controlFlow.ToString().CamelCaseToSpaces();
            }
        }



        #endregion

        #endregion

        #region Init Teardown

        private void SerialTeardown() {
            this.serialStack.MsgReceived -= this.SerialStack_MsgReceivedHandler;
            this.serial.OnSerialConnectionAttemptCompleted -= this.SerialConnectCompleteHandler;
            this.serial.DiscoveredDevices -= this.Serial_DiscoveredDevicesHandler;
            this.serial.OnError -= this.Serial_OnErrorHandler;
        }

        #endregion

        #region Event handlers

        private void Serial_OnErrorHandler(object sender, SerialUsbError err) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003012, "Failure on Serial_OnErrorHandler", () => {
                // TODO - determine if you have to clean up the serial USB
                if (err.PortName.Length > 0) {
                    err.Message = string.Format("{0}:{1} - {2}", MsgCode.Port, err.PortName, this.GetSerialErrText(err.Code));
                }
                else {
                    err.Message = string.Format("{0}", this.GetSerialErrText(err.Code));
                }
                this.SerialOnError?.Invoke(sender, err);
            });
            this.RaiseIfException(report);
        }


        private void Serial_DiscoveredDevicesHandler(object sender, List<SerialDeviceInfo> list) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003013, "Failure on Serial_DiscoveredDevicesHandler", () => {
                this.log.Info("Serial_DiscoveredDevicesHandler", () => string.Format("Is Serial_DiscoveredDevicesHandler null={0}", this.SerialDiscoveredDevices == null));
                // Replace OS params with saved cfg params to be what device is expecting on other side of USB
                this.GetSerialCfgList((index) => {
                    foreach (var item in list) {
                        this.UpdateSerialFromCfg(index, item);
                    }
                }, (err) => { });
                this.SerialDiscoveredDevices?.Invoke(sender, list);
            });
            this.RaiseIfException(report);
        }


        private void SerialStack_MsgReceivedHandler(object sender, byte[] data) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003014, "Failure on SerialStack_MsgReceivedHandler", () => {
                string msg = Encoding.ASCII.GetString(data, 0, data.Length);
                this.log.Info("SerialStack_MsgReceivedHandler", () => string.Format("Msg In: '{0}'", msg));
                this.Serial_BytesReceived?.Invoke(sender, msg);
            });
            this.RaiseIfException(report);
        }


        private string GetSerialErrText(SerialErrorCode code) {
            switch (code) {
                case SerialErrorCode.None: return this.GetText(MsgCode.None);
                case SerialErrorCode.NotFound: return this.GetText(MsgCode.NotFound);
                case SerialErrorCode.NotConnected: return this.GetText(MsgCode.NotConnected);
                case SerialErrorCode.ReadFailure: return this.GetText(MsgCode.ReadFailure);
                case SerialErrorCode.WriteFailure: return this.GetText(MsgCode.WriteFailue);
                case SerialErrorCode.Unknown: return this.GetText(MsgCode.UnknownError);
                //case SerialErrorCode.RetrieveFailed: return this.GetText(MsgCode.);
                default: 
                    return code.ToString().CamelCaseToSpaces();
            }
        }


        private void SerialConnectCompleteHandler(object sender, MsgPumpResults results) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2003015, "Failure on SerialConnectCompleteHandler", () => {
                this.OnSerialConnectionAttemptCompleted?.Invoke(sender, results);
            });
            this.RaiseIfException(report);

        }


        /// <summary>Update the values loaded from OS with saved parameters for that device</summary>
        /// <param name="index">The configuration index</param>
        /// <param name="item">The OS loaded device info to be updated</param>
        private void UpdateSerialFromCfg(List<IIndexItem<SerialIndexExtraInfo>> index, SerialDeviceInfo item) {
            var ndx = index.FirstOrDefault((i) =>
                (i.ExtraInfoObj.PortName == item.PortName) &&
                (i.ExtraInfoObj.USBProductId == item.USB_ProductId) &&
                (i.ExtraInfoObj.USBVendorId == item.USB_VendorId));
            if (ndx != null) {
                this.RetrieveSerialCfg(ndx, (cfg) => {
                    item.Baud = cfg.Baud;
                    item.DataBits = cfg.DataBits;
                    item.StopBits = cfg.StopBits;
                    item.Parity = cfg.Parity;
                    item.FlowHandshake = cfg.FlowHandshake;
                    item.ReadTimeout = cfg.ReadTimeout;
                    item.WriteTimeout = cfg.WriteTimeout;
                    item.HasCfg = true;
                }, (err) => { });
            }
        }


        private void GetSerialIndex(SerialDeviceInfo item, Action<IIndexItem<SerialIndexExtraInfo>> onSuccess, OnErr onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200309999, "Failure on GetSerialIndex", () => {
                this.GetSerialCfgList((index) => {
                    var ndx = index.FirstOrDefault((i) =>
                        (i.ExtraInfoObj.PortName == item.PortName) &&
                        (i.ExtraInfoObj.USBProductId == item.USB_ProductId) &&
                        (i.ExtraInfoObj.USBVendorId == item.USB_VendorId));
                    if (ndx != null) {
                        onSuccess.Invoke(ndx);
                    }
                    else {
                        onError.Invoke(this.GetText(MsgCode.NotFoundSettings));
                    }
                }, onError);
            });
            this.RaiseIfException(report);
        }



        #endregion


    }
}
