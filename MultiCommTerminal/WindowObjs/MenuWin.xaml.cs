using IconFactory.Net.data;
using LanguageFactory.Net.data;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs.BLE;
using MultiCommTerminal.NetCore.WindowObjs.BTWins;
using MultiCommTerminal.NetCore.WindowObjs.EthernetWins;
using MultiCommTerminal.NetCore.WindowObjs.SerialWins;
using MultiCommTerminal.NetCore.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for MenuWin.xaml</summary>
    public partial class MenuWin : Window {

        #region Data

        private Window mainWindow = null;
        private List<MenuItemDataModel> items = new List<MenuItemDataModel>();
        private ICommWrapper wrapper = null;
        private RunPageManager runPageManager = null;

        #endregion

        #region Constructors and windows events

        public MenuWin(Window mainWindow) {
            this.wrapper = DI.Wrapper;
            this.mainWindow = mainWindow;
            this.runPageManager = new RunPageManager(this.mainWindow);
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
            this.runPageManager.CloseAll();
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
                    case MenuCode.Ethernet:
                        this.runPageManager.Open(typeof(EthernetRun));
                        break;
                    case MenuCode.Usb:
                        this.runPageManager.Open(typeof(SerialRun));
                        break;
                    case MenuCode.Wifi:
                        this.runPageManager.Open(typeof(WifiRun));
                        break;
                    case MenuCode.Bluetooth:
                        this.runPageManager.Open(typeof(BTRun));
                        break;
                    case MenuCode.BLE:
                        this.runPageManager.Open(typeof(BLE_Full));
                        break;
                    case MenuCode.CodeSamples:
                        Help_CommunicationMediums cm = new Help_CommunicationMediums(this.mainWindow);
                        cm.ShowDialog();
                        break;
                    case MenuCode.Settings:
                        MainSettings.ShowBox(this.mainWindow);
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
                this.AddItem(MenuCode.Bluetooth, "Bluetooth", UIIcon.BluetoothClassic, "0");
                this.AddItem(MenuCode.Wifi, "WIFI", UIIcon.Wifi, "0");
                this.AddItem(MenuCode.Usb, "USB", UIIcon.Usb, "0");
                this.AddItem(MenuCode.Ethernet, MsgCode.Ethernet, UIIcon.Ethernet, "0");
                this.AddItem(MenuCode.BLE, "BLE", UIIcon.BluetoothLE, "0");
                this.AddItem(MenuCode.Terminators, MsgCode.Terminators, UIIcon.Terminator, "0");
                this.AddItem(MenuCode.Commands, MsgCode.commands, UIIcon.Command, "0"); // TODO Get a new icon
                this.AddItem(MenuCode.Language, MsgCode.language, UIIcon.Language, "0");
                this.AddItem(MenuCode.CodeSamples, MsgCode.CodeSamples, UIIcon.Code, "0");
                this.AddItem(MenuCode.Settings, MsgCode.Settings, UIIcon.Settings, "0");
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
