using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using LogUtils.Net;
using System;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;
using Windows.Storage.Streams;

namespace Bluetooth.UWP.Core {

    /// <summary>
    /// Bind the generic BLE Characteristic info with the OS specific characteristic
    /// </summary>
    public class BLE_CharacteristicBinder {

        private const int BLE_BLOCK_SIZE = 20;
        private ClassLog log = new ClassLog("");


        public GattCharacteristic OSCharacteristic { get; set; }
        public BLE_CharacteristicDataModel DataModel { get; set; }


        public BLE_CharacteristicBinder(GattCharacteristic osCharacteristic, BLE_CharacteristicDataModel dataModel) {
            this.OSCharacteristic = osCharacteristic;
            this.DataModel = dataModel;
            
            this.OSCharacteristic.ValueChanged += this.OSCharacteristicReadValueChangedHandler;
            this.DataModel.WriteRequestEvent += this.onDataModelWriteRequestHandler;
            this.DataModel.ReadRequestEvent += this.onDataModelReadRequestHandler;
        }


        #region Event handlers

        /// <summary>Handles read request from the user via the Characteristic Data Model</summary>
        /// <param name="sender">Originator of request</param>
        /// <param name="args">The event args. Has nothing</param>
        private void onDataModelReadRequestHandler(object sender, EventArgs args) {
            Task.Run(async () => {
                try {
                    GattReadResult result = await this.OSCharacteristic.ReadValueAsync();
                    if (this.ParseGattStatue(result.Status)) {
                        this.DataModel.PushReadDataEvent(result.Value.FromBufferToBytes());
                    }
                }
                catch (Exception ex) {
                    this.log.Exception(9999, "DataModel_ReadRequestEvent", "", ex);
                    this.DataModel.PushCommunicationError(BLE_CharacteristicCommunicationStatus.UnknownError);
                }
            });
        }


        /// <summary>Write out the incoming message to the OS characteristic</summary>
        private void onDataModelWriteRequestHandler(object sender, byte[] data) {
            Task.Run(async () => {
                try {
                    int count = data.Length / BLE_BLOCK_SIZE;
                    int rest = (data.Length % BLE_BLOCK_SIZE);
                    int lastIndex = 0;
                    for (int i = 0; i < count; i++) {
                        lastIndex = i * BLE_BLOCK_SIZE;
                        if (await this.WriteBlock(data, lastIndex, BLE_BLOCK_SIZE) != true) {
                            return;
                        }
                    }
                    // Last block if partial block
                    if (lastIndex > 0 && rest > 0) {
                        lastIndex += BLE_BLOCK_SIZE;
                        await this.WriteBlock(data, lastIndex, rest);
                    }
                }
                catch (Exception e) {
                }
            });
        }


        /// <summary>Handle data changed events from the UWP characteristic</summary>
        /// <param name="sender">The originator</param>
        /// <param name="args">The event args with the data buffer and timestamp</param>
        private void OSCharacteristicReadValueChangedHandler(
            GattCharacteristic sender, GattValueChangedEventArgs args) {
            Task.Run(() => {
                try {
                    this.DataModel.PushReadDataEvent(
                        args.CharacteristicValue.FromBufferToBytes());

                    // We can translate to string here based on the characteristic type and write to CharValue
                    //this.DataModel.CharValue = TranslateBytes...
                }
                catch (Exception e) {
                    this.log.Exception(9999, "OSCharacteristic_ValueChanged", "", e);
                }
            });
        }

        #endregion

        #region Private

        /// <summary>Write a block of data to the UWP characteristic</summary>
        /// <param name="data">Buffer with data to write</param>
        /// <param name="index">Start index of data to read</param>
        /// <param name="size">Number of bytes to read</param>
        /// <returns>true on success, otherwise false with error raised</returns>
        private async Task<bool> WriteBlock(byte[] data, int index, int size) {
            try {
                using (var ms = new DataWriter()) {
                    byte[] part = new byte[size];
                    Array.Copy(data, index, part, 0, part.Length);
                    ms.WriteBytes(part);
                    GattCommunicationStatus result = await 
                        this.OSCharacteristic.WriteValueAsync(ms.DetachBuffer());
                    return this.ParseGattStatue(result);
                }
            }
            catch (Exception e) {
                this.log.Exception(9999, "WriteBlock", "", e);
                return false;
            }
        }


        /// <summary>Parse communications operation status and raise if error</summary>
        /// <param name="gattStatus">The UWP Gatt status</param>
        /// <returns>true on success, otherwise false where an error raised</returns>
        private bool ParseGattStatue(GattCommunicationStatus gattStatus) {
            BLE_CharacteristicCommunicationStatus status = BLE_CharacteristicCommunicationStatus.Success;
            switch (gattStatus) {
                case GattCommunicationStatus.Success:
                    return true;
                case GattCommunicationStatus.Unreachable:
                    status = BLE_CharacteristicCommunicationStatus.Unreachable;
                    break;
                case GattCommunicationStatus.ProtocolError:
                    status = BLE_CharacteristicCommunicationStatus.ProtocolError;
                    break;
                case GattCommunicationStatus.AccessDenied:
                    status = BLE_CharacteristicCommunicationStatus.AccessDenied;
                    break;
                default:
                    status = BLE_CharacteristicCommunicationStatus.UnknownError;
                    break;
            }
            this.DataModel.PushCommunicationError(status);
            return false;
        }

        #endregion

    }
}
