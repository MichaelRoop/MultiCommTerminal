using Bluetooth.UWP.Core;
using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net.interfaces;
using BluetoothRfComm;
using BluetoothRfComm.UWP.Core;
using DependencyInjectorFactory.Net;
using DependencyInjectorFactory.Net.interfaces;
using Ethernet.Common.Net.interfaces;
using Ethernet.UWP.Core;
using IconFactory.Net.interfaces;
using MultiCommTerminalIconFactories;
using MultiCommWrapper.Net.interfaces;
using Serial.UWP.Core;
using SerialCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using Wifi.UWP.Core;
using WifiCommon.Net.interfaces;

namespace MultiCommTerminal.NetCore.DependencyInjection {

    public class WinDiExtraCreators : IObjExtraCreators {
        public Dictionary<Type, ObjCreator> InstanceCreators { get; } = new Dictionary<Type, ObjCreator>();

        public Dictionary<Type, ObjCreator> SingletonCreators { get; } = new Dictionary<Type, ObjCreator>();


        public WinDiExtraCreators() {
            this.SingletonCreators.Add(
                typeof(IIconFactory), new ObjSingletonCreator(() => new WinIconFactory()));

            this.SingletonCreators.Add(
                typeof(IBTInterface), new ObjSingletonCreator(() => new BluetoothRfCommUwpCore()));

            this.SingletonCreators.Add(
                typeof(IBLETInterface), new ObjSingletonCreator(() => new BluetoothLEImplWin32Core()));

            this.SingletonCreators.Add(
                typeof(IWifiInterface), new ObjSingletonCreator(() => new WifiImpleUwp()));

            this.SingletonCreators.Add(
                typeof(ISerialInterface), new ObjSingletonCreator(() => new SerialImplUwp()));

            this.SingletonCreators.Add(
                typeof(IEthernetInterface), new ObjSingletonCreator(() => new EthernetImplUwp()));

            this.SingletonCreators.Add(
                typeof(IStorageManagerSet), new ObjSingletonCreator(() => new StorageManagerSetWin()));

        }
    }

}
