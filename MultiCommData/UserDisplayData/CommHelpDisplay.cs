using MultiCommData.Net.Enumerations;

namespace MultiCommData.Net.UserDisplayData {

    public class CommHelpDisplay {

        /// <summary>The source string of the icon for injection</summary>
        public string IconSource { get; set; } = "";

        public int IconWidth { get; set; }
        public int IconHeight { get; set; }


        /// <summary>String displayed to user</summary>
        public string Display { get; set; }

        /// <summary>Enumeration of type</summary>
        public CommMedium HelpType { get; set; }

        public CommHelpDisplay() { }

        public CommHelpDisplay(string display, CommMedium helpType) {
            this.Display = display;
            this.HelpType = helpType;
        }


        public CommHelpDisplay(string display, string iconSource, CommMedium helpType)
            : this(display, helpType) {
            this.IconSource = iconSource;
        }


    }
}
