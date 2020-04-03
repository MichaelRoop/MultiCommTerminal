using MultiCommTerminal.Data;

namespace MultiCommData.UserDisplayData {

    public class MenuItemDataModel {
        public MenuCode Code { get; set; } = MenuCode.Language;
        public string Display { get; set; } = "NA";

        public object IconSource { get; set; } = new object();


        public string Padding { get; set; }


        public MenuItemDataModel(MenuCode code, string display, object iconSource) {
            this.Code = code;
            this.Display = display;
            this.IconSource = iconSource;
            this.Padding = "5,5";
        }

    }
}
