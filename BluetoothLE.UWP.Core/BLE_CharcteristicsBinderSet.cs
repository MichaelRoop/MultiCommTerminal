using BluetoothLE.Net.Enumerations;
using ChkUtils.Net;
using LogUtils.Net;
using System;
using System.Collections.Generic;

namespace Bluetooth.UWP.Core {

    /// <summary>Manage a list of OS to Data model binders</summary>
    public class BLE_CharcteristicsBinderSet {

        private ClassLog log = new ClassLog("BLE_CharcteristicsBinderSet");
        private List<BLE_CharacteristicBinder> binders = new List<BLE_CharacteristicBinder>();


        /// <summary>Event raised when value changes or the result of a read request</summary>
        public event EventHandler<BLE_CharacteristicReadResult> ReadValueChanged;


        /// <summary>Add a binder to the set</summary>
        /// <param name="binder">The binder to manager</param>
        public void Add(BLE_CharacteristicBinder binder) {
            this.binders.Add(binder);
            binder.DataModel.OnReadValueChanged += onReadValueChanged;
        }


        /// <summary>Tear down all binders including detaching events</summary>
        public void ClearAll() {
            try {
                foreach (BLE_CharacteristicBinder binder in this.binders) {
                    try {
                        binder.DataModel.OnReadValueChanged -= onReadValueChanged;
                        binder.Teardown();
                    }
                    catch(Exception e) {
                        this.log.Exception(9998, "ClearAll", "", e);
                    }
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "ClearAll", "", e);
            }
            finally {
                WrapErr.SafeAction(this.binders.Clear);
            }
        }


        private void onReadValueChanged(object sender, BLE_CharacteristicReadResult result) {
            this.ReadValueChanged?.Invoke(sender, result);
        }
    }

}
