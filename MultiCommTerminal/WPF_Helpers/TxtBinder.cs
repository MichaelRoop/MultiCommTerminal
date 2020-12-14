using LanguageFactory.Net.data;
using LanguageFactory.Net.Languages.en;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.NetCore.DependencyInjection;

namespace MultiCommTerminal.NetCore.WPF_Helpers {

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
        public static string OK { get { return GetTxt(MsgCode.Ok); } }
        public static string Yes { get { return GetTxt(MsgCode.yes); } }
        public static string No { get { return GetTxt(MsgCode.no); } }

        public static string Language { get { return GetTxt(MsgCode.language); } }
        public static string Start { get { return GetTxt(MsgCode.start); } }
        public static string Stop { get { return GetTxt(MsgCode.stop); } }
        public static string Send { get { return GetTxt(MsgCode.send); } }
        public static string Commands { get { return GetTxt(MsgCode.command); } }

        public static string Response { get { return GetTxt(MsgCode.response); } }
        public static string Discover { get { return GetTxt(MsgCode.discover); } }
        public static string Connect { get { return GetTxt(MsgCode.connect); } }
        public static string Disconnect { get { return GetTxt(MsgCode.Disconnect); } }
        public static string Info { get { return GetTxt(MsgCode.info); } }
        public static string Terminators { get { return GetTxt(MsgCode.Terminators); } }
        public static string Name { get { return GetTxt(MsgCode.Name); } }
        public static string EnterName { get { return GetTxt(MsgCode.EnterName); } }
        public static string Continue { get { return GetTxt(MsgCode.Continue); } }
        public  static string Configure { get { return GetTxt(MsgCode.Configure); } }
        public static string PairedDevice { get { return GetTxt(MsgCode.PairedDevices); } }
        public static string Pair { get { return GetTxt(MsgCode.Pair); } }
        public static string UnPair { get { return GetTxt(MsgCode.Unpair); } }

        public static string Password { get { return GetTxt(MsgCode.Password); } }
        public static string HostName { get { return GetTxt(MsgCode.HostName); } }
        public static string NetworkService { get { return GetTxt(MsgCode.NetworkService); } }
        public static string Port { get { return GetTxt(MsgCode.Port); } }

        public static string HostNameIp { get { return string.Format("{0}/IP", HostName); } }
        public static string NetworkServicePort { get { return string.Format("{0}/{1}", NetworkService, Port); } }

        public static string NetworkSecurityKey { get { return GetTxt(MsgCode.NetworkSecurityKey); } }
        public static string Network { get { return GetTxt(MsgCode.Network); } }
        public static string Socket { get { return GetTxt(MsgCode.Socket); } }
        public static string Credentials { get { return GetTxt(MsgCode.Credentials); } }
        public static string Icons { get { return GetTxt(MsgCode.Icons); } }
        public static string About { get { return GetTxt(MsgCode.About); } }
        public static string Author { get { return GetTxt(MsgCode.Author); } }
        public static string Services { get { return GetTxt(MsgCode.Services); } }
        public static string Properties { get { return GetTxt(MsgCode.Properties); } }
        public static string UserManual { get { return GetTxt(MsgCode.UserManual); } }
        public static string Vendor { get { return GetTxt(MsgCode.Vendor); } }
        public static string Product { get { return GetTxt(MsgCode.Product); } }
        public static string Default { get { return GetTxt(MsgCode.Default); } }
        public static string Enabled { get { return GetTxt(MsgCode.Enabled); } }
        public static string BaudRate { get { return GetTxt(MsgCode.BaudRate); } }
        public static string DataBits { get { return GetTxt(MsgCode.DataBits); } }
        public static string StopBits { get { return GetTxt(MsgCode.StopBits); } }
        public static string Parity { get { return GetTxt(MsgCode.Parity); } }
        public static string FlowControl { get { return GetTxt(MsgCode.FlowControl); } }
        public static string Read { get { return GetTxt(MsgCode.Read); } }
        public static string Write { get { return GetTxt(MsgCode.Write); } }
        public static string Timeout { get { return GetTxt(MsgCode.Timeout); } }
        public static string ReadTimeout { get { return GetTxt(MsgCode.ReadTimeout); } }
        public static string WriteTimeout { get { return GetTxt(MsgCode.WriteTimeout); } }
        public static string LogText { get { return GetTxt(MsgCode.Log); } }
        public static string Support { get { return GetTxt(MsgCode.Support); } }
        public static string Ethernet { get { return GetTxt(MsgCode.Ethernet); } }
        public static string Edit { get { return GetTxt(MsgCode.Edit); } }
        public static string Create { get { return GetTxt(MsgCode.Create); } }


        public static string BuildNumber {
            get {
                return App.Build;
            }
        }


        /// <summary>To access the user manual in the browser</summary>
        public static string UserManualUri {
            get {
                return string.Format(@"file:///{0}", DI.Wrapper.UserManualFullFileName);
            }
        }

        public static string SupportlUri {
            get {
                return string.Format(@"mailto:MultiCommTerminal@gmail.com?subject=Multi Comm Terminal Support Question&body=App Build number:{0}", BuildNumber);
            }
        }



        #endregion

        #region Private

        private static string GetTxt(MsgCode code) {
            if (System.ComponentModel.LicenseManager.UsageMode == System.ComponentModel.LicenseUsageMode.Designtime) {
                // If in VS XMAL designer get text from the english language module
                return designLanguage.GetMsg(code).Display;
            }
            // Dependency injector
            return DI.Wrapper.GetText(code);
        }


        #endregion

    }
}
