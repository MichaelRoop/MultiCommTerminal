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

namespace MultiCommTerminal.XamarinForms.Droid.PermissionsObjects {

    public class LocationWhileInUsePermission : Xamarin.Essentials.Permissions.BasePlatformPermission, ILocationWhileInUsePermission {
        public override (string androidPermission, bool isRuntime)[] 
            RequiredPermissions => new List<(string androidPermission, bool isRuntime)> {
            (Android.Manifest.Permission.AccessFineLocation, true),
            (Android.Manifest.Permission.AccessCoarseLocation, true),
            (Android.Manifest.Permission.AccessWifiState, true),
            (Android.Manifest.Permission.ChangeWifiState, true)
        }.ToArray();

    }



#if GIERUWOEUOIEUOI

https://forums.xamarin.com/discussion/165944/asking-user-for-permission
You can create your own service to ask for permissions.
Use Dependency Service to explicitly ask for permissions. If you do not know what is Dependency Service please check the Microsoft Docs.
For Android

 public void RequestLocationPermission()
    {
            if (ContextCompat.CheckSelfPermission(Application.Context, Manifest.Permission.AccessFineLocation) != Permission.Granted)
            {
                MainActivity.Activity.RequestPermissions(new String[] { Manifest.Permission.AccessFineLocation, Manifest.Permission.AccessCoarseLocation }, MainActivity.LocationPermissionRequestCode);
            }

            LocationManager locationManager = (LocationManager)Application.Context.GetSystemService(Context.LocationService);
            if (!locationManager.IsProviderEnabled(LocationManager.GpsProvider))
            {
                Application.Context.StartActivity(new Intent(Android.Provider.Settings.ActionLocat‌​ionSourceSettings));
            }

    }

#endif

}