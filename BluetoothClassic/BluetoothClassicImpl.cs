using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothClassic {
    public class BluetoothClassicImpl : IBTInterface {

        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<bool> DiscoveryComplete;

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
    }
}
