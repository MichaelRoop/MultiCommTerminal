using System;
using System.Collections.Generic;

namespace BluetoothLE.Net.DataModels {

    /// <summary>Information particular to Bluetooth LE devices</summary>
    public class BluetoothLEDeviceInfo {

        /// <summary>Name</summary>
        public string Name { get; set; }

        /// <summary>Id or Address</summary>
        public string Id { get; set; }

        public bool IsDefault { get; set; } // not in Ms example

        public bool IsEnabled { get; set; } // not in Ms example

        public bool CanPair { get; set; } // not in Ms example

        public bool IsPaired { get; set; }

        public string Kind { get; set; }


        public bool IsConnected { get; set; }
        public bool IsConnectable { get; set; }

        public List<Tuple<string,string>> LEProperties { get; set; }

        #region MS extra stuff - OS specific

        // MS example
        //Windows.Devices.Enumeration.DeviceInformation d = null;
        //public IReadOnlyDictionary<string, object> Properties => DeviceInformation.Properties;
        //public BitmapImage GlyphBitmapImage { get; private set; }

        //public event PropertyChangedEventHandler PropertyChanged;

        //public void Update(DeviceInformationUpdate deviceInfoUpdate) {
        //    DeviceInformation.Update(deviceInfoUpdate);

        //    OnPropertyChanged("Id");
        //    OnPropertyChanged("Name");
        //    OnPropertyChanged("DeviceInformation");
        //    OnPropertyChanged("IsPaired");
        //    OnPropertyChanged("IsConnected");
        //    OnPropertyChanged("Properties");
        //    OnPropertyChanged("IsConnectable");

        //    UpdateGlyphBitmapImage();
        //}

        //private async void UpdateGlyphBitmapImage() {
        //    DeviceThumbnail deviceThumbnail = await DeviceInformation.GetGlyphThumbnailAsync();
        //    var glyphBitmapImage = new BitmapImage();
        //    await glyphBitmapImage.SetSourceAsync(deviceThumbnail);
        //    GlyphBitmapImage = glyphBitmapImage;
        //    OnPropertyChanged("GlyphBitmapImage");
        //}

        //protected void OnPropertyChanged(string name) {
        //    PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        //}
        #endregion

        /// <summary>OS 
        /// specific implementations require a specific object 
        /// returned from discovery to make a connection
        /// </summary>
        public object OSSpecificObj { get; set; }  


        public BluetoothLEDeviceInfo() {
            this.LEProperties = new List<Tuple<string,string>>();
        }


    }
}
