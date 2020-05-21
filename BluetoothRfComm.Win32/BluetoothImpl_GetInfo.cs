using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Radios;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothRfComm.Win32 {

    public partial class BluetoothRfCommImpl : IBTInterface {

        private async Task HarvestInfo(BTDeviceInfo dataModel) {
            BluetoothDevice device = null;
            try {
                // TODO
                //BTDeviceInfo i = deviceDataModel;
                //i.Authenticated;
                //i.LastSeen;
                //i.LastUsed;
                //i.Radio;
                //i.ServiceClassInt;
                //i.ServiceClassName;
                //i.Strength;
                // TODO add BluetoothAddress ulong

                //BluetoothSignalStrengthFilter ss = new BluetoothSignalStrengthFilter();

                device = await BluetoothDevice.FromIdAsync(dataModel.Address);
                if (device != null) {
                    this.log.Info("HarvestInfo", () => string.Format("        ---------- FROM BluetoothDevice.FromAsync ----------"));

                    //device.ConnectionStatus == BluetoothConnectionStatus.Connected
                    dataModel.Connected = device.ConnectionStatus == BluetoothConnectionStatus.Connected;
                    this.log.Info("HarvestInfo", () => string.Format("        Connected:{0}", dataModel.Connected));
                    this.log.Info("HarvestInfo", () => string.Format("        Device Access Status:{0}", device.DeviceAccessInformation.CurrentStatus.ToString()));
                    this.log.Info("HarvestInfo", () => string.Format("        Device Kind:{0}", device.DeviceInformation.Kind.ToString()));

                    this.log.Info("HarvestInfo", () => string.Format("        Properties count:{0}", device.DeviceInformation.Properties.Count));
                    foreach (var p in device.DeviceInformation.Properties) {
                        if (p.Value == null) {
                            this.log.Info("HarvestInfo", () => string.Format("            Key:{0} Value:NULL", p.Key));
                        }
                        else if (p.Value is string) {
                            this.log.Info("HarvestInfo", () => string.Format("            Key:{0} Value:{1}", p.Key, p.Value as string));
                        }
                        else if (p.Value is Boolean) {
                            this.log.Info("HarvestInfo", () => string.Format("            Key:{0} Value:{1}", p.Key, (bool)p.Value));
                        }
                        else if (p.Value is Guid) {
                            this.log.Info("HarvestInfo", () => string.Format("            Key:{0} Value:{1}", p.Key, ((Guid)p.Value).ToString()));
                        }
                        else {
                            this.log.Info("HarvestInfo", () => string.Format("            Key:{0} Value:{1}", p.Key, p.Value.GetType().Name));
                        }
                    }


                    //device.BluetoothDeviceId
                    //device.BluetoothAddress
                    //device.DeviceId
                    

                    //device.DeviceInformation.cl
                    //device.DeviceAccessInformation.CurrentStatus == Windows.Devices.Enumeration.DeviceAccessStatus.DeniedBySystem
                    //device.ClassOfDevice.ServiceCapabilities == BluetoothServiceCapabilities.
                    BluetoothClassOfDevice cl = device.ClassOfDevice;
                    if (cl != null) {
                        dataModel.DeviceClassInt = (uint)cl.MajorClass;
                        dataModel.DeviceClassName = string.Format("{0}:{1}", cl.MajorClass.ToString(), cl.MinorClass.ToString());

                        this.log.Info("HarvestInfo", () => string.Format("        Major class:{0}", cl.MajorClass.ToString()));
                        this.log.Info("HarvestInfo", () => string.Format("        Minor class:{0}", cl.MinorClass.ToString()));
                    }
                }



                //try {
                //    this.log.Info("HarvestInfo", "        ---------- Get Adapter ----------");
                //    // I can only get info from the default
                //    //BluetoothAdapter adapter = await BluetoothAdapter.GetDefaultAsync();
                //    string id = "";
                //    //id = device.BluetoothAddress.ToString();  // Element not found
                //    // id = dataModel.Address;                  // Element not found
                //    //id = device.BluetoothDeviceId.Id;         // Element not found

                //    BluetoothAdapter adapter = await BluetoothAdapter.FromIdAsync(id);
                //    //BluetoothAdapter adapter = await BluetoothAdapter.FromIdAsync(device.BluetoothDeviceId.Id);


                //    if (adapter != null) {
                //        this.log.Info("HarvestInfo", "Got Adapter");
                //        Radio radio = await adapter.GetRadioAsync();
                //        if (radio != null) {
                //            this.log.Info("HarvestInfo", "Got Radio");
                //            dataModel.Radio.Manufacturer = radio.Name;
                //            this.log.Info("HarvestInfo", () => string.Format("        Radio Name:{0}", radio.Name));
                //            this.log.Info("HarvestInfo", () => string.Format("        Radio Kind:{0}", radio.Kind.ToString()));
                //            this.log.Info("HarvestInfo", () => string.Format("        Radio State:{0}", radio.State.ToString()));
                //        }

                //    }
                //}
                //catch (Exception ex1) {
                //    this.log.Exception(999, "Exception on Adapter From IdAsync", ex1);
                //}


                try {

                    this.log.Info("HarvestInfo", "        ---------- Get RfCommService ----------");

                    //RfcommServiceId serviceId = RfcommServiceId.SerialPort;
                    //serviceId = RfcommServiceId.GenericFileTransfer;
                    //serviceId = RfcommServiceId.ObexFileTransfer;
                    //serviceId = RfcommServiceId.ObexObjectPush;
                    //serviceId = RfcommServiceId.PhoneBookAccessPce;
                    //serviceId = RfcommServiceId.PhoneBookAccessPse;

                    //device.ConnectionStatus.

                    var syncRfc = device.SdpRecords;
                    foreach (var s in syncRfc) {
                        this.log.Info("HarvestInfo", () => string.Format("             Capacity:{0}", s.Capacity));
                        this.log.Info("HarvestInfo", () => string.Format("             Length:{0}", s.Length));
                    }


                    RfcommDeviceServicesResult rfc = await device.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);
                    //RfcommDeviceServicesResult rfc = await device.GetRfcommServicesForIdAsync(RfcommServiceId.SerialPort);

                    this.log.Info("HarvestInfo", () => string.Format("services count:{0}", rfc.Services.Count));
                    foreach (var service in rfc.Services) {
                        this.log.Info("HarvestInfo", () => string.Format(
                            "             Service Name:{0}", service.ConnectionServiceName));
                        this.log.Info("HarvestInfo", () => string.Format(
                            "             Host Name:{0}", service.ConnectionHostName));
                        this.log.Info("HarvestInfo", () => string.Format(
                            "             Access Status:{0}", service.DeviceAccessInformation.CurrentStatus));


                        // This is how you would write to it
                        // https://docs.microsoft.com/en-us/windows/uwp/devices-sensors/send-or-receive-files-with-rfcomm
                        // Check support


                        this.log.Info("HarvestInfo", "        ---------- Connect ----------");

                        StreamSocket socket = new StreamSocket();
                        await socket.ConnectAsync(
                            service.ConnectionHostName,
                            service.ConnectionServiceName,
                            SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                        using (var writer = new DataWriter(socket.OutputStream)) {
                            writer.WriteString("Just a long meandering string to see if it gets through");
                            await socket.OutputStream.WriteAsync(writer.DetachBuffer());
                        }

                        //IBuffer
                        Windows.Storage.Streams.DataWriter w = new DataWriter();
                        //w.WriteString("This is a very long string to see if anything goes through");


                       //BufferedStream bs = new BufferedStream(w);
                       //await socket.OutputStream.WriteAsync(bs);


                    socket.Dispose();
                    }

                }
                catch (Exception ex2) {
                    this.log.Exception(9999, "Exception on RFComm services", ex2);
                }



            }
            finally {
                if (device != null) {
                    device.Dispose();
                }
            }
        }


    }
}
