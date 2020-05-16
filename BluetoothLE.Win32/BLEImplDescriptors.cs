using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.interfaces;
using BluetoothLE.Net.Parsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace BluetoothLE.Win32 {

    public partial class BluetoothLEImplWin32 : IBLETInterface {


        private async Task BuildDescriptors(GattCharacteristic ch, BLE_CharacteristicDataModel dataModel) {
            dataModel.Descriptors = new Dictionary<string, BLE_DescriptorDataModel>();
            GattDescriptorsResult descriptors = await ch.GetDescriptorsAsync();
            if (descriptors.Status == GattCommunicationStatus.Success) {
                if (descriptors.Descriptors.Count > 0) {
                    foreach (GattDescriptor desc in descriptors.Descriptors) {
                        // New characteristic data model to add to service
                        GattReadResult r = await desc.ReadValueAsync();
                        if (r.Status == GattCommunicationStatus.Success) {
                            BLE_DescriptorDataModel descDataModel = new BLE_DescriptorDataModel() {
                                Uuid = desc.Uuid,
                                AttributeHandle = desc.AttributeHandle,
                                DisplayName = BLE_ParseHelpers.GetDescriptorValueAsString(desc.Uuid, r.Value.FromBufferToBytes())
                            };

                            dataModel.Descriptors.Add(descDataModel.Uuid.ToString(), descDataModel);
                            this.log.Info("ConnectToDevice", () => string.Format("        Descriptor:{0}  Uid:{1} Value:{2}",
                                BLE_DisplayHelpers.GetDescriptorName(desc), desc.Uuid.ToString(), descDataModel.DisplayName));
                        };
                    }
                }
            }
            else {
                this.log.Info("ConnectToDevice", () => string.Format("        Get Descriptors result:{0}", descriptors.Status.ToString()));
            }
        }
    }

}
