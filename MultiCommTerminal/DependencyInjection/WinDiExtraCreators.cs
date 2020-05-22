//using BluetoothClassic.Net;
using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net.interfaces;
//using BluetoothLE.Win32;
using DependencyInjectorFactory;
using DependencyInjectorFactory.interfaces;
using IconFactory.interfaces;
using System;
using System.Collections.Generic;

namespace MultiCommTerminal.DependencyInjection {

    public class WinDiExtraCreators : IObjExtraCreators {
        public Dictionary<Type, ObjCreator> InstanceCreators { get; } = new Dictionary<Type, ObjCreator>();

        public Dictionary<Type, ObjCreator> SingletonCreators { get; } = new Dictionary<Type, ObjCreator>();


        public WinDiExtraCreators() {
            this.SingletonCreators.Add(
                typeof(IIconFactory), new ObjSingletonCreator(() => new MultiCommTerminal.WPF_Helpers.IconFactory()));

            //this.SingletonCreators.Add(
            //    typeof(IBTInterface), new ObjSingletonCreator(() => new BluetoothClassic.Net.BluetoothClassicImpl()));


            this.SingletonCreators.Add(
                typeof(IBTInterface), new ObjSingletonCreator(() => new BluetoothRfComm.Win32.BluetoothRfCommImpl()));


            this.SingletonCreators.Add(
                typeof(IBLETInterface), new ObjSingletonCreator(() => new BluetoothLE.Win32.BluetoothLEImplWin32()));
        }
    }

}
