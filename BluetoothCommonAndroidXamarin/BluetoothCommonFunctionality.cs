using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BluetoothCommon.Net;
using BluetoothCommonAndroidXamarin.Receivers;
using BluetoothLE.Net.DataModels;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BluetoothCommonAndroidXamarin {

    public partial class BluetoothCommonFunctionality {

        #region Data

        private BluetoothDeviceType deviceType = BluetoothDeviceType.Classic;
        private ClassLog log = new ClassLog("BluetoothCommonFunctionality");
        private List<BluetoothDevice> unBondedDevices = new List<BluetoothDevice>();

        #endregion

        #region Events

        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<BluetoothLEDeviceInfo> DiscoveredBLEDevice;

        #endregion

        #region Properties

        public List<BluetoothDevice> UnBondedDevices { get { return this.unBondedDevices; } }

        #endregion


        public BluetoothCommonFunctionality(BluetoothDeviceType deviceType) {
            this.deviceType = deviceType;
        }




        #region Private


        private Context GetContext() {
            return Android.App.Application.Context;
        }

        #endregion

    }
}