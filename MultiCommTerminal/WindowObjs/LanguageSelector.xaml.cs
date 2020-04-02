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

        public LanguageSelector(ILangFactory languages) {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.languages = languages;
            this.languages.LanguageChanged += Languages_LanguageChanged;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            this.lbLanguages.ItemsSource = this.languages.AvailableLanguages;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.languages.LanguageChanged -= Languages_LanguageChanged;
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            LanguageDataModel data = this.lbLanguages.SelectedItem as LanguageDataModel;
            if (data != null) {
                this.languages.SetCurrentLanguage(data.Code);
                // May want to close on on separate button
                //this.Close();
            }

        }


        private void Languages_LanguageChanged(object sender, LanguageFactory.Messaging.SupportedLanguage lang) {
            this.Title = lang.GetText(MsgCode.language);
            this.btnExit.Content = lang.GetMsg(MsgCode.exit).Display;
            this.btnSelect.Content = lang.GetText(MsgCode.select);
            this.btnCancel.Content = lang.GetText(MsgCode.cancel);

            // TODO Other texts

        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {

        }
    }
}
