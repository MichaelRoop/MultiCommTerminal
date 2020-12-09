using Android.Bluetooth;
using Android.Content;
using LogUtils.Net;
using System;
using System.Collections.Generic;

namespace BluetoothCommonAndroidXamarin.Receivers {

    /// <summary>Receiver to get list of unbound devices</summary>
    public class UnboundDeviceDiscoveryReceiver : BroadcastReceiver {

        Action<BluetoothDevice> onFound = null;
        List<BluetoothDevice> unBondedDevices = new List<BluetoothDevice>();
        BluetoothDeviceType deviceType = BluetoothDeviceType.Classic;
        ClassLog log = new ClassLog("DeviceDiscoveryReceiver");


        public UnboundDeviceDiscoveryReceiver(
            BluetoothDeviceType deviceType, 
            Action<BluetoothDevice> onFound, 
            List<BluetoothDevice> unBondedDevices) {
            
            this.deviceType = deviceType;
            this.onFound = onFound;
            this.unBondedDevices = unBondedDevices;
        }


        public override void OnReceive(Context context, Intent intent) {
            if (intent.Action == BluetoothDevice.ActionFound) {
                BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                if (device.Type == this.deviceType) {
                    if (device.BondState != Bond.Bonded) {
                        this.unBondedDevices.Add(device);
                        this.log.Info("OnReceive", () => string.Format(
                            "Found device:{0} MAC address:{1}", device.Name, device.Address));
                        this.onFound(device);
                    }
                }
            }
        }

    }
}