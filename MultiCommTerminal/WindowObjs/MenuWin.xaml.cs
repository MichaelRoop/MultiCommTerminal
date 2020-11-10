using IconFactory.Net.data;
using LanguageFactory.Net.data;
using LanguageFactory.Net.interfaces;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs;
using MultiCommTerminal.NetCore.WindowObjs.EthernetWins;
using MultiCommTerminal.NetCore.WindowObjs.SerialWins;
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

        private Window mainWindow = null;
        private List<MenuItemDataModel> items = new List<MenuItemDataModel>();
        private ICommWrapper wrapper = null;

        #endregion

        #region Constructors and windows events

        public MenuWin(Window mainWindow) {
            this.wrapper = DI.Wrapper;
            this.mainWindow = mainWindow;
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
                        LanguageSelector.ShowBox(this.mainWindow);
                        break;
                    case MenuCode.Terminators:
                        TerminatorDataSelector.ShowBox(this.mainWindow);
                        break;
                    case MenuCode.Commands:
                        Commands cmds = new Commands(this.mainWindow);
                        cmds.ShowDialog();
                        break;
                    case MenuCode.Credentials:
                        WifiCredentialsWin.ShowBox(this.mainWindow);
                        break;
                    case MenuCode.UsbConfig:
                        DeviceSelect_USB.ShowBox(this.mainWindow);
                        break;
                    case MenuCode.Ethernet:
                        DeviceSelect_Ethernet.ShowBox(this.mainWindow);
                        break;
                    case MenuCode.Settings:
                        // TODO - settings window
                        break;
                    case MenuCode.About:
                        AboutWin.ShowBox(this.mainWindow);
                        break;
                    default:
                        // Not supported
                        break;
                }

                this.lbxMenuItems.SelectionChanged -= this.lbxMenuItems_SelectionChanged;
                this.Hide();
                this.lbxMenuItems.UnselectAll();
                lbxMenuItems.SelectionChanged += this.lbxMenuItems_SelectionChanged;

            }
        }


        private void Languages_LanguageChanged(object sender, LanguageFactory.Net.Messaging.SupportedLanguage language) {
            this.Dispatcher.Invoke(this.LoadList);
        }


        private void LoadList() {
            this.Dispatcher.Invoke(() => {
                // Disable selection changed event during load
                lbxMenuItems.SelectionChanged -= this.lbxMenuItems_SelectionChanged;
                this.lbxMenuItems.ItemsSource = null;
                this.items.Clear();
                this.AddItem(MenuCode.Language, MsgCode.language, UIIcon.Language, "0");
                this.AddItem(MenuCode.Terminators, MsgCode.Terminators, UIIcon.Terminator, "0");
                this.AddItem(MenuCode.Commands, MsgCode.command, UIIcon.Command, "0"); // TODO Get a new icon
                this.AddItem(MenuCode.Credentials, MsgCode.Credentials, UIIcon.Credentials, "0");
                this.AddItem(MenuCode.UsbConfig, string.Format("USB {0}", DI.Wrapper.GetText(MsgCode.Settings)), UIIcon.Usb, "0");  // Need 
                this.AddItem(MenuCode.Ethernet, "Ethernet", UIIcon.Ethernet, "0");
                //this.AddItem(MenuCode.Settings, MsgCode.Settings, UIIcon.Settings, "0");
                this.AddItem(MenuCode.About, MsgCode.About, UIIcon.About, "1");

                this.lbxMenuItems.ItemsSource = this.items;
                lbxMenuItems.SelectionChanged += this.lbxMenuItems_SelectionChanged;
            });
        }


        private void AddItem(MenuCode menuCode, MsgCode msgCode, UIIcon uIIcon, string padding) {
            this.AddItem(menuCode, DI.Wrapper.GetText(msgCode), uIIcon, padding);
        }


        private void AddItem(MenuCode menuCode, string display, UIIcon uIIcon, string padding) {
            MenuItemDataModel dataModel = new MenuItemDataModel() {
                Code = menuCode,
                Display = display,
                IconSource = IconBinder.Source(uIIcon),
                Padding = padding,
            };
            this.items.Add(dataModel);
        }


        #endregion

    }
}
