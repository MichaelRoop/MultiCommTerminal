using DependencyInjectorFactory.Net.interfaces;
using MultiCommWrapper.Net.DI;
using MultiCommWrapper.Net.interfaces;
using MultiCommWrapper.Net.WrapCode;

namespace MultiCommTerminal.DependencyInjection {

    /// <summary>Static class to easily access Dependency injected objects</summary>
    public static class DI {

        #region Data

        private static IObjContainer container = null;
        private static ICommWrapper wrapper = null;

        #endregion


        /// <summary>Get the full container of objects</summary>
        /// <returns>The container</returns>
        private static IObjContainer GetContainer() {
            if (DI.container == null) {
                DI.container = new MultiCommIOC();
                DI.container.Initialise(new WinDiExtraCreators());
            }
            return DI.container;
        }

        
        /// <summary>Shortcut to get the application code wrapper</summary>
        /// <returns>The multi comm code wrapper</returns>
        public static ICommWrapper Wrapper {
            get {
                if (DI.wrapper == null) {
                    DI.wrapper = new CommWrapper(DI.GetContainer());
                }
                return DI.wrapper;
            }
        }


    }
}
