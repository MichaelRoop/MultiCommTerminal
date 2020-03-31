using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using InTheHand.Net;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Linq;
using System.Threading;

namespace BluetoothClassic {
    public class BluetoothClassicImpl : IBTInterface {

        private BluetoothClient currentDevice = null;

        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<byte[]> BytesReceived;

        List<BTDeviceInfo> IBTInterface.DiscoverDevices() {
            BluetoothClient cl = null;
            List<BTDeviceInfo> infoList = new List<BTDeviceInfo>();

            // 32feet.Net - async callback
            cl = new BluetoothClient();
            cl.BeginDiscoverDevices(
                25, true, true, true, true, this.DiscoveredDevicesCallback, cl);

            //// 32feet.Net
            //cl = new BluetoothClient();
            ////foreach (BluetoothDeviceInfo dev in cl.DiscoverDevices(20)) {
            //foreach (BluetoothDeviceInfo dev in cl.DiscoverDevicesInRange()) {
            //    BTDeviceInfo info = new BTDeviceInfo() {
            //        Name = dev.DeviceName,
            //        Connected = dev.Connected,
            //        Authenticated = dev.Authenticated,
            //        Address = dev.DeviceAddress.ToString(),
            //        DeviceClassInt = dev.ClassOfDevice.Value,
            //        DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
            //        ServiceClassInt = (int)dev.ClassOfDevice.Service,
            //        ServiceClassName = dev.ClassOfDevice.Service.ToString(),
            //    };
            //    infoList.Add(info);
            //}
            // remove later
            return infoList;

        }


        public void Connect(BTDeviceInfo device) {
            this.Disconnect();
            this.currentDevice = new BluetoothClient();
            this.currentDevice.BeginConnect(
                BluetoothAddress.Parse(device.Address),
                BluetoothService.SerialPort,
                this.ConnectedCallback, 
                this.currentDevice);
        }


        public void Disconnect() {
            if (this.dataStream != null) {
                this.dataStream.Dispose();
                this.dataStream = null;
            }

            if (this.currentDevice != null) {
                this.currentDevice.Dispose();
                this.currentDevice = null;
            }
        }


        public void Send(string msg) {
            if (this.currentDevice != null) {
                if (this.dataStream != null) {
                    byte[] buff = Encoding.ASCII.GetBytes(msg);
                    this.dataStream.Write(buff, 0, buff.Length);
                }
            }
        }


        #region Async Callbacks
        private void DiscoveredDevicesCallback(IAsyncResult result) {
            BluetoothClient thisDevice = result.AsyncState as BluetoothClient;
            if (result.IsCompleted) {
                BluetoothDeviceInfo[] devices = thisDevice.EndDiscoverDevices(result);
                foreach (BluetoothDeviceInfo dev in devices) {
                    if (this.DiscoveredBTDevice != null) {
                        this.DiscoveredBTDevice(this, new BTDeviceInfo() {
                            Name = dev.DeviceName,
                            Connected = dev.Connected,
                            Authenticated = dev.Authenticated,
                            Address = dev.DeviceAddress.ToString(),
                            DeviceClassInt = dev.ClassOfDevice.Value,
                            DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
                            ServiceClassInt = (int)dev.ClassOfDevice.Service,
                            ServiceClassName = dev.ClassOfDevice.Service.ToString(),
                        });
                    }
                }
            }
            // push up completed event
            if (this.DiscoveryComplete != null) {
                this.DiscoveryComplete(this, result.IsCompleted);
            }
        }

        NetworkStream dataStream = null;

        private void ConnectedCallback(IAsyncResult result) {
            BluetoothClient thisDevice = result.AsyncState as BluetoothClient;
            if (result.IsCompleted) {
                thisDevice.EndConnect(result);
                this.dataStream = thisDevice.GetStream();
                this.LaunchRead();
                //byte[] data = Encoding.ASCII.GetBytes("Data sent on connect");
                //this.dataStream.Write(data, 0, data.Length);
            }
            if (this.ConnectionCompleted!= null) {
                this.ConnectionCompleted(this, result.IsCompleted);
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
            if (this.readThread != null) {
                this.doneRead = true;
                if (!this.readDoneEvent.WaitOne(500)) {
                    this.readThread.Abort();
                }
                this.readThread = null;
            }
        }


        private void ReadThread() {
            byte[] buff = new byte[500];
            while (!this.doneRead) {
                try {
                    int bytesRead = this.dataStream.Read(buff, 0, 500);
                    if (bytesRead > 0) {
                        if (this.BytesReceived != null) {
                            byte[] outgoing = new byte[bytesRead];
                            Buffer.BlockCopy(buff, 0, outgoing, 0, bytesRead);
                            this.BytesReceived(this, outgoing);
                        }
                    }
                }
                catch (Exception e) {
                    System.Diagnostics.Debug.WriteLine(e.Message);
                    Thread.Sleep(500);
                }
            }
            this.readDoneEvent.Set();

        }

        #endregion

    }
}
