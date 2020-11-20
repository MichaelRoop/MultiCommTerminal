using LanguageFactory.Net.data;
using System.Collections.Generic;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LanguagePage : ContentPage {

        public LanguagePage() {
            InitializeComponent();
            this.Title = App.Wrapper.GetText(MsgCode.language);

            // Cannot make this work
            //this.IconImageSource = ImageSource.FromResource("icons8_language_50.png");
            App.Wrapper.LanguageList((items) => {
                this.lstLanguages.ItemsSource = items;
            });
            App.Wrapper.LanguageChanged += this.WrapperLanguageChangedHandler;
        }


        private void WrapperLanguageChangedHandler(object sender, LanguageFactory.Net.Messaging.SupportedLanguage e) {
            this.Title = App.Wrapper.GetText(MsgCode.language);
        }


        private void lstLanguages_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            LanguageDataModel dm = (LanguageDataModel)e.SelectedItem;
            App.Wrapper.SaveLanguage(dm.Code, (err) => App.ShowError(this, err));
        }

    }
}