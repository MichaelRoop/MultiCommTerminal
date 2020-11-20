using MultiCommTerminal.XamarinForms.Views;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {
    public class BluetoothViewModel : BaseViewModel {

        public Command PairCommand { get; }
        public Command RunCommand { get; }

        public BluetoothViewModel() {
            this.PairCommand = new Command(this.OnPair);
            this.RunCommand = new Command(this.OnRun);

        }


        public void OnAppearing() {
            // Put in code here to fire on opening
            //IsBusy = true;
            //SelectedItem = null;
        }

        public void OnDisappearing() {
            // Put in code here to fire on disappearing
        }



        private async void OnRun() {
            //await this.DisplayAlert("", "Called OnRun", "OK");
            await Shell.Current.GoToAsync(nameof(BluetoothRunPage));
        }


        private async void OnPair() {
            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(BluetoothPairPage)}");

            //await this.DisplayAlert("", "Called OnPair", "OK");
            await Shell.Current.GoToAsync(nameof(BluetoothPairPage));
        }







    }
}
