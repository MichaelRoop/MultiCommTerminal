using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    public class CommandSetViewModel {

        /// <summary>Command to edit an existing command by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommand { get; }

        public CommandSetViewModel() {
            this.EditCommand = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnEdit);
        }


        private async void OnEdit(IIndexItem<DefaultFileExtraInfo> data) {
            if (data != null) {
                await Shell.Current.GoToAsync(
                    $"{nameof(CommandEditPage)}?CommandEditPage.IndexAsString={JsonConvert.SerializeObject(data)}");
            }
        }

    }
}
