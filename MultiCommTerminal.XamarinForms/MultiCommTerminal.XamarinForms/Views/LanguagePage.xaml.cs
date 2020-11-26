using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {

    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class LanguagePage : ContentPage {

        #region Constructor and page events

        public LanguagePage() {
            InitializeComponent();

            // Cannot make this work - because it doesnt
            //this.IconImageSource = ImageSource.FromResource("icons8_language_50.png");

            // Can do this in constructor since the list never changes
            App.Wrapper.LanguageList((items) => {
                this.lstLanguages.ItemsSource = items;
            });
            App.LanguageUpdated += this.OnLanguageChangedHandler;
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            base.OnAppearing();
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
            this.lbTitle.Text = language.GetText(MsgCode.language);
        }

        #endregion

    }
}