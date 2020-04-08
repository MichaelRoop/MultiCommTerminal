using LanguageFactory.data;
using MultiCommTerminal.DependencyInjection;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for LanguagaSelector.xamlsummary>
    public partial class LanguageSelector : Window {

        private LangCode languageOnEntry = LangCode.English;
        private ICommWrapper wrapper = null;

        public LanguageSelector() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // Move forward stuff to wrapper
            this.wrapper.LanguageChanged += Languages_LanguageChanged;
            this.wrapper.CurrentLanguage((code) => { this.languageOnEntry = code; });
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.wrapper.LanguageList((items) => {
                this.lbLanguages.ItemsSource = items;
                // Only create the selected index here to avoid it firing on load
                this.lbLanguages.SelectionChanged += this.lbLanguages_SelectionChanged;
            });
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbLanguages.SelectionChanged -= this.lbLanguages_SelectionChanged;
            this.wrapper.LanguageChanged -= this.Languages_LanguageChanged;
        }


        // change to save
        private void btnSave_Click(object sender, RoutedEventArgs e) {
            this.wrapper.CurrentLanguage((lang) => {
                if (this.languageOnEntry != lang) {
                    this.wrapper.SaveLanguage(lang, (err) => { MessageBox.Show(err); });
                }
            });
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.wrapper.CurrentLanguage((lang) => {
                if (this.languageOnEntry != lang) {
                    this.wrapper.SetLanguage(this.languageOnEntry);
                }
            });
            this.Close();
        }


        private void lbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            LanguageDataModel data = this.lbLanguages.SelectedItem as LanguageDataModel;
            if (data != null) {
                this.wrapper.SetLanguage(data.Code);
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
