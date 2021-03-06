﻿using IconFactory.Net.data;
using IconFactory.Net.interfaces;

namespace MultiCommTerminalIconFactories {

    /// <summary>Icon factory that returns only the icon name</summary>
    /// <remarks> This is how Xamarin Android requires its resources</remarks>
    public class NoDirIconFactory : IIconFactory {
        public IconDataModel GetIcon(UIIcon code) {
            switch (code) {
                case UIIcon.Language:
                    return new IconDataModel(UIIcon.Language, this.AddDir("icons8_language.png"), "6");
                case UIIcon.LanguageWhite:
                    return new IconDataModel(UIIcon.LanguageWhite, this.AddDir("icons8_language_white.png"), "6");
                case UIIcon.Cancel:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8_close_window_noborder.png"), "6");
                case UIIcon.CancelSmall:
                    return new IconDataModel(UIIcon.CancelSmall, this.AddDir("icons8_cancelSmall.png"), "6");
                case UIIcon.BluetoothClassic:
                    return new IconDataModel(UIIcon.BluetoothClassic, this.AddDir("icons8_bluetooth.png"), "6");
                case UIIcon.BluetoothClassicWhite:
                    return new IconDataModel(UIIcon.BluetoothClassicWhite, this.AddDir("icons8_bluetooth_white.png"), "6");
                case UIIcon.Save:
                    return new IconDataModel(UIIcon.Save, this.AddDir("icons8_checkmark.png"), "0");
                case UIIcon.SaveSmall:
                    return new IconDataModel(UIIcon.SaveSmall, this.AddDir("icons8_checkmarkSmall.png"), "0");
                case UIIcon.Delete:
                    return new IconDataModel(UIIcon.Delete, this.AddDir("icons8_trash_can.png"), "6");
                case UIIcon.Edit:
                    return new IconDataModel(UIIcon.Edit, this.AddDir("icons8_edit.png"), "6");
                case UIIcon.Add:
                    return new IconDataModel(UIIcon.Add, this.AddDir("icons8_add_noborder.png"), "6");
                case UIIcon.Terminator:
                    return new IconDataModel(UIIcon.Terminator, this.AddDir("icons8_null_symbol.png"), "6");
                case UIIcon.TerminatorWhite:
                    return new IconDataModel(UIIcon.TerminatorWhite, this.AddDir("icons8_null_symbol_white.png"), "6");
                case UIIcon.Connect:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8_connected.png"), "6");
                case UIIcon.Disconnect:
                    return new IconDataModel(UIIcon.Disconnect, this.AddDir("icons8_disconnected.png"), "6");
                case UIIcon.Pair:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_link.png"), "1");
                case UIIcon.PairWhite:
                    return new IconDataModel(UIIcon.PairWhite, this.AddDir("icons8_link_white.png"), "1");
                case UIIcon.Unpair:
                    return new IconDataModel(UIIcon.Unpair, this.AddDir("icons8_delete_link.png"), "1");
                case UIIcon.Credentials:
                    return new IconDataModel(UIIcon.Credentials, this.AddDir("icons8_access.png"), "1");
                case UIIcon.CredentialsWhite:
                    return new IconDataModel(UIIcon.CredentialsWhite, this.AddDir("icons8_access_W.png"), "1");
                case UIIcon.About:
                    return new IconDataModel(UIIcon.About, this.AddDir("icons8_about.png"), "1");
                case UIIcon.AboutWhite:
                    return new IconDataModel(UIIcon.AboutWhite, this.AddDir("icons8_about_white.png"), "1");
                case UIIcon.Wifi:
                    return new IconDataModel(UIIcon.Wifi, this.AddDir("icons8_wifi.png"), "6");
                case UIIcon.WifiWhite:
                    return new IconDataModel(UIIcon.WifiWhite, this.AddDir("icons8_wifi_white.png"), "6");
                case UIIcon.Search:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8_search.png"), "6");
                case UIIcon.Run:
                    return new IconDataModel(UIIcon.Run, this.AddDir("icons8_running.png"), "6");
                case UIIcon.Command:
                    return new IconDataModel(UIIcon.Command, this.AddDir("icons8_cmd.png"), "6");
                case UIIcon.CommandWhite:
                    return new IconDataModel(UIIcon.CommandWhite, this.AddDir("icons8_cmd_white.png"), "6");
                case UIIcon.Code:
                    return new IconDataModel(UIIcon.CommandWhite, this.AddDir("icons8_code.png"), "6");
                case UIIcon.CodeWhite:
                    return new IconDataModel(UIIcon.CommandWhite, this.AddDir("icons8_code_white.png"), "6");
                case UIIcon.OpenBook:
                    return new IconDataModel(UIIcon.OpenBook, this.AddDir("icons8_openbook.png"), "6");
                case UIIcon.BackDelete:
                    return new IconDataModel(UIIcon.BackDelete, this.AddDir("icons8_clear_symbol.png"), "6");

                default:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8_close_window_noborder.png"), "6");
            }
        }


        /// <summary>Override if adding path or prefix</summary>
        /// <param name="name">The icon name</param>
        /// <returns>The concatenated string. Here with only icon name</returns>
        protected virtual string AddDir(string name) {
            return name;

        }
    }
}
