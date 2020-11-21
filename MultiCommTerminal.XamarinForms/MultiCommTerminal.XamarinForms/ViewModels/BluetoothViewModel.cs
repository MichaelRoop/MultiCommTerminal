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
            // Put in code here to fire on opening
            //IsBusy = true;
            //SelectedItem = null;
        }

        public void OnDisappearing() {
            // Put in code here to fire on disappearing
        }



        private async void OnRun() {
            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(BluetoothPairPage)}");




            //await this.DisplayAlert("", "Called OnRun", "OK");

            //BTDeviceInfo info = this.lst

            //            // This will push the ItemDetailPage onto the navigation stack
            //await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?{nameof(BluetoothRunPage.BTDevice)}={item.Id}");

            if (this.SelectedInfo != null) {

                // This was the model. The query property 
                // await Shell.Current.GoToAsync($"{nameof(ItemDetailPage)}?{nameof(ItemDetailViewModel.ItemId)}={item.Id}");

                //System.ArgumentException
                //Message = Object of type 'System.String' cannot be converted to type 'BluetoothCommon.Net.BTDeviceInfo'.
                //await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?{nameof(BluetoothRunPage.BTDevice)}={this.SelectedInfo}");

                //await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?BTDevice={this.SelectedInfo}");


                //Shell.Current.GoToAsync($”{ nameof(Page2)}?Count ={ Count}”);

                await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?BTDevice={ JsonConvert.SerializeObject(SelectedInfo)}");


                //await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?BTDevice={this.SelectedInfo}");

                // Assert on string of param
                //await Shell.Current.GoToAsync($"{nameof(BluetoothRunPage)}?{nameof(BluetoothRunPage.BTDevice)}={this.SelectedInfo}");
            }
            else {
                //App.ShowError(this, App.GetText(LanguageFactory.Net.data.MsgCode.NothingSelected));
            }


            //await Shell.Current.GoToAsync(nameof(BluetoothRunPage));
        }


        private async void OnPair() {
            //await this.DisplayAlert("", "Called OnPair", "OK");
            await Shell.Current.GoToAsync(nameof(BluetoothPairPage));
        }







    }
}
