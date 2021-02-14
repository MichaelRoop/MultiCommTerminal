using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using BluetoothLE.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth.GenericAttributeProfile;

namespace Bluetooth.UWP.Core {

    public partial class BluetoothLEImplWin32Core : IBLETInterface {


        private async Task BuildDescriptors(GattCharacteristic ch, BLE_CharacteristicDataModel dataModel) {
            try {
                // TODO - change to an awaitable with result
                this.log.InfoEntry("BuildDescriptors");
                dataModel.Descriptors = new List<BLE_DescriptorDataModel>();
                GattDescriptorsResult descriptors = await ch.GetDescriptorsAsync();
                if (descriptors.Status == GattCommunicationStatus.Success) {
                    if (descriptors.Descriptors.Count > 0) {
                        foreach (GattDescriptor desc in descriptors.Descriptors) {
                            GattReadResult r = await desc.ReadValueAsync();
                            if (r.Status == GattCommunicationStatus.Success) {
                                // New characteristic data model to add to service
                                IDescParser parser = this.descParserfactory.GetParser(desc.Uuid, desc.AttributeHandle);
                                BLE_DescriptorDataModel descDataModel = new BLE_DescriptorDataModel() {
                                    Uuid = desc.Uuid,
                                    AttributeHandle = desc.AttributeHandle,
                                    ProtectionLevel = (BLE_ProtectionLevel)desc.ProtectionLevel,
                                    DisplayName = parser.Parse(r.Value.FromBufferToBytes()),
                                    Parser = parser,
                                };

                                dataModel.Descriptors.Add(descDataModel);
                                this.log.Info("ConnectToDevice", () => string.Format("        Descriptor:{0}  Uid:{1} Value:{2}",
                                    BLE_DisplayHelpers.GetDescriptorName(desc), desc.Uuid.ToString(), descDataModel.DisplayName));
                            };
                        }
                    }
                }
                else {
                    this.log.Error(9999, "BuildDescriptors", () => string.Format("Get Descriptors result:{0}", descriptors.Status.ToString()));
                }
            }
            catch(Exception e) {
                this.log.Exception(9999, " BuildDescriptors", "", e);
            }
        }


        private void DebugDumpDescriptor(GattDescriptor d) {

        }

    }

}
