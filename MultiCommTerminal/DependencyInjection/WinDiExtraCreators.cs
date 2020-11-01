using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net.interfaces;
using Bluetooth.UWP.Core;
using BluetoothRfComm.UWP.Core;
using Communications.UWP.Core.MsgPumps;
using CommunicationStack.Net.interfaces;
using DependencyInjectorFactory.Net;
using DependencyInjectorFactory.Net.interfaces;
using IconFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using Wifi.UWP.Core;
using WifiCommon.Net.interfaces;
using SerialCommon.Net.interfaces;
using Serial.UWP.Core;

namespace MultiCommTerminal.DependencyInjection {

    public class WinDiExtraCreators : IObjExtraCreators {
        public Dictionary<Type, ObjCreator> InstanceCreators { get; } = new Dictionary<Type, ObjCreator>();

        public Dictionary<Type, ObjCreator> SingletonCreators { get; } = new Dictionary<Type, ObjCreator>();


        public WinDiExtraCreators() {
            this.SingletonCreators.Add(
                typeof(IIconFactory), new ObjSingletonCreator(() => new MultiCommTerminal.WPF_Helpers.IconFactory()));

            //this.SingletonCreators.Add(
            //    typeof(IBTInterface), new ObjSingletonCreator(() => new BluetoothClassic.Net.BluetoothClassicImpl()));


            //this.SingletonCreators.Add(
            //    typeof(IBTInterface), new ObjSingletonCreator(() => new BluetoothRfComm.Win32.BluetoothRfCommImpl()));
            this.SingletonCreators.Add(
                typeof(IBTInterface), new ObjSingletonCreator(() => new BluetoothRfCommUwpCore()));


            //this.SingletonCreators.Add(
            //    typeof(IBLETInterface), new ObjSingletonCreator(() => new BluetoothLE.Win32.BluetoothLEImplWin32()));
            this.SingletonCreators.Add(
                typeof(IBLETInterface), new ObjSingletonCreator(() => new BluetoothLEImplWin32Core()));

            this.SingletonCreators.Add(
                typeof(IWifiInterface), new ObjSingletonCreator(() => new WifiImpleUwp()));

            this.SingletonCreators.Add(
                typeof(ISerialInterface), new ObjSingletonCreator(() => new SerialImplUwp()));

            this.InstanceCreators.Add(
                typeof(IMsgPump<SocketMsgPumpConnectData>), new ObjInstanceCreator(() => new SocketMsgPump()));

            //BluetoothLEImplWin32Core

        }
    }

}
