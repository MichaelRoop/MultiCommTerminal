﻿using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for DeviceEdit_Ethernet.xaml</summary>
    public partial class DeviceEdit_Ethernet : Window {

        #region Data

        private enum Mode {
            Edit,
            Create,
        }

        private Mode mode = Mode.Edit;
        private Window parent = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private IIndexItem<EthernetExtraInfo> index = null;
        private EthernetParams data = new ();

        #endregion

        #region Properties

        public bool IsChanged { get; set; } = false;

        #endregion

        #region Constructors and windows events

        public static bool ShowBoxEdit(Window parent, IIndexItem<EthernetExtraInfo> index) {
            DeviceEdit_Ethernet win = new (parent, index);
            win.ShowDialog();
            return win.IsChanged;
        }


        public static bool ShowBoxAdd(Window parent, IIndexItem<EthernetExtraInfo> index) {
            return ShowBoxEdit(parent, null);
        }



        public DeviceEdit_Ethernet(Window parent, IIndexItem<EthernetExtraInfo> index) {
            this.parent = parent;
            this.index = index;
            this.mode = this.index == null ? Mode.Create : Mode.Edit;
            InitializeComponent();
            this.Init();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSave);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.widthManager.Teardown();
        }

        #endregion

        #region Button event handlers

        private void btnSave_Click(object sender, RoutedEventArgs e) {
            this.data.Display = this.txtName.Text;
            this.data.EthernetAddress = this.txtHostName.Text;
            this.data.EthernetServiceName = this.txtServiceName.Text;

            switch (this.mode) {
                case Mode.Edit:
                    DI.Wrapper.SaveEthernetData(
                        this.index, this.data, this.OnSaveOk, this.OnFailure);
                    break;
                case Mode.Create:
                    DI.Wrapper.CreateNewEthernetData(this.data, this.OnSaveOk, this.OnFailure);
                    break;
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.IsChanged = false;
            this.Close();
        }

        #endregion

        #region Private

        private void Init() {
            switch (this.mode) {
                case Mode.Edit:
                    DI.Wrapper.RetrieveEthernetData(this.index,
                        (data) => {
                            this.data = data;
                            this.Title = DI.Wrapper.GetText(MsgCode.Edit);
                        },
                        this.OnFailure);
                    break;
                case Mode.Create:
                    this.data.Display = "SampleName";
                    this.data.EthernetAddress = "192.168.1.100";
                    this.data.EthernetServiceName = "9999";
                    this.Title = DI.Wrapper.GetText(MsgCode.Create);
                    break;
            }

            this.txtName.Text = this.data.Display;
            this.txtHostName.Text = this.data.EthernetAddress;
            this.txtServiceName.Text = this.data.EthernetServiceName;
        }


        private void OnSaveOk() {
            this.IsChanged = true;
            this.Close();
        }


        private void OnFailure(string err) {
            App.ShowMsg(err);
            this.Close();
        }

        #endregion

    }
}
