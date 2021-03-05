using BluetoothLE.Net.Enumerations;
using MultiCommData.Net.interfaces;
using System;
using System.Collections.Generic;

namespace MultiCommData.Net.StorageDataModels {

    /// <summary>Storage of multiple BLE commands in one set of same data type</summary>
    public class BLECommandSetDataModel : IDisplayableData, IIndexible {

        /// <summary>The UID for storage identification</summary>
        public string UId { get; set; } = "";

        /// <summary>User friendly name</summary>
        public string Display { get; set; } = "NA";


        /// <summary>
        /// If empty the set will appear for any characteristic selected which 
        /// matches the data type. Otherwise the set will only appear when the
        /// characteristic of this name is selected
        /// </summary>
        public string CharacteristicName { get; set; } = string.Empty;


        /// <summary>The unsigned data type for andy command in the set</summary>
        /// <remarks>
        /// Limited to UInt_8bit, UInt_16bit, UInt_32bit, UInt_64bit
        /// Either used as enumerated or with masking to send multiple 
        /// commands at a time
        /// </remarks>
        public BLE_DataType DataType { get; set; }

        /// <summary>List of scripts containing numeric commands saved as string</summary>
        public List<ScriptItem> Items { get; set; } = new List<ScriptItem>();


        public BLECommandSetDataModel() {
            this.UId = Guid.NewGuid().ToString();
        }


        public BLECommandSetDataModel(List<ScriptItem> scripts) : this() {
            this.Items = new List<ScriptItem>();
            foreach (ScriptItem script in scripts) {
                this.Items.Add(script);
            }
        }

    }
}
