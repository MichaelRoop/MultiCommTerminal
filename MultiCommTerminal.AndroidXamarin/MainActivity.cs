#define USEWIFI
//#define USESOCKET
using Android.App;
using Android.OS;
using Android.Runtime;
using Android.Support.Design.Widget;
using Android.Support.V4.View;
using Android.Support.V4.Widget;
using Android.Support.V7.App;
using Android.Views;
using Android.Widget;
using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.DataModels;
using LogUtils.Net;
using MultiCommTerminal.AndroidXamarin.DependencyInjection;
using System;
using System.Threading.Tasks;
using WifiCommon.Net.DataModels;
using Xamarin.Essentials;
using static Xamarin.Essentials.Permissions;

namespace MultiCommTerminal.AndroidXamarin {
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        LogHelper logHelper = new LogHelper();
        ClassLog log = new ClassLog("MainActivity");


#if USEWIFI
#elif USESOCKET
        NetSocketMsgPump socketPump = new NetSocketMsgPump();
#endif

        protected override void OnCreate(Bundle savedInstanceState)
        {
            base.OnCreate(savedInstanceState);
            Xamarin.Essentials.Platform.Init(this, savedInstanceState);
            SetContentView(Resource.Layout.activity_main);
            Android.Support.V7.Widget.Toolbar toolbar = FindViewById<Android.Support.V7.Widget.Toolbar>(Resource.Id.toolbar);
            SetSupportActionBar(toolbar);

            FloatingActionButton fab = FindViewById<FloatingActionButton>(Resource.Id.fab);
            fab.Click += FabOnClick;

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(this, drawer, toolbar, Resource.String.navigation_drawer_open, Resource.String.navigation_drawer_close);
            drawer.AddDrawerListener(toggle);
            toggle.SyncState();

            NavigationView navigationView = FindViewById<NavigationView>(Resource.Id.nav_view);
            navigationView.SetNavigationItemSelectedListener(this);

            //this.logHelper.RawCurrentVerbotsityMsgInfoEvent += this.OnLogMsgRaised;
            this.logHelper.Setup("1.0.0.0", MsgLevel.Info, true, 5);
            this.SetupDependencyInjection();

            this.log.Error(1234, "Test error message");


#if USEWIFI
#elif USESOCKET
            this.socketPump.MsgReceivedEvent += this.SocketPump_MsgReceivedEvent;
            this.socketPump.MsgPumpConnectResultEvent += this.SocketPump_MsgPumpConnectResultEvent;
#endif
            this.OnStartupOk();

            // Impossible to actually see the files in the directories but they are there
            DI.Wrapper.CurrentLanguage((code) => {
                string msg = string.Format("Current language is:{0}", code);
                this.log.Info("On Start", msg);
                Toast.MakeText(ApplicationContext, msg, ToastLength.Long).Show();
            });

            this.CheckRunTimePermissions();
        }




#if USEWIFI
#elif USESOCKET
        private void SocketPump_MsgPumpConnectResultEvent(object sender, CommunicationStack.Net.DataModels.MsgPumpResults e) {
            this.log.Info("SocketPump_MsgPumpConnectResultEvent", "***** WE ARE CONNECTED  *****");
        }

        private void SocketPump_MsgReceivedEvent(object sender, byte[] e) {
            this.log.Info("SocketPump_MsgReceivedEvent", () => string.Format("***** RECEIVED MSG : {0} *****", e.ToAsciiString()));
        }
#endif


        public override void OnBackPressed()
        {
            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            if(drawer.IsDrawerOpen(GravityCompat.Start))
            {
                drawer.CloseDrawer(GravityCompat.Start);
            }
            else
            {
                base.OnBackPressed();
            }
        }

        public override bool OnCreateOptionsMenu(IMenu menu)
        {
            MenuInflater.Inflate(Resource.Menu.menu_main, menu);
            return true;
        }

        public override bool OnOptionsItemSelected(IMenuItem item)
        {
            int id = item.ItemId;
            if (id == Resource.Id.action_settings)
            {
                return true;
            }

            return base.OnOptionsItemSelected(item);
        }

        private void FabOnClick(object sender, EventArgs eventArgs)
        {
            View view = (View) sender;
            Snackbar.Make(view, "Replace with your own action", Snackbar.LengthLong)
                .SetAction("Action", (Android.Views.View.IOnClickListener)null).Show();
        }

        public bool OnNavigationItemSelected(IMenuItem item)
        {
            int id = item.ItemId;

            if (id == Resource.Id.nav_camera)
            {
                // Handle the camera action
                // Connect
                this.log.Info("OnNavigationItemSelected", "Camera - Connect");

#if USEWIFI
                var info = new WifiNetworkInfo() {
                    SSID = "MikieArduinoWifi",
                    Password = "1234567890",
                    RemoteHostName = "192.168.4.1",
                    RemoteServiceName = "80",
                };
                //this.wifi.ConnectAsync(info);
                DI.Wrapper.WifiConnectAsync(info);
#elif USESOCKET
                this.socketPump.ConnectAsync(new NetSocketConnectData("192.168.1.88", 9999));
#endif
            }
            else if (id == Resource.Id.nav_gallery)
            {
                // Disconnect
                this.log.Info("OnNavigationItemSelected", "Gallery - Disconnect");

#if USEWIFI
                DI.Wrapper.WifiDisconect();
#elif USESOCKET
                this.socketPump.Disconnect();
#endif
            }
            else if (id == Resource.Id.nav_slideshow)
            {
                // Send
                this.log.Info("OnNavigationItemSelected", "Slideshow - send msg");
#if USEWIFI
                DI.Wrapper.WifiSend("OpenDoor\r\n");
#elif USESOCKET
                this.socketPump.WriteAsync("OpenDoor\r\n".ToAsciiByteArray());
#endif
            }
            else if (id == Resource.Id.nav_manage)
            {
#if USEWIFI
                DI.Wrapper.WifiDiscoverAsync();
#elif USESOCKET
#endif

            }
            else if (id == Resource.Id.nav_share)
            {

            }
            else if (id == Resource.Id.nav_send)
            {

            }

            DrawerLayout drawer = FindViewById<DrawerLayout>(Resource.Id.drawer_layout);
            drawer.CloseDrawer(GravityCompat.Start);
            return true;
        }
        public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
        {
            Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

            base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
        }


        private void SetupDependencyInjection() {
            // Start it here for first load and retrieve stored language
            ErrReport err;
            WrapErr.ToErrReport(out err, 9999, () => {
                DI.Wrapper.CurrentStoredLanguage();
                Toast.MakeText(ApplicationContext, "DI Initialized", ToastLength.Long);
            });
            if (err.Code != 0) {
                Toast.MakeText(ApplicationContext, err.Msg, ToastLength.Long);

                //MessageBox.Show(err.Msg, "Critical Error loading DI container");
                //Application.Current.Shutdown();
            }
        }



        private void OnStartupOk() {
            DI.Wrapper.DiscoveredWifiNetworks += DiscoveredWifiNetworksHandler;
            DI.Wrapper.OnWifiError += OnWifiErrorHandler;
            DI.Wrapper.OnWifiConnectionAttemptCompleted += OnWifiConnectionAttemptCompletedHandler;
            DI.Wrapper.CredentialsRequestedEvent += WifiCredentialsRequestedEventHandler;
            DI.Wrapper.Wifi_BytesReceived += Wifi_BytesReceivedHandler;
        }

        private void Wifi_BytesReceivedHandler(object sender, string msg) {
            this.log.Info("Wifi_MsgReceivedEvent", () => string.Format("***** RECEIVED MSG : {0} *****", msg));
        }

        private void WifiCredentialsRequestedEventHandler(object sender, WifiCredentials e) {
            this.log.Info("OnWifiErrorHandler", () => string.Format("***** WIFI CREDENTIALS REQUESTED *****"));
        }

        private void OnWifiConnectionAttemptCompletedHandler(object sender, MsgPumpResults e) {
            this.log.Info("OnWifiConnectionAttemptCompletedHandler", () => string.Format(
                "***** Connect attempt complete : {0} - {1} - {2} *****",
                e.Code, e.SocketErr, e.ErrorString));
        }

        private void OnWifiErrorHandler(object sender, WifiError e) {
            this.log.Info("OnWifiErrorHandler", () => string.Format("***** WIFI ERROR:{0} - {1} *****", e.Code, e.ExtraInfo));
        }

        private void DiscoveredWifiNetworksHandler(object sender, System.Collections.Generic.List<WifiNetworkInfo> e) {
            this.log.Info("DiscoveredWifiNetworksHandler", () => string.Format("***** Discovered : {0} networks *****", e.Count));
            Toast.MakeText(ApplicationContext, string.Format("***** Discovered : {0} networks *****", e.Count), ToastLength.Long).Show();

        }


        private void CheckRunTimePermissions() {


            //// TODO - follow up on runtime permissions
            ////https://stackoverflow.com/questions/54186699/discovering-nearby-bluetooth-devices-showing-nothing

            //const int locationPermissionsRequestCode = 1000;

            //var locationPermissions = new[]
            //{
            //    Manifest.Permission.AccessCoarseLocation,
            //    Manifest.Permission.AccessFineLocation
            //};

            //// check if the app has permission to access coarse location
            //var coarseLocationPermissionGranted =
            //    ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessCoarseLocation);

            //// check if the app has permission to access fine location
            //var fineLocationPermissionGranted =
            //    ContextCompat.CheckSelfPermission(this, Manifest.Permission.AccessFineLocation);

            //// if either is denied permission, request permission from the user
            //if (coarseLocationPermissionGranted == Permission.Denied ||
            //    fineLocationPermissionGranted == Permission.Denied) {
            //    ActivityCompat.RequestPermissions(this, locationPermissions, locationPermissionsRequestCode);
            //}


            //var status = Permissions.RequestAsync<ReadWriteStoragePermission>();

            //var status = Permissions.CheckStatusAsync<Permissions.StorageRead>();

            //status = Permissions.RequestAsync<Permissions.StorageWrite>();
            //status = Permissions.RequestAsync<Permissions.StorageRead>();


            //await ChkPermissions();


        }



        protected async Task ChkPermissions()  {
            PermissionStatus status = await Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status != PermissionStatus.Granted) {
                status = await Permissions.RequestAsync<Permissions.StorageRead>();
            }

            status = await Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status != PermissionStatus.Granted) {
                status = await Permissions.RequestAsync<Permissions.StorageWrite>();
            }


            //status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            //if (status != PermissionStatus.Granted) {
            //    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            //}



        }

        protected void ChkPermissions2() {
            var status = Permissions.CheckStatusAsync<Permissions.StorageRead>();
            if (status.Result != PermissionStatus.Granted) {
                status = Permissions.RequestAsync<Permissions.StorageRead>();
            }

            status = Permissions.CheckStatusAsync<Permissions.StorageWrite>();
            if (status.Result != PermissionStatus.Granted) {
                status = Permissions.RequestAsync<Permissions.StorageWrite>();
            }


            //status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();
            //if (status != PermissionStatus.Granted) {
            //    status = await Permissions.RequestAsync<Permissions.LocationWhenInUse>();
            //}



        }


        //public async Task<PermissionStatus> CheckAndRequestLocationPermission() {
        //    var status = await Permissions.CheckStatusAsync<Permissions.LocationWhenInUse>();

        //    if (status == PermissionStatus.Granted)
        //        return status;


        //    //if (status == PermissionStatus.Denied && DeviceInfo.Platform == DevicePlatform.iOS) {
        //    //    // Prompt the user to turn on in settings
        //    //    // On iOS once a permission has been denied it may not be requested again from the application
        //    //    return status;
        //    //}

        //    if (Permissions.ShouldShowRationale<Permissions.LocationWhenInUse>()) {
        //        // Prompt the user with additional information as to why the permission is needed
        //    }

        //    status = Permissions.RequestAsync<Permissions.LocationWhenInUse>();

        //    return status;
        //}


        public async Task<PermissionStatus> CheckAndRequestPermissionAsync<T>(T permission)
            where T : BasePermission {
            var status = await permission.CheckStatusAsync();
            if (status != PermissionStatus.Granted) {
                status = await permission.RequestAsync();
            }

            return status;
        }




    }
}

