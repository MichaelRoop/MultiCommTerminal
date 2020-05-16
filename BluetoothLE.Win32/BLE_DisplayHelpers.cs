using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.Parsers;
using System.Collections.Generic;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothLE.Win32 {

    /// <summary>
    /// From MS examples. Created OS independent copy and kept this as wrapper
    /// </summary>
    public static class BLE_DisplayHelpers {

        public static string GetServiceName(GattDeviceService service) {
            return BLE_ParseHelpers.GetServiceName(service.Uuid);
        }


        public static string GetCharacteristicName(GattCharacteristic characteristic) {
            return BLE_ParseHelpers.GetCharacteristicName(characteristic.Uuid, characteristic.UserDescription);
        }


        public static GattNativeCharacteristicUuid GetCharacteristicEnum(GattCharacteristic characteristic) {
            return BLE_ParseHelpers.GetCharacteristicEnum(characteristic.Uuid, characteristic.UserDescription);
        }



        public static string GetDescriptorName(GattDescriptor descriptor) {
            return BLE_ParseHelpers.GetDescriptorName(descriptor.Uuid);
        }


        public static List<CharacteristicProperties> BuildFlagList( this GattCharacteristic ch) {
            List<CharacteristicProperties> flags = new List<CharacteristicProperties>();
            flags.Add(CharacteristicProperties.None);
            if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) {
                flags.Add(CharacteristicProperties.Write);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Broadcast)) {
                flags.Add(CharacteristicProperties.Broadcast);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.ExtendedProperties)) {
                flags.Add(CharacteristicProperties.ExtendedProperties);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Indicate)) {
                flags.Add(CharacteristicProperties.Indicate);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Notify)) {
                flags.Add(CharacteristicProperties.Notify);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Read)) {
                flags.Add(CharacteristicProperties.Read);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.ReliableWrites)) {
                flags.Add(CharacteristicProperties.ReliableWrites);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WritableAuxiliaries)) {
                flags.Add(CharacteristicProperties.WritableAuxiliaries);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.Write)) {
                flags.Add(CharacteristicProperties.Write);
            }
            else if (ch.CharacteristicProperties.HasFlag(GattCharacteristicProperties.WriteWithoutResponse)) {
                flags.Add(CharacteristicProperties.WriteWithoutResponse);
            }
            return flags;
        }

    }


    // TODO - review and compare to my cross platform info
    /// <summary>
    ///     Display class used to represent a BluetoothLEDevice in the Device list
    /// </summary>
    //public class BluetoothLEDeviceDisplay : INotifyPropertyChanged {
    //    public BluetoothLEDeviceDisplay(DeviceInformation deviceInfoIn) {
    //        DeviceInformation = deviceInfoIn;
    //        UpdateGlyphBitmapImage();
    //    }

    //    public DeviceInformation DeviceInformation { get; private set; }

    //    public string Id => DeviceInformation.Id;
    //    public string Name => DeviceInformation.Name;
    //    public bool IsPaired => DeviceInformation.Pairing.IsPaired;
    //    public bool IsConnected => (bool?)DeviceInformation.Properties["System.Devices.Aep.IsConnected"] == true;
    //    public bool IsConnectable => (bool?)DeviceInformation.Properties["System.Devices.Aep.Bluetooth.Le.IsConnectable"] == true;

    //    public IReadOnlyDictionary<string, object> Properties => DeviceInformation.Properties;

    //    public BitmapImage GlyphBitmapImage { get; private set; }

    //    public event PropertyChangedEventHandler PropertyChanged;

    //    public void Update(DeviceInformationUpdate deviceInfoUpdate) {
    //        DeviceInformation.Update(deviceInfoUpdate);

    //        OnPropertyChanged("Id");
    //        OnPropertyChanged("Name");
    //        OnPropertyChanged("DeviceInformation");
    //        OnPropertyChanged("IsPaired");
    //        OnPropertyChanged("IsConnected");
    //        OnPropertyChanged("Properties");
    //        OnPropertyChanged("IsConnectable");

    //        UpdateGlyphBitmapImage();
    //    }

    //    private async void UpdateGlyphBitmapImage() {
    //        DeviceThumbnail deviceThumbnail = await DeviceInformation.GetGlyphThumbnailAsync();
    //        var glyphBitmapImage = new BitmapImage();
    //        await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
    //        GlyphBitmapImage = glyphBitmapImage;
    //        OnPropertyChanged("GlyphBitmapImage");
    //    }

    //    protected void OnPropertyChanged(string name) {
    //        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
    //    }
    //}



}
