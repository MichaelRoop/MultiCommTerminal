using Android.App;
using Android.Content.PM;
using Android.OS;
using Android.Runtime;
using Android.Widget;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.Droid.DependencyInjection;
using MultiCommTerminal.XamarinForms.Droid.PermissionsObjects;
using MultiCommTerminal.XamarinForms.interfaces;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.Droid {
    [Activity(Label = "MultiCommTerminal.XamarinForms", Icon = "@mipmap/icon", Theme = "@style/MainTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize)]
    public class MainActivity : global::Xamarin.Forms.Platform.Android.FormsAppCompatActivity {

        #region Data

        LogHelper logHelper = new LogHelper();
        ClassLog log = new ClassLog("MainActivity");

        #endregion

        #region FormsAppCompatActivity overrides

        protected override void OnCreate(Bundle savedInstanceState) {
            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            base.OnCreate(savedInstanceState);

            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);

            this.SetupLogging();
            this.SetupDependencyInjection();

            DependencyService.Register<ILocationWhileInUsePermission, LocationWhileInUsePermission>();
            DependencyService.Register<IBluetoothPermissions, DroidBluetoothPermissions>();
            DependencyService.Register<ICloseApplication, AndroidCloseApp>();
            LoadApplication(new App(DI.Wrapper));
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }

        #endregion

        #region Private

        /// <summary>Load up the dependency injection system</summary>
        private void SetupDependencyInjection() {
            // Start it here for first load and retrieve stored language
            ErrReport err;
            WrapErr.ToErrReport(out err, 9999, () => {
                DI.Wrapper.CurrentStoredLanguage();
                //Toast.MakeText(ApplicationContext, "DI Initialized", ToastLength.Long);
            });
            if (err.Code != 0) {
                Toast.MakeText(ApplicationContext, err.Msg, ToastLength.Long);
            }
        }


        /// <summary>Setup the log system. Here only debug ouput</summary>
        private void SetupLogging() {
            // Will not handle level events, just the debug outputs
            // TODO - need a build number
            this.logHelper.Setup("1.0.0.0", MsgLevel.Info, true, 5);
        }

        #endregion

    }
}