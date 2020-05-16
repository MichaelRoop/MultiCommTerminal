using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using BluetoothLE.Net.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothLE.Win32 {

    public partial class BluetoothLEImplWin32 : IBLETInterface {


        private async Task HarvestDeviceInfo(BluetoothLEDeviceInfo deviceInfo) {
            this.log.Info("HarvestDeviceInfo", () => string.Format("Attempting connection to {0}: FromIdAsync({1})",
                deviceInfo.Name, deviceInfo.Id));

            BluetoothLEDevice device = null;

            try {
                // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs
                this.log.Info("HarvestDeviceInfo", () => string.Format("--------------------------------------------------------------------"));
                this.log.Info("HarvestDeviceInfo", () => string.Format(" Param Device Info ID {0}", deviceInfo.Id));
                device = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);

                // Clear services and get a new set for the passed in device info
                deviceInfo.Services.Clear();
                GattDeviceServicesResult services = await device.GetGattServicesAsync();
                if (services.Status == GattCommunicationStatus.Success) {
                    if (services.Services != null) {
                        if (services.Services.Count > 0) {
                            foreach (GattDeviceService service in services.Services) {
                                this.log.Info("ConnectToDevice", () => string.Format("Gatt Service:{0}  Uid:{1}",
                                    BLE_DisplayHelpers.GetServiceName(service), service.Uuid.ToString()));

                                // New service data model to add to device info
                                BLE_ServiceDataModel dmService = new BLE_ServiceDataModel() {
                                    Characteristics = new Dictionary<string, BLE_CharacteristicDataModel>(),
                                    AttributeHandle = service.AttributeHandle, 
                                    DeviceId = deviceInfo.Id,
                                    DisplayName = BLE_DisplayHelpers.GetServiceName(service),
                                    Uuid = service.Uuid,
                                };

                                // Get the characteristics for the service
                                GattCharacteristicsResult characteristics = await service.GetCharacteristicsAsync();
                                if (characteristics.Status == GattCommunicationStatus.Success) {
                                    if (characteristics.Characteristics != null) {
                                        if (characteristics.Characteristics.Count > 0) {
                                            foreach (GattCharacteristic ch in characteristics.Characteristics) {
                                                await this.BuildCharacteristicDataModel(ch, dmService);
                                            }
                                        }
                                        else {
                                            this.log.Info("ConnectToDevice", () => string.Format("No characteristics"));
                                        }
                                    }
                                }
                                else {
                                    this.log.Error(9999, "HarvestDeviceInfo", () => string.Format("Failed to get Characteristics result {0}", characteristics.Status.ToString()));
                                }

                                // Add the service data model to the device info data model
                                deviceInfo.Services.Add(dmService.Uuid.ToString(), dmService);

                            } // End of foreach Services
                        }
                        else {
                            this.log.Info("HarvestDeviceInfo", "No services exposed");
                        }
                    }
                    else {
                        this.log.Error(9999, "Null services");
                    }
                }
                else {
                    this.log.Error(9999, "HarvestDeviceInfo", () => string.Format("    Get Services Failed {0}", services.Status.ToString()));
                }

            }
            catch (Exception e) {
                this.log.Exception(9999, "On harvest device info", e);
                // TODO - raise event with null device


            }
            finally {
                try {
                    if (device != null) {
                        device.Dispose();
                        device = null;
                    }

                    // Raise event
                    this.DeviceInfoAssembled?.Invoke(this, deviceInfo);
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "On fail to disconnect harvesting device data", ex);
                }
            }
        }


    }
}
