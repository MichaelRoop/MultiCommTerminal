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
using MultiCommWrapper.Net.interfaces;
using System;
using System.Diagnostics;
using System.Threading.Tasks;
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
            // Move the init of the logging and DI init to start so it overlaps with other things being setup here
            //Stopwatch sw = new Stopwatch();
            //long loging = 0;
            //sw.Start();
            this.SetupLogAndDI();
            //long setupDi = sw.ElapsedMilliseconds;
            //sw.Stop();

            TabLayoutResource = Resource.Layout.Tabbar;
            ToolbarResource = Resource.Layout.Toolbar;

            //https://stackoverflow.com/questions/56088208/unsupported-api-warnings-in-google-play-store
#if DEBUG
            // To detect where the non conforming calls are being made
            StrictMode.SetThreadPolicy(
                new StrictMode.ThreadPolicy.Builder()
                    .DetectAll()
                    .PenaltyLog()
                    .Build());

            StrictMode.SetVmPolicy(
               new StrictMode.VmPolicy.Builder()
                    .DetectNonSdkApiUsage()
                    .PenaltyLog()
                    .Build());
#endif // DEBUG



            //sw.Restart();
            base.OnCreate(savedInstanceState);
            //sw.Stop();
            //long baseInit = sw.ElapsedMilliseconds;

            //sw.Restart();
            Rg.Plugins.Popup.Popup.Init(this);
            //sw.Stop();
            //long popuUpInit = sw.ElapsedMilliseconds;

            //sw.Restart();
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            //sw.Stop();
            //long essInit = sw.ElapsedMilliseconds;

            //sw.Restart();
            global::Xamarin.Forms.Forms.Init(this, savedInstanceState);
            //sw.Stop();
            //long formsInit = sw.ElapsedMilliseconds;

            //sw.Restart();
            //this.SetupLogging();
            //sw.Stop();
            //long loging = sw.ElapsedMilliseconds;
            //sw.Restart();


            //this.SetupDependencyInjection();
            //sw.Stop();
            //long setupDi = sw.ElapsedMilliseconds;
            //sw.Restart();



            //sw.Restart();
            DependencyService.Register<ILocationWhileInUsePermission, LocationWhileInUsePermission>();
            DependencyService.Register<IBluetoothPermissions, DroidBluetoothPermissions>();
            DependencyService.Register<ICloseApplication, AndroidCloseApp>();
            //sw.Stop();
            //long reg = sw.ElapsedMilliseconds;

            //sw.Restart();
            LoadApplication(new App(this.DiCreator));
            //sw.Stop();
            //long loadApp = sw.ElapsedMilliseconds;

            //Log.Info("MainActivity", "OnCreate", () => string.Format("Base Create:{0}", baseInit));
            //Log.Info("MainActivity", "OnCreate", () => string.Format("Rg Popup Init:{0}", popuUpInit));
            //Log.Info("MainActivity", "OnCreate", () => string.Format("Essentials Init:{0}", essInit));
            //Log.Info("MainActivity", "OnCreate", () => string.Format("Forms Init:{0}", formsInit));
            //Log.Info("MainActivity", "OnCreate", () => string.Format("Setup Logging:{0}", loging));
            //Log.Info("MainActivity", "OnCreate", () => string.Format("Setup DI:{0}", setupDi));
            //Log.Info("MainActivity", "OnCreate", () => string.Format("Register permissions:{0}", reg));
            //Log.Info("MainActivity", "OnCreate", () => string.Format("Load Application:{0}", loadApp));
        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        public override void OnBackPressed() {
            if (Rg.Plugins.Popup.Popup.SendBackPressed(base.OnBackPressed)) {

            }
            else {
                // TODO - figure this one out
                // https://github.com/rotorgames/Rg.Plugins.Popup/wiki/Getting-started#android-back-button
                base.OnBackPressed();
            }
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
#if DEBUG
            this.logHelper.Setup("1.0.0.0", MsgLevel.Info, true, 5);
#else
            // Probably not needed since the logger is not started but just to make sure
            // that log messages with ()=> string.Format never execute the function
            Log.SetVerbosity(MsgLevel.Exception);
#endif
        }

        private void SetupLogAndDI() {
            Task.Run(() => {
                this.SetupLogging();
                this.SetupDependencyInjection();
            });
        }

        /// <summary>Function to pass to the App where it can fire up the DI in a thread</summary>
        /// <returns>The DI CommWrapper instance</returns>
        private ICommWrapper DiCreator() {
            return DI.Wrapper;
        }


        #endregion

    }
}