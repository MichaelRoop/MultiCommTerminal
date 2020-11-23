﻿using IconFactory.Net.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    public static class IconBinder {

        public static string Language { get { return Source(UIIcon.Language); } }
        public static string Language_W { get { return Source(UIIcon.LanguageWhite); } }
        public static string Bluetooth { get { return Source(UIIcon.BluetoothClassic); } }
        public static string Cancel { get { return Source(UIIcon.Cancel); } }
        public static string Save { get { return Source(UIIcon.Save); } }
        public static string Delete { get { return Source(UIIcon.Delete); } }
        public static string Edit { get { return Source(UIIcon.Edit); } }
        public static string Add { get { return Source(UIIcon.Add); } }
        public static string Terminator { get { return Source(UIIcon.Terminator); } }
        public static string Connect { get { return Source(UIIcon.Connect); } }
        public static string Pair { get { return Source(UIIcon.Pair); } }
        public static string Unpair { get { return Source(UIIcon.Unpair); } }
        public static string Credentials { get { return Source(UIIcon.Credentials); } }
        public static string Credentials_W { get { return Source(UIIcon.CredentialsWhite); } }
        public static string About { get { return Source(UIIcon.About); } }
        public static string Wifi { get { return Source(UIIcon.Wifi); } }
        public static string Run { get { return Source(UIIcon.Run); } }
        public static string Discover { get { return Source(UIIcon.Search); } }



        private static string Source(UIIcon code) {
            return App.Wrapper.IconSource(code); 
        } 

    }



}

