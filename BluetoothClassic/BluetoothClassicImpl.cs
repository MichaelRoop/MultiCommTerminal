using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using VariousUtils;

namespace BluetoothClassic.Net {
    public class BluetoothClassicImpl : IBTInterface {

        private ClassLog log = new ClassLog("BluetoothClassicImpl");
        private BluetoothClient currentDevice = null;
        private NetworkStream dataStream = null;

        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<byte[]> MsgReceivedEvent;

        void IBTInterface.DiscoverDevicesAsync() {
            this.log.InfoEntry("DiscoverDevicesAsync+++");
            BluetoothClient cl = null;

            try {
                // 32feet.Net - async callback
                cl = new BluetoothClient();
                cl.BeginDiscoverDevices(
                    25, true, true, true, true, this.DiscoveredDevicesCallback, cl);

                this.log.InfoExit("DiscoverDevicesAsync---");
            }
            catch (Exception e) {
                this.log.Exception(9000, "", e);
            }
        }


        public void ConnectAsync(BTDeviceInfo device) {
            try {
                this.Disconnect();
                this.currentDevice = new BluetoothClient();

                this.log.Info("ConnectAsyn",
                    () => string.Format("Starting connection of:{0} {1}",
                    device.Name, device.Address));

                IAsyncResult result = this.currentDevice.BeginConnect(
                    BluetoothAddress.Parse(device.Address),
                    BluetoothService.SerialPort,
                    this.ConnectedCallback,
                    this.currentDevice);
                this.log.Info("ConnectAsyn",
                    () => string.Format("IsComplete:{0} IsCompletedSynchronously:{1}",
                    result.IsCompleted, result.CompletedSynchronously));
            }
            catch (Exception e) {
                this.log.Exception(7777, "", e);
                this.ConnectionCompleted?.Invoke(this, false);
            }
        }


        public bool Connect(BTDeviceInfo device) {
            try {
                this.log.InfoEntry("Connect");
                this.Disconnect();
                this.currentDevice = new BluetoothClient();
                BluetoothAddress address;

                if (!BluetoothAddress.TryParse(device.Address, out address)) {
                    this.log.Error(9999, () => string.Format("Failed to parse out the address {0} on device {1}", device.Address, device.Name));
                    return false;
                }

                this.currentDevice.Connect(address, BluetoothService.SerialPort);
                this.log.Info("Connect", () => string.Format("IsConnected:{0}", this.currentDevice.Connected));
                return this.currentDevice.Connected;
            }
            catch (Exception e) {
                this.log.Exception(8888, "", e);
                this.log.Info("Connect", "Exiting FALSE");
                return false;
            }
        }


        public void Disconnect() {
            this.log.InfoEntry("Disconnect");
            WrapErr.ToErrReport(9999, () => {
                this.KillRead();
                if (this.dataStream != null) {
                    this.dataStream.Dispose();
                    this.dataStream = null;
                }

                if (this.currentDevice != null) {
                    this.currentDevice.Dispose();
                    this.currentDevice = null;
                }
            });
            this.log.InfoExit("Disconnect");
        }


        /// <summary>Used by ICommStackLevel0 to send msg after adding terminator</summary>
        /// <param name="msg"></param>
        /// <returns></returns>
        public bool SendOutMsg(byte[] msg) {
            try {
                this.log.InfoEntry("SendOutMsg");
                if (this.currentDevice != null) {
                    if (this.dataStream != null) {
                        this.dataStream.Write(msg, 0, msg.Length);
                        return true;
                    }
                    else {
                        this.log.Error(9111, "Data stream null");
                    }
                }
                else {
                    this.log.Error(9111, "Current device null");
                }
            }
            catch (Exception e) {
                this.log.Exception(9898, "", e);
            }
            return false;
        }


        #region Async Callbacks
        private void DiscoveredDevicesCallback(IAsyncResult result) {
            this.log.InfoEntry("DiscoveredDevicesCallback");
            try {
                BluetoothClient thisDevice = result.AsyncState as BluetoothClient;
                if (result.IsCompleted) {
                    BluetoothDeviceInfo[] devices = thisDevice.EndDiscoverDevices(result);
                    foreach (BluetoothDeviceInfo dev in devices) {

                        this.log.Info("DiscoveredDevicesCallback", () => string.Format("Device:{0} Number services:{1}",
                            dev.DeviceName, dev.InstalledServices.Length));
                        foreach (var s in dev.InstalledServices) {
                            this.log.Info("InfoDiscoveredDevicesCallback", () => string.Format("     Service:{0}", s.ToString()));
                        }

                        if (this.DiscoveredBTDevice != null) {
                            this.log.Info("DiscoveredDevicesCallback", () =>
                                string.Format("Discovered:{0} - {1}", dev.DeviceName, dev.DeviceAddress.ToString()));
                            this.DiscoveredBTDevice(this, new BTDeviceInfo() {
                                Name = dev.DeviceName,
                                Connected = dev.Connected,
                                Authenticated = dev.Authenticated,
                                Address = dev.DeviceAddress.ToString(),
                                DeviceClassInt = dev.ClassOfDevice.Value,
                                DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
                                ServiceClassInt = (int)dev.ClassOfDevice.Service,
                                ServiceClassName = dev.ClassOfDevice.Service.ToString(),
                                Strength = dev.Rssi,
                                LastSeen = dev.LastSeen,
                                LastUsed = dev.LastUsed,
                                Radio = this.BuildRadioDataModel(dev),
                            });
                        }
                        else {
                            this.log.Warning(9999, "No subscribers to DiscoveredBTDevice");
                        }
                    }
                }
                else {
                    this.log.Error(9990, "DiscoveredDevicesCallback", "Asychronous Operation Not Completed");
                }
                // push up completed event
                if (this.DiscoveryComplete != null) {
                    this.DiscoveryComplete(this, result.IsCompleted);
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        private void ConnectedCallback(IAsyncResult result) {
                try { 
                this.log.InfoEntry("ConnectedCallback");
                BluetoothClient thisDevice = result.AsyncState as BluetoothClient;
                if (result.IsCompleted) {
                    thisDevice.EndConnect(result);
                    if (this.dataStream != null) {
                        this.dataStream.Close();
                        this.dataStream.Dispose();
                        this.dataStream = null;
                    }
                    this.dataStream = thisDevice.GetStream();
                    this.LaunchRead();
                }
                else {
                    this.log.Error(9999, "Failed to complete connection");
                }
                this.ConnectionCompleted?.Invoke(this, result.IsCompleted);
            }
            catch(Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        private BTRadioInfo BuildRadioDataModel(BluetoothDeviceInfo device) {
            // TODO Until we fix the connect issue just return empty radio info
            return new BTRadioInfo();

            try {
                this.log.InfoEntry("BuildRadioDataModel");
                BTDeviceInfo info = new BTDeviceInfo() {
                    Name = device.DeviceName,
                    Address = device.DeviceAddress.ToString(),
                };

                // Need to connect to each device to get radio info
                RadioVersions devRadio = null;
                if (this.Connect(info)) {
                    devRadio = device.GetVersions();
                    this.Disconnect();
                }
                else {
                    this.log.Error(9991, "NOT CONNECTED TO READ RADIO INFO");
                }

                //this.Disconnect();
                if (devRadio == null) {
                    this.log.Info("", "NULL Radio info");
                    return new BTRadioInfo();
                }

                string tmp = devRadio.LmpSupportedFeatures.ToString();
                string[] pieces = tmp.Split(',');
                List<string> features = new List<string>(pieces.Length);
                for (int i = 0; i < pieces.Length; i++) {
                    features.Add(pieces[i].Replace("_", "").CamelCaseToSpaces());
                }

                return new BTRadioInfo() {
                    Manufacturer = devRadio.Manufacturer.ToString().CamelCaseToSpaces(),
                    LinkManagerProtocol = string.Format("{0} ({1})", devRadio.LmpVersion.LmpVerToString(), devRadio.LmpSubversion),
                    Features = features,
                };
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                return new BTRadioInfo();
            }
        }

        #endregion

        #region Read Thread

        private bool doneRead = false;
        private Thread readThread = null;
        private ManualResetEvent readDoneEvent = new ManualResetEvent(false);

        private void LaunchRead() {
            this.KillRead();
            this.readDoneEvent.Reset();
            this.doneRead = false;
            this.readThread = new Thread(this.ReadThread);
            this.readThread.Start();
        }


        private void KillRead() {
            WrapErr.ToErrReport(9999, () => {
                this.log.InfoEntry("KillRead++++");
                if (this.readThread != null) {
                    this.doneRead = true;
                    // send dummy message so thread can wake and end
                    this.dataStream.Write(new byte[1] { 0 }, 0, 1);
                    if (!this.readDoneEvent.WaitOne(500)) {
                        // TODO not supported on this platform. Core only.
                        WrapErr.ToErrReport(9999, () => this.readThread.Abort());
                    }
                    else {
                        this.log.Info("KillRead", "The done read has completed properly");
                    }
                    this.readThread = null;
                }
                this.log.InfoExit("KillRead----");
            });
        }


        private void ReadThread() {
            this.log.InfoEntry("ReadThread");
            byte[] buff = new byte[500];
            while (!this.doneRead) {
                try {
                    int bytesRead = this.dataStream.Read(buff, 0, 500);
                    if (bytesRead > 0) {
                        if (!this.doneRead && this.MsgReceivedEvent != null) {
                            byte[] outgoing = new byte[bytesRead];
                            Buffer.BlockCopy(buff, 0, outgoing, 0, bytesRead);
                            this.MsgReceivedEvent(this, outgoing);
                        }
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "From the read thread", e);
                    Thread.Sleep(100);
                }
            }
            this.log.Info("ReadThread", "Exiting read thread....");
            this.readDoneEvent.Set();
        }

        #endregion

    }
}
