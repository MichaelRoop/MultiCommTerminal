using BluetoothCommon.Net;
using BluetoothCommon.Net.Enumerations;
using ChkUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;

namespace BluetoothRfComm.Win32 {

    public partial class BluetoothClassicUwpWrapper : IDisposable {

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
                            BT_ServiceType serviceType = BT_ParseHelpers.GetServiceType(service.ConnectionServiceName);
                            this.log.Info("GetExtraInfo", () => string.Format("Device {0} Connection host name {1} Service name {2} Type {3}", 
                                deviceInfo.Name, 
                                service.ConnectionHostName, 
                                service.ConnectionServiceName,
                                serviceType.ToString()));
                            if (serviceType == BT_ServiceType.SerialPort) {
                                // TODO get extra info on attributes
                                //var sdpAttr = await service.GetSdpRawAttributesAsync(BluetoothCacheMode.Uncached);
                                //foreach (var attr in sdpAttr) {
                                //    this.log.Info("HarvestInfo", () => string.Format("             SDP Attribute:{0} Capacity:{1} Length:{2}", attr.Key, attr.Value.Capacity, attr.Value.Length));
                                //}
                                // Sample output. See: https://www.bluetooth.com/specifications/assigned-numbers/service-discovery/
                                //SDP Attribute id | Capacity | Length | Description(?)
                                //    256               7           7
                                //      0               5           5
                                //      6              11          11
                                //      4              14          14
                                //      1               5           5      (service class ID list
                                deviceInfo.ServiceType = BT_ServiceType.SerialPort;
                                deviceInfo.RemoteHostName = service.ConnectionHostName.ToString();
                                deviceInfo.RemoteServiceName = service.ConnectionServiceName;
                                // TODO info on access 
                                //service.DeviceAccessInformation.CurrentStatus == DeviceAccessStatus.Allowed
                                this.log.Info("****", () => string.Format("Device:{0} Host Name:{1} Service:{2}",
                                    deviceInfo.Name, deviceInfo.RemoteHostName, deviceInfo.RemoteServiceName));
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


    }

}
