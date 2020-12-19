using LanguageFactory.Net.data;

namespace MultiCommData.Net.StorageDataModels {

    public class SettingItems {

        /// <summary>Currently selected language</summary>
        public LangCode Language { get; set; } = LangCode.English;

        /// <summary>Just for user if they look in the JSON file</summary>
        public string LanguageName { get; set; } = LangCode.English.ToString();

        /// <summary>Currently selected terminator</summary>
        public TerminatorDataModel CurrentTerminator { get; set; } = new TerminatorDataModel();

        /// <summary>Currently selected command script</summary>
        public ScriptDataModel CurrentScript { get; set; } = new ScriptDataModel();


        // Added to end because of storage of existing
        public TerminatorDataModel CurrentTerminatorBT { get; set; } = null;
        public TerminatorDataModel CurrentTerminatorBLE { get; set; } = null;
        public TerminatorDataModel CurrentTerminatorEthernet { get; set; } = null;
        public TerminatorDataModel CurrentTerminatorUSB { get; set; } = null;
        public TerminatorDataModel CurrentTerminatorWIFI { get; set; } = null;


        public ScriptDataModel CurrentScriptBT { get; set; } = null;
        public ScriptDataModel CurrentScriptBLE { get; set; } = null;
        public ScriptDataModel CurrentScriptEthernet { get; set; } = null;
        public ScriptDataModel CurrentScriptUSB { get; set; } = null;
        public ScriptDataModel CurrentScriptWIFI { get; set; } = null;



    }
}
