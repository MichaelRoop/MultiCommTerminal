using BluetoothCommon.Net;
using BluetoothCommon.Net.Enumerations;
using LogUtils.Net;
using System;
using System.Collections.Generic;
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
        private bool continueReading = false;
        private static uint READ_BUFF_MAX_SIZE = 256;

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

        #endregion

        #region Constructors


        #endregion

        #region Public

        /// <summary>
        /// Launch asynchronous device discovery where DeviceDiscovered is raised on each device
        /// discovered, and DiscoveryComplete when the discovery ends
        /// </summary>
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


        /// <summary>Tear down an existing connection are reset for next</summary>
        public void Disconnect() {
            this.TearDown();
            this.Connected = false;
        }


        /// <summary>Run asynchronous connection where ConnectionCompleted is raised on completion</summary>
        /// <param name="deviceDataModel">The data model with information on the device</param>
        public void ConnectAsync(BTDeviceInfo deviceDataModel) {
            this.Disconnect();
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    await this.GetExtraInfo(deviceDataModel);

                    this.socket = new StreamSocket();
                    await this.socket.ConnectAsync(
                        new HostName(deviceDataModel.RemoteHostName),
                        deviceDataModel.RemoteServiceName,
                        SocketProtectionLevel.BluetoothEncryptionAllowNullAuthentication);

                    this.writer = new DataWriter(this.socket.OutputStream);
                    this.writer.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    
                    this.reader = new DataReader(this.socket.InputStream);
                    this.reader.InputStreamOptions = InputStreamOptions.Partial;
                    this.reader.UnicodeEncoding = Windows.Storage.Streams.UnicodeEncoding.Utf8;
                    this.reader.ByteOrder = ByteOrder.LittleEndian;

                    this.readCancelationToken = new CancellationTokenSource();
                    this.readCancelationToken.Token.ThrowIfCancellationRequested();
                    this.continueReading = true;

                    this.Connected = true;
                    this.LaunchReadTask();

                    this.ConnectionCompleted?.Invoke(this, true);
                }
                catch (Exception e) {
                    this.log.Exception(9999, "", e);
                    this.ConnectionCompleted?.Invoke(this, false);
                }
            });
        }


        /// <summary>Run an asynchronous write</summary>
        /// <param name="msg">The message bytes to write</param>
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

        #endregion

        #region IDisposable

        public void Dispose() {
            this.TearDown();
        }

        #endregion

        #region Private

        /// <summary>Tear down any connections, dispose and reset all resources</summary>
        private void TearDown() {
            if (this.writer != null) {
                this.writer.DetachStream();
                this.writer.Dispose();
                this.writer = null;
            }

            this.continueReading = false;
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
        }


        /// <summary>Discover devices</summary>
        /// <param name="paired">If discovery limited to paired or non paired devices</param>
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


        /// <summary>Get extra info for connection and other not gathered at discovery to save time</summary>
        /// <param name="deviceInfo">The device information data model to populate</param>
        /// <returns>An asynchronous task result</returns>
        /// <returns>An asynchronous task result</returns>
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


        /// <summary>Get the boolean value from a Device Information property</summary>
        /// <param name="property">The device information property</param>
        /// <param name="key">The property key to lookup</param>
        /// <param name="defaultValue">Default value on error</param>
        /// <returns></returns>
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


        /// <summary>The read task</summary>
        private void LaunchReadTask() {
            Task.Run(async () => {
                this.log.InfoEntry("DoReadTask +++");
                while (this.continueReading) {
                    try {
                        int count = (int)await this.reader.LoadAsync(READ_BUFF_MAX_SIZE);
                        if (count > 0) {
                            byte[] tmpBuff = new byte[count];
                            this.reader.ReadBytes(tmpBuff);
                            this.MsgReceivedEvent?.Invoke(this, tmpBuff);
                        }
                    }
                    catch (TaskCanceledException) {
                        this.log.Info("DoReadTask", "Cancelation");
                        break;
                    }
                    catch (Exception e) {
                        this.log.Exception(9999, "", e);
                        break;
                    }
                }
                this.log.InfoExit("DoReadTask ---");
            });
        }

        #endregion

    }
}
