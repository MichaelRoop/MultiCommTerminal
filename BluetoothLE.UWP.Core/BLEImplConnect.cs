using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using System;
using System.Threading.Tasks;

namespace Bluetooth.UWP.Core {


    public partial class BluetoothLEImplWin32Core : IBLETInterface {


        private void AttachEvents() {
            this.currentDevice.NameChanged += CurrentDevice_NameChanged;


        }

        private async Task ConnectToDevice(BluetoothLEDeviceInfo deviceInfo) {
            this.log.Info("ConnectToDevice", () => string.Format("Attempting connection to {0}: FromIdAsync({1})",
                deviceInfo.Name, deviceInfo.Id));

            try {
                BLEGetInfoStatus result = await this.GetBLEDeviceInfo(deviceInfo, true);
                if (result.Status == BluetoothLE.Net.Enumerations.BLEOperationStatus.Success) {
                    if (this.currentDevice == null) {
                        // TODO raise failed
                    }
                    else {
                        // We have a device connected
                        this.log.Info("ConnectToDevice", () => string.Format("Device:{0} Connection status {1}",
                            this.currentDevice.Name, this.currentDevice.ConnectionStatus.ToString()));
                    }
                }
                else {
                    // TODO raise failed
                }


                #region OLD
                //// https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs
                //this.log.Info("ConnectToDevice", () => string.Format("--------------------------------------------------------------------"));
                //this.log.Info("ConnectToDevice", () => string.Format(" Param Device Info ID {0}", deviceInfo.Id));
                //this.currentDevice = await BluetoothLEDevice.FromIdAsync(deviceInfo.Id);

                //if (this.currentDevice == null) {
                //    this.log.Info("ConnectToDevice", "Connection failed");
                //}
                //else {
                //    this.log.Info("ConnectToDevice", "Connection ** OK **");
                //    this.currentDevice.NameChanged += CurrentDevice_NameChanged;
                //}


                //this.log.Info("ConnectToDevice", () => string.Format("Device:{0} Connection status {1}",
                //    this.currentDevice.Name, this.currentDevice.ConnectionStatus.ToString()));

                /*
                // This gives the catastrophic failure message.  I connected once but after that NADA.
                GattDeviceServicesResult services = await this.currentDevice.GetGattServicesAsync();
                if (services.Status == GattCommunicationStatus.Success) {
                    if (services.Services != null) {
                        if (services.Services.Count > 0) {
                            foreach (GattDeviceService serv in services.Services) {
                                // Service
                                this.log.Info("ConnectToDevice", () => string.Format("Gatt Service:{0}  Uid:{1}",
                                    BLE_DisplayHelpers.GetServiceName(serv), serv.Uuid.ToString()));

                                GattCharacteristicsResult characteristics = await serv.GetCharacteristicsAsync();
                                if (characteristics.Status == GattCommunicationStatus.Success) {
                                    if (characteristics.Characteristics != null) {
                                        if (characteristics.Characteristics.Count > 0) {
                                            foreach (GattCharacteristic ch in characteristics.Characteristics) {
                                                // TODO - this also does the dummy read write
                                                await this.DumpCharacteristic(ch);
                                                GattDescriptorsResult descriptors = await ch.GetDescriptorsAsync();
                                                if (descriptors.Status == GattCommunicationStatus.Success) {
                                                    if (descriptors.Descriptors.Count > 0) {
                                                        foreach (GattDescriptor desc in descriptors.Descriptors) {
                                                            GattReadResult r = await desc.ReadValueAsync();
                                                            string descName = "N/A";
                                                            if (r.Status == GattCommunicationStatus.Success) {
                                                                //descName = Encoding.ASCII.GetString(r.Value.FromBufferToBytes());
                                                                descName = BLE_ParseHelpers.GetDescriptorValueAsString(desc.Uuid, r.Value.FromBufferToBytes());
                                                            }

                                                            // descriptors have read and write
                                                            this.log.Info("ConnectToDevice", () => string.Format("        Descriptor:{0}  Uid:{1} Value:{2}",
                                                                BLE_DisplayHelpers.GetDescriptorName(desc), desc.Uuid.ToString(), descName));
                                                        }
                                                    }
                                                }
                                                else {
                                                    this.log.Info("ConnectToDevice", () => string.Format("        Get Descriptors result:{0}", descriptors.Status.ToString()));
                                                }
                                            }
                                        }
                                        else {
                                            this.log.Info("ConnectToDevice", () => string.Format("No characteristics"));
                                        }
                                    }
                                }
                                else {
                                    this.log.Info("ConnectToDevice", () => string.Format("    Characteristics result {0}", characteristics.Status.ToString()));
                                }
                            }
                        }
                        else {
                            this.log.Info("ConnectToDevice", "No services exposed");
                        }
                    }
                    else {
                        this.log.Error(9999, "Null services");
                    }
                }
                else {
                    this.log.Info("ConnectToDevice", () => string.Format("    Get Services result {0}", services.Status.ToString()));
                }

                this.log.Info("ConnectToDevice", () => string.Format("--------------------------------------------------------------------"));
                */
                #endregion

            }
            catch (Exception e) {
                this.log.Exception(9999, "BLE Connect Exception", e);
            }

            try {
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
            catch(Exception ex) {
                this.log.Exception(9999, "on main task", ex);
            }

        }

    }
}
