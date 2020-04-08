using IconFactory.data;
using LanguageFactory.data;
using LanguageFactory.interfaces;
using LanguageFactory.Messaging;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for MenuWin.xaml</summary>
    public partial class MenuWin : Window {

        #region Data

        List<MenuItemDataModel> items = new List<MenuItemDataModel>();
        ICommWrapper wrapper = null;

        #endregion

        #region Constructors and windows events

        public MenuWin() {
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // TODO - move to wrapper
            //this.languageFactory = App.Languages;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            // TODO - replace with calls to wrapper
            this.LoadList(DI.GetObj<ILangFactory>().CurrentLanguage);
            this.wrapper.LanguageChanged += this.Languages_LanguageChanged;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            lbxMenuItems.SelectionChanged -= this.lbxMenuItems_SelectionChanged;
            this.wrapper.LanguageChanged -= this.Languages_LanguageChanged;
        }

        #endregion

        #region List box items mangement

        private void lbxMenuItems_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            MenuItemDataModel item = this.lbxMenuItems.SelectedItem as MenuItemDataModel;
            if (item != null) {
                this.Hide();
                switch (item.Code) {
                    case MenuCode.Language:
                        LanguageSelector win = new LanguageSelector();
                        win.ShowDialog();
                        break;
                    case MenuCode.Settings:
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

                // Move to wrapper?
                // TODO - entry for Settings in language modules
                this.items.Add(this.GetMenuDM(MenuCode.Language, lang.GetText(MsgCode.language), IconBinder.LanguageDM, "2"));
                this.items.Add(this.GetMenuDM(MenuCode.Settings, lang.GetText(MsgCode.Settings), IconBinder.SettingsDM, "4"));

                this.lbxMenuItems.ItemsSource = this.items;
                lbxMenuItems.SelectionChanged += this.lbxMenuItems_SelectionChanged;
            });
        }


        MenuItemDataModel GetMenuDM(MenuCode menuCode, string display, IconDataModel idm, string padding) {
            return new MenuItemDataModel() {
                Code = menuCode,
                Display = display,
                IconSource = idm.IconSource as string,
                Padding = padding,
            };
        }

        #endregion

    }
}
