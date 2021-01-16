using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {
    
        /// <summary>Update any previous information from discovery or other connections</summary>
        /// <param name="device">The BLE device returned</param>
        /// <param name="deviceDataModel">The portable data model to receive values</param>
        public void UpdateDeviceInfo(BluetoothLEDevice device, BluetoothLEDeviceInfo deviceDataModel) {
            if (device.DeviceInformation != null) {
                DeviceInformation di = device.DeviceInformation;
                deviceDataModel.IsDefault = di.IsDefault;
                deviceDataModel.IsEnabled = di.IsEnabled;
                deviceDataModel.DeviceKind = (BLE_DeviceInfoKind)di.Kind; 
                if (di.Pairing != null) {
                    deviceDataModel.CanPair = di.Pairing.CanPair;
                    deviceDataModel.IsPaired = di.Pairing.IsPaired;
                    deviceDataModel.ProtectionLevel = (BLE_ProtectionLevel)di.Pairing.ProtectionLevel;
                }
                if (di.EnclosureLocation != null) {
                    deviceDataModel.EnclosureLocation.InDock = di.EnclosureLocation.InDock;
                    deviceDataModel.EnclosureLocation.InLid = di.EnclosureLocation.InLid;
                    deviceDataModel.EnclosureLocation.Location = (BLE_DevicePanelLocation)di.EnclosureLocation.Panel;
                    deviceDataModel.EnclosureLocation.ClockWiseRotationInDegrees = di.EnclosureLocation.RotationAngleInDegreesClockwise;
                }
            }

            if (device.BluetoothDeviceId != null) {
                if (device.BluetoothDeviceId.IsClassicDevice) {
                    deviceDataModel.TypeBluetooth = BluetoothType.Classic;
                }
                else if (device.BluetoothDeviceId.IsLowEnergyDevice) {
                    deviceDataModel.TypeBluetooth = BluetoothType.LowEnergy;
                }
            }

            if (device.DeviceAccessInformation != null) {
                deviceDataModel.AccessStatus = (BLE_DeviceAccessStatus)device.DeviceAccessInformation.CurrentStatus; 
            }

            deviceDataModel.WasPairedUsingSecureConnection = device.WasSecureConnectionUsedForPairing;
            deviceDataModel.AddressType = (BLE_AddressType)device.BluetoothAddressType;
            deviceDataModel.IsConnected = device.ConnectionStatus == BluetoothConnectionStatus.Connected;
            deviceDataModel.AddressAsULong = device.BluetoothAddress;

            // TODO 
            //device.Appearance

        }



    }

}
