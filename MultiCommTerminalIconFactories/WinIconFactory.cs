﻿using IconFactory.Net.data;
using IconFactory.Net.interfaces;

namespace MultiCommTerminalIconFactories {

    public class WinIconFactory : IIconFactory {

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
                    return new IconDataModel(UIIcon.Settings, this.AddDir("icons8_maintenance.png"), "6");
                case UIIcon.SettingsWhite:
                    return new IconDataModel(UIIcon.Settings, this.AddDir("icons8_maintenance_white.png"), "6");
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
                    return new IconDataModel(UIIcon.Wifi, this.AddDir("icons8-wi-fi-32.png"), "6");
                case UIIcon.WifiWhite:
                    return new IconDataModel(UIIcon.WifiWhite, this.AddDir("icons8_wifi_white.png"), "6");
                case UIIcon.BluetoothClassic:
                    return new IconDataModel(UIIcon.BluetoothClassic, this.AddDir("icons8-bluetooth-50.png"), "6");
                case UIIcon.BluetoothClassicWhite:
                    return new IconDataModel(UIIcon.BluetoothClassicWhite, this.AddDir("icons8-bluetooth-white-50.png"), "6");
                case UIIcon.BluetoothLE:
                    return new IconDataModel(UIIcon.BluetoothLE, this.AddDir("icons8-bluetooth-50.png"), "6");
                case UIIcon.BluetoothLEWhite:
                    return new IconDataModel(UIIcon.BluetoothLEWhite, this.AddDir("icons8-bluetooth-white-50.png"), "6");
                case UIIcon.Ethernet:
                    return new IconDataModel(UIIcon.Ethernet, this.AddDir("icons8-ethernet-on-50.png"), "6");
                case UIIcon.Usb:
                    return new IconDataModel(UIIcon.Usb, this.AddDir("icons8-usb-2-50.png"), "6");
                case UIIcon.UsbWhite:
                    return new IconDataModel(UIIcon.Usb, this.AddDir("icons8-usb-2-50-white.png"), "6");
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
                case UIIcon.Phone:
                    return new IconDataModel(UIIcon.Phone, this.AddDir("icons8-phonelink-no-ring-50.png"), "2");
                case UIIcon.PhoneEmitting:
                    return new IconDataModel(UIIcon.PhoneEmitting, this.AddDir("icons8-phonelink-ring-50.png"), "2");
                case UIIcon.Board:
                    return new IconDataModel(UIIcon.Board, this.AddDir("icons8-arduino-board-50.png"), "2");
                case UIIcon.BoardEmitting:
                    return new IconDataModel(UIIcon.BoardEmitting, this.AddDir("icons8-arduino-board-emitting-50.png"), "2");
                case UIIcon.ArduinoIcon:
                    return new IconDataModel(UIIcon.ArduinoIcon, this.AddDir("icons8-arduino-50.png"), "3");
                case UIIcon.Help:
                    return new IconDataModel(UIIcon.Help, this.AddDir("icons8-help-50.png"), "2");
                case UIIcon.HelpWhite:
                    return new IconDataModel(UIIcon.HelpWhite, this.AddDir("icons8-help-white-50.png"), "1");
                case UIIcon.Configure:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-job-50.png"), "1");
                case UIIcon.Pair:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-link-100.png"), "1");
                case UIIcon.Unpair:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-delete-link-52.png"), "1");
                case UIIcon.Credentials:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-access-50.png"), "1");
                case UIIcon.CredentialsWhite:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-access-50_W.png"), "1");
                case UIIcon.About:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-about-50.png"), "1");
                case UIIcon.AboutWhite:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-about-50_W.png"), "1");
                case UIIcon.Services:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-services-50.png"), "1");
                case UIIcon.ServicesWhite:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-services-52-white.png"), "1");
                case UIIcon.Properties:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-table-properties-64.png"), "1");
                case UIIcon.PropertiesWhite:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-table-properties-64-white.png"), "1");
                case UIIcon.LogIcon:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-index-50.png"), "1");
                case UIIcon.EthernetWhite:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8-ethernet-on-50-white.png"), "1");
                case UIIcon.Code:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_code.png"), "1");
                case UIIcon.CodeWhite:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_code_white.png"), "1");
                case UIIcon.Read:
                    return new IconDataModel(UIIcon.Read, this.AddDir("icons8_openbook.png"), "1");

                default:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8-close-window-50-noborder.png"), "6");

            }
        }



        private string AddDir(string name) {
            // Windows requires path
            return string.Format("/images/icons/{0}", name);
        }

    }
}
