using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net;
using BluetoothLE.Net.interfaces;
using BluetoothRfComm.AndroidXamarin;
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
using Wifi.AndroidXamarin;
using WifiCommon.Net.interfaces;

namespace MultiCommTerminal.XamarinForms.Droid.DependencyInjection {

    /// <summary>OS specific creators to inject into the Android Dependency Injector</summary>
    public class AndroidDIExtraCreators : IObjExtraCreators {

        #region IObjExtraCreators Properties

        public Dictionary<Type, ObjCreator> InstanceCreators { get; } = new Dictionary<Type, ObjCreator>();

        public Dictionary<Type, ObjCreator> SingletonCreators { get; } = new Dictionary<Type, ObjCreator>();

        #endregion

        public AndroidDIExtraCreators() {

            // These are expected in the common Wrapper class but not used in Android
            this.SingletonCreators.Add(
                typeof(ISerialInterface), 
                new ObjSingletonCreator(() => new SerialDoNothingImplementation()));
            this.SingletonCreators.Add(
                typeof(IEthernetInterface), 
                new ObjSingletonCreator(() => new EthernetDoNothingImplementation()));


            // TODO These will need implementations
            this.SingletonCreators.Add(
                typeof(IBLETInterface), 
                new ObjSingletonCreator(() => new BLE_DoNothingImplementation()));


            // In progress
            this.SingletonCreators.Add(
                typeof(IIconFactory), 
                new ObjSingletonCreator(() => new NoDirIconFactory()));
            this.SingletonCreators.Add(
                typeof(IBTInterface), 
                new ObjSingletonCreator(() => new BluetoothRfCommAndroidXamarinImpl()));
            this.SingletonCreators.Add(
                typeof(IWifiInterface), 
                new ObjSingletonCreator(() => new WifiImplAndroidXamarin()));

        }


    }
}