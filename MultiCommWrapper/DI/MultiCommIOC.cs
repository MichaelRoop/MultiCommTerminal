using BluetoothCommon.Net.interfaces;
using DependencyInjectorFactory;
using IconFactory.interfaces;
using LanguageFactory.interfaces;
using LanguageFactory.Messaging;
using MultiCommWrapper.Net.interfaces;
using MultiCommWrapper.Net.WrapCode;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.DI {

    public class MultiCommIOC : ObjContainer {

        public MultiCommIOC() : base() { }

        protected override void LoadCreators(
            Dictionary<Type, ObjCreator> instanceCreators, 
            Dictionary<Type, ObjCreator> singletonCreators) {
            
            singletonCreators.Add(typeof(ILangFactory), new ObjSingletonCreator(() => new SupportedLanguageFactory()));

            singletonCreators.Add(
                typeof(ICommWrapper), 
                    new ObjSingletonCreator(() => 
                        new CommWrapper(
                            this.GetObjSingleton<ILangFactory>(), 
                            this.GetObjSingleton<IIconFactory>(),
                            this.GetObjSingleton<IBTInterface>(), 
                            this.GetObjSingleton<IBLETInterface>())));
        }
    }
}
