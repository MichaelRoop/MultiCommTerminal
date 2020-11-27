using MultiCommTerminal.XamarinForms.interfaces;
using System.Collections.Generic;

namespace MultiCommTerminal.XamarinForms.Droid.PermissionsObjects {

    /// <summary>Android implementation to check and request WIFI permissions</summary>
    public class LocationWhileInUsePermission : Xamarin.Essentials.Permissions.BasePlatformPermission, ILocationWhileInUsePermission {

        public override (string androidPermission, bool isRuntime)[] 
            RequiredPermissions => new List<(string androidPermission, bool isRuntime)> {
            (Android.Manifest.Permission.AccessFineLocation, true),
            (Android.Manifest.Permission.AccessCoarseLocation, true),
            (Android.Manifest.Permission.AccessWifiState, true),
            (Android.Manifest.Permission.ChangeWifiState, true)
        }.ToArray();

    }

}