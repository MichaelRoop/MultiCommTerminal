using BluetoothCommon.Net;
using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {

    /// <summary>Handle navigation for the BluetoothPage view</summary>
    public class BluetoothViewModel : BaseViewModel {

        #region Commands

        /// <summary>Command to navigate to the BluetoothPairPage</summary>
        public Command PairCommand { get; }

        /// <summary>Command to navigate to the BluetoothRunPage</summary>
        public Command<BTDeviceInfo> RunCommand { get; }

        #endregion

        #region Constructors

        public BluetoothViewModel() {
            this.PairCommand = new Command(this.OnPair);
            this.RunCommand = new Command<BTDeviceInfo>((info) => this.OnRun(info));
        }

        #endregion

        #region Private Command delegates

        private async void OnRun(BTDeviceInfo info) {
            if (info != null) {
                // Seem to only be able to pass strings as params
                await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?BTDevice={JsonConvert.SerializeObject(info)}");
            }
        }


        private async void OnPair() {
            await Shell.Current.GoToAsync(nameof(BluetoothPairPage));
        }

        #endregion

    }
}
