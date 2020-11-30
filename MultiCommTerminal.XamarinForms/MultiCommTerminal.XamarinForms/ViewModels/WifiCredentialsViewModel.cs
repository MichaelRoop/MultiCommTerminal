using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    public class WifiCredentialsViewModel : BaseViewModel {

        public Command<IIndexItem<DefaultFileExtraInfo>> CredEditCmd;
        public Command CredAddCmd;
        

        public WifiCredentialsViewModel() {
            this.CredEditCmd = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnCredEdit);
            this.CredAddCmd = new Command(this.OnCredAdd);
        }

        // Seems a bug with routing so we will just go absolute
        private string ROUTE_NAME = nameof(WifiCredentialsModalEditPage);
            //string.Format("{0}/{1}", nameof(WifiCredentialsPage), nameof(WifiCredentialsModalEditPage));

        private async void OnCredAdd() {
            await Shell.Current.GoToAsync($"{ROUTE_NAME}?WifiCredentialsModalEditPage.IndexAsString={""}");
        }


        private async void OnCredEdit(IIndexItem<DefaultFileExtraInfo> index) {
            if (index != null) {
                await Shell.Current.GoToAsync($"{ROUTE_NAME}?WifiCredentialsModalEditPage.IndexAsString={JsonConvert.SerializeObject(index)}");
            }
        }

    }
}
