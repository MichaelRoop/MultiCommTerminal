using DependencyInjectorFactory.Net.interfaces;
using MultiCommWrapper.Net.DI;
using MultiCommWrapper.Net.interfaces;
using MultiCommWrapper.Net.WrapCode;

namespace MultiCommTestCases.Core.Wrapper.Utils {

    public static class TDI {

        #region Data

        private static IObjContainer container = null;
        private static ICommWrapper wrapper = null;

        #endregion

        /// <summary>Get the full container of objects</summary>
        /// <returns>The container</returns>
        public static IObjContainer GetContainer() {
            if (TDI.container == null) {
                TDI.container = new MultiCommIOC();
                TDI.container.Initialise(new TestDIExtraCreators());
            }
            return TDI.container;
        }


        /// <summary>Shortcut to get the application code wrapper</summary>
        /// <returns>The multi comm code wrapper</returns>
        public static ICommWrapper Wrapper {
            get {
                if (TDI.wrapper == null) {
                    TDI.wrapper = new CommWrapper(TDI.GetContainer());
                }
                return TDI.wrapper;
            }
        }



    }
}
