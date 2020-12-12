using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    public static class TextBinder {

        public static string SupportlUri {
            get {
                return string.Format(@"mailto:MultiCommTerminal@gmail.com?subject=Multi Comm Terminal Support Question&body=App Build number:{0}", BuildNumber);
            }
        }


        public static string AuthorUri { get { return "https://linkedin.com/in/michael-roop-21800b19"; } }


        public static string IconsUri { get { return "https://icons8.com"; } }


        public static string BuildNumber { get { return "2020.12.11.1"; } }

    }

}
