using LanguageFactory.data;
using LanguageFactory.interfaces;
using MultiCommTerminal.DependencyInjection;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for LanguagaSelector.xamlsummary>
    public partial class LanguageSelector : Window {

        private LangCode languageOnEntry = LangCode.English;

        public LanguageSelector() {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // Move forward stuff to wrapper
            DI.Wrapper().LanguageChanged += Languages_LanguageChanged;
            this.languageOnEntry = DI.Language().CurrentLanguageCode;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            this.lbLanguages.ItemsSource = DI.Language().AvailableLanguages;
            // Only create the selected index here to avoid it firing on load
            this.lbLanguages.SelectionChanged += this.lbLanguages_SelectionChanged;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbLanguages.SelectionChanged -= this.lbLanguages_SelectionChanged;
            DI.Wrapper().LanguageChanged -= this.Languages_LanguageChanged;
        }


        // change to save
        private void btnSave_Click(object sender, RoutedEventArgs e) {
            // TODO Save new language to file
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            LangCode currentSelected = DI.Language().CurrentLanguageCode;
            if (this.languageOnEntry != currentSelected) {
                DI.Language().SetCurrentLanguage(this.languageOnEntry);
            }
            this.Close();
        }


        private void lbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            LanguageDataModel data = this.lbLanguages.SelectedItem as LanguageDataModel;
            if (data != null) {
                DI.Language().SetCurrentLanguage(data.Code);
            }
        }

        private void Languages_LanguageChanged(object sender, LanguageFactory.Messaging.SupportedLanguage lang) {
            this.Dispatcher.Invoke(() => { 
                this.lbTitle.Content = lang.GetText(MsgCode.language);
                this.btnSave.Content = lang.GetText(MsgCode.save);
                this.btnCancel.Content = lang.GetText(MsgCode.cancel);

                // TODO Other texts
            });
        }


        private void lbTitle_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.DragMove();
        }

    }
}
