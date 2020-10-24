namespace MultiCommWrapper.Net.DataModels {

    /// <summary>Generic key value display data model</summary>
    public class KeyValuePropertyDisplay {
        public string Key { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public KeyValuePropertyDisplay() { }
        public KeyValuePropertyDisplay(string key, string value) {
            this.Key = key;
            this.Value = value;
        }
    }
}
