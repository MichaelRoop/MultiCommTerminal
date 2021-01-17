using BluetoothLE.Net.interfaces;
using ChkUtils.Net;
using System;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {

        /// <summary>Execute the disconnection routines</summary>
        private void DoDisconnect() {
            if (this.currentDevice != null) {
                this.TeardownBinders();
                this.DisconnectBTLEDeviceEvents();
                this.DisposeServices();
                try {
                    this.currentDevice.Dispose();
                    this.currentDevice = null;
                }
                catch(Exception e) {
                    this.log.Exception(9998, "DoDisconnect", "", e);
                }
            }
        }


        /// <summary>Disconnect the BLE Device events</summary>
        private void DisconnectBTLEDeviceEvents() {
            if (this.currentDevice != null) {
                this.currentDevice.ConnectionStatusChanged -= this.CurrentDevice_ConnectionStatusChanged;
                this.currentDevice.GattServicesChanged -= this.CurrentDevice_GattServicesChanged;
                this.currentDevice.NameChanged -= this.CurrentDevice_NameChanged;
            }
        }


        /// <summary>Dispose the services manualy since the device dispose does not do it</summary>
        private void DisposeServices() {
            try {
                foreach (GattDeviceService service in this.currentServices) {
                    try {
                        service.Dispose();
                    }
                    catch (Exception e) {
                        this.log.Exception(9998, "DisposeServices", "", e);
                    }
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "DisposeServices", "", e);
            }
            finally {
                WrapErr.SafeAction(this.currentServices.Clear);
            }
        }


        /// <summary>Remove the event chain from OS-DataModel characteristics</summary>
        private void TeardownBinders() {
            try {
                foreach (var binder in this.characteristicBinders) {
                    try {
                        binder.Teardown();
                    }
                    catch(Exception e) {
                        this.log.Exception(9998, "TeardownBinders", "", e);
                    }
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "TeardownBinders", "", e);
            }
            finally {
                WrapErr.SafeAction(this.characteristicBinders.Clear);
            }
        }

    }

}
