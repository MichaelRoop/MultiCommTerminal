using MultiCommData.Net.UserDisplayData;

namespace MultiCommData.UserDisplayData.Net {


    /// <summary>Pass up medial communication media help information to display </summary>
    public class CommMediumHelp {

        /// <summary>Unique identifier for medium type</summary>
        public CommHelpType Id { get; set; }

        /// <summary>OS Specific source of the icon to display</summary>
        public object Icon { get; set; }

        /// <summary>Help section title</summary>
        public string Title { get; set; }

        /// <summary>Help text</summary>
        public string Text { get; set; }

        /// <summary>Sample code to accompany help</summary>
        public string Code { get; set; }

    }


}
