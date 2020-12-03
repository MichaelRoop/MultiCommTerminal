using MultiCommTerminal.XamarinForms.Views;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Text;
using WifiCommon.Net.DataModels;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {
    
    public class WifiRunViewModel : BaseViewModel {

        public Command<WifiCredentials> PopUpCred;

        public WifiRunViewModel() {
            this.PopUpCred = new Command<WifiCredentials>(this.OnPopUpCred);
        }

        private async void OnPopUpCred(WifiCredentials cred) {
            // New instance on each call
            //https://stackoverflow.com/questions/58749785/xamarin-form-how-to-use-rg-plugins-popup-pages-in-mvvmcross
            //await PopupNavigation.Instance.PushAsync(new WifiCredRequestPopUpPage(cred));
        }

    }
}
