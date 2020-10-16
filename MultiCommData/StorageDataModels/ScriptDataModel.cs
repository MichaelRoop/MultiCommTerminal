using System;
using System.Collections.Generic;

namespace MultiCommData.Net.StorageDataModels {

    /// <summary>Storage data for one script with multiple commands</summary>
    public class ScriptDataModel {

        #region Properties

        /// <summary>The UID for storage identification</summary>
        public string UId { get; set; } = "";

        /// <summary>User friendly name</summary>
        public string Display { get; set; } = "NA";

        /// <summary>List of scripts which contain commands</summary>
        public List<ScriptItem> Items { get; set; } = new List<ScriptItem>();

        #endregion

        #region Constructors

        public ScriptDataModel() {
            this.UId = Guid.NewGuid().ToString();
        }


        public ScriptDataModel(List<ScriptItem> scripts) : this() {
            this.Items = new List<ScriptItem>();
            foreach(ScriptItem script in scripts) {
                this.Items.Add(script);
            }
        }

        #endregion
    }
}
