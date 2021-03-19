using BluetoothLE.Net.Enumerations;

namespace MultiCommTerminal.NetCore.UserControls.BLE {

    public class BLETypeDisplay {
        public string Display { get; set; } = string.Empty;
        public BLE_DataType DataType { get; set; } = BLE_DataType.UInt_8bit;

        public BLETypeDisplay() { }

        public BLETypeDisplay(BLE_DataType dataType) {
            this.Display = dataType.ToStr();
            this.DataType = dataType;
        }

    }
}
