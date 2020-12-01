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
    
    public class DroidBluetoothPermissions : Xamarin.Essentials.Permissions.BasePlatformPermission, IBluetoothPermissions {

        public override (string androidPermission, bool isRuntime)[]
            RequiredPermissions => new List<(string androidPermission, bool isRuntime)> {
            (Android.Manifest.Permission.AccessFineLocation, true),
            (Android.Manifest.Permission.AccessCoarseLocation, true),
            (Android.Manifest.Permission.Bluetooth, true),
            (Android.Manifest.Permission.BluetoothAdmin, true)
        }.ToArray();
        // TODO - don't think I need BluetoothPriviledged

    }
}