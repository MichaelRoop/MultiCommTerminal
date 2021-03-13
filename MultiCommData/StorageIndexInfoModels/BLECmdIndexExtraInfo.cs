using BluetoothLE.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;

namespace MultiCommData.Net.StorageIndexInfoModels {

    /// <summary>Extra display info for BLE commands index</summary>
    public class BLECmdIndexExtraInfo {

        public string CharacteristicName { get; set; } = string.Empty;
        public string DataTypeDisplay { get; set; } = string.Empty;
        public BLE_DataType DataType { get; set; } = BLE_DataType.UInt_8bit;


        public BLECmdIndexExtraInfo() {
            // Must have a default constructor to deserialize or check the param for null
        }


        public BLECmdIndexExtraInfo(BLECommandSetDataModel dm) {
            this.Update(dm);
        }


        public void Update(BLECommandSetDataModel dm) {
            this.CharacteristicName = dm.CharacteristicName;
            this.DataTypeDisplay = dm.DataType.ToStr();
            this.DataType = dm.DataType;
        }


    }
}
