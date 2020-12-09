//using Android.Bluetooth;
//using Android.Content;
//using BluetoothCommon.Net;
//using BluetoothCommon.Net.interfaces;
//using BluetoothCommonAndroidXamarin;
//using BluetoothCommonAndroidXamarin.Receivers;
//using System;
//using System.Threading;
//using System.Threading.Tasks;

//namespace BluetoothRfComm.AndroidXamarin {

//    public partial class BluetoothRfCommAndroidXamarinImpl : IBTInterface {

//        #region Data1

//        UnboundDeviceDiscoveryReceiver discoverReceiver = null;

//        //private BluetoothCommonFunctionality common = new BluetoothCommonFunctionality(BluetoothDeviceType.Classic);


//        #endregion

//        private void DoDiscovery(bool paired) {
//            if (paired) {
//                this.DoDiscoveryPaired();
//            }
//            else {
//                this.DoDiscoveryUnpaired();
//            }
//        }


//        private void DoDiscoveryPaired() {
//            try {
//                this.log.InfoEntry("DoDiscoveryPaired");
//                if (BluetoothAdapter.DefaultAdapter != null) {
//                    if (!BluetoothAdapter.DefaultAdapter.IsEnabled) {
//                        BluetoothAdapter.DefaultAdapter.Enable();
//                    }

//                    if (BluetoothAdapter.DefaultAdapter.IsEnabled) {
//                        this.log.Info("DoDiscoveryPaired", () => string.Format("Number of paired devices"));
//                        foreach (BluetoothDevice device in BluetoothAdapter.DefaultAdapter.BondedDevices) {
//                            if (device.Type == BluetoothDeviceType.Classic) {
//                                this.log.Info("DoDiscoveryPaired", () => string.Format("Found paired device:{0}", device.Name));
//                                this.RaiseDeviceDiscovered(device);
//                            }
//                        }
//                        this.DiscoveryComplete?.Invoke(this, true);
//                    }
//                    else {
//                        this.log.Error(9, "Default adapter failed to enabled");
//                        this.DiscoveryComplete?.Invoke(this, false);
//                    }
//                }
//                else {
//                    this.log.Error(9, "Default adapter null");
//                    // TODO - need error event
//                    this.DiscoveryComplete?.Invoke(this, false);
//                }
//            }
//            catch(Exception e) {
//                this.log.Exception(9999, "", e);
//            }
//        }


//        private void DoDiscoveryUnpaired() {
//            try {
//                this.KillDiscoverReceiver();
//                this.discoverReceiver = new
//                    UnboundDeviceDiscoveryReceiver(
//                        BluetoothDeviceType.Classic,
//                        this.RaiseDeviceDiscovered,
//                        this.unBondedDevices);
//                this.GetContext().RegisterReceiver(
//                    this.discoverReceiver, new IntentFilter(BluetoothDevice.ActionFound));
//                BluetoothAdapter.DefaultAdapter.StartDiscovery();
//                this.StartAutoEnd();
//            }
//            catch (Exception e) {
//                this.log.Exception(9999, "", e);
//            }
//        }


//        private void RaiseDeviceDiscovered(BluetoothDevice device) {
//            BTDeviceInfo info = new BTDeviceInfo() {
//                IsPaired = device.BondState == Bond.Bonded,
//                Name = device.Name,
//                DeviceClassName = device.Class.Name,
//                Address = device.Address,
//                // TODO - any others as needed
//            };

//            this.log.Info("RaiseDeviceDiscovered", () => string.Format(
//                "{0} - {1} - {2}", info.Name, info.DeviceClassName, device.Address));

//            this.DiscoveredBTDevice?.Invoke(this, info);
//        }


//        private void KillDiscoverReceiver() {
//            if (this.discoverReceiver != null) {
//                this.GetContext().UnregisterReceiver(this.discoverReceiver);
//                this.discoverReceiver.Dispose();
//                this.discoverReceiver = null;
//            }
//        }


//        private void StartAutoEnd() {
//            Task.Run(() => {
//                AutoResetEvent timeout = new AutoResetEvent(false);
//                timeout.WaitOne(15000);
//                this.KillDiscoverReceiver();
//                this.DiscoveryComplete?.Invoke(this, false);
//            });
//        }



//    }

//}