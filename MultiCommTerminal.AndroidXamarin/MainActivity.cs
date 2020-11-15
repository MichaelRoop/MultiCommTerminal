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
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.MsgPumps;
using LogUtils.Net;
using LogUtils.Net.Interfaces;
using VariousUtils.Net;

namespace MultiCommTerminal.AndroidXamarin
{
    [Activity(Label = "@string/app_name", Theme = "@style/AppTheme.NoActionBar", MainLauncher = true)]
    public class MainActivity : AppCompatActivity, NavigationView.IOnNavigationItemSelectedListener
    {

        LogHelper logHelper = new LogHelper();
        ClassLog log = new ClassLog("MainActivity");
        NetSocketMsgPump socketPump = new NetSocketMsgPump();
        I_OS_ConsoleWriter writer = new OsConsoleWriter();

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

            this.log.Error(1234, "Test error message");

            this.socketPump.MsgReceivedEvent += this.SocketPump_MsgReceivedEvent;
            this.socketPump.MsgPumpConnectResultEvent += this.SocketPump_MsgPumpConnectResultEvent;

        }

        //private void OnLogMsgRaised(MsgLevel level, ErrReport report) {
        //    this.writer.WriteToConsole(level, report);
        //}


        private void SocketPump_MsgPumpConnectResultEvent(object sender, CommunicationStack.Net.DataModels.MsgPumpResults e) {
            this.log.Info("SocketPump_MsgPumpConnectResultEvent", "***** WE ARE CONNECTED  *****");
        }

        private void SocketPump_MsgReceivedEvent(object sender, byte[] e) {
            this.log.Info("SocketPump_MsgReceivedEvent", () => string.Format("***** RECEIVED MSG : {0} *****", e.ToAsciiString()));
        }

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
                this.socketPump.ConnectAsync(new NetSocketConnectData("192.168.1.88", 9999));
            }
            else if (id == Resource.Id.nav_gallery)
            {
                // Disconnect
                this.log.Info("OnNavigationItemSelected", "Gallery - Disconnect");
                this.socketPump.Disconnect();

            }
            else if (id == Resource.Id.nav_slideshow)
            {
                // Send
                this.log.Info("OnNavigationItemSelected", "Slideshow - send msg");
                this.socketPump.WriteAsync("OpenDoor\r\n".ToAsciiByteArray());
            }
            else if (id == Resource.Id.nav_manage)
            {

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
    }
}

