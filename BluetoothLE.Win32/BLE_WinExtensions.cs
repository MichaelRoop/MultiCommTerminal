using BluetoothLE.Net.DataModels;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using Windows.Devices.Enumeration;
using Windows.Storage.Streams;

namespace BluetoothLE.Win32 {

    /// <summary>Windows specific extensions for helper</summary>
    public static class BLE_WinExtensions {

        private static ClassLog log = new ClassLog("BLE_WinExtensions");
        private const string IS_CONNECTED_KEY = "System.Devices.Aep.IsConnected";
        private const string IS_CONNECTABLE_KEY = "System.Devices.Aep.IsConnectable";
        private const string CAN_PAIR = "System.Devices.Aep.CanPair";
        private const string IS_PAIRED = "System.Devices.Aep.IsPaired";
        private const string CONTAINER_ID = "System.Devices.Aep.ContainerId";
        private const string ICON_PATH = "System.Devices.Icon";
        private const string GLYPH_ICON_PATH = "System.Devices.GlyphIcon";
        private const string ITEM_NAME_DISPLAY = "System.ItemNameDisplay";



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


        /// <summary>Uses the Win BLE DeviceInformation Properties with Connectable key</summary>
        /// <param name="info">The Win device information object</param>
        /// <returns>true if connectable, otherwise false</returns>
        public static bool IsConnectable(this DeviceInformation info) {
            if (info.HasProperty(IS_CONNECTABLE_KEY)) {
                return (bool?)info.Properties[IS_CONNECTABLE_KEY] == true;
            }
            return false;
        }


        /// <summary>Uses the Win BLE DeviceInformation Properties with Connected key</summary>
        /// <param name="info">The Win device information object</param>
        /// <returns>true if connected, otherwise false</returns>
        public static bool IsConnected(this DeviceInformation info) {
            if (info.HasProperty(IS_CONNECTED_KEY)) {
                return (bool?)info.Properties[IS_CONNECTED_KEY] == true;
            }
            return false;
        }


        private static bool HasProperty(this DeviceInformation info, string key) {
            if (info.Properties != null) {
                return info.Properties.ContainsKey(key);
            }
            return false;
        }


        public static Dictionary<string, BLE_PropertyDataModel> CreatePropertiesDictionary(this DeviceInformation info) {
            //Dictionary<string, BLE_PropertyDataModel> properties = new Dictionary<string, BLE_PropertyDataModel>();
            if (info != null) {
                return CreatePropertiesDictionary(info.Properties);
                //if (info.Properties != null) {
                //    foreach (var p in info.Properties) {
                //        BLE_PropertyDataModel model = new BLE_PropertyDataModel() {
                //            Key = p.Key,
                //            Target = GetPropertyTarget(p.Key),
                //        };
                //        SetPropertyValue(p.Value, model);
                //        if (!properties.ContainsKey(model.Key)) {
                //            properties.Add(model.Key, model);
                //        }
                //        else {
                //            log.Error(9999, "CreatePropertiesDictionary", () => string.Format("Duplicate property key '{0}'", model.Key));
                //        }
                //    }
                //}
            }
            //return properties;
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
            switch (key) {
                case IS_CONNECTED_KEY: return PropertyType.IsConnected;
                case IS_CONNECTABLE_KEY: return PropertyType.IsConnectable;
                case CAN_PAIR: return PropertyType.CanPair;
                case IS_PAIRED: return PropertyType.IsPaired;
                case CONTAINER_ID: return PropertyType.ContainerId;
                case ICON_PATH: return PropertyType.IconPath;
                case GLYPH_ICON_PATH: return PropertyType.GlyphIconPath;
                case ITEM_NAME_DISPLAY: return PropertyType.DisplayName;
                default: 
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
