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
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.LoadList();
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
                        // TODO - settings window
                        break;
                    case MenuCode.Commands:
                        Commands cmds = new Commands();
                        cmds.ShowDialog();
                        this.Close();
                        break;
                    default:
                        // Not supported
                        break;
                }
            }
        }


        private void Languages_LanguageChanged(object sender, LanguageFactory.Messaging.SupportedLanguage language) {
            this.Dispatcher.Invoke(this.LoadList);
        }


        private void LoadList() {
            this.Dispatcher.Invoke(() => {
                // Disable selection changed event during load
                lbxMenuItems.SelectionChanged -= this.lbxMenuItems_SelectionChanged;
                this.lbxMenuItems.ItemsSource = null;
                this.items.Clear();
                this.AddItem(MenuCode.Language, MsgCode.language, UIIcon.Language, "2");
                this.AddItem(MenuCode.Settings, MsgCode.Settings, UIIcon.Settings, "4");
                this.AddItem(MenuCode.Commands, MsgCode.command, UIIcon.Add, "2"); // TODO Get a new icon

                this.lbxMenuItems.ItemsSource = this.items;
                lbxMenuItems.SelectionChanged += this.lbxMenuItems_SelectionChanged;
            });
        }


        private void AddItem(MenuCode menuCode, MsgCode msgCode, UIIcon uIIcon, string padding) {
            this.wrapper.GetMenuItemDataModel(menuCode, msgCode, uIIcon, padding,
                (dm) => { this.items.Add(dm); }, (dm) => { this.items.Add(dm); });
        }

        #endregion

    }
}
