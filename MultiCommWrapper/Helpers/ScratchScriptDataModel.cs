using MultiCommData.Net.StorageDataModels;

namespace MultiCommWrapper.Net.Helpers {

    /// <summary>
    /// To pass around a script set in memory to different
    /// mobile screens because they can only route a string
    /// representation. Loses instance
    /// </summary>
    public class ScratchScriptDataModel {

        public ScratchMode Mode { get; set; } = ScratchMode.Edit;
        public ScriptDataModel ScriptSet { get; set; } = new ScriptDataModel();

    }
}
