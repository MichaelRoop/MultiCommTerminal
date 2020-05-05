﻿using IconFactory.data;
using IconFactory.interfaces;
using MultiCommTerminal.DependencyInjection;
using System.Windows;

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
        public static string HamburgMenu_W { get { return Source(UIIcon.HamburgerMenuWhite); } }
        public static string Settings { get { return Source(UIIcon.Settings); } }
        public static string Language { get { return Source(UIIcon.Language); } }
        public static string Language_W { get { return Source(UIIcon.LanguageWhite); } }
        public static string Delete { get { return Source(UIIcon.Delete); } }
        public static string Edit { get { return Source(UIIcon.Edit); } }
        public static string View { get { return Source(UIIcon.View); } }
        public static string Add { get { return Source(UIIcon.Add); } }
        public static string Search { get { return Source(UIIcon.Search); } }
        public static string Connect { get { return Source(UIIcon.Connect); } }
        public static string Command { get { return Source(UIIcon.Command); } }
        public static string Command_W { get { return Source(UIIcon.CommandWhite); } }
        public static string Send { get { return Source(UIIcon.Run); } }
        public static string Terminator { get { return Source(UIIcon.Terminator); } }
        public static string Terminator_W { get { return Source(UIIcon.TerminatorWhite); } }
        public static string SpinIcon { get { return Source(UIIcon.SpinIcon); } }
        public static string SerialPort_W { get { return Source(UIIcon.SerialPortWhite); } }
        public static string BluetoothClassic { get { return Source(UIIcon.BluetoothClassic); } }
        public static string BluetoothClassic_W { get { return Source(UIIcon.BluetoothClassicWhite); } }
        public static string BluetoothLE { get { return Source(UIIcon.BluetoothLE); } }
        public static string BluetoothLE_W { get { return Source(UIIcon.BluetoothLEWhite); } }
        public static string Phone { get { return Source(UIIcon.Phone); } }
        public static string PhoneEmitting { get { return Source(UIIcon.PhoneEmitting); } }
        public static string Board { get { return Source(UIIcon.Board); } }
        public static string BoardEmitting { get { return Source(UIIcon.BoardEmitting); } }

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
