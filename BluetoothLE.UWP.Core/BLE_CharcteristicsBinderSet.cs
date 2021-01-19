using BluetoothLE.Net.Enumerations;
using ChkUtils.Net;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Text;

namespace Bluetooth.UWP.Core {
    
    public class BLE_CharcteristicsBinderSet {

        private ClassLog log = new ClassLog("BLE_CharcteristicsBinderSet");

        public event EventHandler<BLE_CharacteristicReadResult> ReadValueChanged;

        private List<BLE_CharacteristicBinder> binders = new List<BLE_CharacteristicBinder>();


        public void Add(BLE_CharacteristicBinder binder) {
            this.binders.Add(binder);
            binder.DataModel.OnReadValueChanged += onReadValueChanged;
            

        }

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
            this.log.InfoEntry("onReadValueChanged");
            this.ReadValueChanged?.Invoke(sender, result);
        }
    }
}
