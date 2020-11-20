using LanguageFactory.Net.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class BluetoothRunPage : ContentPage {

        public BluetoothRunPage() {
            InitializeComponent();
            this.UpdateLanguage();

            App.Wrapper.LanguageChanged += OnLanguageChanged;
        }

        private void OnLanguageChanged(object sender, SupportedLanguage e) {
            this.UpdateLanguage();
        }

        protected override void OnAppearing() {
            // TODO move this to a view model to do a busy and finish?
            
            this.UpdateLanguage();
            base.OnAppearing();
        }

        protected override void OnDisappearing() {
            // Disconnect
            base.OnDisappearing();
        }



        private void UpdateLanguage() {
            this.Title = App.GetText(LanguageFactory.Net.data.MsgCode.connect);

        }


    }
}