using CommunicationStack.Net.interfaces;
using CommunicationStack.Net.Stacks;
using DependencyInjectorFactory.Net;
using LanguageFactory.Net.interfaces;
using LanguageFactory.Net.Messaging;
using MultiCommWrapper.Net.Factories;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using System;
using System.Collections.Generic;

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
            singletonCreators.Add(
                typeof(IStorageManagerFactory), 
                new ObjSingletonCreator(() => 
                new MultiCommTerminalStorageFactory(this.GetObjSingleton<IStorageManagerSet>())));

        }
    }
}
