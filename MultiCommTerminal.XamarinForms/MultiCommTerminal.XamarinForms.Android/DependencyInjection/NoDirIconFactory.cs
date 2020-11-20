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
                case UIIcon.Cancel:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8_close_window_50_noborder.png"), "6");
                default:
                    return new IconDataModel(UIIcon.Cancel, this.AddDir("icons8_close_window_50_noborder.png"), "6");
            }
        }


        private string AddDir(string name) {
            // TODO - look at moving win to a common area and have virtual AddDir
            // Just so we can bring entries over from Win
            return name;

        }
    }
}