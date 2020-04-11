using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommData.Net.StorageDataModels {
    
    public class ScriptInfoDataModel {
        public string Display { get; set; } = "** NA **";


        public ScriptInfoDataModel() {

        }

        public ScriptInfoDataModel(string display) {
            this.Display = display;
        }


    }
}
