using BluetoothCommon.Net;
using BluetoothCommon.Net.Enumerations;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils;
using Windows.Devices.Bluetooth;
using Windows.Devices.Bluetooth.Rfcomm;
using Windows.Devices.Enumeration;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothRfComm.Win32 {
    public class BluetoothClassicUwpWrapper : IDisposable {

        #region Data

        ClassLog log = new ClassLog("BluetoothClassicUwpWrapper");

        private readonly string KEY_CAN_PAIR = "System.Devices.Aep.CanPair";
        private readonly string KEY_IS_PAIRED = "System.Devices.Aep.IsPaired";
        private readonly string KEY_CONTAINER_ID = "System.Devices.Aep.ContainerId";

        private StreamSocket socket = null;
        private DataWriter writer = null;
        private DataReader reader = null;
        private CancellationTokenSource readCancelationToken = null;

        #endregion

        #region Events

        /// <summary>Raised when the discovery is complete. T/F indicates success</summary>
        public event EventHandler<bool> DiscoveryComplete;

        /// <summary>Raised on each device discovered</summary>
        public event EventHandler<BTDeviceInfo> DeviceDiscovered;

        /// <summary>Raised when connection completed good or bad</summary>
        public event EventHandler<bool> ConnectionCompleted;

        /// <summary>Raised when bytes are read</summary>
        public event EventHandler<byte[]> MsgReceivedEvent;


        #endregion

        #region Properties

        public bool Connected { get; private set; } = false;

        public bool InputDataAvailable { get { return this.nextInputIndex > 0; }  }

        #endregion

        #region Constructors


        #endregion

        #region Public

        public void DiscoverPairedDevicesAsync() {
            Task.Run(() => {
                try {
                    this.DoDiscovery(true);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                }
            });
        }


        public void Disconnect() {
            this.ClearAll();
            this.Connected = false;
        }


        public void ConnectAsync(BTDeviceInfo deviceInfo) {
            this.Disconnect();
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    await this.GetExtraInfo(deviceInfo);

                    this.socket = new StreamSocket();
                    await this.socket.ConnectAsync(
                        new HostName(deviceInfo.RemoteHostName),
                        deviceInfo.RemoteServiceName,
                        SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                    this.writer = new DataWriter(this.socket.OutputStream);
                    this.writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    
                    this.reader = new DataReader(this.socket.InputStream);
                    this.reader.InputStreamOptions = InputStreamOptions.Partial;
                    this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    this.reader.ByteOrder = ByteOrder.LittleEndian;

                    this.readCancelationToken = new CancellationTokenSource();
                    this.readCancelationToken.Token.ThrowIfCancellationRequested();

                    //deviceInfo.Connected = true;

                    this.Connected = true;
                    this.DoReadTask();

                    this.ConnectionCompleted?.Invoke(this, true);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                    this.ConnectionCompleted?.Invoke(this, false);
                }
            });
        }


        private static int READ_BUFF_SIZE = 5000;
        private static uint INTERMEDIATE_BUFF_SIZE = 256;
        private int nextInputIndex = 0;
        private byte[] inputBuffer = new byte[READ_BUFF_SIZE];

        private void DoReadTask() {
            Task.Run(async () => {
                this.log.InfoEntry("DoReadTask +++");
                while (true) {
                    try {
                        int count = (int)await this.reader.LoadAsync(INTERMEDIATE_BUFF_SIZE);
                        if (count > 0) {
                            byte[] tmpBuff = new byte[count];
                            this.reader.ReadBytes(tmpBuff);
                            this.MsgReceivedEvent?.Invoke(this, tmpBuff);

                            //lock (this.inputBuffer) {
                            //    Array.Copy(tmpBuff, 0, this.inputBuffer, this.nextInputIndex, count);
                            //    this.nextInputIndex += count;
                            //}
                        }
                    }
                    catch (TaskCanceledException) {
                        this.log.Info("DoReadTask", "Cancelation");
                        break;
                    }
                    catch(Exception e) {
                        this.log.Exception(9999, "", e);
                    }
                }
                this.log.InfoExit("DoReadTask ---");
            });
        }



        public void WriteAsync(byte[] msg) {
            if (this.Connected) {
                if (this.socket != null) {
                    Task.Run(async () => {
                        try {
                            this.log.Info("WriteAsync", () =>
                                string.Format("Sent:{0}", msg.ToFormatedByteString()));

                            this.writer.WriteBytes(msg);
                            await this.socket.OutputStream.WriteAsync(this.writer.DetachBuffer());
                        }
                        catch (Exception e) {
                            this.log.Exception(9999, "", e);
                        }
                    });
                }
                else {
                    this.log.Error(9999, "Socket is null");
                }
            }
            else {
                this.log.Error(9999, "Not Connected");
            }
        }


        public int Read(byte[] buff, int buffMaxSize) {
            int bytesRead = 0;
            if (this.InputDataAvailable) {
                lock (this.inputBuffer) {
                    if (this.nextInputIndex > buffMaxSize) {
                        Array.Copy(this.inputBuffer, buff, buffMaxSize);
                        this.nextInputIndex -= buffMaxSize;
                        Array.Copy(this.inputBuffer, buffMaxSize, this.inputBuffer, 0, this.nextInputIndex);
                        bytesRead = buffMaxSize;
                    }
                    else {
                        Array.Copy(this.inputBuffer, buff, this.nextInputIndex);
                        bytesRead = this.nextInputIndex;
                        this.nextInputIndex = 0;
                    }
                }
            }
            return bytesRead;
        }


        #endregion

        #region IDisposable

        public void Dispose() {
            this.ClearAll();
        }

        #endregion


        #region Private

        private void ClearAll() {
            if (this.writer != null) {
                this.writer.DetachStream();
                this.writer.Dispose();
                this.writer = null;
            }
            if (this.readCancelationToken != null) {
                this.readCancelationToken.Cancel();
                this.readCancelationToken.Dispose();
                this.readCancelationToken = null;
            }
            if (this.reader != null) {
                this.reader.DetachStream();
                this.reader.Dispose();
                this.reader = null;
            }
            if (this.socket != null) {
                this.socket.Dispose();
                this.socket = null;
            }

            this.nextInputIndex = 0;
        }

        private async void DoDiscovery(bool paired) {
            try {
                DeviceInformationCollection devices = await DeviceInformation.FindAllAsync(
                    BluetoothDevice.GetDeviceSelectorFromPairingState(paired));
                foreach (DeviceInformation info in devices) {
                    try {
                        this.log.Info("DoDiscovery", () => string.Format("Found device {0}", info.Name));

                        BTDeviceInfo deviceInfo = new BTDeviceInfo() {
                            Name = info.Name,
                            Connected = false,
                            Address = info.Id,
                        };

                        // TODO - put this off until connect or info request

                        
                        using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(info.Id)) {
                            deviceInfo.Connected = device.ConnectionStatus == BluetoothConnectionStatus.Connected;
                            deviceInfo.CanPair = this.GetBoolProperty(device.DeviceInformation.Properties, KEY_CAN_PAIR, false);
                            deviceInfo.IsPaired = this.GetBoolProperty(device.DeviceInformation.Properties, KEY_IS_PAIRED, false);
                            // Container Id also
                            //device.DeviceAccessInformation.CurrentStatus == DeviceAccessStatus. // Allowed, denied by user, by system, unspecified
                            //device.DeviceInformation.EnclosureLocation.; // Dock, lid, panel, etc
                            //device.DeviceInformation.IsDefault;
                            //device.DeviceInformation.IsEnabled;
                            //device.DeviceInformation.Kind == //AssociationEndpoint, Device, etc
                            //device.DeviceInformation.Properties

                            if (device.ClassOfDevice != null) {
                                deviceInfo.DeviceClassInt = (uint)device.ClassOfDevice.MajorClass;
                                deviceInfo.DeviceClassName = string.Format("{0}:{1}",
                                    device.ClassOfDevice.MajorClass.ToString(),
                                    device.ClassOfDevice.MinorClass.ToString());
                                //device.ClassOfDevice.ServiceCapabilities == BluetoothServiceCapabilities.ObjectTransferService, etc
                            }


                            //// TODO - defer this to before connection or info request                            
                            //// SDP records only after services
                            //// Must use uncached
                            //RfcommDeviceServicesResult serviceResult = await device.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);
                            //if (serviceResult.Error == BluetoothError.Success) {
                            //    foreach (var service in serviceResult.Services) {
                            //        BT_ServiceType serviceType = BT_ParseHelpers.GetServiceType(service.ConnectionServiceName);
                            //        if (serviceType == BT_ServiceType.SerialPort) {
                            //            // TODO get extra info on attributes
                            //            //var sdpAttr = await service.GetSdpRawAttributesAsync(BluetoothCacheMode.Uncached);
                            //            //foreach (var attr in sdpAttr) {
                            //            //    this.log.Info("HarvestInfo", () => string.Format("             SDP Attribute:{0} Capacity:{1} Length:{2}", attr.Key, attr.Value.Capacity, attr.Value.Length));
                            //            //}
                            //            // Sample output. See: https://www.bluetooth.com/specifications/assigned-numbers/service-discovery/
                            //            //SDP Attribute id | Capacity | Length | Description(?)
                            //            //    256               7           7
                            //            //      0               5           5
                            //            //      6              11          11
                            //            //      4              14          14
                            //            //      1               5           5      (service class ID list
                            //            deviceInfo.ServiceType = BT_ServiceType.SerialPort;
                            //            deviceInfo.RemoteHostName = service.ConnectionHostName.ToString();
                            //            deviceInfo.RemoteServiceName = service.ConnectionServiceName;
                            //            // TODO info on access 
                            //            //service.DeviceAccessInformation.CurrentStatus == DeviceAccessStatus.Allowed
                            //            this.log.Info("****", () => string.Format("Device:{0} Host Name:{1} Service:{2}", 
                            //                deviceInfo.Name, deviceInfo.RemoteHostName, deviceInfo.RemoteServiceName));
                            //        }
                            //        else {
                            //            // Not used. 
                            //        }
                            //    }
                            //}
                            //else {
                            //    this.log.Error(9999, () => string.Format("Get Service result:{0}", serviceResult.Error.ToString()));
                            //}
                            

                            // Serial port service name
                            // Bluetooth#Bluetooth10:08:b1:8a:b0:02-20:16:04:07:61:01#RFCOMM:00000000:{00001101-0000-1000-8000-00805f9b34fb}
                            // TODO - determine if all the info in device is disposed by the device.Dispose
                        } // end of using (device)
                        
                        this.DeviceDiscovered?.Invoke(this, deviceInfo);
                    }
                    catch (Exception ex2) {
                        this.log.Exception(9999, "", ex2);
                    }

                }


                this.DiscoveryComplete?.Invoke(this, true);
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                this.DiscoveryComplete?.Invoke(this, false);
            }
        }


        private async Task GetExtraInfo(BTDeviceInfo deviceInfo) {
            this.log.InfoEntry("GetExtraInfo");

            if (deviceInfo.RemoteHostName.Length == 0) {
                using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(deviceInfo.Address)) {
                    // TODO - defer this to before connection or info request                            
                    // SDP records only after services
                    // Must use uncached
                    RfcommDeviceServicesResult serviceResult = await device.GetRfcommServicesAsync(BluetoothCacheMode.Uncached);
                    if (serviceResult.Error == BluetoothError.Success) {
                        foreach (var service in serviceResult.Services) {
                            BT_ServiceType serviceType = BT_ParseHelpers.GetServiceType(service.ConnectionServiceName);
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


        private bool GetBoolProperty(IReadOnlyDictionary<string, object> property, string key, bool defaultValue) {
            if (property.ContainsKey(key)) {
                if (property[key] is Boolean) {
                    return (bool)property[key];
                }
                this.log.Error(9999, () => string.Format(
                    "{0} Property is {1} rather than Boolean", key, property[key].GetType().Name));
            }
            return defaultValue;
        }


        #endregion

    }
}
