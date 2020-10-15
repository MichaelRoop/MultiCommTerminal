namespace MultiCommData.Net.StorageDataModels {

    /// <summary>One command item to send via terminal</summary>
    public class ScriptItem {

        /// <summary>User friendly name of command</summary>
        public string Display { get; set; } = "NA";

        /// <summary>The actual string sent via the terminal</summary>
        public string Command { get; set; } = "NA";

    }
}
