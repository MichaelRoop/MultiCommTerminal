using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothClassic {
    public class BluetoothClassicImpl : IBTInterface {

        public event EventHandler<BTDeviceInfo> DiscoveredDevice;

        //private void DiscoveredDevicesCallback(IAsyncResult result) {
        //    BluetoothClient thisDevice = result.AsyncState as BluetoothClient;
        //    if (result.IsCompleted) {
        //        BluetoothDeviceInfo[] devices = thisDevice.EndDiscoverDevices(result);
        //        foreach (BluetoothDeviceInfo dev in devices) {
        //            if (this.DiscoveredDevice != null) {
        //                this.DiscoveredDevice(this, new BTDeviceInfo() {
        //                    Name = dev.DeviceName,
        //                    Connected = dev.Connected,
        //                    Authenticated = dev.Authenticated,
        //                    Address = dev.DeviceAddress.ToString(),
        //                    DeviceClassInt = dev.ClassOfDevice.Value,
        //                    DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
        //                    ServiceClassInt = (int)dev.ClassOfDevice.Service,
        //                    ServiceClassName = dev.ClassOfDevice.Service.ToString(),
        //                });
        //            }
        //            //this.Invoke(new Action(delegate () {
        //            //    listBox1.Items.Add(d.DeviceName);
        //            //}));
        //        }
        //    }
        //}


        List<BTDeviceInfo> IBTInterface.DiscoverDevices() {
            BluetoothClient cl = null;
            List<BTDeviceInfo> infoList = new List<BTDeviceInfo>();

            //cl = new BluetoothClient();
            //cl.BeginDiscoverDevices(
            //    10, true, true, true, false, this.DiscoveredDevicesCallback, cl);
            //var l = new BluetoothListener();
            //IEnumerable < BluetoothDeviceInfo >

            cl = new BluetoothClient();
            //foreach (BluetoothDeviceInfo dev in cl.DiscoverDevices(20)) {
            foreach (BluetoothDeviceInfo dev in cl.DiscoverDevicesInRange()) {
                BTDeviceInfo info = new BTDeviceInfo() {
                    Name = dev.DeviceName,
                    Connected = dev.Connected,
                    Authenticated = dev.Authenticated,
                    Address = dev.DeviceAddress.ToString(),
                    DeviceClassInt = dev.ClassOfDevice.Value,
                    DeviceClassName = string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
                    ServiceClassInt = (int)dev.ClassOfDevice.Service,
                    ServiceClassName = dev.ClassOfDevice.Service.ToString(),
                };
                infoList.Add(info);
            }
            return infoList;

        }
    }
}
