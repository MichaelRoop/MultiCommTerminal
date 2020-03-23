using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using InTheHand.Net.Bluetooth;
using InTheHand.Net.Sockets;
using System;
using System.Collections.Generic;
using System.Text;

namespace BluetoothClassic {
    public class BluetoothClassicImpl : IBTInterface {

        public void blah() {
            

        }


        List<BTDeviceInfo> IBTInterface.DiscoverDevices() {
            BluetoothClient cl = null;
            cl = new BluetoothClient();
            List<BTDeviceInfo> infoList = new List<BTDeviceInfo>();

            //BluetoothDevicePicker p = new BluetoothDevicePicker();
            //var xx = p.ClassOfDevices;
            //IReadOnlyCollection<BluetoothDeviceInfo> dd = cl.DiscoverDevices(255);
            //var xyz = cl.PairedDevices;



            //foreach (var x in p.ClassOfDevices) {
            //    BTDeviceInfo info = new BTDeviceInfo() {
            //        Name = x.Device.ToString(),// dev.DeviceName,
            //        Connected = true, //dev.Connected,
            //        Authenticated = true, //dev.Authenticated,
            //        //Address = dev.DeviceAddress.ToString(),
            //        DeviceClassInt = (uint)x.Value,// dev.ClassOfDevice.Value,
            //        DeviceClassName = string.Format("{0}:{1}", x.MajorDevice.GetTypeCode(), x.MajorDevice.ToString()),
            //        //DeviceClassName = x.Device.ToString(),// string.Format("{0}:{1}", dev.ClassOfDevice.MajorDevice, dev.ClassOfDevice.Device),
            //        ServiceClassInt = (int)x.Service,//  (int)dev.ClassOfDevice.Service,
            //        ServiceClassName = x.Service.ToString(),// dev.ClassOfDevice.Service.ToString(),
            //    };
            //    infoList.Add(info);
            //}





            //cl.PairedDevices
            //IReadOnlyCollection<BluetoothDeviceInfo> dd = cl.DiscoverDevices(255);

            //IEnumerable < BluetoothDeviceInfo >


            foreach (BluetoothDeviceInfo dev in /*cl.PairedDevices*/ cl.DiscoverDevices()) {
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
