using LanguageFactory.data;
using MultiCommTerminal.DependencyInjection;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for LanguagaSelector.xamlsummary>
    public partial class LanguageSelector : Window {

        #region Data

        private LangCode languageOnEntry = LangCode.English;
        private ICommWrapper wrapper = null;
        private bool cancelResized = false;
        private bool saveResized = false;

        #endregion

        #region Constructors and windows events

        public LanguageSelector() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // Connect to language event
            this.wrapper.LanguageChanged += Languages_LanguageChanged;
            this.wrapper.CurrentLanguage((code) => { this.languageOnEntry = code; });

            // Call before the SizeChanged event and rendering which will trigger initial resize events
            WpfHelperClasses.Core.WPF_ControlHelpers.ForceButtonMinMax(this.btnCancel, this.btnSave);
            this.btnCancel.SizeChanged += this.Button_SizeChanged;
            this.btnSave.SizeChanged += this.Button_SizeChanged;
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


        private void BrdTitleBorder_MouseDown(object sender, System.Windows.Input.MouseButtonEventArgs e) {
            this.DragMove();

        }

        #endregion

        #region Events from controls

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
                
                // Force min max values and connect SizeChanged event BEFORE content changes
                WpfHelperClasses.Core.WPF_ControlHelpers.ForceButtonMinMax(this.btnCancel, this.btnSave);
                this.btnCancel.SizeChanged += this.Button_SizeChanged;
                this.btnSave.SizeChanged += this.Button_SizeChanged;

                // Now change the content to fire the Size changed event
                this.btnSave.Content = lang.GetText(MsgCode.save);
                this.btnCancel.Content = lang.GetText(MsgCode.cancel);
                // Content changes trigger SizeChange events where we will synchronize size of both buttons
            });
        }


        private void Button_SizeChanged(object sender, SizeChangedEventArgs e) {
            Button b = sender as Button;
            if (b.Name == nameof(this.btnCancel)) {
                this.cancelResized = true;
            }
            if (b.Name == nameof(this.btnSave)) {
                this.saveResized = true;
            }
            if (this.cancelResized && this.saveResized) {
                this.saveResized = false;
                this.cancelResized = false;
                // Need to disconnect events to prevent recall loop on the resize
                this.btnCancel.SizeChanged -= Button_SizeChanged;
                this.btnSave.SizeChanged -= Button_SizeChanged;
                WpfHelperClasses.Core.WPF_ControlHelpers.ResizeToWidest(this.btnCancel, this.btnSave);
            }
        }


        #endregion


    }
}
