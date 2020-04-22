using IconFactory.data;
using IconFactory.interfaces;

namespace MultiCommTerminal.WPF_Helpers {

    public class IconFactory : IIconFactory {

        /// <summary>Return info required to render an icon</summary>
        /// <param name="code">Icon code</param>
        /// <returns>The icon info object</returns>
        public IconDataModel GetIcon(UIIcon code) {
            switch (code) {
                case UIIcon.Cancel:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8-close-window-50-noborder.png"), "6");
                case UIIcon.Save:
                    return new IconDataModel(UIIcon.Save, this.AddDir("icons8-checkmark-50.png"), "0");
                case UIIcon.Exit:
                    return new IconDataModel(UIIcon.Exit, this.AddDir("icons8-exit-50.png"), "2");
                case UIIcon.HamburgerMenuWhite:
                    return new IconDataModel(UIIcon.HamburgerMenuWhite, this.AddDir("icons8-menu-white-50.png"), "6");
                case UIIcon.Settings:
                    return new IconDataModel(UIIcon.Settings, this.AddDir("icons8-gear-32.png"), "6");
                case UIIcon.Language:
                    return new IconDataModel(UIIcon.Language, this.AddDir("icons8-language-50.png"), "6");
                case UIIcon.LanguageWhite:
                    return new IconDataModel(UIIcon.LanguageWhite, this.AddDir("icons8-language-white-50.png"), "6");
                case UIIcon.Delete:
                    return new IconDataModel(UIIcon.Delete, this.AddDir("icons8-trash-can-50.png"), "6");
                case UIIcon.Edit:
                    return new IconDataModel(UIIcon.Edit, this.AddDir("icons8-edit-50.png"), "6");
                case UIIcon.View:
                    return new IconDataModel(UIIcon.View, this.AddDir("icons8-eye-50.png"), "6");
                case UIIcon.Add:
                    return new IconDataModel(UIIcon.Add, this.AddDir("icons8-add-50-noborder.png"), "6");
                case UIIcon.Search:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-search-50.png"), "6");
                case UIIcon.Wifi:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-wi-fi-32.png"), "6");
                case UIIcon.BluetoothClassic:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-bluetooth-50.png"), "6");
                case UIIcon.BluetoothClassicWhite:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-bluetooth-white-50.png"), "6");
                case UIIcon.BluetoothLE:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-bluetooth-50.png"), "6");
                case UIIcon.BluetoothLEWhite:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-bluetooth-white-50.png"), "6");
                case UIIcon.Ethernet:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-ethernet-on-50.png"), "6");
                case UIIcon.Connect:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8-quick-mode-on-100.png"), "6");
                case UIIcon.Command:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8-cmd-50.png"), "6");
                case UIIcon.CommandWhite:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8-cmd-white-50.png"), "6");
                case UIIcon.Run:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8-running-24.png"), "6");
                case UIIcon.Terminator:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8-null-symbol-30.png"), "6");
                case UIIcon.TerminatorWhite:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8-null-symbol-white-30.png"), "6");
                case UIIcon.SpinIcon:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8-iphone-spinner-100.png"), "6");
                case UIIcon.SerialPortWhite:
                    return new IconDataModel(UIIcon.SerialPortWhite, this.AddDir("icons8-rs-232-male-white-50.png"), "6");
                default:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8-close-window-50-noborder.png"), "6");
            }

        }

        private string AddDir(string name) {
            return string.Format("/MultiCommTerminal;component/images/icons/{0}", name);
        }





    }
}
