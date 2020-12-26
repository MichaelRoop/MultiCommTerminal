using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using System.Threading.Tasks;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LanguagePage : ContentPage {

        #region Constructor and page events

        public LanguagePage() {
            InitializeComponent();
            this.lstLanguages.HeightRequest = 1000;
            Task.Run(() => {
                // Can do this in constructor since the list never changes
                App.Wrapper.LanguageList((items) => {
                    Device.BeginInvokeOnMainThread(() => {
                        this.lstLanguages.ItemsSource = items;
                    });
                });
            });
        }


        protected override void OnAppearing() {
            App.LanguageUpdated += this.OnLanguageChangedHandler;
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            App.LanguageUpdated -= this.OnLanguageChangedHandler;
            base.OnDisappearing();
        }

        #endregion

        #region Controller events

        private void lstLanguages_ItemSelected(object sender, SelectedItemChangedEventArgs e) {
            LanguageDataModel dm = (LanguageDataModel)e.SelectedItem;
            App.Wrapper.SaveLanguage(dm.Code, (err) => App.ShowError(this, err));
        }

        #endregion

        #region Wrapper event handlers

        private void OnLanguageChangedHandler(object sender, SupportedLanguage e) {
            this.UpdateLanguage(e);
        }


        private void UpdateLanguage(SupportedLanguage language) {
            Device.BeginInvokeOnMainThread(() => this.lbTitle.Text = language.GetText(MsgCode.language));
        }

        #endregion

    }
}