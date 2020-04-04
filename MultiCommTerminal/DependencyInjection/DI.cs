﻿using DependencyInjectorFactory.interfaces;
using MultiCommWrapper.Net.DI;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.DependencyInjection {

    /// <summary>Static class to easily access Dependency injected objects</summary>
    public static class DI {

        private static IObjContainer container = null;

        /// <summary>Get the full container of objects</summary>
        /// <returns>The container</returns>
        public static IObjContainer Get() {
            if (DI.container == null) {
                DI.container = new MultiCommIOC();
                DI.container.Initialise(new WinDiExtraCreators());
            }
            return DI.container;
        }

        /// <summary>Returns a singleton instance of the type</summary>
        /// <typeparam name="T">The type instance to retrieve</typeparam>
        /// <returns>The singleton instance of the type</returns>
        public static T GetObj<T>() where T : class {
            // Since we only use singletons in the terminal this avoids confusion
            return DI.Get().GetObjSingleton<T>();
        }

        
        /// <summary>Shortcut to get the application code wrapper</summary>
        /// <returns>The multi comm code wrapper</returns>
        public static ICommWrapper Wrapper() {
            return DI.GetObj<ICommWrapper>();
        }


    }
}
