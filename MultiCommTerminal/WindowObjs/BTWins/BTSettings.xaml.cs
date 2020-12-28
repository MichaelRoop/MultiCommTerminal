﻿using BluetoothCommon.Net;
using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using MultiCommWrapper.Net.DataModels;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.BTWins {

    /// <summary>Interaction logic for BTSettings.xaml</summary>
    public partial class BTSettings : Window {

        Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #region Constructors and window events

        public static void ShowBox(Window parent) {
            BTSettings win = new BTSettings(parent);
            win.ShowDialog();
        }


        public BTSettings(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.btnCmdsHc05.Content = string.Format("{0} (HC-05)", DI.Wrapper.GetText(LanguageFactory.Net.data.MsgCode.commands));
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.widthManager = new ButtonGroupSizeSyncManager(
                this.btnPair, this.btnUnpair, this.btnExit, this.btnCmdsHc05);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            DI.Wrapper.BT_PairStatus -= this.pairStatusHandler;
            DI.Wrapper.BT_UnPairStatus -= this.unPairStatusHandler;
            DI.Wrapper.BT_PairInfoRequested -= this.pairInfoRequested;

            this.widthManager.Teardown();
        }

        #endregion

        #region Button events

        private void btnPair_Click(object sender, RoutedEventArgs e) {
            BTDeviceInfo device = BTSelect.ShowBox(this, false);
            if (device != null) {
                var result = MsgBoxYesNo.ShowBox(
                    this, DI.Wrapper.GetText(LanguageFactory.Net.data.MsgCode.Pair), device.Name);
                if (result == MsgBoxYesNo.MsgBoxResult.Yes) {
                    this.gridWait.Show();
                    DI.Wrapper.BT_PairStatus -= this.pairStatusHandler;
                    DI.Wrapper.BT_PairStatus += this.pairStatusHandler;
                    DI.Wrapper.BT_PairInfoRequested -= this.pairInfoRequested;
                    DI.Wrapper.BT_PairInfoRequested += this.pairInfoRequested;
                    DI.Wrapper.BTClassicPairAsync(device);
                }
            }
        }


        private void btnUnpair_Click(object sender, RoutedEventArgs e) {
            BTDeviceInfo device = BTSelect.ShowBox(this, true);
            if (device != null) {
                var result = MsgBoxYesNo.ShowBox(
                    this, DI.Wrapper.GetText(LanguageFactory.Net.data.MsgCode.Unpair), device.Name);
                if (result == MsgBoxYesNo.MsgBoxResult.Yes) {
                    this.gridWait.Show();
                    DI.Wrapper.BT_UnPairStatus -= this.unPairStatusHandler;
                    DI.Wrapper.BT_UnPairStatus += this.unPairStatusHandler;
                    DI.Wrapper.BTClassicUnPairAsync(device);
                }
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        #endregion

        #region Wrapper event handlers

        private void unPairStatusHandler(object sender, BTUnPairOperationStatus e) {
            this.Dispatcher.Invoke(() => {
                Log.InfoEntry("BTSettings", "unPairStatusHandler");
                this.gridWait.Collapse();
                DI.Wrapper.BT_UnPairStatus -= this.unPairStatusHandler;
                if (!e.IsSuccessful) {
                    App.ShowMsg(e.UnpairStatus.ToString());
                }
            });
        }


        private void pairStatusHandler(object sender, BTPairOperationStatus e) {
            this.Dispatcher.Invoke(() => {
                Log.InfoEntry("BTSettings", "pairStatusHandler");
                this.gridWait.Collapse();
                DI.Wrapper.BT_PairStatus -= this.pairStatusHandler;
                if (!e.IsSuccessful) {
                    App.ShowMsg(e.PairStatus.ToString());
                }
            });
        }


        private void pairInfoRequested(object sender, BT_PairingInfoDataModel info) {
            this.Dispatcher.Invoke(() => {
                this.gridWait.Collapse();
                if (info.IsPinRequested) {
                    var result = MsgBoxEnterText.ShowBox(
                        this, info.RequestTitle, info.RequestMsg);
                    info.PIN = result.Text;
                    info.HasUserConfirmed = 
                        (result.Result == MsgBoxEnterText.MsgBoxTextInputResult.OK);
                }
                else {
                    MsgBoxYesNo.MsgBoxResult result2 = 
                        MsgBoxYesNo.ShowBox(this, info.RequestMsg, info.RequestMsg);
                    info.HasUserConfirmed = (result2 == MsgBoxYesNo.MsgBoxResult.Yes);
                }
            });
        }

        #endregion

        private void btnCmdsHc05_Click(object sender, RoutedEventArgs e) {
            if (MsgBoxYesNo.ShowBox(
                this, DI.Wrapper.GetText(MsgCode.Create), 
                string.Format("{0} (HC-05)", DI.Wrapper.GetText(MsgCode.commands))) == MsgBoxYesNo.MsgBoxResult.Yes) {
                DI.Wrapper.CreateHC05AtCmds(() => { }, App.ShowMsg);
            }


        }
    }
}
