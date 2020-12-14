using Ethernet.Common.Net.DataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Collections.Generic;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for DeviceSelect_Ethernet.xaml</summary>
    public partial class DeviceSelect_Ethernet : Window {

        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private List<IIndexItem<DefaultFileExtraInfo>> lstInfo = new List<IIndexItem<DefaultFileExtraInfo>>();

        #endregion

        #region Properties

        public EthernetParams EthernetInfoObject { get; set; }

        #endregion

        #region Constructor and windows events

        public static EthernetParams ShowBox(Window parent) {
            DeviceSelect_Ethernet win = new DeviceSelect_Ethernet(parent);
            win.ShowDialog();
            return win.EthernetInfoObject;
        }


        public DeviceSelect_Ethernet(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.Init();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnExit, this.btnSelect);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.ReloadList(true);
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }

        #endregion

        #region Button event handlers

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.EthernetInfoObject = null;
            this.Close();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.lbEthernetDevices.GetSelected<IIndexItem<DefaultFileExtraInfo>>(
                (info) => {
                    DI.Wrapper.RetrieveEthernetData(
                        info, 
                        (data) => {
                            this.EthernetInfoObject = data;
                            this.Close();
                        }, App.ShowMsg);
                }, () => {
                });
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            this.lbEthernetDevices.GetSelectedChk<IIndexItem<DefaultFileExtraInfo>>(
                (info) => {
                    this.ReloadList(DeviceEdit_Ethernet.ShowBoxEdit(this, info));
                }, App.ShowMsg);
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            this.lbEthernetDevices.GetSelectedChk<IIndexItem<DefaultFileExtraInfo>>(
                (info) => {
                    if (MsgBoxYesNo.ShowBoxDelete(this, info.Display) == MsgBoxYesNo.MsgBoxResult.Yes) {
                        DI.Wrapper.DeleteEthernetData(info, this.ReloadList, App.ShowMsg);
                    }
                }, App.ShowMsg);
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            this.ReloadList(DeviceEdit_Ethernet.ShowBoxAdd(this, null));
        }

        #endregion

        #region Private

        private void Init() {
            // Load any data here into list box
        }


        private void ReloadList(bool isChanged) {
            if (isChanged) {
                DI.Wrapper.GetEthernetDataList((newList) => {
                    lbEthernetDevices.SetNewSource(ref this.lstInfo, newList);
                }, App.ShowMsg);
            }
        }

        #endregion

    }
}
