using MultiCommData.Net.StorageDataModels;

namespace MultiCommWrapper.Net.Helpers {

    /// <summary>
    /// Holds a ScriptItem to be stored in memory between mobile
    /// screens which can only route parameters as strings where
    /// we lose instance on change
    /// </summary>
    public class ScratchScriptCommand {
        public ScratchMode Mode { get; set; } = ScratchMode.Edit;
        public ScriptItem Item { get; set; } = new ScriptItem();
    }
}
