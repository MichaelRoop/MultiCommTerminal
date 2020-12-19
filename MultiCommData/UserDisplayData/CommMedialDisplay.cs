
using MultiCommData.Net.Enumerations;

namespace MultiCommData.UserDisplayData.Net {

    public class CommMedialDisplay {

        /// <summary>The source string of the icon for injection</summary>
        public string IconSource { get; set; } = "";

        public int IconWidth { get; set; }
        public int IconHeight { get; set; }


        /// <summary>String displayed to user</summary>
        public string Display { get; set; }

        /// <summary>Enumeration of type</summary>
        public CommMedium MediumType { get; set; }

        public CommMedialDisplay() { }

        public CommMedialDisplay(string display, CommMedium mediumType) {
            this.Display = display;
            this.MediumType = mediumType;
        }


        public CommMedialDisplay(string display, string iconSource, CommMedium mediumType) 
            : this(display, mediumType) {
            this.IconSource = iconSource;
        }

    }
}
