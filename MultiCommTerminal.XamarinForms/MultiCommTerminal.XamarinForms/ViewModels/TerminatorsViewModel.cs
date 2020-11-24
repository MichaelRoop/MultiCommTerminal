using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    public class TerminatorsViewModel {

        /// <summary>Command to edit an existing terminator set by index</summary>
        public Command<IIndexItem<DefaultFileExtraInfo>> EditTerminatorSet { get; }


        public TerminatorsViewModel() {
            this.EditTerminatorSet = new Command<IIndexItem<DefaultFileExtraInfo>>(this.OnEdit);
        }


        /// <summary>Navigate to the CommandSetPage and set the IndexAsString property</summary>
        /// <param name="data">The index to set</param>
        private async void OnEdit(IIndexItem<DefaultFileExtraInfo> data) {
            if (data != null) {
                await Shell.Current.GoToAsync($"{nameof(TerminatorSetPage)}?TerminatorSetPage.IndexAsString={JsonConvert.SerializeObject(data)}");
            }
        }

    }
}
