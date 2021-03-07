using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using MultiCommWrapper.Net.DataModels;
using StorageFactory.Net.interfaces;
using System.Collections.Generic;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for DeviceSelect_Ethernet.xaml</summary>
    public partial class DeviceSelect_Ethernet : Window {

        #region Data

        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private List<IIndexItem<EthernetExtraInfo>> lstInfo = new List<IIndexItem<EthernetExtraInfo>>();

        #endregion

        #region Properties

        public EthernetSelectResult SelectedEthernet { get; private set; } = null;



        #endregion

        #region Constructor and windows events

        public static EthernetSelectResult ShowBox(Window parent, bool isSelect) {
            DeviceSelect_Ethernet win = new DeviceSelect_Ethernet(parent, isSelect);
            win.ShowDialog();
            return win.SelectedEthernet;
        }


        public DeviceSelect_Ethernet(Window parent, bool isSelect) {
            this.parent = parent;
            InitializeComponent();
            this.Init();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
            if (!isSelect) {
                this.btnCancel.Collapse();
                this.btnSelect.Collapse();
                this.btnExit.Show();
            }
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
            this.SelectedEthernet = null;
            this.Close();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            this.lvEthernetDevices.GetSelected<EthernetDisplayDataModel>(
                (info) => {
                    DI.Wrapper.RetrieveEthernetData(
                        info.Index, 
                        (data) => {
                            this.SelectedEthernet = new EthernetSelectResult() {
                                Index = info.Index,
                                DataModel = data,
                            };
                            this.Close();
                        }, App.ShowMsg);
                }, () => {
                });
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            this.lvEthernetDevices.GetSelectedChk<IIndexItem<EthernetExtraInfo>>(
                (info) => {
                    this.ReloadList(DeviceEdit_Ethernet.ShowBoxEdit(this, info));
                }, App.ShowMsg);
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            this.lvEthernetDevices.GetSelectedChk<IIndexItem<EthernetExtraInfo>>(
                (info) => {
                    DI.Wrapper.DeleteEthernetData(
                        info, info.Display, this.DeleteDecision, this.ReloadList, App.ShowMsg);
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


        private void ListLoad(List<IIndexItem<EthernetExtraInfo>> newList) {
            //this.lstInfo[0].ExtraInfoObj.Address
            lvEthernetDevices.SetNewSource(ref this.lstInfo, newList);
        }


        private bool DeleteDecision(string name) {
            return MsgBoxYesNo.ShowBoxDelete(this, name) == MsgBoxYesNo.MsgBoxResult.Yes;
        }



        #endregion

    }
}
