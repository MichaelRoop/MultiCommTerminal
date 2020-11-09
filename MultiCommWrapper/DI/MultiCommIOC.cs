using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net.interfaces;
using CommunicationStack.Net.interfaces;
using CommunicationStack.Net.Stacks;
using DependencyInjectorFactory.Net;
using Ethernet.Common.Net.interfaces;
using IconFactory.Net.interfaces;
using LanguageFactory.Net.interfaces;
using LanguageFactory.Net.Messaging;
using MultiCommWrapper.Net.Factories;
using MultiCommWrapper.Net.interfaces;
using MultiCommWrapper.Net.WrapCode;
using SerialCommon.Net.interfaces;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;
using WifiCommon.Net.interfaces;

namespace MultiCommWrapper.Net.DI {

    public class MultiCommIOC : ObjContainer {

        public MultiCommIOC() : base() { }

        protected override void LoadCreators(
            Dictionary<Type, ObjCreator> instanceCreators, 
            Dictionary<Type, ObjCreator> singletonCreators) {

            // Instance creators
            instanceCreators.Add(
                typeof(ICommStackLevel0), 
                new ObjInstanceCreator(()=> new CommStackLevel0()));

            singletonCreators.Add(typeof(ILangFactory), new ObjSingletonCreator(() => new SupportedLanguageFactory()));
            singletonCreators.Add(typeof(IStorageManagerFactory), new ObjSingletonCreator(() => new MultiCommTerminalStorageFactory()));

            singletonCreators.Add(
                typeof(ICommWrapper),
                    new ObjSingletonCreator(() =>
                        new CommWrapper(
                            this.GetObjSingleton<IStorageManagerFactory>(),
                            this.GetObjSingleton<ILangFactory>(),
                            this.GetObjSingleton<IIconFactory>(),
                            this.GetObjSingleton<IBTInterface>(),
                            this.GetObjInstance<ICommStackLevel0>(),
                            this.GetObjSingleton<IBLETInterface>(),
                            this.GetObjInstance<ICommStackLevel0>(),
                            this.GetObjSingleton<IWifiInterface>(),
                            this.GetObjInstance<ICommStackLevel0>(),
                            this.GetObjSingleton<ISerialInterface>(),
                            this.GetObjInstance<ICommStackLevel0>(),
                            this.GetObjSingleton<IEthernetInterface>(),
                            this.GetObjInstance<ICommStackLevel0>())));

        }
    }
}
