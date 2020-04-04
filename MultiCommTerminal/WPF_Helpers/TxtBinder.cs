using LanguageFactory.data;
using LanguageFactory.Languages.en;
using LanguageFactory.Messaging;
using MultiCommTerminal.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.WPF_Helpers {

    /// <summary>Provide static functions that XAML can use to bind text to controls</summary>
    /// <example>
    /// Declare namespace at start of XAML file window info
    /// xmlns:wpfHelper="clr-namespace:MultiCommTerminal.WPF_Helpers" 
    /// 
    /// In the XAML file you can bind text as the following
    /// 
    ///  <TextBlock Text="{Binding Source={x:Static wpfHelper:TxtBinder.Stop}}" />
    /// 
    /// The Stop will correspond to a static function in the static TxtBinder class
    /// </example>
    public static class TxtBinder {

        #region Data

        /// <summary>
        /// Language module to use at design time because XAML designer cannot 
        /// access DI injector which is only initialised at runtime
        /// </summary>
        private static SupportedLanguage designLanguage = new English();

        #endregion

        #region Text strings

        public static string Save { get { return GetTxt(MsgCode.save); } }
        public static string Copy { get { return GetTxt(MsgCode.copy); } }
        public static string Select { get { return GetTxt(MsgCode.select); } }
        public static string Cancel { get { return GetTxt(MsgCode.cancel); } }
        public static string Exit { get { return GetTxt(MsgCode.exit); } }

        public static string Language { get { return GetTxt(MsgCode.language); } }
        public static string Start { get { return GetTxt(MsgCode.start); } }
        public static string Stop { get { return GetTxt(MsgCode.stop); } }
        public static string Send { get { return GetTxt(MsgCode.send); } }
        public static string Commands { get { return GetTxt(MsgCode.command); } }

        public static string Response { get { return GetTxt(MsgCode.response); } }
        public static string Discover { get { return GetTxt(MsgCode.discover); } }
        public static string Connect { get { return GetTxt(MsgCode.connect); } }
        public static string Info { get { return GetTxt(MsgCode.info); } }

        #endregion

        #region Private

        private static string GetTxt(MsgCode code) {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) {
                // If in VS XMAL designer get text from the english language module
                return designLanguage.GetMsg(code).Display;
            }
            // Dependency injector
            return DI.GetText(code);
        }


        #endregion

    }
}
