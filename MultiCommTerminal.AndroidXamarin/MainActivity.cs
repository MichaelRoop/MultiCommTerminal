#define USEWIFI
//#define USESOCKET
using System;
using Android;
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
using CommunicationStack.Net.MsgPumps;
using LanguageFactory.Net.data;
using LogUtils.Net;
using LogUtils.Net.Interfaces;
using MultiCommTerminal.AndroidXamarin.DependencyInjection;
using VariousUtils.Net;
using Wifi.AndroidXamarin;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.interfaces;


namespace MultiCommTerminal.AndroidXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        LogHelper logHelper = new LogHelper();
        ClassLog log = new ClassLog("MainActivity");


#if USEWIFI
        //IWifiInterface wifi = new WifiImplAndroidXamarin();
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
            //this.wifi.MsgReceivedEvent += Wifi_MsgReceivedEvent;
            //this.wifi.OnWifiConnectionAttemptCompleted += Wifi_OnWifiConnectionAttemptCompleted;
            //this.wifi.DiscoveredNetworks += Wifi_DiscoveredNetworks;
#elif USESOCKET
            this.socketPump.MsgReceivedEvent += this.SocketPump_MsgReceivedEvent;
            this.socketPump.MsgPumpConnectResultEvent += this.SocketPump_MsgPumpConnectResultEvent;
#endif
            this.OnStartupOk();

            //DI.Wrapper.SetLanguage(LangCode.French);
            DI.Wrapper.CurrentLanguage((code) => {
                this.log.Info("On Start", () => string.Format("Current language is:{0}", code));
            });

        }




#if USEWIFI
        //private void Wifi_OnWifiConnectionAttemptCompleted(object sender, MsgPumpResults e) {
        //    this.log.Info("Wifi_MsgReceivedEvent", () => string.Format(
        //        "***** Connect attempt complete : {0} - {1} - {2} *****", 
        //        e.Code, e.SocketErr, e.ErrorString));
        //}

        //private void Wifi_MsgReceivedEvent(object sender, byte[] e) {
        //    this.log.Info("Wifi_MsgReceivedEvent", () => string.Format("***** RECEIVED MSG : {0} *****", e.ToAsciiString()));
        //}

        //private void Wifi_DiscoveredNetworks(object sender, System.Collections.Generic.List<WifiNetworkInfo> e) {
        //    this.log.Info("Wifi_DiscoveredNetworks", () => string.Format("***** Discovered : {0} networks *****", e.Count));
        //}

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
                    SSID = "MikieArduinoWifiXXX",
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
                //this.wifi.Disconnect();
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
                //this.wifi.SendOutMsg("OpenDoor\r\n".ToAsciiByteArray());
                DI.Wrapper.WifiSend("OpenDoor\r\n");
#elif USESOCKET
                this.socketPump.WriteAsync("OpenDoor\r\n".ToAsciiByteArray());
#endif

            }
            else if (id == Resource.Id.nav_manage)
            {
#if USEWIFI
                //this.wifi.DiscoverWifiAdaptersAsync();
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
        }


    }
}

