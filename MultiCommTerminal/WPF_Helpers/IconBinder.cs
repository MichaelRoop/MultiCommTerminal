﻿using IconFactory.Net.data;
using IconFactory.Net.interfaces;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Media.Imaging;

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
        public static string ArduinoIcon { get { return Source(UIIcon.ArduinoIcon); } }
        public static string Help { get { return Source(UIIcon.Help); } }
        public static string HelpWhite { get { return Source(UIIcon.HelpWhite); } }
        public static string Configure { get { return Source(UIIcon.Configure); } }

        #endregion


        /// <summary>Get the icon corresponding to the Communication Medium Type</summary>
        /// <param name="medium">The medium type</param>
        /// <returns>Icon source string</returns>
        public static string CommMediumSource(this CommMediumType medium) {
            switch (medium) {
                case CommMediumType.Bluetooth: return BluetoothClassic;
                case CommMediumType.BluetoothLE: return BluetoothLE;
                case CommMediumType.Ethernet: return Source(UIIcon.Ethernet);
                case CommMediumType.Wifi: return Source(UIIcon.Wifi);
                default: return Cancel;
            }
        }


        public static string CommMediumSourceWhite(this CommMediumType medium) {
            switch (medium) {
                case CommMediumType.Bluetooth:
                    return BluetoothClassic_W;
                case CommMediumType.BluetoothLE:
                    return BluetoothLE_W;
                case CommMediumType.Ethernet:
                    // TODO need white
                    return Source(UIIcon.Ethernet);
                case CommMediumType.Wifi:
                    return Source(UIIcon.Wifi);
                default:
                    return Cancel;
            }
        }


        /// <summary>Access to the 
        /// 
        /// </summary>
        /// <param name="icon"></param>
        /// <returns></returns>
        public static string PacketSource(this UIIcon icon) {
            return string.Format("pack://application:,,,{0}", IconBinder.Source(icon));
        }


        public static string PacketSource(this CommMediumType medium) {
            return string.Format("pack://application:,,,{0}", IconBinder.CommMediumSource(medium));
        }


        public static BitmapImage ResourceBlackBitmap(this UIIcon icon) {
            return new BitmapImage(new Uri(icon.PacketSource(), UriKind.Absolute));
        }


        public static BitmapImage ResourceWhiteBitmap(this CommMediumType medium) {
            return new BitmapImage(new Uri(medium.PacketSourceWhite(), UriKind.Absolute));
        }

        public static BitmapImage ResourceBlackBitmap(this CommMediumType medium) {
            return new BitmapImage(new Uri(medium.PacketSource(), UriKind.Absolute));
        }


        public static string PacketSourceWhite(this CommMediumType medium) {
           return string.Format("pack://application:,,,{0}", IconBinder.CommMediumSourceWhite(medium));
        }


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
