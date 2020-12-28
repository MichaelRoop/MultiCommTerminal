using System.Collections.Generic;

namespace MultiCommData.Net.StorageDataModels {

    /// <summary>One command item to send via terminal</summary>
    public class ScriptItem {

        /// <summary>User friendly name of command</summary>
        public string Display { get; set; } = "NA";

        /// <summary>The actual string sent via the terminal</summary>
        public string Command { get; set; } = "NA";

        public ScriptItem() { }
        public ScriptItem(string name, string command) {
            this.Display = name;
            this.Command = command;
        }

    }


    public static class ScriptItemExtensions {

        public static void AddNew(this List<ScriptItem> list, string name, string cmd) {
            list.Add(new ScriptItem(name, cmd));
        }


    }



}
