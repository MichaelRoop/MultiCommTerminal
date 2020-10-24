using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothLE.UWP.Core {

    /// <summary>Windows specific extensions for helper</summary>
    public static class BLE_WinExtensions {

        private static ClassLog log = new ClassLog("BLE_WinExtensions");
        private static IPropertyKeys KEYS = new BLE_WinPropertyKeys();


        /// <summary>Convert from Windows IBuffer to properly sized byte array</summary>
        /// <param name="buffer">The buffer containing data</param>
        /// <returns>Byte array</returns>
        public static byte[] FromBufferToBytes(this IBuffer buffer) {
            uint dataLength = buffer.Length;
            byte[] data = new byte[dataLength];
            using (DataReader reader = DataReader.FromBuffer(buffer)) {
                reader.ReadBytes(data);
            }
            return data;
        }


        // TODO - never seen it so remove from variables. Can still access through Properties
        ///// <summary>Uses the Win BLE DeviceInformation Properties with Connectable key</summary>
        ///// <param name="info">The Win device information object</param>
        ///// <returns>true if connectable, otherwise false</returns>
        //public static bool IsConnectable(this DeviceInformation info) {
        //    if (info.HasProperty(KEYS.IsConnectable)) {
        //        Log.Error(5555, "+++++++ IS CONNECTABLE IS IN PROPERTIES +++++++");
        //
        //        return (bool?)info.Properties[KEYS.IsConnectable] == true;
        //    }
        //    Log.Error(9999, "*** IS CONNECTABLE NOT IN PROPERTIES ***");
        //    return false;
        //}


        /// <summary>Uses the Win BLE DeviceInformation Properties with Connected key</summary>
        /// <param name="info">The Win device information object</param>
        /// <returns>true if connected, otherwise false</returns>
        //public static bool IsConnected(this DeviceInformation info) {
        //    if (info.HasProperty(KEYS.IsConnected)) {
        //        Log.Error(5555, "+++++++ IS CONNECTED IS IN PROPERTIES +++++++");
        //        return (bool?)info.Properties[KEYS.IsConnected] == true;
        //    }
        //    Log.Error(9999, "*** IS CONNECTED NOT IN PROPERTIES ***");
        //    return false;
        //}


        private static bool HasProperty(this DeviceInformation info, string key) {
            if (info.Properties != null) {
                return info.Properties.ContainsKey(key);
            }
            return false;
        }


        public static Dictionary<string, BLE_PropertyDataModel> CreatePropertiesDictionary(this DeviceInformation info) {
            if (info != null) {
                return CreatePropertiesDictionary(info.Properties);
            }
            return new Dictionary<string, BLE_PropertyDataModel>();
        }


        public static BLE_PropertiesUpdateDataModel CreatePropertiesUpdateData(this DeviceInformationUpdate updateInfo) {
            BLE_PropertiesUpdateDataModel dm = new BLE_PropertiesUpdateDataModel() {
                Id = updateInfo.Id,
                ServiceProperties = CreatePropertiesDictionary(updateInfo.Properties),
            };
            return dm;
        }

        private static Dictionary<string, BLE_PropertyDataModel> CreatePropertiesDictionary(IReadOnlyDictionary<string,object> propertyDict) {
            Dictionary<string, BLE_PropertyDataModel> properties = new Dictionary<string, BLE_PropertyDataModel>();
            if (propertyDict != null) {
                foreach (var p in propertyDict) {
                    BLE_PropertyDataModel model = new BLE_PropertyDataModel() {
                        Key = p.Key,
                        Target = GetPropertyTarget(p.Key),
                    };
                    SetPropertyValue(p.Value, model);
                    if (!properties.ContainsKey(model.Key)) {
                        properties.Add(model.Key, model);
                    }
                    else {
                        log.Error(9999, "CreatePropertiesDictionary", () => string.Format("Duplicate property key '{0}'", model.Key));
                    }
                }

            }
            return properties;
        }



        private static PropertyType GetPropertyTarget(string key) {
            if (key == KEYS.IsConnected) {
                return PropertyType.IsConnected;
            }
            else if (key == KEYS.IsConnected) {
                return PropertyType.IsConnected;
            }
            else if (key == KEYS.IsConnectable) {
                return PropertyType.IsConnectable;
            }
            else if (key == KEYS.CanPair) {
                return PropertyType.CanPair;
            }
            else if (key == KEYS.IsPaired) {
                return PropertyType.IsPaired;
            }
            else if (key == KEYS.ContainerId) {
                return PropertyType.ContainerId;
            }
            else if (key == KEYS.IconPath) {
                return PropertyType.IconPath;
            }
            else if (key == KEYS.GlyphIconPath) {
                return PropertyType.GlyphIconPath;
            }
            else if (key == KEYS.ItemNameDisplay) {
                return PropertyType.ItemNameDisplay;
            }
            else {
                return PropertyType.UnHandled;
            }
        }


        private static void SetPropertyValue(object value, BLE_PropertyDataModel model) {
            if (value == null) {
                model.DataType = PropertyDataType.TypeString;
                model.Value = "";
            }
            else {
                model.Value = value;
                if (value is bool) {
                    model.DataType = PropertyDataType.TypeBool;
                }
                else if (value is string) {
                    model.DataType = PropertyDataType.TypeString;
                }
                else if (value is Guid) {
                    model.DataType = PropertyDataType.TypeGuid;
                }
                else {
                    model.DataType = PropertyDataType.TypeUnknown;
                }
            }
        }

    }
}
