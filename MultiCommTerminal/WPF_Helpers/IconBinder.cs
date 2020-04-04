using IconFactory.data;
using IconFactory.interfaces;
using MultiCommTerminal.DependencyInjection;

namespace MultiCommTerminal.WPF_Helpers {

    /// <summary>Provide static functions that XAML can use to bind Image source to Image</summary>
    /// <example>
    /// Declare namespace at start of XAML file window info
    /// xmlns:wpfHelper="clr-namespace:MultiCommTerminal.WPF_Helpers" 
    /// 
    /// In the XAML file you can bind text as the following
    /// 
    ///  <Image ImageSource="{Binding Source={x:Static wpfHelper:IconBinder.Stop}}" />
    /// 
    /// The Stop will correspond to a static function in the static IconBinder class
    /// </example>
    public static class IconBinder {

        /// <summary>Used in VS XAML design when the DI cannot be loaded</summary>
        private static IIconFactory designFactory = new IconFactory();

        #region Icon paths for UI

        public static string Save { get { return GetSource(UIIcon.Save); } }
        public static string Cancel { get { return GetSource(UIIcon.Cancel); } }
        public static string Exit { get { return GetSource(UIIcon.Exit); } }

        public static string Settings { get { return GetSource(UIIcon.Settings); } }
        public static string Language { get { return GetSource(UIIcon.Language); } }
        public static string Language_W { get { return GetSource(UIIcon.LanguageWhite); } }

        #endregion

        #region Get Icon Data Model

        public static IconDataModel SaveDM { get { return GetDM(UIIcon.Save); } }
        public static IconDataModel CancelDM { get { return GetDM(UIIcon.Cancel); } }
        public static IconDataModel ExitDM { get { return GetDM(UIIcon.Exit); } }

        public static IconDataModel SettingsDM { get { return GetDM(UIIcon.Settings); } }
        public static IconDataModel LanguageDM { get { return GetDM(UIIcon.Language); } }
        public static IconDataModel LanguageDM_W { get { return GetDM(UIIcon.LanguageWhite); } }

        #endregion


        private static string GetSource(UIIcon code) {
            return GetDM(code).IconSource as string;
        }


        private static IconDataModel GetDM(UIIcon code) {
            return IconBinder.GetFactory().GetIcon(code);
        }


        private static IIconFactory GetFactory() {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) {
                return IconBinder.designFactory;
            }
            return DI.GetIconFactory();
        }

    }
}
