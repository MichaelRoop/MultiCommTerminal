using BluetoothLE.Net.interfaces;
using ChkUtils.Net;
using LogUtils.Net;
using System;
using System.Collections.Generic;

namespace BluetoothLE.Net.DataModels {


    public class StringProperyUpdate {
        public string Key { get; set; } = "";
        public string Value { get; set; } = "";
        public StringProperyUpdate(string key, string value) {
            this.Key = key;
            this.Value = value;
        }
    }

    public class BoolProperyUpdate {
        public string Key { get; set; } = "";
        public bool Value { get; set; } = false;
        public BoolProperyUpdate(string key, bool value) {
            this.Key = key;
            this.Value = value;
        }
    }


    public class GuidProperyUpdate {
        public string Key { get; set; } = "";
        public Guid Value { get; set; } = Guid.NewGuid();
        public GuidProperyUpdate(string key, Guid value) {
            this.Key = key;
            this.Value = value;
        }
    }



    /// <summary>Information particular to Bluetooth LE devices</summary>
    public class BluetoothLEDeviceInfo {

        private ClassLog log = new ClassLog("BluetoothLEDeviceInfo");
        private IPropertyKeys propertyKeys = null;

        public event EventHandler<StringProperyUpdate> OnStringPropertyChanged;
        public event EventHandler<BoolProperyUpdate> OnBoolPropertyChanged;
        public event EventHandler<GuidProperyUpdate> OnGuidPropertyChanged;


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


        public Dictionary<string, BLE_PropertyDataModel> ServiceProperties { get; set; } = new Dictionary<string, BLE_PropertyDataModel>();

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


        public BluetoothLEDeviceInfo(IPropertyKeys propertyKeys) {
            this.propertyKeys = propertyKeys;
        }


        public void Update(Dictionary<string, BLE_PropertyDataModel> properties) {
            // Need some kind of event that something has changed
            foreach (var property in properties) {
                if (this.ServiceProperties.ContainsKey(property.Key)) {
                    WrapErr.ToErrReport(9999, 
                        () => string.Format("Failed on update of '{0}'", property.Key), 
                        () => {
                            this.ServiceProperties[property.Key].Value = property.Value.Value;
                            this.ChangeValueOnUpdate(this.ServiceProperties[property.Key]);
                            this.RaiseChangedEvent(this.ServiceProperties[property.Key]);
                        });
                }
                else {
                    this.log.Error(9999, "Update", () => string.Format("Property key '{0}' does not exist", property.Key));
                }
            }
        }


        /// <summary>Change class values based on updated property</summary>
        /// <param name="dm">The property data model</param>
        private void ChangeValueOnUpdate(BLE_PropertyDataModel dm) {
            if (dm.Key == this.propertyKeys.ItemNameDisplay) {
                this.Name = dm.Value as string;
            }
            else if (dm.Key == this.propertyKeys.CanPair) {
                this.CanPair = (bool)dm.Value;
            }
            else if (dm.Key == this.propertyKeys.IsPaired) {
                this.IsPaired = (bool)dm.Value;
            }
            else if (dm.Key == this.propertyKeys.IsConnected) {
                this.IsConnected = (bool)dm.Value;
            }
            else if (dm.Key == this.propertyKeys.IsConnectable) {
                this.IsConnectable = (bool)dm.Value;
            }

            // These are in the properties only 
            //this.propertyKeys.ContainerId
            //this.propertyKeys.IconPath
            //this.propertyKeys.GlyphIconPath

        }

        private void RaiseChangedEvent(BLE_PropertyDataModel dm) {
            this.log.Info("++++++++++++", 
                () => string.Format("Updating {0} : {1} : {2}",
                dm.Key, dm.DataType, dm.Value.ToString()));

            switch (dm.DataType) {
                case PropertyDataType.TypeBool:
                    this.OnBoolPropertyChanged?.Invoke(this, new BoolProperyUpdate(dm.Key, (bool)dm.Value));
                    break;
                case PropertyDataType.TypeString:
                    this.OnStringPropertyChanged?.Invoke(this, new StringProperyUpdate(dm.Key, dm.Value as string));
                    break;
                case PropertyDataType.TypeGuid:
                    this.OnGuidPropertyChanged?.Invoke(this, new GuidProperyUpdate(dm.Key, (Guid)dm.Value));
                    break;
                case PropertyDataType.TypeUnknown:
                    this.OnStringPropertyChanged?.Invoke(this, new StringProperyUpdate(dm.Key, dm.Value.ToString()));
                    break;
            }
        }



    }
}
