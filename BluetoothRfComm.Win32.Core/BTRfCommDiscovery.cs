////#define USING_RFCOMM
//#define USING_BLUETOOTH_CLASSIC
//using BluetoothCommon.Net.interfaces;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//#if USING_RFCOMM
//using Windows.Devices.Bluetooth.Rfcomm;
//#endif
//using Windows.Devices.Enumeration;
////using Windows.Storage.Streams;
////using Windows.Networking.Sockets;
//using BluetoothCommon.Net;
//using Windows.Devices.Bluetooth;

//namespace BluetoothRfComm.Win32 {


//    public partial class BluetoothRfCommImpl : IBTInterface {

//        async void DoDiscovery() {

//#if USING_BLUETOOTH_CLASSIC

//            // This is actually using the Bluetooth Classic API instead of RFComm
//            this.log.InfoEntry("DoDiscovery");
//            try {

//                //BluetoothSerial


//                string selector = BluetoothDevice.GetDeviceSelectorFromPairingState(true);
//                var devices = await DeviceInformation.FindAllAsync(selector);
//                foreach (DeviceInformation info in devices) {
//                    // TODO - figure out how to remove old cached BlueRadios BT profiles

//                    if (!info.Name.Contains("BlueRadios")) {
//                        this.log.Info("DoDiscovery", () => string.Format("Found device {0}", info.Name));
//                        if (this.DiscoveredBTDevice != null) {
//                            BTDeviceInfo deviceInfo = new BTDeviceInfo() {
//                                Name = info.Name,
//                                Connected = false,
//                                Address = info.Id,
//                            };

//                            //dev.Pairing.IsPaired;
//                            //dev.Pairing.ProtectionLevel
//                            if (info.Properties != null) {
//                                this.log.Info("DoDiscovery", "Found properties");
//                                this.log.Info("DoDiscovery", () => string.Format(
//                                    "Number of Properties for {0} - {1}", info.Name, info.Properties.Count));
//                                foreach (var v in info.Properties) {
//                                    if (v.Value == null) {
//                                        this.log.Info("DoDiscovery", () => string.Format(
//                                            "    Key:{0} Value:NULL", v.Key));
//                                    }
//                                    else {
//                                        if (v.Value is string) {
//                                            this.log.Info("DoDiscovery", () => string.Format(
//                                                "    Key:{0} String Value:{1}", v.Key, (string)v.Value));
//                                        }
//                                        else if (v.Value is Boolean) {
//                                            this.log.Info("DoDiscovery", () => string.Format(
//                                                "    Key:{0} Boolean Value:{1}", v.Key, (bool)v.Value));
//                                        }
//                                        else {
//                                            this.log.Info("DoDiscovery", () => string.Format(
//                                                "    Key:{0} Type:{1}", v.Key, v.Value.GetType().Name));
//                                        }
//                                    }
//                                }

//                                await this.HarvestInfo(deviceInfo);

//                            }

//                            //// TODO remove after doing experiments
//                            //if (deviceInfo.Name.Contains("HC-05")) {
//                            //    await this.HarvestInfo(deviceInfo);
//                            //}


//                            this.DiscoveredBTDevice(this, deviceInfo);
//                        }
//                        else {
//                            this.log.Info("DoDiscovery", "No subscribers to discovered event");
//                            break;
//                        }
//                    }
//                    this.log.InfoExit("DoDiscovery");
//                    this.DiscoveryComplete?.Invoke(this, true);
//                }

//            }
//            catch (Exception e) {
//                this.log.Exception(9999, "", e);
//                this.DiscoveryComplete?.Invoke(this, false);
//            }
//            this.log.InfoExit("DoDiscovery");
//            this.DiscoveryComplete?.Invoke(this, true);



//#endif

//#if USING_RFCOMM

//        //https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/send-or-receive-files-with-rfcomm
//        //RfcommDeviceService service = null;
//        //StreamSocket socket = null;


//            RfcommServiceId s1 = RfcommServiceId.GenericFileTransfer; // 0
//            RfcommServiceId s2 = RfcommServiceId.ObexFileTransfer; // 0
//            RfcommServiceId s3 = RfcommServiceId.ObexObjectPush; // 0
//            RfcommServiceId s4 = RfcommServiceId.PhoneBookAccessPce; // 0
//            RfcommServiceId s5 = RfcommServiceId.PhoneBookAccessPse;
//            RfcommServiceId s6 = RfcommServiceId.SerialPort; // COM1 and Dev B (Dev B is the Arduino shield)

//            DeviceInformationCollection deviceInfoCollection =
//                await DeviceInformation.FindAllAsync(
//                    RfcommDeviceService.GetDeviceSelector(
//                        s6));


//            this.log.Info("DoDiscovery", () => string.Format("Returned from FindAllAsync - count {0}", 
//                deviceInfoCollection.Count));
//            foreach (DeviceInformation dev in deviceInfoCollection) {
//                try {
//                    this.log.Info("DoDiscovery", () => string.Format("Found device {0}", dev.Name));
//                    if (!dev.Name.StartsWith("COM")) {
//                        if (dev.Name == "Dev B") {
//                            this.log.Info("***********", dev.Id);

//                            //var c = BLuetoothSerial


//                        }


//                        if (this.DiscoveredBTDevice != null) {



//                            BTDeviceInfo deviceInfo = new BTDeviceInfo() {
//                                Name = dev.Name,
//                                Connected = false,
//                                Address = dev.Id,
//                            };
//                            //dev.


//                            //dev.Pairing.IsPaired;
//                            //dev.Pairing.ProtectionLevel
//                            if (dev.Properties != null) {
//                                this.log.Info("DoDiscovery", "Found properties");
//                                this.log.Info("DoDiscovery", () => string.Format(
//                                    "Number of Properties for {0} - {1}", dev.Name, dev.Properties.Count));
//                                foreach (var v in dev.Properties) {
//                                    if (v.Key == "System.ItemNameDisplay") {
//                                        // Type is string
//                                        this.log.Info("DoDiscovery", () => string.Format(
//                                            "Property:{0} Value:{1}", v.Key, (string)v.Value));
//                                    }
//                                    else {
//                                        this.log.Info("DoDiscovery", () => string.Format(
//                                            "Property:{0} Type:{1}", v.Key, v.Value == null ? "NULL" : v.Value.GetType().Name));
//                                    }
//                                }
//                            }
//                            this.DiscoveredBTDevice(this, deviceInfo);

//                            //this.DiscoveredBTDevice(this, new BTDeviceInfo() {
//                            //    //Authenticated = dev.Authenticated,
//                            //    //DeviceClassInt = dev.ClassOfDevice.Value,
//                            //    //DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
//                            //    //ServiceClassInt = (int)dev.ClassOfDevice.Service,
//                            //    //ServiceClassName = dev.ClassOfDevice.Service.ToString(),
//                            //    //Strength = dev.Rssi,
//                            //    //LastSeen = dev.LastSeen,
//                            //    //LastUsed = dev.LastUsed,
//                            //    //Radio = this.BuildRadioDataModel(dev),
//                            //});
//                        }
//                        else {
//                            this.log.Info("DoDiscovery", "No subscribers to discovered event");


//                            //this.DiscoveredBTDevice(this, new BTDeviceInfo() {
//                            //    Name = "BURP",
//                            //});
//                            break;
//                        }
//                        //this.DiscoveryComplete?.Invoke(this, true);

//                    }
//                }
//                catch (Exception e) {
//                    this.log.Exception(9999, "", e);
//                    this.DiscoveryComplete?.Invoke(this, false);
//                }

//            }

//            this.log.InfoExit("DoDiscovery");
//            this.DiscoveryComplete?.Invoke(this, true);


//#endif
//        }


//    }
//}
