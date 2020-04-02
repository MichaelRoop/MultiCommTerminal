using LanguageFactory.data;
using LanguageFactory.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for LanguagaSelector.xamlsummary>
    public partial class LanguageSelector : Window {

        private ILangFactory languages = null;
        private LangCode languageOnEntry = LangCode.English;

        public LanguageSelector(ILangFactory languages) {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.languages = languages;
            this.languages.LanguageChanged += Languages_LanguageChanged;
            this.languageOnEntry = this.languages.GetCurrentLanguage();
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            this.lbLanguages.ItemsSource = this.languages.AvailableLanguages;
            // Only create the selected index here to avoid it firing on load
            this.lbLanguages.SelectionChanged += this.lbLanguages_SelectionChanged;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbLanguages.SelectionChanged -= this.lbLanguages_SelectionChanged;
            this.languages.LanguageChanged -= this.Languages_LanguageChanged;
        }


        // change to save
        private void btnSave_Click(object sender, RoutedEventArgs e) {
            // TODO Save new language to file
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            LangCode currentSelected = this.languages.GetCurrentLanguage();
            if (this.languageOnEntry != currentSelected) {
                this.languages.SetCurrentLanguage(this.languageOnEntry);
            }
            this.Close();
        }


        private void lbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            LanguageDataModel data = this.lbLanguages.SelectedItem as LanguageDataModel;
            if (data != null) {
                this.languages.SetCurrentLanguage(data.Code);
            }
        }

        private void Languages_LanguageChanged(object sender, LanguageFactory.Messaging.SupportedLanguage lang) {
            this.Dispatcher.Invoke(() => { 
                this.Title = lang.GetText(MsgCode.language);
                this.btnSave.Content = lang.GetText(MsgCode.save);
                this.btnCancel.Content = lang.GetText(MsgCode.cancel);
                // TODO Other texts
            });
        }

    }
}
