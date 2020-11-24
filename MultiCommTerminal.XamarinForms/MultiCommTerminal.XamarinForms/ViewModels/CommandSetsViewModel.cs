using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.Views {

    public class CommandSetsViewModel {

        /// <summary>Command to edit an existing command by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditCommandSet { get; }


        public CommandSetsViewModel() {
            this.EditCommandSet = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnEdit);
        }


        /// <summary>Navigate to the CommandSetPage and set the IndexAsString property</summary>
        /// <param name="data">The index to set</param>
        private async void OnEdit(IIndexItem<DefaultFileExtraInfo> data) {
            if (data != null) {
                await Shell.Current.GoToAsync($"{nameof(CommandSetPage)}?CommandSetPage.IndexAsString={JsonConvert.SerializeObject(data)}");
            }
        }

    }
}
