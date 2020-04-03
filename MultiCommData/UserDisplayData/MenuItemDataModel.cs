using MultiCommTerminal.Data;

namespace MultiCommData.UserDisplayData {

    public class MenuItemDataModel {
        public MenuCode Code { get; set; } = MenuCode.Language;
        public string Display { get; set; } = "NA";

        public MenuItemDataModel(MenuCode code, string display) {
            this.Code = code;
            this.Display = display;
        }

    }
}
