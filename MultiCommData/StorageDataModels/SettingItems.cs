using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;

namespace MultiCommData.Net.StorageDataModels {

    public class SettingItems {

        /// <summary>Currently selected language</summary>
        public LangCode Language { get; set; } = LangCode.English;

        /// <summary>Just for user if they look in the JSON file</summary>
        public string LanguageName { get; set; } = LangCode.English.ToString();

        /// <summary>Currently selected terminator</summary>
        public TerminatorDataModel CurrentTerminator { get; set; } = new TerminatorDataModel();

    }
}
