using DependencyInjectorFactory.interfaces;
using IconFactory.data;
using IconFactory.interfaces;
using LanguageFactory.data;
using LanguageFactory.interfaces;
using MultiCommWrapper.Net.DI;
using MultiCommWrapper.Net.interfaces;

namespace MultiCommTerminal.DependencyInjection {

    /// <summary>Static class to easily access Dependency injected objects</summary>
    public static class DI {

        #region Data

        private static IObjContainer container = null;

        #endregion


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


        public static ILangFactory Language() {
            return DI.GetObj<ILangFactory>();
        }

        public static string GetText(MsgCode code) {
            return DI.Language().GetMsgDisplay(code);
        }

        public static IIconFactory GetIconFactory() {
            return DI.Get().GetObjSingleton<IIconFactory>();
        }


        public static IconDataModel GetIcon(UIIcon code) {
            return DI.GetIconFactory().GetIcon(code);
        }


        public static string GetIconSource(UIIcon code) {
            return DI.GetIcon(code).IconSource as string;
        }

    }
}
