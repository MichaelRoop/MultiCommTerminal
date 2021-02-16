using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.Tools;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using Common.Net.Network;
using LanguageFactory.Net.data;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Data

        private BLERangeValidator validator = new BLERangeValidator();

        #endregion

        #region Events

        /// <summary>Event raised when a device is discovered</summary>
        public event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceDiscovered;

        /// <summary>Event raised when a device is removed</summary>
        public event EventHandler<string> BLE_DeviceRemoved;

        /// <summary>Event raised when device discovery complete</summary>
        public event EventHandler<bool> BLE_DeviceDiscoveryComplete;

        /// <summary>Event raised when BLE device properties change</summary>
        public event EventHandler<NetPropertiesUpdateDataModel> BLE_DeviceUpdated;

        /// <summary>Raised when BLE info on a device is finished gathering</summary>
        public event EventHandler<BLEGetInfoStatus> BLE_DeviceInfoGathered;

        /// <summary>Raised on completion of a connection attempt</summary>
        public event EventHandler<BLEGetInfoStatus> BLE_DeviceConnectResult;

        /// <summary>Raised when BLE info on a device is finished gathering</summary>
        public event EventHandler<BLE_CharacteristicReadResult> BLE_CharacteristicReadValueChanged;

        /// <summary>Used to track device provoked disconnection after connection</summary>
        public event EventHandler<BLE_ConnectStatusChangeInfo> BLE_ConnectionStatusChanged;

        #endregion

        #region Event handlers

        private void BLE_DeviceDiscoveredHandler(object sender, BluetoothLEDeviceInfo e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200050, "Failure on BLE_DeviceDiscoveredHandler", () => {
                this.BLE_DeviceDiscovered?.Invoke(this, e);
            });
            this.RaiseIfException(report);
        }


        private void BLE_DeviceRemovedHandler(object sender, string e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200051, "Failure on BLE_DeviceRemovedHandler", () => {
                this.BLE_DeviceRemoved?.Invoke(this, e);
            });
        }


        private void BLE_DeviceUpdatedHandler(object sender, NetPropertiesUpdateDataModel args) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200052, "Failure on BLE_DeviceUpdatedHandler", () => {
                this.BLE_DeviceUpdated?.Invoke(this, args);
            });
        }


        private void BLE_DeviceDiscoveryCompleted(object sender, bool e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200053, "Failure on BLE_DeviceDiscoveryCompleted", () => {
                this.BLE_DeviceDiscoveryComplete?.Invoke(this, e);
            });
        }


        private void BleStack_MsgReceived(object sender, byte[] e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200054, "Failure on BleStack_MsgReceived", () => {
            });
        }


        private void BLE_CharacteristicReadValueChangeHandler(object sender, BLE_CharacteristicReadResult args) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200055, "Failure on BLE_CharacteristicReadValueChangeHandler", () => {
                this.BLE_CharacteristicReadValueChanged?.Invoke(sender, args);
            });
        }
        private void BleDeviceInfoAssembledHandler(object sender, BLEGetInfoStatus info) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200056, "Failure on BLEGetInfoStatus", () => {
                info.Message = this.Translate(info.Status);
                this.BLE_DeviceInfoGathered?.Invoke(this, info);
            });
            this.RaiseIfException(report);
        }


        private void BleDeviceConnectResultHandler(object sender, BLEGetInfoStatus info) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200057, "Failure on BLEGetInfoStatus", () => {
                info.Message = this.Translate(info.Status);
                this.BLE_DeviceConnectResult?.Invoke(this, info);
            });
            this.RaiseIfException(report);
        }


        private void BLEBluetooth_ConnectionStatusChanged(object sender, BLE_ConnectStatusChangeInfo e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200058, "Failure on BLE_ConnectStatusChangeInfo", () => {
                e.Message = e.Status == BLE_ConnectStatus.Connected 
                    ? this.GetText(MsgCode.Connected) 
                    : this.GetText(MsgCode.Disconnected);
                this.BLE_ConnectionStatusChanged?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }


        private string Translate(BLEOperationStatus status) {
            switch (status) {
                case BLEOperationStatus.Success:
                    return this.GetText(MsgCode.Ok);
                case BLEOperationStatus.NotFound:
                    return this.GetText(MsgCode.NotFound);
                case BLEOperationStatus.NoServices:
                    return this.GetText(MsgCode.NoServices);
                case BLEOperationStatus.GetServicesFailed:
                    return this.GetText(MsgCode.ServicesFailure);
                case BLEOperationStatus.Failed:
                    return this.GetText(MsgCode.UnknownError);
                case BLEOperationStatus.UnhandledError:
                    return this.GetText(MsgCode.UnhandledError);
                case BLEOperationStatus.UnknownError:
                    return this.GetText(MsgCode.UnknownError);
                default:
                    return status.ToString().CamelCaseToSpaces();
            }
        }

        #endregion

        #region Public

        public void BLE_DiscoverAsync() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200056, "Failure on BLE_DiscoverAsync", () => {
                this.bleBluetooth.DiscoverDevices();
            });
        }


        public void BLE_CancelDiscover() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200057, "Failure on BLE_CancelDiscover", () => {
                this.bleBluetooth.CancelDiscoverDevices();
            });
        }


        public void BLE_ConnectAsync(BluetoothLEDeviceInfo device) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200058, "Failure on BLE_ConnectAsync", () => {
                this.bleBluetooth.Connect(device);
            });
        }



        public void BLE_GetInfo(BluetoothLEDeviceInfo device) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200059, "Failure on BluetoothLEDeviceInfo", () => {
                this.bleBluetooth.GetInfo(device);
            });
        }


        public void BLE_GetDbgInfoStringDump(object obj, Action<string, string> onComplete) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, "", () => {
                BluetoothLEDeviceInfo info = obj as BluetoothLEDeviceInfo;
                if (info == null) {
                    onComplete("** NOT  A BLE DEVICE **", "** NOT  A BLE DEVICE **");
                    return;
                }

                StringBuilder sb = new StringBuilder();
                sb.Append(string.Format("        Id: {0}", info.Id)).AppendLine();
                sb.Append(string.Format(" IsDefault: {0}", info.IsDefault)).AppendLine();
                sb.Append(string.Format(" IsEnabled: {0}", info.IsEnabled)).AppendLine();
                //sb.Append("     Kind: {0}", device.Kind);
                // Properties
                sb.Append(string.Format("Properties: ({0})", info.ServiceProperties.Count)).AppendLine();
                foreach (var p in info.ServiceProperties) {
                    sb.Append("    ").Append(p.Key).Append(" : ").Append(p.Value.Value.ToString()).AppendLine();
                }
                #region OS Specific to implement in the abstract
                //// Enclosure location
                //if (device.EnclosureLocation != null) {
                //    System.Diagnostics.Debug.WriteLine("EnclosureLocation:");
                //    System.Diagnostics.Debug.WriteLine("     InDock: {0}", device.EnclosureLocation.InDock);
                //    System.Diagnostics.Debug.WriteLine("      InLid: {0}", device.EnclosureLocation.InLid);
                //    System.Diagnostics.Debug.WriteLine("      Panel: {0}", device.EnclosureLocation.Panel);
                //    System.Diagnostics.Debug.WriteLine("      Angle: {0}", device.EnclosureLocation.RotationAngleInDegreesClockwise);
                //}
                //else {
                //    System.Diagnostics.Debug.WriteLine("EnclosureLocation: null");
                //}
                //// Pairing
                //if (device.Pairing != null) {
                //    System.Diagnostics.Debug.WriteLine("Pairing:");
                //    System.Diagnostics.Debug.WriteLine("    CanPair: {0}", device.Pairing.CanPair);
                //    System.Diagnostics.Debug.WriteLine("   IsPaired: {0}", device.Pairing.IsPaired);
                //    System.Diagnostics.Debug.WriteLine(" Protection: {0}", device.Pairing.ProtectionLevel);
                //    if (device.Pairing.Custom != null) {
                //        System.Diagnostics.Debug.WriteLine("     Custom: not null");
                //    }
                //    else {
                //        System.Diagnostics.Debug.WriteLine("Custom: null");
                //    }
                //}
                //else {
                //    System.Diagnostics.Debug.WriteLine("Custom: null");
                //}

                //MessageBox.Show(sb.ToString(), info.Name);

                //return "";
                #endregion
                onComplete.Invoke(info.Name, sb.ToString());
            });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => onComplete("** FAILED **", "** FAILED **"));
            }
        }


        public void BLE_Disconnect() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200060, "Failure on BLE_Disconnect", () => {
                this.bleBluetooth.Disconnect();
            });
        }


        public List<KeyValuePropertyDisplay> BLE_GetDeviceInfoForDisplay(BluetoothLEDeviceInfo info) {
            try {
                // TODO - language
                List<KeyValuePropertyDisplay> list = new List<KeyValuePropertyDisplay>();
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Name), info.Name));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Id), info.Id));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.AccessStatus), info.AccessStatus.ToString().CamelCaseToSpaces()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Address), info.AddressAsULong.ToString()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.AddressType), info.AddressType.ToString().CamelCaseToSpaces()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Default), info.IsDefault.ToString()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Enabled), info.IsEnabled.ToString()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Kind), info.DeviceKind.ToString().UnderlineToSpaces()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.PairingAllowed), info.CanPair.ToString()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Paired), info.IsPaired.ToString()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.PairedWithSecureConnection), info.WasPairedUsingSecureConnection.ToString()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Connected), info.IsConnected.ToString()));
                list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.ProtectionLevel), info.ProtectionLevel.ToString().CamelCaseToSpaces()));
                list.Add(new KeyValuePropertyDisplay(string.Format("{0} (Bluetooth)", this.GetText(MsgCode.Kind)), info.TypeBluetooth.ToString().CamelCaseToSpaces()));
                list.Add(new KeyValuePropertyDisplay("Enclosure(Dock)", info.EnclosureLocation.InDock.ToString()));
                list.Add(new KeyValuePropertyDisplay("Enclosure(Lid)", info.EnclosureLocation.InLid.ToString()));
                list.Add(new KeyValuePropertyDisplay("Enclosure(Clockwise Rotation)", info.EnclosureLocation.ClockWiseRotationInDegrees.ToString()));
                list.Add(new KeyValuePropertyDisplay("Enclosure(Panel)", info.EnclosureLocation.Location.ToString()));
                return list;
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                return new List<KeyValuePropertyDisplay>();
            }
        }


        public List<NetPropertyDataModelDisplay> BLE_GetServiceProperties(BluetoothLEDeviceInfo info) {
            try {
                List<NetPropertyDataModelDisplay> list = new List<NetPropertyDataModelDisplay>();
                foreach (var sp in info.ServiceProperties) {
                    list.Add(new NetPropertyDataModelDisplay(sp.Value));
                }
                return list;
            }
            catch(Exception e) {
                this.log.Exception(9999, "", e);
                return new List<NetPropertyDataModelDisplay>();
            }
        }


        public void BLE_Send(string value, BLE_CharacteristicDataModel dataModel, Action onSuccess, OnErr onError) {
            try {
                if (dataModel != null) {
                    RangeValidationResult result = dataModel.Write(value);
                    if (result.Status == BLE_DataValidationStatus.Success) {
                        onSuccess.Invoke();
                    }
                    else {
                        onError.Invoke(this.Translate(result));
                    }
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                WrapErr.SafeAction(() => {
                    onError.Invoke(this.GetText(MsgCode.UnhandledError));
                });
            }
        }


        public void BLE_GetRangeDisplay(BLE_CharacteristicDataModel dataModel, Action<string, string> onSuccess, OnErr onError) {
            try {
                if (dataModel != null) {
                    DataTypeDisplay display = this.validator.GetRange(dataModel.Parser.DataType);
                    onSuccess(
                        dataModel.CharName,
                        string.Format("{0}: {1},  {2}: {3},  {4}: {5}",
                        this.GetText(MsgCode.DataType), display.DataType,
                        this.GetText(MsgCode.Min), display.Min,
                        this.GetText(MsgCode.Max), display.Max));
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                WrapErr.SafeAction(() => {
                    onError.Invoke(this.GetText(MsgCode.UnhandledError));
                });
            }
        }

        #endregion

        #region Init and teardown

        private void BLE_TearDown() {
            this.bleBluetooth.Disconnect();
            this.bleStack.MsgReceived -= this.BleStack_MsgReceived;
            this.bleBluetooth.DeviceDiscovered -= this.BLE_DeviceDiscoveredHandler;
            this.bleBluetooth.DeviceRemoved -= this.BLE_DeviceRemovedHandler;
            this.bleBluetooth.DeviceUpdated -= BLE_DeviceUpdatedHandler;
            this.bleBluetooth.DeviceDiscoveryCompleted -= this.BLE_DeviceDiscoveryCompleted;
            this.bleBluetooth.DeviceInfoAssembled -= this.BleDeviceInfoAssembledHandler;
            this.bleBluetooth.DeviceConnectResult -= this.BleDeviceConnectResultHandler;
            this.bleBluetooth.CharacteristicReadValueChanged -= this.BLE_CharacteristicReadValueChangeHandler;
            this.bleBluetooth.ConnectionStatusChanged -= this.BLEBluetooth_ConnectionStatusChanged;
        }


        private string Translate(RangeValidationResult result) {
            switch (result.Status) {
                case BLE_DataValidationStatus.Success:
                    return this.GetText(MsgCode.Ok);
                case BLE_DataValidationStatus.OutOfRange:
                    return "Out of range"; // TODO
                case BLE_DataValidationStatus.StringConversionFailed:
                    return "Parse failed"; // TODO
                case BLE_DataValidationStatus.Empty:
                    return this.GetText(MsgCode.EmptyParameter);
                case BLE_DataValidationStatus.InvalidInput:
                    return "Invalid Input"; // TODO
                case BLE_DataValidationStatus.NotHandled:
                    return this.GetText(MsgCode.UnhandledError);
                case BLE_DataValidationStatus.UnhandledError:
                    return this.GetText(MsgCode.UnhandledError);
                default:
                    return "ERR";
            }
        }

        #endregion

    }
}
