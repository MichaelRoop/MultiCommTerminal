using Android.Bluetooth;
using BluetoothCommonAndroidXamarin.Data_models;
using CommunicationStack.Net.Enumerations;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace BluetoothCommonAndroidXamarin {

    public partial class BluetoothCommonFunctionality {

        public void ConnectAsync(string name) {
            Task.Run(() => {
                try {
                    this.Disconnect();
                    this.device = BluetoothAdapter.DefaultAdapter.BondedDevices.FirstOrDefault(d => d.Name == name);
                    this.msgPump.ConnectAsync2(new BTAndroidMsgPumpConnectData(this.device));
                }
                catch (Exception e) {
                    this.log.Exception(9999, "ConnectAsync", "", e);
                }
            });
        }


        public void Disconnect() {
            try {
                if (this.connected) {
                    this.msgPump.Disconnect();
                    // Never dispose the device since it came from the Adaptor Bonded list
                    if (this.device != null) {
                        this.device = null;
                    }
                    this.connected = false;
                }
            }
            catch (Exception e) {
                this.log.Exception(9898, "Disconnect", "", e);
            }
            this.connected = false;
        }


        #region Event handlers

        private void ConnectResultHandler(object sender, CommunicationStack.Net.DataModels.MsgPumpResults results) {
            this.connected = results.Code == MsgPumpResultCode.Connected;

            this.log.Info("MsgPumpConnectResultEvent", () => string.Format(
                "Connect result:{0}  Socket:{1} Msg:{2}",
                results.Code, results.SocketErr, results.ErrorString));
            this.ConnectionCompleted?.Invoke(this, this.connected);
        }

        #endregion

    }

}