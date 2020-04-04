using LanguageFactory.Messaging;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Events

        public event EventHandler<SupportedLanguage> LanguageChanged;

        #endregion



        private void Event_LanguageChanged(object sender, SupportedLanguage newLanguage) {
            if (this.LanguageChanged != null) {
                this.LanguageChanged(sender, newLanguage);
            }
        }



    }
}
