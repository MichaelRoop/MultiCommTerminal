namespace MultiCommData.Net.UserDisplayData {

    public class CommHelpDisplay {

        /// <summary>The source string of the icon for injection</summary>
        public string IconSource { get; set; } = "";

        public int IconWidth { get; set; }
        public int IconHeight { get; set; }


        /// <summary>String displayed to user</summary>
        public string Display { get; set; }

        /// <summary>Enumeration of type</summary>
        public CommHelpType HelpType { get; set; }

        public CommHelpDisplay() { }

        public CommHelpDisplay(string display, CommHelpType helpType) {
            this.Display = display;
            this.HelpType = helpType;
        }


        public CommHelpDisplay(string display, string iconSource, CommHelpType helpType)
            : this(display, helpType) {
            this.IconSource = iconSource;
        }


    }
}
