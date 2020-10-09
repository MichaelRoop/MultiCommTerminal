using BluetoothCommon.Net;
using BluetoothCommon.Net.Enumerations;
using BluetoothCommon.Net.interfaces;
using System;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;

namespace BluetoothRfComm.UWP.Core {

    /// <summary>Extra info portion of RFComm implementation: BluetoothRfCommImpl_GetExtraInfo</summary>
    public partial class BluetoothRfCommUwpCore : IBTInterface {

        /// <summary>Complete by connecting and filling in the device information</summary>
        /// <param name="device"></param>
        public void GetDeviceInfoAsync(BTDeviceInfo deviceDataModel) {
            Task.Run(async () => {
                try {
                    await this.GetExtraInfo(deviceDataModel, true);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                    // TODO - raise error event
                }
            });
        }


        /// <summary>Get extra info for connection and other not gathered at discovery to save time</summary>
        /// <param name="deviceInfo">The device information data model to populate</param>
        /// <param name="forceRetrieve">Force re-reading of all extra information</param>
        /// <returns>An asynchronous task result</returns>
        private async Task GetExtraInfo(BTDeviceInfo deviceInfo, bool forceRetrieve) {
            this.log.InfoEntry("GetExtraInfo");

            // 0 length remote host name indicates that we require more information for connection
            if (deviceInfo.RemoteHostName.Length == 0 || forceRetrieve) {
                this.log.Info("GetExtraInfo", () => string.Format("Getting info to fill in host name for address:{0}", deviceInfo.Address));

                using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(deviceInfo.Address)) {

                    // TODO - defer this to before connection or info request                            
                    // SDP records only after services
                    // Must use uncached
                    RfcommDeviceServicesResult serviceResult = await device.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);

                    this.log.Info("GetExtraInfo", () => string.Format("Success. Number of services:{0}", serviceResult.Services.Count));

                    //WrapErr.ChkTrue(serviceResult.Services.Count > 0, 9999, () => string.Format("No services for BT:{0}", deviceInfo.Name));
                    if (serviceResult.Services.Count == 0) {
                        throw new Exception(string.Format("No services for BT:{0}", deviceInfo.Name));
                    }


                    if (serviceResult.Error == BluetoothError.Success) {
                        foreach (var service in serviceResult.Services) {
                            BT_ServiceType serviceType = BT_ParseHelpers.GetServiceType(service.ServiceId.AsShortId());
                            this.log.Info("GetExtraInfo", () => string.Format("Device {0} Connection host name {1} Service name {2} Type {3}",
                                deviceInfo.Name,
                                service.ConnectionHostName,
                                service.ConnectionServiceName,
                                serviceType.ToString()));
                            if (serviceType == BT_ServiceType.SerialPort) {
                                //await this.GetListSDPAttributes(service);
                                deviceInfo.ServiceType = BT_ServiceType.SerialPort;
                                deviceInfo.RemoteHostName = service.ConnectionHostName.ToString();
                                deviceInfo.RemoteServiceName = service.ConnectionServiceName;
                                deviceInfo.ServiceClassName = serviceType.ToString().CamelCaseToSpaces(); //BTTools.GetServiceClassName(service.ServiceId.AsShortId());
                                deviceInfo.ServiceClassInt = (int)service.ServiceId.AsShortId();

                                // TODO info on access 
                                //service.DeviceAccessInformation.CurrentStatus == DeviceAccessStatus.Allowed
                                this.log.Info("****", () => string.Format("Device:{0} Host Name:{1} Service:{2}",
                                    deviceInfo.Name, deviceInfo.RemoteHostName, deviceInfo.RemoteServiceName));

                                //deviceInfo.Strength = 0;
                                this.ListDeviceInfoProperties(device);

                                //await this.GetRadioInfo(device, deviceInfo, service.ConnectionServiceName);


                                this.BT_DeviceInfoGathered?.Invoke(this, deviceInfo);
                            }
                            else {
                                // Not used. 
                            }
                        }
                    }
                    else {
                        this.log.Error(9999, () => string.Format("Get Service result:{0}", serviceResult.Error.ToString()));
                    }
                }
            }
        }


        private async Task GetListSDPAttributes(RfcommDeviceService service) {
            // Sample output. See: https://www.bluetooth.com/specifications/assigned-numbers/service-discovery/
            //SDP Attribute id | Capacity | Length | Description(?)
            //    256               7           7
            //      0               5           5
            //      6              11          11
            //      4              14          14
            //      1               5           5      (service class ID list
            try {
                // TODO get extra info on attributes - ServiceDiscoveryProtocol
                var sdpAttr = await service.GetSdpRawAttributesAsync(BluetoothCacheMode.Uncached);
                foreach (var attr in sdpAttr) {
                    this.log.Info("HarvestInfo", () => string.Format("             SDP Attribute:{0} (0x{0:X}) Capacity:{1} Length:{2}", 
                        attr.Key, attr.Value.Capacity, attr.Value.Length));
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }



        }


        private void ListDeviceInfoProperties(BluetoothDevice device) {
            this.log.Info("GetDeviceInfo", () => string.Format("DeviceInformation.Properties count {0}", device.DeviceInformation.Properties.Count));
            foreach (var p in device.DeviceInformation.Properties) {
                string key = p.Key == null ? "" : p.Key;
                string value = p.Value == null ? "NA" : p.Value.ToString();
                this.log.Info("GetDeviceInfo", () => string.Format("{0} - {1}", key, value));
            }
        }


        private async Task GetRadioInfo(BluetoothDevice device, BTDeviceInfo deviceInfo, string id = "") {

            // Cannot get this one to work as I cannot find an ID to access the remote adaptor object

            try {
                //device.DeviceInformation.

                //string id = "";
                ////id = deviceInfo.Address;
                //id = device.DeviceId;
                ////id = device.DeviceInformation.Id;
                //this.log.Info("GetDeviceInfo", () => string.Format("Property count {0}", device.DeviceInformation.Properties.Count));
                //foreach (var p in device.DeviceInformation.Properties) {
                //    string key = p.Key == null ? "" : p.Key;
                //    string value = p.Value == null ? "NA" : p.Value.ToString();
                //    this.log.Info("GetDeviceInfo", () => string.Format("{0} - {1}", key, value));
                //    //this.log.Info("GetDeviceInfo", () => string.Format("{0} - {1}", p.Key, p.Value.GetType().ToString()));
                //}


                //this.log.Info("GetExtraInfo", () => string.Format("Id:{0}", id));

                // https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.bluetoothadapter.fromidasync?view=winrt-19041
                // ID consists of registry information
                // 1. MatchingDeviceId
                // 2. MAC address
                // 3. GUID representing a device class
                // see: https://xspdf.com/questions/39527.shtml
                // DeviceId is Id:Bluetooth#Bluetooth10:08:b1:8a:b0:02-20:16:04:07:61:01
                //this.log.Info("GetExtraInfo", () => string.Format("Default Id:{0}", def.));
                //var dd = device.BluetoothDeviceId;
                ////BluetoothAdapter.GetDefaultAsync();
                //id = dd.Id;
                //id = dd.ToString();

                this.log.Info("GetExtraInfo", () => string.Format(" ** Id:{0}", id));


                var adapter = await BluetoothAdapter.FromIdAsync(id);
                if (adapter != null) {
                    Windows.Devices.Radios.Radio radio = await adapter.GetRadioAsync();
                    if (radio != null) {
                        //radio.Kind

                        deviceInfo.Radio.Manufacturer = radio.Name;

                    }

                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }

        }




    }
}
