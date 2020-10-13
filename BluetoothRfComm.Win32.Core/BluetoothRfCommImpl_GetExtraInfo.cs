using BluetoothCommon.Net;
using BluetoothCommon.Net.Enumerations;
using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Devices.Radios;

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
                                deviceInfo.ServiceType = serviceType;
                                deviceInfo.RemoteHostName = service.ConnectionHostName.ToString();
                                deviceInfo.RemoteServiceName = service.ConnectionServiceName;
                                deviceInfo.ServiceClassName = serviceType.ToString().CamelCaseToSpaces();
                                deviceInfo.ServiceClassInt = (int)service.ServiceId.AsShortId();

                                // TODO info on access 
                                //service.DeviceAccessInformation.CurrentStatus == DeviceAccessStatus.Allowed

                                this.log.Info("****", () => string.Format("Device:{0} Host Name:{1} Service:{2}",
                                    deviceInfo.Name, deviceInfo.RemoteHostName, deviceInfo.RemoteServiceName));



                                //deviceInfo.Strength = 0;
                                this.ListDeviceInfoProperties(device);

                                this.log.Info("****", () => string.Format("device.BluetoothAddress: {0}", device.BluetoothAddress));
                                this.log.Info("****", () => string.Format("device.BluetoothDeviceId.id: {0}", device.BluetoothDeviceId.Id));
                                this.log.Info("****", () => string.Format("device.DeviceId: {0}", device.DeviceId));
                                this.log.Info("****", () => string.Format("device.DeviceInformation.Id: {0}", device.DeviceInformation.Id));
                                this.log.Info("****", () => string.Format("device.ClassOfDevice: {0}", device.ClassOfDevice.RawValue));
                                //this.log.Info("****", () => string.Format(":{0}", ));
                                //this.log.Info("****", () => string.Format(":{0}", ));

                                // Experimental. See the note on the method
                                //await this.GetRadioInfo(device, deviceInfo, service.ConnectionServiceName);
                                //await this.GetRadioInfo(device, deviceInfo, deviceInfo.RemoteHostName);

                                // List of radios available on current device
                                //await this.ListRadios();


                                this.BT_DeviceInfoGathered?.Invoke(this, deviceInfo);
                            }
                            else {
                                //Not used. 
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
            this.ListProperties(device.DeviceInformation.Properties);
        }


        private void ListProperties(IReadOnlyDictionary<string,object> dict) {
            this.log.Info("GetDeviceInfo", () => string.Format("Properties count {0}", dict.Count));
            foreach (var p in dict) {
                string key = p.Key == null ? "" : p.Key;
                string value = p.Value == null ? "NA" : p.Value.ToString();
                this.log.Info("GetDeviceInfo", () => string.Format("{0} - {1}", key, value));
            }
        }



        /// <summary>
        /// Mostly experimental at the moment since I cannot find the right ID to get the
        /// correct adapter that holds radio information
        /// </summary>
        /// <param name="device"></param>
        /// <param name="deviceInfo"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        private async Task GetRadioInfo(BluetoothDevice device, BTDeviceInfo deviceInfo, string id = "") {

            // Cannot get adapter to work as I cannot find an ID to access the remote adaptor object

            try {
                // https://docs.microsoft.com/en-us/uwp/api/windows.devices.bluetooth.bluetoothadapter.fromidasync?view=winrt-19041
                // ID consists of registry information
                // 1. MatchingDeviceId
                // 2. MAC address - think this is the RemoteHostName: 20:16:04:07:61:01
                // 3. GUID representing a device class
                // see: https://xspdf.com/questions/39527.shtml
                // DeviceId is Id:Bluetooth#Bluetooth10:08:b1:8a:b0:02-20:16:04:07:61:01
                //this.log.Info("GetExtraInfo", () => string.Format("Default Id:{0}", def.));
                //var dd = device.BluetoothDeviceId;
                ////BluetoothAdapter.GetDefaultAsync();
                //id = dd.Id;
                //id = dd.ToString();


                this.log.Info("****", "----------------- Local BT Device-----------------------------");
                var localAdapter = await BluetoothAdapter.GetDefaultAsync();
                this.log.Info("GetRadioInfo", () => string.Format(" LOCAL ADAPTER ** DeviceId:{0}", localAdapter.DeviceId));
                this.log.Info("GetRadioInfo", () => string.Format(" LOCAL ADAPTER **  Address:{0}", localAdapter.BluetoothAddress));

                this.log.Info("****", "--------------------------------------------------------------");

                // This is local adapter
                // LOCAL ADAPTER** DeviceId:\\?\USB#VID_0CF3&PID_3004#6&32c5d224&0&3#{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}
                //                         "\\\\?\\USB#VID_0CF3&PID_3004#6&32c5d224&0&3#{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}"
                // LOCAL ADAPTER** BluetoothAddress:17629524439042
                // From the Properties object of the DeviceInformation from this object
                //BluetoothRfCommImpl.GetRadioInfo Info  
                // Name: MICHAEL - RED - Front 
                // Default: False 
                // id:\\?\USB#VID_0CF3&PID_3004#6&32c5d224&0&3#{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}
                //------------------ Info--------------------------------------
                //GetDeviceInfo Properties count 8
                //GetDeviceInfo System.ItemNameDisplay - MICHAEL - RED - Front
                //GetDeviceInfo System.Devices.DeviceInstanceId - USB\VID_0CF3 & PID_3004\6 & 32c5d224 & 0 & 3
                //GetDeviceInfo System.Devices.Icon - C:\Windows\System32\DDORes.dll,-2068
                //GetDeviceInfo System.Devices.GlyphIcon - C:\Windows\System32\DDORes.dll,-3001
                //GetDeviceInfo System.Devices.InterfaceEnabled - True
                //GetDeviceInfo System.Devices.IsDefault - False
                //GetDeviceInfo System.Devices.PhysicalDeviceLocation - System.Byte[]
                //GetDeviceInfo System.Devices.ContainerId - 00000000 - 0000 - 0000 - ffff - ffffffffffff
                // ***--------------------------------------------------------------


                // Here is the list from my Arduino test device
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetRadioInfo Info  Name:
                //MikieBtRfCom Default:False id:\\?\BTHENUM#{00001101-0000-1000-8000-00805F9B34FB}_LOCALMFG&001D#8&D98CD69&0&201604076101_C00000000#{86e0d1e0-8089-11d0-9ce4-08003e301f73}
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo Properties count 8
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.ItemNameDisplay - MikieBtRfCom
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.Devices.DeviceInstanceId - BTHENUM\{ 00001101 - 0000 - 1000 - 8000 - 00805f9b34fb}
                //                _LOCALMFG & 001d\8 & d98cd69 & 0 & 201604076101_C00000000
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.Devices.Icon - C:\Windows\System32\DDORes.dll,-2001
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.Devices.GlyphIcon - C:\Windows\System32\DDORes.dll,-3001
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.Devices.InterfaceEnabled - True
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.Devices.IsDefault - False
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.Devices.PhysicalDeviceLocation - NA
                //11:59:48 985    I        0  BluetoothRfCommImpl.GetDeviceInfo System.Devices.ContainerId - c1576083 - 3be3 - 5b67 - b161 - b22c54822171



                var infos = await DeviceInformation.FindAllAsync();
                foreach (var i in infos) {
                    //this.log.Info("****", "\n------------------ Info --------------------------------------");
                    //this.log.Info("GetRadioInfo", () => string.Format(
                    //    "    Info  Name:{0} Default:{1} id:{2}", i.Name, i.IsDefault, i.Id));
                    //this.ListProperties(i.Properties);
                    //this.log.Info("****", "--------------------------------------------------------------");
                    if (i.Name == deviceInfo.Name) {
                        this.log.Info("GetRadioInfo", () => string.Format(
                            "    Info  Name:{0} Default:{1} id:{2}", i.Name, i.IsDefault, i.Id));
                        this.ListProperties(i.Properties);
                        id = i.Id;
                        // The id is 
                        // \\?\BTHENUM#{00001101-0000-1000-8000-00805F9B34FB}_LOCALMFG&001D#8&D98CD69&0&201604076101_C00000000#{86e0d1e0-8089-11d0-9ce4-08003e301f73} 
                        //                    CLASS ID (1101) Serial Port
                        break;
                    }
                }

                // Try to do search on the ID from locl - THIS WORKS SO THE FORMAT IS OK
                //id = localAdapter.DeviceId;
                // \\?\USB#VID_0CF3&PID_3004#6&32c5d224&0&3#{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}
                // \\?\USB  #VID_0CF3&PID_3004  #6&32c5d224&0&3  #{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}
                //            Device id         | MAC            | class id?
                // From the Device manager the MAX is 10:08:b1:8a: b0: 02

                // \\?\USB  #VID_0CF3&PID_3004  #6&32c5d224&0&3  #{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}



                // The arduino string returned is 

                // RemoteHostName =                  (20:16:04:07:61:01)
                // in the ID:       10:08:b1:8a:b0:02-20:16:04:07:61:01
                // Quick hack - cannot find specified file. Same Exception as get radio - does not get the element not found so the format may be correct
                // MAC
                //  10:08:b1:8a:b0:02 -  The system cannot find the file specified. (0x80070002)
                //  20:16:04:07:61:01 -  The system cannot find the file specified. (0x80070002) 
                // (20:16:04:07:61:01) - The system cannot find the file specified. (0x80070002)  // RemoteHostName
                //id = string.Format(@"\\?\BTHENUM#{0}#{1}",
                //    deviceInfo.RemoteHostName, 
                //    "{00001101-0000-1000-8000-00805F9B34FB}");
                //** Id:\\?\BTHENUM#(20:16:04:07:61:01)#{00001101-0000-1000-8000-00805F9B34FB}

                //  Local Id: \\?\USB#VID_0CF3&PID_3004#6&32c5d224&0&3#{92383b0e-f90e-4ac9-8d44-8c2d0d0ebda2}
                // Remote Id: \\?\BTHENUM#{00001101-0000-1000-8000-00805F9B34FB}_LOCALMFG&001D#8&D98CD69&0&201604076101_C00000000#{86e0d1e0-8089-11d0-9ce4-08003e301f73} 
                // Remote Device instance Id: BTHENUM\{00001101-0000-1000-8000-00805f9b34fb}_LOCALMFG&001d\8&d98cd69&0&201604076101_C00000000

                //id = @"\\?\BTHENUM# {00001101-0000-1000-8000-00805F9B34FB}_LOCALMFG&001D# 8&D98CD69&0&201604076101_C00000000# {86e0d1e0-8089-11d0-9ce4-08003e301f73}";
                //id = @"\\?\BTHENUM#8&D98CD69&0&201604076101_C00000000#{00001101-0000-1000-8000-00805F9B34FB}";
                //id = @"\\?\BTHENUM#{00001101-0000-1000-8000-00805F9B34FB}_LOCALMFG&001D# 8&D98CD69&0&201604076101_C00000000# {86e0d1e0-8089-11d0-9ce4-08003e301f73}";


                // From the MS Doc
                // The DeviceId value that identifies the BluetoothAdapter instance. This is a 
                // composite string combining registry information that includes 
                // 1.the MatchingDeviceId, 
                // 2. the MAC address, 
                // 3. and a GUID representing a device class. 
                //This is different than Windows.Devices.Enumeration.DeviceInformation.Id.However, both contain the MAC address of the Bluetooth radio device embedded within the identifier string.


                // From Device Manager - Arduino device info
                /*
                Device BTHENUM\Dev_201604076101\8 & 28fa0f8c & 0 & BluetoothDevice_201604076101 was configured.

                Driver Name: bth.inf
                Class Guid:
                    { e0cbf06c - cd8b - 4647 - bb8a - 263b43f0f974}
                    Driver Date: 06 / 21 / 2006
                    Driver Version: 10.0.19041.488
                    Driver Provider: Microsoft
                    Driver Section:
                                    BthGenericDevice.NT
                    Driver Rank:
                                    0xFF2000
                    Matching Device Id:
                                    BTHENUM\GENERIC_DEVICE
                    Outranked Drivers:
                                    Device Updated: false
                    Parent Device: BTH\MS_BTHBRB\7 & dcffc4e & 21 & 1
                */
                id = string.Format(@"\\?\BTHENUM#{0}#{1}",
                    deviceInfo.RemoteHostName,
                    "{e0cbf06c-cd8b-4647-bb8a-263b43f0f974}");


                id = @"\\?\BTHENUM\Dev_201604076101\8&28fa0f8c&0&BluetoothDevice_201604076101";



                this.log.Info("GetRadioInfo", () => string.Format(" ** Id:{0}", id));
                // This throws an exception: "Element not found"
                var adapter = await BluetoothAdapter.FromIdAsync(id);
                



                if (adapter != null) {
                    this.log.Info("GetExtraInfo", () => string.Format("Adapter"));
                    this.log.Info("GetExtraInfo", () => string.Format("Advertisement Offload {0}", adapter.IsAdvertisementOffloadSupported));
                    this.log.Info("GetExtraInfo", () => string.Format("Central Role {0}", adapter.IsCentralRoleSupported));
                    this.log.Info("GetExtraInfo", () => string.Format("Classic {0}", adapter.IsClassicSupported));
                    this.log.Info("GetExtraInfo", () => string.Format("Extended Advertisement {0}", adapter.IsExtendedAdvertisingSupported));
                    this.log.Info("GetExtraInfo", () => string.Format("Low Energy {0}", adapter.IsLowEnergySupported));
                    this.log.Info("GetExtraInfo", () => string.Format("Peripheral Role {0}", adapter.IsPeripheralRoleSupported));
                    this.log.Info("GetExtraInfo", () => string.Format("Max advertisement length {0}", adapter.MaxAdvertisementDataLength));

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



        private async Task ListRadios() {
            try {
                // This is only local radios?
                var radios = await Radio.GetRadiosAsync();
                foreach (var radio in radios) {
                    this.log.Info("DoDiscovery", () => string.Format("Radio {0} State {1} Kind {2}", radio.Name, radio.State, radio.Kind));
                }
                // Looks like it is only for the local device
            }
            catch (Exception ex) {
                this.log.Exception(8888, "", ex);
            }

        }



        private async Task ListRadio(string deviceId) {
            try {
                //// This is only local radios
                //var radios = await Radio.GetRadiosAsync();
                //foreach (var radio in radios) {
                //    radio.
                //}

                // File Not Found exception
                // Looks like it is only for the local device
                Radio r = await Radio.FromIdAsync(deviceId);
                this.log.Info("DoDiscovery", () => string.Format("Radio {0} State {1} Kind {2}", r.Name, r.State, r.Kind));
            }
            catch (Exception ex) {
                this.log.Exception(8888, "", ex);
            }
        }



    }
}
