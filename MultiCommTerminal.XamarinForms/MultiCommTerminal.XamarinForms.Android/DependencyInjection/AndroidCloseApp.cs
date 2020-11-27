
using MultiCommTerminal.XamarinForms.interfaces;

namespace MultiCommTerminal.XamarinForms.Droid.DependencyInjection {

    /// <summary>Android implementation of the close app service interface</summary>
    public class AndroidCloseApp : ICloseApplication {

        public void CloseApp() {
            // https://forums.xamarin.com/discussion/100564/how-to-close-android-app
            Android.OS.Process.KillProcess(Android.OS.Process.MyPid());
        }

    }
}