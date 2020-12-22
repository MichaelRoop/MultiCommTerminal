using LanguageFactory.Net.data;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    public static class ButtonExtensions {

        /// <summary>Apply an accessibility screen reader tag for icon only buttons</summary>
        /// <param name="button">The target button for the tag</param>
        /// <param name="name">The screen reader name</param>
        public static void SetScreenReader(this Button button, string name) {
            button.SetValue(AutomationProperties.IsInAccessibleTreeProperty, true);
            button.SetValue(AutomationProperties.NameProperty, name);
        }


        /// <summary>Apply an accessibility screen reader tag for icon only buttons</summary>
        /// <param name="button">The target button for the tag</param>
        /// <param name="code">The language name code</param>
        public static void SetScreenReader(this Button button, MsgCode code) {
            button.SetScreenReader(App.GetText(code));
        }



    }
}
