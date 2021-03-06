﻿using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothLE.Win32 {

    public partial class BluetoothLEImplWin32 : IBLETInterface {

        /// <summary>Build the GATT service data model</summary>
        /// <param name="service">The OS GATT service object</param>
        /// <param name="deviceDataModel">The portable GATT session data model</param>
        /// <returns>The async task</returns>
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
                SharingMode = (BLE_SharingMode)service.SharingMode,
            };

            if (service.DeviceAccessInformation != null) {
                serviceDataModel.DeviceAccess = (BLE_DeviceAccessStatus)service.DeviceAccessInformation.CurrentStatus;
            }
            this.BuildSessionDataModel(service.Session, serviceDataModel.Session);

            // TODO
            //service.ParentServices

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


        /// <summary>Populate the portable session data model with Win objects</summary>
        /// <param name="session">The Windows GATT session</param>
        /// <param name="dataModel">The portable data model</param>
        private void BuildSessionDataModel(GattSession session, BLE_GattSession dataModel) {
            if (session != null) {
                dataModel.CanConnectionBeMaintained = session.CanMaintainConnection;
                dataModel.ShouldConnectionBeMaintained = session.MaintainConnection;
                dataModel.MaxPDUSize = session.MaxPduSize;
                dataModel.SessionStatus = (BLE_GattSessionStatus)session.SessionStatus;

                if (session.DeviceId != null) {
                    dataModel.DeviceId = session.DeviceId.Id;
                    dataModel.IsClassic = session.DeviceId.IsClassicDevice;
                    dataModel.IsLowEnergy = session.DeviceId.IsLowEnergyDevice;
                }
            }
        }

    }

}
