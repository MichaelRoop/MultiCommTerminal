using BluetoothCommon.Net;
using MultiCommTerminal.XamarinForms.Views;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.ViewModels {
    public class BluetoothViewModel : BaseViewModel {


        public Command PairCommand { get; }
        public Command RunCommand { get; }

        public BTDeviceInfo SelectedInfo { get; set; } = null;


        public BluetoothViewModel() {
            this.PairCommand = new Command(this.OnPair);
            this.RunCommand = new Command(this.OnRun);
        }


        public void OnAppearing() {
            this.SelectedInfo = null;
        }

        public void OnDisappearing() {
            // Put in code here to fire on disappearing
        }



        private async void OnRun() {
            if (this.SelectedInfo != null) {
                // Seem to only be able to pass strings as params
                await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?BTDevice={JsonConvert.SerializeObject(SelectedInfo)}");
            }
        }


        private async void OnPair() {
            await Shell.Current.GoToAsync(nameof(BluetoothPairPage));
        }







    }
}
