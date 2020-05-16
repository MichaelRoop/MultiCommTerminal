using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothLE.Win32 {

    public partial class BluetoothLEImplWin32 : IBLETInterface {

        public async Task BuildServiceDataModel(GattDeviceService service, BluetoothLEDeviceInfo deviceDataModel) {
            this.log.Info("BuildServiceDataModel", () => string.Format("Gatt Service:{0}  Uid:{1}",
               BLE_DisplayHelpers.GetServiceName(service), service.Uuid.ToString()));

            // New service data model to add to device info
            BLE_ServiceDataModel serviceDataModel = new BLE_ServiceDataModel() {
                Characteristics = new Dictionary<string, BLE_CharacteristicDataModel>(),
                AttributeHandle = service.AttributeHandle,
                DeviceId = deviceDataModel.Id,
                DisplayName = BLE_DisplayHelpers.GetServiceName(service),
                Uuid = service.Uuid,
            };

            // Get the characteristics for the service
            GattCharacteristicsResult characteristics = await service.GetCharacteristicsAsync();
            if (characteristics.Status == GattCommunicationStatus.Success) {
                if (characteristics.Characteristics != null) {
                    if (characteristics.Characteristics.Count > 0) {
                        foreach (GattCharacteristic ch in characteristics.Characteristics) {
                            await this.BuildCharacteristicDataModel(ch, serviceDataModel);
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
            deviceDataModel.Services.Add(serviceDataModel.Uuid.ToString(), serviceDataModel);
        }

    }

}
