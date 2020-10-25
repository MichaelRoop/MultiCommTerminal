using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Enumeration;
using Windows.Devices.SerialCommunication;
using Windows.Devices.Usb;

namespace Serial.UWP.Core {

    public partial class SerialImplUwp {

        //https://docs.microsoft.com/en-us/uwp/api/windows.devices.serialcommunication.serialdevice?view=winrt-19041

        ClassLog log = new ClassLog("SerialImplUwp");

        // Query looked good but only retrieved Apple virtual USB
        // The Guid is differnent than that from the SerialDevice.GetDevice selectors
        private string usbAQS = string.Format("System.Devices.InterfaceClassGuid:=\"{0}\"", "{DEE824EF-729B-4A0E-9C14-B7117D33A817}");

        //private string usbAQS2 = string.Format(
        //    "System.Devices.InterfaceClassGuid:=\"{0}\" {1} {2} {3}",
        //    "{86E0D1E0-8089-11D0-9CE4-08003E301F73}",
        //    "AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True",
        //    "AND System.DeviceInterface.Serial.UsbVendorId:=0",
        //    "AND System.DeviceInterface.Serial.UsbProductId:=0");


        // Yay! this one works
        // Much faster (20x) than getting all the devices and parsing the ID for USB
        // 22ms rather than 1353ms
        // Even worst if a USB device is not connected. 
        //   They still show up on the list but still attempt 
        //   to connect and time out on connect. Takes over 5000ms for 
        //   each not connected
        // On the other hand, if not connected, it completes in 1ms with this querry
        private string usbAQS2 = string.Format(
            "System.Devices.InterfaceClassGuid:=\"{0}\" {1} {2} {3}",
            "{86E0D1E0-8089-11D0-9CE4-08003E301F73}",
            "AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True",
            "AND System.DeviceInterface.Serial.UsbVendorId:>0",
            "AND System.DeviceInterface.Serial.UsbProductId:>0");


        //GetDeviceSelectorFromUsbVidPid:  System.Devices.InterfaceClassGuid:="{86E0D1E0-8089-11D0-9CE4-08003E301F73}" AND System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True AND System.DeviceInterface.Serial.UsbVendorId:=10 AND System.DeviceInterface.Serial.UsbProductId:=10

        public void DiscoverAsync() {
            try {
                Task.Run(this.DoDiscovery);
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        private async void DoDiscovery() {
            try {
                //DeviceInformationCollection infos = await DeviceInformation.FindAllAsync(SerialDevice.GetDeviceSelector());

                //DeviceInformationCollection infos =
                //    await DeviceInformation.FindAllAsync(
                //        SerialDevice.GetDeviceSelector());




                string s = string.Empty;
                s =  SerialDevice.GetDeviceSelector();
                this.log.Info("", () => string.Format("            GetDeviceSelector:  {0}", s));
                s = SerialDevice.GetDeviceSelectorFromUsbVidPid(10, 10);
                this.log.Info("", () => string.Format("GetDeviceSelectorFromUsbVidPid:  {0}", s));
                s = this.usbAQS;
                this.log.Info("", () => string.Format(" Custom USB AQS{0}", s));
                s = UsbDevice.GetDeviceSelector(9025, 67); // Arduino
                this.log.Info("", () => string.Format("     UsbDevice.GetDeviceSelector  {0}", s));

                s = usbAQS2;
                this.log.Info("", () => string.Format("     GetDeviceSelectorFromUsbVidPid with ids != 0  \n{0}", s));



                //this.log.Info("", () => string.Format("{0}", s));
                //this.log.Info("", () => string.Format("{0}", s));
                //this.log.Info("", () => string.Format("{0}", s));


                DeviceInformationCollection infos = await DeviceInformation.FindAllAsync(s);



                //DeviceInformationCollection infos = 
                //    await DeviceInformation.FindAllAsync(
                //        UsbDevice.GetDeviceClassSelector(new UsbDeviceClass()));

                // Fail
                // ActiveSync
                // CdcControl
                // Irda
                // Measurement
                // Physical 
                // VendorSpecific
                //DeviceInformationCollection infos =
                //    await DeviceInformation.FindAllAsync(
                //        UsbDevice.GetDeviceClassSelector(UsbDeviceClasses.Measurement));


                this.log.Info("**", () => string.Format("AQS = '{0}'", this.usbAQS));


                Stopwatch sw = new Stopwatch();
                sw.Start();

                // Arduino, then ATMEL
                // \\?\USB#VID_2341&PID_0043#75736303436351017171#{86e0d1e0-8089-11d0-9ce4-08003e301f73}
                // \\?\USB#VID_03EB&PID_2145&MI_01#7&111c2a96&1&0001#{86e0d1e0-8089-11d0-9ce4-08003e301f73}

                int found = 0;
                foreach (DeviceInformation info in infos) {
                    // Strange but cannot find an AQS string to narrow to only and any USB
                    if (true /*info.Id.Contains("USB") && info.Id.Contains("VID_") && info.Id.Contains("PID")*/) {
                        found++;
                        this.log.Info("DoDiscovery", "--------------------------------------------------------------");
                        this.log.Info("DoDiscovery", () => string.Format("{0}", info.Name));
                        this.log.Info("DoDiscovery", "--------------------------------------------------------------");
                        this.log.Info("DoDiscovery", () => string.Format("        id:{0}", info.Id));
                        this.log.Info("DoDiscovery", () => string.Format("      Kind:{0}", info.Kind.ToString())); // same as BLE
                        this.log.Info("DoDiscovery", () => string.Format("   Default:{0}", info.IsDefault));
                        this.log.Info("DoDiscovery", () => string.Format("   Enabled:{0}", info.IsEnabled));
                        this.log.Info("DoDiscovery", () => string.Format("   {0}", "Properties -------------"));
                        foreach (var p in info.Properties) {
                            string key = p.Key == null ? "''" : p.Key;
                            string value = p.Value == null ? "''" : p.Value.ToString();
                            this.log.Info("DoDiscovery", () => string.Format("       {0} : {1}", key, value));
                        }

                        this.log.Info("DoDiscovery", () => string.Format("{0}", "   Serial Device -------------------"));
                        // Extra. Connect to get more info. Temp
                        try {
                            SerialDevice dev = await SerialDevice.FromIdAsync(info.Id);
                            this.log.Info("DoDiscovery", () => string.Format("                   Port Name:{0}", dev.PortName));
                            this.log.Info("DoDiscovery", () => string.Format("                   Baud Rate:{0}", dev.BaudRate));
                            this.log.Info("DoDiscovery", () => string.Format("          Break Signal State:{0}", dev.BreakSignalState));
                            this.log.Info("DoDiscovery", () => string.Format("               Byte Received:{0}", dev.BytesReceived));
                            this.log.Info("DoDiscovery", () => string.Format("              Carrier Detect:{0}", dev.CarrierDetectState));
                            this.log.Info("DoDiscovery", () => string.Format("               Clear to Send:{0}", dev.ClearToSendState));
                            this.log.Info("DoDiscovery", () => string.Format("                   Data Bits:{0}", dev.DataBits));
                            this.log.Info("DoDiscovery", () => string.Format("             D ata Set Ready:{0}", dev.DataSetReadyState));
                            this.log.Info("DoDiscovery", () => string.Format("                   Handshake:{0}", dev.Handshake.ToString()));
                            this.log.Info("DoDiscovery", () => string.Format("         Data Terminal Ready:{0}", dev.IsDataTerminalReadyEnabled));
                            this.log.Info("DoDiscovery", () => string.Format("        Request Send Enabled:{0}", dev.IsRequestToSendEnabled));
                            this.log.Info("DoDiscovery", () => string.Format("                      Parity:{0}", dev.Parity.ToString()));
                            this.log.Info("DoDiscovery", () => string.Format("             Read Timeout ms:{0}", dev.ReadTimeout.TotalMilliseconds));
                            this.log.Info("DoDiscovery", () => string.Format("                   Stop Bits:{0}", dev.StopBits));
                            this.log.Info("DoDiscovery", () => string.Format("              USB Product Id:0x{0:X}", dev.UsbProductId));
                            this.log.Info("DoDiscovery", () => string.Format("               USB Vendor Id:0x{0:X}", dev.UsbVendorId));
                            this.log.Info("DoDiscovery", () => string.Format("               Write Timeout:{0}", dev.WriteTimeout.TotalMilliseconds));

                            //if (dev.UsbVendorId != 0) {
                            //   // UsbDevice u = UsbDevice.FromIdAsync(dev.id)
                            //}
                            //UsbDevice.

                            //UsbDevice device = await UsbDevice.FromIdAsync();


                            

                            //int i = 0;

                            this.log.Info("", () => string.Format("AQS = '{0}'", UsbDevice.GetDeviceSelector(dev.UsbVendorId, dev.UsbProductId)));
                            // AQS = 'System.Devices.InterfaceClassGuid:="{DEE824EF-729B-4A0E-9C14-B7117D33A817}" AND 
                            //        System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True AND 
                            //        System.DeviceInterface.WinUsb.UsbVendorId:=9025 AND 
                            //        System.DeviceInterface.WinUsb.UsbProductId:=67'
                            // 
                            // AQS = 'System.Devices.InterfaceClassGuid:="{DEE824EF-729B-4A0E-9C14-B7117D33A817}" AND 
                            //        System.Devices.InterfaceEnabled:=System.StructuredQueryType.Boolean#True AND 
                            //        System.DeviceInterface.WinUsb.UsbVendorId:=1003 AND 
                            //        System.DeviceInterface.WinUsb.UsbProductId:=8517'



                        }
                        catch (Exception e) {
                            this.log.Exception(2222, "", e);
                        }
                    } // If USB
                } // For each

                sw.Stop();
                this.log.Info("DoDiscovery", () => string.Format("**** Number Devices {0} *****", found));
                this.log.Info("DoDiscovery", () => string.Format("**** Elapsed MS {0} *****", sw.ElapsedMilliseconds));

                //SerialDevice device = await SerialDevice.f
            }
            catch(Exception e) {
                this.log.Exception(1111, "", e);
            }

        }




    }
}
