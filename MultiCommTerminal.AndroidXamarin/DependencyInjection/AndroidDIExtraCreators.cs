using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net;
using BluetoothLE.Net.interfaces;
using DependencyInjectorFactory.Net;
using DependencyInjectorFactory.Net.interfaces;
using Ethernet.Common.Net;
using Ethernet.Common.Net.interfaces;
using IconFactory.Net;
using IconFactory.Net.interfaces;
using SerialCommon.Net;
using SerialCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Wifi.AndroidXamarin;
using WifiCommon.Net.interfaces;

namespace MultiCommTerminal.AndroidXamarin.DependencyInjection {

    public class AndroidDIExtraCreators : IObjExtraCreators {

        #region IObjExtraCreators Properties

        public Dictionary<Type, ObjCreator> InstanceCreators { get; } = new Dictionary<Type, ObjCreator>();

        public Dictionary<Type, ObjCreator> SingletonCreators { get; } = new Dictionary<Type, ObjCreator>();


        #endregion


        /// <summary>Initialise Android specific DI creators here</summary>
        public AndroidDIExtraCreators() {

            // These are expected in the common Wrapper class but not used in Android
            this.SingletonCreators.Add(
                typeof(ISerialInterface), new ObjSingletonCreator(() => new SerialDoNothingImplementation()));
            this.SingletonCreators.Add(
                typeof(IEthernetInterface), new ObjSingletonCreator(() => new EthernetDoNothingImplementation()));

            // TODO These will need implementations
            this.SingletonCreators.Add(
                typeof(IIconFactory), new ObjSingletonCreator(() => new IconFactoryDoNothingImplementation()));
            this.SingletonCreators.Add(
                typeof(IBTInterface), new ObjSingletonCreator(() => new BT_DoNothingImplementation()));
            this.SingletonCreators.Add(
                typeof(IBLETInterface), new ObjSingletonCreator(() => new BLE_DoNothingImplementation()));

            // In progress
            this.SingletonCreators.Add(
                typeof(IWifiInterface), new ObjSingletonCreator(() => new WifiImplAndroidXamarin()));

        }



    }
}