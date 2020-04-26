using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
// The using for the RFCOMM to communicate with classic items
using Windows.Devices.Bluetooth.Rfcomm;

namespace BluetoothLE.Win32 {


    public partial class BluetoothLEImplWin32 : IBLETInterface {


        private async Task ConnectToDevice(BluetoothLEDeviceInfo deviceInfo) {
            this.log.Info("ConnectToDevice", () => string.Format("Attempting connection to {0}: FromIdAsync({1})",
                deviceInfo.Name, deviceInfo.Id));

            try {
                // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs

                this.log.Info("ConnectToDevice", () => string.Format("--------------------------------------------------------------------"));
                this.log.Info("ConnectToDevice", () => string.Format("Stored Device Info ID {0}", this.id));
                this.log.Info("ConnectToDevice", () => string.Format(" Param Device Info ID {0}", deviceInfo.Id));

                //this.currentDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);
                this.currentDevice = await BluetoothLEDevice.FromIdAsync(this.id);

                if (this.currentDevice == null) {
                    this.log.Info("ConnectToDevice", "Connection failed");
                }
                else {
                    this.log.Info("ConnectToDevice", "Connection ** OK **");
                }

                // This just does the easy serial communications - this is using a regular HC-05 Classic (RFCOMM) board
                //RfcommDeviceService s = await RfcommDeviceService.FromIdAsync(this.id);
                //BluetoothDevice.GetRfcommServicesAsync();


                this.log.Info("ConnectToDevice", () => string.Format("Device {0} Connection status {1}",
                    this.currentDevice.Name, this.currentDevice.ConnectionStatus.ToString()));

                // TODO Flipped the define until I can get this going with newer API
#if USING_OLDER_UWP
                this.log.Info("ConnectToDevice", () => string.Format("Device {0} Gatt services count {1}",
                    this.currentDevice.Name, this.currentDevice.GattServices == null ? "NULL" : this.currentDevice.GattServices.Count.ToString()));

                if (this.currentDevice.GattServices != null) {
                    foreach (var serv in this.currentDevice.GattServices) {
                        this.log.Info("ConnectToDevice", () => string.Format("Gatt Service:{0}  Uid:{1}",
                            BLE_DisplayHelpers.GetServiceName(serv), serv.Uuid.ToString()));
                        //GattDeviceService s = this.currentDevice.GetGattService(serv.Uuid);
                        this.log.Info("ConnectToDevice", "    CHARACTERISTICS");
                        foreach (var ch in serv.GetAllCharacteristics()) {
                            this.log.Info("ConnectToDevice", () => string.Format("    Characteristic:{0}  Uid:{1} - Desc:'{2}' ",
                                BLE_DisplayHelpers.GetCharacteristicName(ch), ch.Uuid.ToString(), ch.UserDescription));
                            foreach (var desc in ch.GetAllDescriptors()) {
                                // descriptors have read and write
                                this.log.Info("ConnectToDevice", () => string.Format("        Descriptor:{0}  Uid:{1}",
                                    BLE_DisplayHelpers.GetDescriptorName(desc), desc.Uuid.ToString()));
                            }
                        }

                    }
                }
#else
                GattDeviceServicesResult services = await this.currentDevice.GetGattServicesAsync();
                // Short resume
                this.log.Info("ConnectToDevice", () => string.Format(
                    "Overview services. Search result:{0} Count:{1}", services.Status.ToString(), services.Services.Count));
                foreach (var s in services.Services) {
                    this.log.Info("ConnectToDevice", () => string.Format(
                        "Service Description:{0}  Uid:{1}",
                        BLE_DisplayHelpers.GetServiceName(s), s.Uuid.ToString()));
                }

                this.log.Info("ConnectToDevice", "Details");

                //services.Status == GattCommunicationStatus.Success
                this.log.Info("ConnectToDevice", () => string.Format("Device {0} Get Services status {1}",
                    this.currentDevice.Name, services.Status.ToString()));
                if (services.Status == GattCommunicationStatus.Success) {
                    if (services.Services != null) {
                        this.log.Info("ConnectToDevice", () => string.Format("Device {0} Gatt services count {1}",
                            this.currentDevice.Name, services.Services.Count.ToString()));
                        foreach (GattDeviceService serv in services.Services) {
                            // Service
                            this.log.Info("ConnectToDevice", () => string.Format("Gatt Service:{0}  Uid:{1}",
                                BLE_DisplayHelpers.GetServiceName(serv), serv.Uuid.ToString()));
                            
                            this.log.Info("ConnectToDevice", "    CHARACTERISTICS");
                            GattCharacteristicsResult characteristics = await serv.GetCharacteristicsAsync();
                            this.log.Info("ConnectToDevice", () => string.Format("    Characteristics result {0}", characteristics.Status.ToString()));
                            if (characteristics.Status == GattCommunicationStatus.Success) {
                                if (characteristics.Characteristics != null) {
                                    foreach (var ch in characteristics.Characteristics) {
                                        this.log.Info("ConnectToDevice", () => string.Format("    Characteristic:{0}  Uid:{1} - Desc:'{2}' ",
                                            BLE_DisplayHelpers.GetCharacteristicName(ch), ch.Uuid.ToString(), ch.UserDescription));
                                        var descriptors = await ch.GetDescriptorsAsync();
                                        this.log.Info("ConnectToDevice", "        DESCRIPTORS");
                                        this.log.Info("ConnectToDevice", () => string.Format("        Get Descriptors result:{0}", descriptors.Status.ToString()));
                                        if (descriptors.Status == GattCommunicationStatus.Success) {
                                            this.log.Info("ConnectToDevice", () => string.Format("        Descriptors Count:{0}", descriptors.Descriptors.Count));
                                            foreach (var desc in descriptors.Descriptors) {
                                                // descriptors have read and write
                                                this.log.Info("ConnectToDevice", () => string.Format("        Descriptor:{0}  Uid:{1}",
                                                    BLE_DisplayHelpers.GetDescriptorName(desc), desc.Uuid.ToString()));
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                    else {
                        this.log.Error(9999, "Null services");
                    }
                }

#endif


                this.log.Info("ConnectToDevice", "Get GATT services");
#if USING_OLDER_UWP
                //// This blows up with OLD and New
                //// ArgumentException : Value does not fall within the expected range.
                //GattDeviceService service = await GattDeviceService.FromIdAsync(this.id);
                //foreach (var s in service.GetAllCharacteristics()) {
                //    this.log.Info("ConnectToDevice", () => string.Format("Service Description: {0}",
                //        s.Uuid.ToString()));
                //}
#else
                //GattDeviceServicesResult result = await this.currentDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                //this.log.Info("ConnectToDevice", () => string.Format("Service search result {0}", result.Status.ToString()));
                //foreach (var s in result.Services) {
                //    this.log.Info("ConnectToDevice", () => string.Format(
                //        "Service Description:{0}  Uid:{1}",
                //        BLE_DisplayHelpers.GetServiceName(s), s.Uuid.ToString()));
                //}
#endif
                this.log.Info("ConnectToDevice", () => string.Format("--------------------------------------------------------------------"));

            }
            catch (Exception e) {
                this.log.Exception(9999, "Exception", e);
            }

            if (this.currentDevice == null) {
                // report error
                this.log.Info("ConnectToDevice", () => string.Format("NULL device returned for {0}", deviceInfo.Id));
                return;
            }
            else {
                // Note: BluetoothLEDevice.GattServices property will return an empty list for unpaired devices. For all uses we recommend using the GetGattServicesAsync method.
                // BT_Code: GetGattServicesAsync returns a list of all the supported services of the device (even if it's not paired to the system).
                // If the services supported by the device are expected to change during BT usage, subscribe to the GattServicesChanged event.
                //GattDeviceServicesResult result =
                //    await this.currentDevice.GetGattServicesAsync(BluetoothCacheMode.Uncached);
                ////GattDeviceServicesResult result = await BluetoothLEDevice.FromIdAsync(this.currentDevice.DeviceId);
                //System.Diagnostics.Debug.WriteLine("Device Connected {0}", this.currentDevice.BluetoothAddress);
                this.log.Info("ConnectToDevice", () => string.Format("Device Connected {0}", this.currentDevice.BluetoothAddress));
            }

        }



    }
}
