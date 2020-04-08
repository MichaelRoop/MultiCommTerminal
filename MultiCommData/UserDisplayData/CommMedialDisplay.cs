
namespace MultiCommData.UserDisplayData.Net {

    public class CommMedialDisplay {

        /// <summary>String displayed to user</summary>
        public string Display { get; set; }

        /// <summary>Enumeration of type</summary>
        public CommMediumType MediumType { get; set; }

        public CommMedialDisplay(string display, CommMediumType mediumType) {
            this.Display = display;
            this.MediumType = mediumType;
        }

    }
}
