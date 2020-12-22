﻿using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using ChkUtils.Net;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {


        private async Task HarvestDeviceInfo(BluetoothLEDeviceInfo deviceDataModel) {
            this.log.Info("HarvestDeviceInfo", () => string.Format("Attempting connection to {0}: FromIdAsync({1})",
                deviceDataModel.Name, deviceDataModel.Id));

            try {
                BLEGetInfoStatus result = await this.GetBLEDeviceInfo(deviceDataModel, false);
                this.DeviceInfoAssembled?.Invoke(this, result);
            }
            catch (Exception e) {
                this.log.Exception(9999, "On raise event", e);
                try {
                    this.DeviceInfoAssembled?.Invoke(this, new BLEGetInfoStatus(deviceDataModel, BLEOperationStatus.UnknownError));
                }
                catch (Exception e2) {
                    this.log.Exception(9999, "On raise event unknown error", e2);
                }
            }


            #region Factored out into common GetBLEDeviceInfo
            //BluetoothLEDevice device = null;
            //BLEGetInfoStatus result = new BLEGetInfoStatus() {
            //    DeviceInfo = deviceDataModel,
            //};

            //try {
            //    // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs
            //    this.log.Info("HarvestDeviceInfo", () => string.Format("--------------------------------------------------------------------"));
            //    this.log.Info("HarvestDeviceInfo", () => string.Format(" Param Device Info ID {0}", deviceDataModel.Id));
            //    device = await BluetoothLEDevice.FromIdAsync(deviceDataModel.Id);
            //    deviceDataModel.InfoAttempted = true;
            //    this.UpdateDeviceOnConnect(device, deviceDataModel);

            //    // Clear services and get a new set for the passed in device info
            //    deviceDataModel.Services.Clear();

            //    try {
            //        // Attempts pair and causes catastropic failure if not supported
            //        // This will happen with Arduinos
            //        GattDeviceServicesResult services = await device.GetGattServicesAsync();
            //        if (services.Status == GattCommunicationStatus.Success) {
            //            if (services.Services != null) {
            //                if (services.Services.Count > 0) {
            //                    foreach (GattDeviceService service in services.Services) {
            //                        await this.BuildServiceDataModel(service, deviceDataModel);
            //                    }
            //                    result.Status = BLEOperationStatus.Success;
            //                }
            //                else {
            //                    result.Status = BLEOperationStatus.NoServices;
            //                    this.log.Info("HarvestDeviceInfo", "No services exposed");
            //                }
            //            }
            //            else {
            //                result.Status = BLEOperationStatus.GetServicesFailed;
            //                this.log.Error(9999, "Null services");
            //            }
            //        }
            //        else {
            //            result.Status = BLEOperationStatus.GetServicesFailed;
            //            this.log.Error(9999, "HarvestDeviceInfo", () => string.Format("    Get Services Failed {0}", services.Status.ToString()));
            //        }
            //    }
            //    catch(Exception e) {
            //        result.Status = BLEOperationStatus.GetServicesFailed;
            //        this.log.Exception(9999, "HarvestDeviceInfo", "Failure", e);
            //    }

            //}
            //catch (Exception e) {
            //    this.log.Exception(9999, "On harvest device info", e);
            //    // TODO - raise event with null device
            //    result.Status = BLEOperationStatus.UnhandledError;
            //}
            //finally {
            //    try {
            //        if (device != null) {
            //            device.Dispose();
            //            device = null;
            //        }
            //    }
            //    catch (Exception ex) {
            //        this.log.Exception(9999, "On fail to disconnect harvesting device data", ex);
            //    }
            //}
            //try {
            //    // Raise event
            //    this.DeviceInfoAssembled?.Invoke(this, result);
            //}
            //catch(Exception e) {
            //    this.log.Exception(9999, "On raise event", e);
            //}
            #endregion
        }



        private async Task<BLEGetInfoStatus> GetBLEDeviceInfo(BluetoothLEDeviceInfo deviceDataModel, bool forConnection) {
            this.log.Info("HarvestDeviceInfo", () => string.Format("Attempting connection to {0}: FromIdAsync({1})",
                deviceDataModel.Name, deviceDataModel.Id));

            BluetoothLEDevice device = null;
            BLEGetInfoStatus result = new BLEGetInfoStatus() {
                DeviceInfo = deviceDataModel,
            };

            try {
                // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs
                this.log.Info("HarvestDeviceInfo", () => string.Format("--------------------------------------------------------------------"));
                this.log.Info("HarvestDeviceInfo", () => string.Format(" Param Device Info ID {0}", deviceDataModel.Id));
                device = await BluetoothLEDevice.FromIdAsync(deviceDataModel.Id);
                deviceDataModel.InfoAttempted = true;
                this.UpdateDeviceOnConnect(device, deviceDataModel);

                // Clear services and get a new set for the passed in device info
                deviceDataModel.Services.Clear();

                try {
                    // Attempts pair and causes catastropic failure if not supported
                    // This will happen with Arduinos
                    GattDeviceServicesResult services = await device.GetGattServicesAsync();
                    if (services.Status == GattCommunicationStatus.Success) {
                        if (services.Services != null) {
                            if (services.Services.Count > 0) {
                                foreach (GattDeviceService service in services.Services) {
                                    await this.BuildServiceDataModel(service, deviceDataModel);
                                }
                                result.Status = BLEOperationStatus.Success;
                            }
                            else {
                                result.Status = BLEOperationStatus.NoServices;
                                this.log.Info("HarvestDeviceInfo", "No services exposed");
                            }
                        }
                        else {
                            this.log.Error(9999, "Null services");
                            result.Status = BLEOperationStatus.GetServicesFailed;
                        }
                    }
                    else {
                        this.log.Error(9999, "HarvestDeviceInfo", () => string.Format("    Get Services Failed {0}", services.Status.ToString()));
                        result.Status = BLEOperationStatus.GetServicesFailed;
                    }
                }
                catch (Exception e) {
                    result.Status = BLEOperationStatus.GetServicesFailed;
                    this.log.Exception(9999, "HarvestDeviceInfo", "Failure", e);
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "On harvest device info", e);
                // TODO - raise event with null device
                result.Status = BLEOperationStatus.UnhandledError;
            }
            finally {
                try {
                    if (!forConnection && device != null) {
                        device.Dispose();
                        device = null;
                    }
                    else {
                        if (result.Status == BLEOperationStatus.Success) {
                            this.currentDevice = device;
                        }
                    }
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "On fail to disconnect harvesting device data", ex);
                }
            }

            return result;
        }





    }
}
