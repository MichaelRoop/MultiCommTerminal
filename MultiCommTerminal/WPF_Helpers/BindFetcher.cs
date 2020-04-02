using LanguageFactory.data;
using LanguageFactory.interfaces;
using LanguageFactory.Languages.en;
using LanguageFactory.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.WPF_Helpers {

    /// <summary>
    /// Static wrapper so that the WPF windows can access resources through
    /// Dynamic binding
    /// </summary>
    /// <example>
    /// 
    /// Declare namespace at start of XAML file window info
    /// xmlns:wpfHelper="clr-namespace:MultiCommTerminal.WPF_Helpers" 
    /// 
    /// In the XAML file you can bind text as the following
    /// 
    ///  <TextBlock Text="{Binding Source={x:Static wpfHelper:BindFetcher.Stop}}" />
    /// 
    /// </example>
    public class BindFetcher {

        #region Data

        private static SupportedLanguage designLanguage = null;

        #endregion

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





        //private static string XX { get { return GetTxt(MsgCode.exit); } }


        private static string GetTxt(MsgCode code) {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) {
                // If in VS XMAL designer you can just get the text from the english language module here
                if (designLanguage == null) {
                    designLanguage = new English();
                }
                return designLanguage.GetMsg(code).Display;
            }
            else {
                // You would need to get a singleton of the factory here
                return App.Languages.GetMsgDisplay(code);
            }
        }

    }
}
