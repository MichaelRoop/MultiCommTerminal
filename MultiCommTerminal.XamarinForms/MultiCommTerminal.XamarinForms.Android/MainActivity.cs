using System;

using Android.App;
using Android.Content.PM;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Android.OS;
using MultiCommTerminal.XamarinForms;
using MultiCommTerminal.XamarinForms.Droid.DependencyInjection;
using LogUtils.Net;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using Xamarin.Essentials;
using System.Threading.Tasks;
using MultiCommTerminal.XamarinForms.Droid.PermissionsObjects;
using AndroidX.Core.App;
using Java.Interop;
using Android;
using Plugin.Permissions;
using Xamarin.Forms;
using MultiCommTerminal.XamarinForms.interfaces;

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
            this.log.Error(9191, "SAMPLE ERROR");


            //Task.Run(async () => {
            //    await this.CheckAndRequestNetStatePermission();
            //    await this.CheckAndRequestLocationStatePermission();
            //});



            //if (!(CheckPermissionGranted(Android.Manifest.Permission.AccessCoarseLocation) &&
            //           CheckPermissionGranted(Android.Manifest.Permission.AccessFineLocation))) {
            //    RequestLocationPermission();
            //}
            //else {
            //    InitializeLocationManager();
            //}


            //this.ChkPermissions2();


            DependencyService.Register<ILocationWhileInUsePermission, LocationWhileInUsePermission>();
            DependencyService.Register<ICloseApplication, AndroidCloseApp>();

            LoadApplication(new App(DI.Wrapper));


            //this.ChkPermissions2();
            //Task.Run(async () => await this.ChkPermissions3());


        }


        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults) {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            this.log.Error(88888, "XXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXXX", requestCode.ToString());

            //PermissionsImplementation.Current.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }




        //private void RequestLocationPermission() {
        //    if (ActivityCompat.ShouldShowRequestPermissionRationale(this, Manifest.Permission.AccessFineLocation)) {
        //        // Provide an additional rationale to the user if the permission was not granted
        //        // and the user would benefit from additional context for the use of the permission.
        //        // For example if the user has previously denied the permission.
        //        ActivityCompat.RequestPermissions(this, PermissionsLocation, REQUEST_LOCATION);

        //    }
        //    else {
        //        // Camera permission has not been granted yet. Request it directly.
        //        ActivityCompat.RequestPermissions(this, PermissionsLocation, REQUEST_LOCATION);
        //    }
        //}


        //[Export]
        //public bool CheckPermissionGranted(string Permissions) {
        //    // Check if the permission is already available.
        //    if (ActivityCompat.CheckSelfPermission(this, Permissions) != Permission.Granted) {
        //        return false;
        //    }
        //    else {
        //        return true;
        //    }
        //}




        protected async Task ChkPermissions3() {
            try {
                //Task<PermissionStatus> status;
                //var status = Permissions.CheckStatusAsync<Permissions.StorageRead>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.StorageRead>();
                //}

                //status = Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.StorageWrite>();
                //}

                var status = await Permissions.CheckStatusAsync<Permissions.NetworkState>();
                if (status != PermissionStatus.Granted) {
                    status = await Permissions.RequestAsync<Permissions.NetworkState>();
                }

                status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) {
                    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status != PermissionStatus.Granted) {
                        // Need message to tell them to turn it on to allow WIFI to scan
                    }
                }

                status = await Permissions.RequestAsync<WifiStateChangePermission>();
                if (status != PermissionStatus.Granted) {
                    status = await Permissions.RequestAsync<WifiStateChangePermission>();
                    if (status != PermissionStatus.Granted) {
                        // Need message to tell them to turn it on to allow WIFI to scan
                    }
                }


                //status = Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.StorageWrite>();
                //}


                //WifiStateChangePermission



                //status = Permissions.CheckStatusAsync<Permissions..NetworkState>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.NetworkState>();
                //}



                //status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                //if (status != PermissionStatus.Granted) {
                //    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                //}
            }
            catch (Exception e) {
                this.log.Exception(55555, "", e);
            }


        }






        //https://docs.microsoft.com/en-us/xamarin/essentials/permissions?tabs=android
        protected void ChkPermissions2() {
            try {
                Task<PermissionStatus> status;
                //var status = Permissions.CheckStatusAsync<Permissions.StorageRead>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.StorageRead>();
                //}

                //status = Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.StorageWrite>();
                //}

                status = Permissions.CheckStatusAsync<Permissions.NetworkState>();
                if (status.Result != PermissionStatus.Granted) {
                    status = Permissions.RequestAsync<Permissions.NetworkState>();
                }

                status = Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                if (status.Result != PermissionStatus.Granted) {
                    status = Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                    if (status.Result != PermissionStatus.Granted) {
                        // Need message to tell them to turn it on to allow WIFI to scan
                    }
                }

                status = Permissions.RequestAsync<WifiStateChangePermission>();
                if (status.Result != PermissionStatus.Granted) {
                    status = Permissions.RequestAsync<WifiStateChangePermission>();
                    if (status.Result != PermissionStatus.Granted) {
                        // Need message to tell them to turn it on to allow WIFI to scan
                    }
                }


                //status = Permissions.CheckStatusAsync<Permissions.StorageWrite>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.StorageWrite>();
                //}


                //WifiStateChangePermission



                //status = Permissions.CheckStatusAsync<Permissions..NetworkState>();
                //if (status.Result != PermissionStatus.Granted) {
                //    status = Permissions.RequestAsync<Permissions.NetworkState>();
                //}



                //status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
                //if (status != PermissionStatus.Granted) {
                //    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                //}
            }
            catch (Exception e) {
                this.log.Exception(55555, "", e);
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
            this.logHelper.Setup("1.0.0.0", MsgLevel.Info, true, 5);
        }

        #endregion


        #region Permissions

        // List wifi permissions
        //https://www.hellojava.com/a/65384.html
        // StartScan - ACCESS_FINE_LOCATION or ACCESS_COARSE_LOCATION, CHANGE_WIFI_STATE, location services enabled
        // GetScanResults - ACCESS_FINE_LOCATION or ACCESS_COARSE_LOCATION, ACCESS_WIFI_STATE, location services enabled



        // Example above says wifi state
        public async Task<PermissionStatus> CheckAndRequestNetStatePermission() {
            //var status = await Permissions.CheckStatusAsync<Permissions.NetworkState>();

            //if (status != PermissionStatus.Granted) {
            //    if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS) {
            //        // Prompt the user to turn on in settings
            //        // On iOS once a permission has been denied it may not be requested again from the application
            //        return status;
            //    }
            //    //if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>()) {
            //    //    // Prompt the user with additional information as to why the permission is needed
            //    //}
            //    status = await Permissions.RequestAsync<Permissions.NetworkState>();
            //}

            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.NetworkState>();
            if (status != PermissionStatus.Granted) {
                // this is where you could check if IOS
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) {
                    // Need message to tell them to turn it on to allow WIFI to scan
                }
            }

            status = await Permissions.RequestAsync<WifiStateChangePermission>();
            if (status != PermissionStatus.Granted) {
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) {
                    // Need message to tell them to turn it on to allow WIFI to scan
                }
            }

            return status;
        }


        public async Task<PermissionStatus> CheckAndRequestLocationStatePermission() {
            //var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            //if (status != PermissionStatus.Granted) {
            //    if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS) {
            //        // Prompt the user to turn on in settings
            //        // On iOS once a permission has been denied it may not be requested again from the application
            //        return status;
            //    }
            //    //if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>()) {
            //    //    // Prompt the user with additional information as to why the permission is needed
            //    //}
            //    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            //}
            //return status;

            
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            if (status != PermissionStatus.Granted) {
                // this is where you could check if IOS
                status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
                if (status != PermissionStatus.Granted) {
                    // Need message to tell them to turn it on to allow WIFI to scan
                }
            }
            return status;
        }


        #endregion

    }
}