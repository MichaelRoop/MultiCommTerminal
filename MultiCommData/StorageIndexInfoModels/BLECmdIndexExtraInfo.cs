using BluetoothLE.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;

namespace MultiCommData.Net.StorageIndexInfoModels {

    /// <summary>Extra display info for BLE commands index</summary>
    public class BLECmdIndexExtraInfo {

        public string CharacteristicName { get; set; } = string.Empty;
        public string DataTypeDisplay { get; set; } = string.Empty;
        public BLE_DataType DataType { get; set; } = BLE_DataType.UInt_8bit;


        public BLECmdIndexExtraInfo(BLECommandSetDataModel dm) {
            this.CharacteristicName = dm.CharacteristicName;
            this.DataTypeDisplay = dm.DataType.ToString();
            this.DataType = dm.DataType;
        }


    }
}
