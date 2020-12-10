using Android.App;
using Android.Bluetooth;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BluetoothCommon.Net;
using BluetoothCommon.Net.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace BluetoothCommonAndroidXamarin {

    public partial class BluetoothCommonFunctionality {

        public void Pair(string name) {
            BluetoothDevice unbonded = this.UnBondedDevices.FirstOrDefault(device => device.Name == name);
            // Status is failed by default 
            // TODO - check if needed another for BLE
            BTPairOperationStatus status = new BTPairOperationStatus() {
                Name = name,
            };
            try {
                if (unbonded != null) {
                    if (unbonded.CreateBond()) {
                        status.IsSuccessful = true;
                        status.PairStatus = BT_PairingStatus.Paired;
                    }
                    else {
                        status.PairStatus = BT_PairingStatus.AuthenticationFailure;
                    }
                }
                else {
                    status.PairStatus = BT_PairingStatus.NoParingObject;
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "PairgAsync", "", e);
                status.PairStatus = BT_PairingStatus.Failed;
            }

            this.BT_PairStatus?.Invoke(this, status);
        }


        public void UnPairAsync(string name) {
            // Also need to check if current device.
            // Status is failed by default
            BTUnPairOperationStatus status = new BTUnPairOperationStatus() {
                Name = name,
            };
            Task.Run(() => {
                try {
                    var d = BluetoothAdapter.DefaultAdapter.BondedDevices.FirstOrDefault(dev => dev != null && dev.Name == name);
                    if (d != null) {
                        var mi = d.Class.GetMethod("removeBond", null);
                        var sdfd = mi.Invoke(d, null);
                        // Need to sleep a bit for the method invocation to complete
                        Thread.Sleep(200);
                        // Suppose it to be successful if exception not thrown. Reload list to see if it is still there
                        status.IsSuccessful = true;
                        status.UnpairStatus = BT_UnpairingStatus.Success;
                    }
                    else {
                        this.log.Error(9999, "UnPairAsync", "Already unpaired - null device or not of name");
                        status.UnpairStatus = BT_UnpairingStatus.AlreadyUnPaired;
                    }
                }
                catch (Exception e) {
                    this.log.Exception(9999, "UnPairAsync", "", e);
                    status.UnpairStatus = BT_UnpairingStatus.Failed;
                }
                this.BT_UnPairStatus?.Invoke(this, status);
            });
        }

    }
}