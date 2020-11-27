using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using MultiCommTerminal.XamarinForms.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
//using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.Droid.DependencyInjection {
    public class AndroidCloseApp : ICloseApplication {
        public void CloseApp() {
            // https://forums.xamarin.com/discussion/100564/how-to-close-android-app
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }
    }
}