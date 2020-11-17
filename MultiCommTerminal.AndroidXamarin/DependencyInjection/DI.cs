using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using DependencyInjectorFactory.Net.interfaces;
using MultiCommWrapper.Net.DI;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MultiCommTerminal.AndroidXamarin.DependencyInjection {

    /// <summary>Static class to easily access Dependency injected objects</summary>
    public static class DI {

        #region Data

        private static IObjContainer container = null;

        #endregion


        /// <summary>Get the full container of objects</summary>
        /// <returns>The container</returns>
        private static IObjContainer GetContainer() {
            if (DI.container == null) {
                DI.container = new MultiCommIOC();
                DI.container.Initialise(new AndroidDIExtraCreators());
            }
            return DI.container;
        }


        /// <summary>Shortcut to get the application code wrapper</summary>
        /// <returns>The multi comm code wrapper</returns>
        public static ICommWrapper Wrapper {
            get {
                return DI.GetContainer().GetObjSingleton<ICommWrapper>();
            }
        }



    }
}