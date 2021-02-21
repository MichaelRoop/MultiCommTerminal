using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using BluetoothLE.Net.Parsers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;


// To use the full 

// Example code read write
// https://www.c-sharpcorner.com/code/1912/uwp-bluetooth-le-implementation
// https://github.com/nexussays/ble.net/blob/master/Readme.md

namespace Bluetooth.UWP.Core {

    // Using the UWP Bluetooth LE calls from Win 32 desktop - NOT USING THIS ONE ANYMORE
    //https://social.msdn.microsoft.com/Forums/vstudio/en-US/e261aeb5-104d-43ba-9b2b-97447614dec0/how-to-use-windowsdevices-api-in-a-c-winform-desktop-application-in-windows-10?forum=winforms
    // Add Reference to  \Program Files\Windows Kits\10\UnionMetadata\Windows.winmd
    // Add Reference to \Program Files\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll
    // Regardless what Target Framework i'm using, the only working version of System.Runtime.WindowsRuntime.dll was v4.5. All other version (wich corresponds to the Target Framework) caused runtime errors.


    // NOW USING THIS METHOD TO USE THE FULL POWER OF UWP    
    // https://docs.microsoft.com/en-us/windows/apps/desktop/modernize/desktop-to-uwp-enhance

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        /// <summary>Build the GATT service data model</summary>
        /// <param name="service">The OS GATT service object</param>
        /// <param name="deviceDataModel">The portable GATT session data model</param>
        /// <returns>The async task</returns>
        public async Task BuildServiceDataModel(GattDeviceService service, BLEGetInfoStatus status) {
            try {
                this.log.Info("BuildServiceDataModel", () => string.Format("Gatt Service:{0}  Uid:{1}",
                   BLE_DisplayHelpers.GetServiceName(service), service.Uuid.ToString()));

                // New service data model to add to device info
                BLE_ServiceDataModel serviceDataModel = new BLE_ServiceDataModel() {
                    Characteristics = new List<BLE_CharacteristicDataModel>(),
                    AttributeHandle = service.AttributeHandle,
                    DeviceId = status.DeviceInfo.Id,
                    ServiceTypeEnum = BLE_ParseHelpers.GetServiceTypeEnum(service.Uuid),
                    DisplayName = BLE_DisplayHelpers.GetServiceName(service),
                    Uuid = service.Uuid,
                    SharingMode = (BLE_SharingMode)service.SharingMode,
                };

                if (service.DeviceAccessInformation != null) {
                    serviceDataModel.DeviceAccess = (BLE_DeviceAccessStatus)service.DeviceAccessInformation.CurrentStatus;
                }
                this.currentServices.Add(service);
                this.BuildSessionDataModel(service.Session, serviceDataModel.Session);

                // TODO
                //service.ParentServices

                // Get the characteristics for the service
                GattCharacteristicsResult characteristics = await service.GetCharacteristicsAsync();
                if (characteristics.Status == GattCommunicationStatus.Success) {
                    if (characteristics.Characteristics != null) {
                        if (characteristics.Characteristics.Count > 0) {
                            foreach (GattCharacteristic ch in characteristics.Characteristics) {
                                try {
                                    await this.BuildCharacteristicDataModel(ch, serviceDataModel);
                                }
                                catch (Exception e1) {
                                    this.log.Exception(9999, "HarvestDeviceInfo", e1);
                                }
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
                status.DeviceInfo.Services.Add(serviceDataModel);
            }
            catch(Exception e) {
                this.log.Exception(9999, "BuildServiceDataModel", "", e);
            }
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
