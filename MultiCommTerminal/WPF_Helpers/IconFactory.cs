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
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("close.png"), "6");
                case UIIcon.Save:
                    return new IconDataModel(UIIcon.Save, this.AddDir("check-1.png"), "6");
                case UIIcon.Exit:
                    return new IconDataModel(UIIcon.Exit, this.AddDir("logout-1.png"), "6");
                case UIIcon.Settings:
                    return new IconDataModel(UIIcon.Settings, this.AddDir("cog.png"), "6");
                case UIIcon.Language:
                    return new IconDataModel(UIIcon.Language, this.AddDir("language.png"), "6");
                case UIIcon.LanguageWhite:
                    return new IconDataModel(UIIcon.LanguageWhite, this.AddDir("language-white.png"), "6");
                case UIIcon.Delete:
                    return new IconDataModel(UIIcon.Delete, this.AddDir("bin-2.png"), "6");
                case UIIcon.Edit:
                    return new IconDataModel(UIIcon.Edit, this.AddDir("pencil.png"), "6");
                case UIIcon.View:
                    return new IconDataModel(UIIcon.View, this.AddDir("view.png"), "6");
                case UIIcon.Add:
                    return new IconDataModel(UIIcon.Add, this.AddDir("add.png"), "6");
                case UIIcon.Search:
                    return new IconDataModel(UIIcon.Search, this.AddDir("search.png"), "6");
                case UIIcon.Wifi:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-wi-fi-32.png"), "6");
                case UIIcon.BluetoothClassic:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-bluetooth-50.png"), "6");
                case UIIcon.BluetoothLE:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8-bluetooth-50.png"), "6");
                case UIIcon.Ethernet:
                    return new IconDataModel(UIIcon.Search, this.AddDir("hierarchy-5.png"), "6");
                case UIIcon.Connect:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("flash-1.png"), "6");
                default:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("close.png"), "6");
            }

        }

        private string AddDir(string name) {
            return string.Format("/MultiCommTerminal;component/images/icons/{0}", name);
        }





    }
}
