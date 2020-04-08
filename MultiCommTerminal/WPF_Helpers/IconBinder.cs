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

        public static string Save { get { return Source(UIIcon.Save); } }
        public static string Cancel { get { return Source(UIIcon.Cancel); } }
        public static string Exit { get { return Source(UIIcon.Exit); } }

        public static string Settings { get { return Source(UIIcon.Settings); } }
        public static string Language { get { return Source(UIIcon.Language); } }
        public static string Language_W { get { return Source(UIIcon.LanguageWhite); } }

        #endregion


        /// <summary>Get the source from factory directly if in design mode or from DI</summary>
        /// <param name="code">The icon identifier code </param>
        /// <returns>String with icon path</returns>
        private static string Source(UIIcon code) {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) {
                string result = IconBinder.designFactory.GetIcon(code).IconSource as string;
                return result != null ? result : "";
            }
            return DI.Wrapper.IconSource(code);
        }

    }
}
