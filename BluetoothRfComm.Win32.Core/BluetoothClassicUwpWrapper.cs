using BluetoothCommon.Net;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using VariousUtils.Net;
using Windows.Networking;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothRfComm.UWP.Core {

    public partial class BluetoothRfCommWrapperUwpCore : IDisposable {

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
        private ManualResetEvent readFinishedEvent = new ManualResetEvent(false);

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
            this.TearDown(false);
        }


        /// <summary>Run asynchronous connection where ConnectionCompleted is raised on completion</summary>
        /// <param name="deviceDataModel">The data model with information on the device</param>
        public void ConnectAsync(BTDeviceInfo deviceDataModel) {
            Task.Run(async () => {
                try {
                    this.log.InfoEntry("ConnectAsync");
                    this.TearDown(true);
                    await this.GetExtraInfo(deviceDataModel, false);
                        
                    this.log.Info("ConnectAsync", () => string.Format(
                        "Host:{0} Service:{1}", deviceDataModel.RemoteHostName, deviceDataModel.RemoteServiceName));

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
                    this.log.Exception(9999, "Connect Asyn Error", e);
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
            this.TearDown(false);
        }

        #endregion

        #region Private

        /// <summary>Tear down any connections, dispose and reset all resources</summary>
        private void TearDown(bool sleepAfterSocketDispose) {
            try {
                #region Cancel Read Thread
                this.continueReading = false;
                if (this.readCancelationToken != null) {
                    this.readCancelationToken.Cancel();
                    this.readCancelationToken.Dispose();
                    this.readCancelationToken = null;
                    if (!this.readFinishedEvent.WaitOne(2000)) {
                        this.log.Error(9999, "Timed out waiting for read cancelation");
                    }
                }
                #endregion

                #region Close Writer and Reader
                if (this.writer != null) {
                    try { this.writer.DetachStream(); } catch(Exception e) { this.log.Exception(9999, "", e); }
                    this.writer.Dispose();
                    this.writer = null;
                }

                if (this.reader != null) {
                    try { this.reader.DetachStream(); } catch (Exception e) { this.log.Exception(9999, "", e); }
                    this.reader.Dispose();
                    this.reader = null;
                }
                #endregion

                #region Close socket
                if (this.socket != null) {
                    // The socket was closed so cannot cancel IO
                    this.socket.Dispose();
                    this.socket = null;
                    // Seems socket does not shut itself down fast enough before next call to connect
                    if (sleepAfterSocketDispose) {
                        Thread.Sleep(500);
                    }
                }
                #endregion
                this.Connected = false;
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
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
                this.readFinishedEvent.Reset();

                while (this.continueReading) {
                    try {
                        int count = (int)await this.reader.LoadAsync(READ_BUFF_MAX_SIZE).AsTask(this.readCancelationToken.Token);
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
                this.readFinishedEvent.Set();
            });
        }

        #endregion

    }
}
