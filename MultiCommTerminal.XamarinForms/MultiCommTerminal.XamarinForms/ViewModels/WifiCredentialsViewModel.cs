using MultiCommTerminal.XamarinForms.Views;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {
    
    public class WifiCredentialsViewModel : BaseViewModel {

        public Command<IIndexItem<DefaultFileExtraInfo>> GoToCredEdit;

        public WifiCredentialsViewModel() {
            this.GoToCredEdit = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnCredEdit);
        }


        private async void OnCredEdit(IIndexItem<DefaultFileExtraInfo> index) {
            // Temp for test
            await Shell.Current.GoToAsync(nameof(WifiCredentialsModalEditPage));



            if (index != null) {
                // temp for compiler - until index param
                //await Shell.Current.GoToAsync(nameof(WifiCredentialsModalEditPage));

                //await Shell.Current.GoToAsync(
                //    $"{nameof(CommandEditPage)}?CommandEditPage.IndexAsString={JsonConvert.SerializeObject(data)}");
            }
        }

    }
}
