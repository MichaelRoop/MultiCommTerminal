using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LogUtils.Net;
using Android.Bluetooth;

namespace BluetoothRfComm.AndroidXamarin.Receivers {

    /// <summary>Register as a receiver for non paired Classic devices
    /// 
    /// </summary>
    public class BT_DeviceUnpairedDiscoveryReceiver : BroadcastReceiver {

        Action<BluetoothDevice> raiseAction = null;
        List<BluetoothDevice> unBondedDevices = new List<BluetoothDevice>();
        ClassLog log = new ClassLog("BT_DeviceDiscoveryReceiver");


        public BT_DeviceUnpairedDiscoveryReceiver(Action<BluetoothDevice> raiseAction, List<BluetoothDevice> unBondedDevices) {
            this.raiseAction = raiseAction;
            this.unBondedDevices = unBondedDevices;
            this.unBondedDevices.Clear();
        }


        public override void OnReceive(Context context, Intent intent) {
            if (intent.Action == BluetoothDevice.ActionFound) {
                BluetoothDevice device = (BluetoothDevice)intent.GetParcelableExtra(BluetoothDevice.ExtraDevice);
                if (device.Type == BluetoothDeviceType.Classic) {
                    if (device.BondState != Bond.Bonded) {
                        this.log.Info("", () => string.Format(
                            "Found device:{0} MAC address:{1}", device.Name, device.Address));
                        this.raiseAction(device);
                    }
                }
            }
        }


    }
}