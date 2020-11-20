using IconFactory.Net.data;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    public static class IconBinder {

        public static string Language { get { return App.Wrapper.IconSource(UIIcon.Language); } }

    }
}
