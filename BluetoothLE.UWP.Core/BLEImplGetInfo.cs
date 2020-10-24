using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {


        private async Task HarvestDeviceInfo(BluetoothLEDeviceInfo deviceDataModel) {
            this.log.Info("HarvestDeviceInfo", () => string.Format("Attempting connection to {0}: FromIdAsync({1})",
                deviceDataModel.Name, deviceDataModel.Id));

            BluetoothLEDevice device = null;

            try {
                // https://github.com/microsoft/Windows-universal-samples/blob/master/Samples/BluetoothLE/cs/Scenario2_Client.xaml.cs
                this.log.Info("HarvestDeviceInfo", () => string.Format("--------------------------------------------------------------------"));
                this.log.Info("HarvestDeviceInfo", () => string.Format(" Param Device Info ID {0}", deviceDataModel.Id));
                device = await BluetoothLEDevice.FromIdAsync(deviceDataModel.Id);
                deviceDataModel.InfoAttempted = true;
                this.UpdateDeviceOnConnect(device, deviceDataModel);

                // Clear services and get a new set for the passed in device info
                deviceDataModel.Services.Clear();
                GattDeviceServicesResult services = await device.GetGattServicesAsync();
                if (services.Status == GattCommunicationStatus.Success) {
                    if (services.Services != null) {
                        if (services.Services.Count > 0) {
                            foreach (GattDeviceService service in services.Services) {
                                await this.BuildServiceDataModel(service, deviceDataModel);
                            }
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

                // Raise event
                this.DeviceInfoAssembled?.Invoke(this, deviceDataModel);
            }
            catch (Exception e) {
                this.log.Exception(9999, "On harvest device info", e);
                // TODO - raise event with null device
                this.DeviceInfoAssembled?.Invoke(this, null);
            }
            finally {
                try {
                    if (device != null) {
                        device.Dispose();
                        device = null;
                    }
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "On fail to disconnect harvesting device data", ex);
                }
            }
        }


    }
}
