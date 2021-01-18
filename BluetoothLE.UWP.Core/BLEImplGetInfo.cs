using BluetoothLE.Net.DataModels;
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
            this.log.Info("HarvestDeviceInfo", () => string.Format("Harvesting from {0}: FromIdAsync({1})",
                deviceDataModel.Name, deviceDataModel.Id));

            try {
                BLEGetInfoStatus result = await this.GetBLEDeviceInfo(deviceDataModel);
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


        private async Task<BLEGetInfoStatus> GetBLEDeviceInfo(BluetoothLEDeviceInfo deviceDataModel) {
            this.log.InfoEntry("GetBLEDeviceInfo");
            BLEGetInfoStatus result = await this.GetDevice(deviceDataModel);
            if (result.Status != BLEOperationStatus.Success) {
                return result;
            }

            try {
                deviceDataModel.Services.Clear();
                GattDeviceServicesResult services = await this.currentDevice.GetGattServicesAsync(BluetoothCacheMode.Cached);
                if (services.Status != GattCommunicationStatus.Success) {
                    return this.BuildConnectFailure(BLEOperationStatus.GetServicesFailed, services.Status.ToString());
                }

                if (services.Services == null) {
                    return this.BuildConnectFailure(BLEOperationStatus.GetServicesFailed, "Null Services");
                }

                if (services.Services.Count == 0) {
                    return this.BuildConnectFailure(BLEOperationStatus.NoServices, "No services exposed");
                }

                result.Status = BLEOperationStatus.Success;
                result.DeviceInfo = deviceDataModel;
                foreach (GattDeviceService service in services.Services) {
                    // TODO make sure status is set in functions
                    await this.BuildServiceDataModel(service, result);
                }

                return result;
            }
            catch (Exception e) {
                this.log.Exception(9999, "HarvestDeviceInfo", "Failure", e);
                return this.BuildConnectFailure(BLEOperationStatus.GetServicesFailed, "Exception on getting services");
            }
        }


        private async Task<BLEGetInfoStatus> GetDevice(BluetoothLEDeviceInfo deviceDataModel) {
            this.log.Info("GetBLEDevice", () => string.Format("Attempting to get device for {0}: FromIdAsync({1})",
                deviceDataModel.Name, deviceDataModel.Id));
            try {
                // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs
                this.log.Info("HarvestDeviceInfo", () => string.Format("--------------------------------------------------------------------"));
                this.log.Info("HarvestDeviceInfo", () => string.Format(" Param Device Info ID {0}", deviceDataModel.Id));
                this.currentDevice = await BluetoothLEDevice.FromIdAsync(deviceDataModel.Id);
                deviceDataModel.InfoAttempted = true;
                this.UpdateDeviceInfo(this.currentDevice, deviceDataModel);
                return new BLEGetInfoStatus(BLEOperationStatus.Success);
            }
            catch (Exception e) {
                this.log.Exception(9999, "On harvest device info", e);
                return this.BuildConnectFailure(BLEOperationStatus.UnhandledError, "Exception connecting to device");
            }
        }


        private BLEGetInfoStatus BuildConnectFailure(BLEOperationStatus status, string logMsg) {
            this.log.Error(9999, "BuildConnectFailure", () => string.Format("{0} {1}", logMsg, status.ToString()));
            this.DoDisconnect();
            return new BLEGetInfoStatus(status);
        }


    }
}
