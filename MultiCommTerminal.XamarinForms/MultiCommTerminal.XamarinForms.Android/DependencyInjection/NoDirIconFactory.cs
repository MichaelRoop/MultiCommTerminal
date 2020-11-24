using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using IconFactory.Net.data;
using IconFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiCommTerminal.XamarinForms.Droid.DependencyInjection {

    public class NoDirIconFactory : IIconFactory {
        public IconDataModel GetIcon(UIIcon code) {
            switch (code) {
                case UIIcon.Language:
                    return new IconDataModel(UIIcon.Language, this.AddDir("icons8_language_50.png"), "6");
                case UIIcon.LanguageWhite:
                    return new IconDataModel(UIIcon.LanguageWhite, this.AddDir("icons8_language_white_50.png"), "6");
                case UIIcon.Cancel:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8_close_window_50_noborder.png"), "6");
                case UIIcon.BluetoothClassic:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8_bluetooth_50.png"), "6");
                case UIIcon.Save:
                    return new IconDataModel(UIIcon.Save, this.AddDir("icons8_checkmark_50.png"), "0");
                case UIIcon.Delete:
                    return new IconDataModel(UIIcon.Delete, this.AddDir("icons8_trash_can_50.png"), "6");
                case UIIcon.Edit:
                    return new IconDataModel(UIIcon.Edit, this.AddDir("icons8_edit_50.png"), "6");
                case UIIcon.Add:
                    return new IconDataModel(UIIcon.Add, this.AddDir("icons8_add_50_noborder.png"), "6");
                case UIIcon.Terminator:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8_null_symbol_30.png"), "6");
                case UIIcon.TerminatorWhite:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8_null_symbol_white_30.png"), "6");
                case UIIcon.Connect:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8_quick_mode_on_100.png"), "6");
                case UIIcon.Pair:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_link_100.png"), "1");
                case UIIcon.Unpair:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_delete_link_52.png"), "1");
                case UIIcon.Credentials:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_access_50.png"), "1");
                case UIIcon.CredentialsWhite:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_access_50_W.png"), "1");
                case UIIcon.About:
                    return new IconDataModel(UIIcon.Configure, this.AddDir("icons8_about_50.png"), "1");
                case UIIcon.Wifi:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8_wi_fi_32.png"), "6");
                case UIIcon.Search:
                    return new IconDataModel(UIIcon.Search, this.AddDir("icons8_search_50.png"), "6");
                case UIIcon.Run:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8_running_24.png"), "6");
                case UIIcon.Command:
                    return new IconDataModel(UIIcon.Connect, this.AddDir("icons8_cmd.png"), "6");


                default:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8_close_window_50_noborder.png"), "6");
            }
        }


        private string AddDir(string name) {
            // TODO _ look at moving win to a common area and have virtual AddDir
            // Just so we can bring entries over from Win
            return name;

        }
    }
}