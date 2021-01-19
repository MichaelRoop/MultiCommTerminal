using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using System;
using System.Threading.Tasks;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        /// <summary>Raised on BLE connection attempt</summary>
        public event EventHandler<BLEGetInfoStatus> DeviceConnectResult;

        /// <summary>Raised when BLE info on a device is finished gathering</summary>
        public event EventHandler<BLE_CharacteristicReadResult> CharacteristicReadValueChanged;


        private async Task ConnectToDeviceAsync(BluetoothLEDeviceInfo deviceInfo) {
            try {
                this.log.Info("ConnectToDeviceAsync", () => 
                    string.Format("{0} ID:{1}", deviceInfo.Name, deviceInfo.Id));
                BLEGetInfoStatus result = await this.GetBLEDeviceInfo(deviceInfo);
                this.ConnectBTLEDeviceEvents(result.Status);
                this.RaiseConnectAttemptResult(result);
            }
            catch (Exception e) {
                this.log.Exception(9999, "BLE Connect Exception", e);
                this.RaiseConnectAttemptResult(BLEOperationStatus.UnknownError);
            }
        }


        private void ConnectBTLEDeviceEvents(BLEOperationStatus status) {
            if (this.currentDevice != null && status == BLEOperationStatus.Success) {
                this.currentDevice.ConnectionStatusChanged += this.CurrentDevice_ConnectionStatusChanged;
                this.currentDevice.GattServicesChanged += this.CurrentDevice_GattServicesChanged;
                this.currentDevice.NameChanged += this.CurrentDevice_NameChanged;
            }
            if (status == BLEOperationStatus.Success) {
                this.binderSet.ReadValueChanged += BinderSet_ReadValueChanged;
            }
        }


        private void BinderSet_ReadValueChanged(object sender, BLE_CharacteristicReadResult e) {
            Task.Run(() => {
                try {
                    this.log.InfoEntry("BinderSet_ReadValueChanged");
                    this.CharacteristicReadValueChanged?.Invoke(sender, e);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "RaiseConnectAttemptResult", "", e);
                }
            });
        }

        private void RaiseConnectAttemptResult(BLEGetInfoStatus status) {
            Task.Run(() => {
                try {
                    this.DeviceConnectResult?.Invoke(this, status);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "RaiseConnectAttemptResult", "", e);
                }
            });
        }


        private void RaiseConnectAttemptResult(BLEOperationStatus status) {
            this.RaiseConnectAttemptResult(new BLEGetInfoStatus(status));
        }


        #region OLD Connection experiments
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

}
