using LanguageFactory.data;
using LanguageFactory.interfaces;
using LanguageFactory.Messaging;
using MultiCommData.UserDisplayData;
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

    /// <summary>Interaction logic for MenuWin.xaml</summary>
    public partial class MenuWin : Window {

        List<MenuItemDataModel> items = new List<MenuItemDataModel>();
        ILangFactory languageFactory = null;

        public MenuWin() {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // TODO - move to wrapper
            this.languageFactory = App.Languages;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            this.LoadList(App.Languages.CurrentLanguage);
            App.Languages.LanguageChanged += this.Languages_LanguageChanged;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            lbxMenuItems.SelectionChanged -= this.lbxMenuItems_SelectionChanged;
            App.Languages.LanguageChanged -= this.Languages_LanguageChanged;
        }



        #region List box items mangement

        private void lbxMenuItems_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            MenuItemDataModel item = this.lbxMenuItems.SelectedItem as MenuItemDataModel;
            if (item != null) {
                this.Hide();
                switch (item.Code) {
                    case Data.MenuCode.Language:
                        LanguageSelector win = new LanguageSelector(App.Languages);
                        win.ShowDialog();
                        break;
                    case Data.MenuCode.Settings:
                        break;
                    default:
                        // Not supported
                        break;
                }
            }
        }


        private void Languages_LanguageChanged(object sender, LanguageFactory.Messaging.SupportedLanguage language) {
            this.Dispatcher.Invoke(() => {
                this.LoadList(language);
            });
        }


        private void LoadList(SupportedLanguage lang) {
            this.Dispatcher.Invoke(() => {
                // Disable selection changed event during load
                lbxMenuItems.SelectionChanged -= this.lbxMenuItems_SelectionChanged;

                this.lbxMenuItems.ItemsSource = null;
                this.items.Clear();

                // Move the filling of list to wrapper
                this.items.Add(new MenuItemDataModel(Data.MenuCode.Language, lang.GetText(MsgCode.language)));
                this.items.Add(new MenuItemDataModel(Data.MenuCode.Settings, "Settings"));

                this.lbxMenuItems.ItemsSource = this.items;
                lbxMenuItems.SelectionChanged += this.lbxMenuItems_SelectionChanged;

            });
        }

        #endregion

    }
}
