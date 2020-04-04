using IconFactory.data;
using LanguageFactory.data;
using LanguageFactory.interfaces;
using LanguageFactory.Messaging;
using MultiCommData.UserDisplayData;
using MultiCommTerminal.Data;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for MenuWin.xaml</summary>
    public partial class MenuWin : Window {

        #region Data

        List<MenuItemDataModel> items = new List<MenuItemDataModel>();
        ILangFactory languageFactory = null;

        #endregion

        #region Constructors and windows events

        public MenuWin() {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // TODO - move to wrapper
            //this.languageFactory = App.Languages;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            // TODO - replace with calls to wrapper
            this.LoadList(DI.GetObj<ILangFactory>().CurrentLanguage);
            DI.Wrapper().LanguageChanged += this.Languages_LanguageChanged;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            lbxMenuItems.SelectionChanged -= this.lbxMenuItems_SelectionChanged;
            DI.Wrapper().LanguageChanged -= this.Languages_LanguageChanged;
        }

        #endregion

        #region List box items mangement

        private void lbxMenuItems_SelectionChanged(object sender, SelectionChangedEventArgs args) {
            MenuItemDataModel item = this.lbxMenuItems.SelectedItem as MenuItemDataModel;
            if (item != null) {
                this.Hide();
                switch (item.Code) {
                    case Data.MenuCode.Language:
                        LanguageSelector win = new LanguageSelector();
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

                // Move to wrapper?
                // TODO - entry for Settings in language modules
                this.items.Add(this.GetMenuDM(MenuCode.Language, lang.GetText(MsgCode.language), BindFetcher.IconLanguageDM, "2"));
                this.items.Add(this.GetMenuDM(MenuCode.Settings, "Settings" /*lang.GetText(MsgCode.)*/, BindFetcher.IconSettingsDM, "4"));

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
