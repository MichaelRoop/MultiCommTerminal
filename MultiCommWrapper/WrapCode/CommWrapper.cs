using BluetoothCommon.Net.interfaces;
using IconFactory.interfaces;
using LanguageFactory.interfaces;
using LanguageFactory.Messaging;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {

        #region Data

        ILangFactory languages = null;
        IIconFactory iconFactory = null;
        IBTInterface classicBluetooth = null;
        IBLETInterface bleBluetooth = null;

        #endregion

        /// <summary>
        /// 
        /// </summary>
        public CommWrapper(
            ILangFactory languages,
            IIconFactory iconFactory,
            IBTInterface classicBluetooth,
            IBLETInterface bleBluetooth ) {

            this.languages = languages;
            this.iconFactory = iconFactory;
            this.classicBluetooth = classicBluetooth;
            this.bleBluetooth = bleBluetooth;

            this.languages.LanguageChanged += Event_LanguageChanged;
        }


        public void Teardown() {
            // TODO - shut down anything needed and dispose
            this.languages.LanguageChanged -= this.Event_LanguageChanged;
        }

    }
}
