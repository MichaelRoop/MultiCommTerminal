using LanguageFactory.data;
using MultiCommTerminal.DependencyInjection;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for LanguagaSelector.xamlsummary>
    public partial class LanguageSelector : Window {

        #region Data

        private LangCode languageOnEntry = LangCode.English;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #endregion

        #region Constructors and windows events

        public LanguageSelector() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // Connect to language event
            this.wrapper.LanguageChanged += Languages_LanguageChanged;
            this.wrapper.CurrentLanguage((code) => { this.languageOnEntry = code; });

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSave);
            this.widthManager.PrepForChange();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.wrapper.LanguageList((items) => {
                this.lbxLanguages.ItemsSource = items;
                // Only create the selected index here to avoid it firing on load
                this.lbxLanguages.SelectionChanged += this.lbLanguages_SelectionChanged;
            });
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbxLanguages.SelectionChanged -= this.lbLanguages_SelectionChanged;
            this.wrapper.LanguageChanged -= this.Languages_LanguageChanged;
            this.widthManager.Teardown();
        }


        private void BrdTitleBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.DragMove();

        }

        #endregion

        #region Events from controls

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            this.wrapper.CurrentLanguage(this.SaveIfDifferent);
            this.Close();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.wrapper.CurrentLanguage(this.RevertIfDifferent);
            this.Close();
        }


        private void lbLanguages_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            LanguageDataModel data = this.lbxLanguages.SelectedItem as LanguageDataModel;
            if (data != null) {
                this.wrapper.SetLanguage(data.Code);
            }
        }


        private void Languages_LanguageChanged(object sender, LanguageFactory.Messaging.SupportedLanguage lang) {
            this.Dispatcher.Invoke(() => { 
                this.lbTitle.Content = lang.GetText(MsgCode.language);

                // Prep for change then change button contents
                this.widthManager.PrepForChange();
                this.btnSave.Content = lang.GetText(MsgCode.save);
                this.btnCancel.Content = lang.GetText(MsgCode.cancel);
            });
        }

        #endregion

        #region Helpers

        private void RevertIfDifferent(LangCode code) {
            if (this.languageOnEntry != code) {
                this.wrapper.SetLanguage(this.languageOnEntry);
            }
        }

        private void SaveIfDifferent(LangCode code) {
            if (this.languageOnEntry != code) {
                this.wrapper.SaveLanguage(code, (err) => { MessageBox.Show(err); });
            }
        }


        #endregion

    }
}
