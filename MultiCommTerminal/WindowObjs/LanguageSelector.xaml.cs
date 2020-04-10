using LanguageFactory.data;
using LogUtils.Net;
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

        #endregion

        #region Constructors and windows events

        public LanguageSelector() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Called before the SizeChanged event on the buttons which will call the initial resize
            WpfHelperClasses.Core.WPF_ControlHelpers.ForceButtonMinMax(this.btnCancel, this.btnSave);

            // Move forward stuff to wrapper
            this.wrapper.LanguageChanged += Languages_LanguageChanged;
            this.wrapper.CurrentLanguage((code) => { this.languageOnEntry = code; });

            this.btnCancel.SizeChanged += BtnCancel_SizeChanged;
            this.btnSave.SizeChanged += BtnSave_SizeChanged;

            this.btnCancel.DataContextChanged += BtnCancel_DataContextChanged;

        }

        private void BtnCancel_DataContextChanged(object sender, DependencyPropertyChangedEventArgs e) {
            //throw new NotImplementedException();

            Log.Error(34, "DATA CONTEXT CHANGED - Cancel");

        }

        bool cancelResized = false;
        bool saveResized = false;

        private void BtnSave_SizeChanged(object sender, SizeChangedEventArgs e) {
            this.saveResized = true;
            if (this.cancelResized) {
                this.DoButtonResize();
            }
            else {
                Log.Error(1, "SaveResized - single");
            }
        }


        private void BtnCancel_SizeChanged(object sender, SizeChangedEventArgs e) {
            this.cancelResized = true;
            if (this.saveResized) {
                this.DoButtonResize();
            }
            else {
                Log.Error(1, "CancelResized - single");
            }
        }

        private void DoButtonResize() {
            Log.Error(10, "ButtonResize");
            this.saveResized = false;
            this.cancelResized = false;
            Log.Error(11, "");
            Log.Error(11, () => string.Format("{0} | {1} - {2}|{3}",
                btnCancel.Width, btnCancel.ActualWidth, btnSave.Width, btnSave.ActualWidth));

            // Need to disconnect events to prevent recall loop on the resize
            this.btnCancel.SizeChanged -= BtnCancel_SizeChanged;
            this.btnSave.SizeChanged -= BtnSave_SizeChanged;
            WpfHelperClasses.Core.WPF_ControlHelpers.ResizeToWidest(this.btnCancel, this.btnSave);
        }



        private void Window_ContentRendered(object sender, EventArgs e) {
            this.wrapper.LanguageList((items) => {
                this.lbLanguages.ItemsSource = items;
                // Only create the selected index here to avoid it firing on load
                this.lbLanguages.SelectionChanged += this.lbLanguages_SelectionChanged;
            });
            //WpfHelperClasses.Core.WPF_ControlHelpers.ResizeToWidest(this.btnCancel, this.btnSave);
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
                this.btnCancel.SizeChanged += BtnCancel_SizeChanged;
                this.btnSave.SizeChanged += BtnSave_SizeChanged;

                // Now change the content to fire the Size changed event
                this.btnSave.Content = lang.GetText(MsgCode.save);
                this.btnCancel.Content = lang.GetText(MsgCode.cancel);
            });
        }

        #endregion


    }
}
