using Ethernet.Common.Net.DataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using MultiCommWrapper.Net.DataModels;
using System.Collections.Generic;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for DeviceSelect_Ethernet.xaml</summary>
    public partial class DeviceSelect_Ethernet : Window {

        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private List<EthernetDisplayDataModel> lstInfo = new List<EthernetDisplayDataModel>();

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
            this.lvEthernetDevices.GetSelected<EthernetDisplayDataModel>(
                (info) => {
                    DI.Wrapper.RetrieveEthernetData(
                        info.Index, 
                        (data) => {
                            this.EthernetInfoObject = data;
                            this.Close();
                        }, App.ShowMsg);
                }, () => {
                });
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            this.lvEthernetDevices.GetSelectedChk<EthernetDisplayDataModel>(
                (info) => {
                    this.ReloadList(DeviceEdit_Ethernet.ShowBoxEdit(this, info.Index));
                }, App.ShowMsg);
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            this.lvEthernetDevices.GetSelectedChk<EthernetDisplayDataModel>(
                (info) => {
                    DI.Wrapper.DeleteEthernetData(
                        info.Index, info.Name, this.DeleteDecision, this.ReloadList, App.ShowMsg);
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
                DI.Wrapper.GetEthernetDataList(this.ListLoad, App.ShowMsg);
            }
        }


        private void ListLoad(List<EthernetDisplayDataModel> newList) {
            lvEthernetDevices.SetNewSource(ref this.lstInfo, newList);
        }


        private bool DeleteDecision(string name) {
            return MsgBoxYesNo.ShowBoxDelete(this, name) == MsgBoxYesNo.MsgBoxResult.Yes;
        }



        #endregion

    }
}
