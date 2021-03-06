using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using MultiCommData.Net.Enumerations;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using SerialCommon.Net.DataModels;
using System;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.SerialWins {

    /// <summary>Interaction logic for SerialRun.xaml</summary>
    public partial class SerialRun : Window {


        private Window parent = null;
        private SerialDeviceInfo selectedSerial = null;

        public static int Instances { get; private set; }

        public event EventHandler<Type> CloseRequest;


        public SerialRun(Window parent) {
            this.parent = parent;
            Instances++;
            InitializeComponent();

            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;

            this.ui.ExitClicked += this.OnUiExit;
            this.ui.ConnectCicked += this.OnUiConnect;
            this.ui.DisconnectClicked += this.OnUiDisconnect;
            this.ui.SendClicked += this.OnUiSend;
            this.ui.InfoClicked += this.OnUiInfo;
            this.ui.SettingsClicked += this.OnUiSettings;
            DI.Wrapper.OnSerialConfigRequest += this.onSerialConfigRequest;
            DI.Wrapper.SerialOnError += this.onSerialError;
            DI.Wrapper.Serial_BytesReceived += this.bytesReceived;
            DI.Wrapper.OnSerialConnectionAttemptCompleted += this.onSerialConnectionAttemptCompleted;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.ui.OnLoad(this, CommMedium.Usb);
        }



        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.ui.ExitClicked -= this.OnUiExit;
            this.ui.ConnectCicked -= this.OnUiConnect;
            this.ui.DisconnectClicked -= this.OnUiDisconnect;
            this.ui.SendClicked -= this.OnUiSend;
            this.ui.InfoClicked -= this.OnUiInfo;
            this.ui.SettingsClicked -= this.OnUiSettings;
            DI.Wrapper.OnSerialConnectionAttemptCompleted -= this.onSerialConnectionAttemptCompleted;
            DI.Wrapper.OnSerialConfigRequest -= this.onSerialConfigRequest;
            DI.Wrapper.SerialOnError -= this.onSerialError;
            DI.Wrapper.Serial_BytesReceived -= this.bytesReceived;
            this.ui.OnClosing();
            Instances--;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        #region Serial event handlers

        private void bytesReceived(object sender, string msg) {
            this.ui.AddResponse(msg);
        }


        private void onSerialConnectionAttemptCompleted(object sender, MsgPumpResults e) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                if (e.Code == MsgPumpResultCode.Connected) {
                    this.ui.SetConnected();
                }
                else {
                    App.ShowMsg(string.Format("{0} '{1}'", e.Code, e.ErrorString));
                }
            });
        }


        private void onSerialError(object sender, SerialUsbError e) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                App.ShowMsgTitle(e.PortName, string.Format("{0} '{1}'", e.Code, e.PortName, e.Message));
            });
        }


        /// <summary>Raises popup if no saved Serial configurations for port</summary>
        private void onSerialConfigRequest(object sender, SerialDeviceInfo e) {
            this.Dispatcher.Invoke(() => {
                this.ui.IsBusy = false;
                DeviceEdit_USB.ShowBox(this, e);
                this.ui.IsBusy = true;
            });
        }

        #endregion

        #region UI Event handlers

        private void OnUiExit(object sender, EventArgs e) {
            this.CloseRequest?.Invoke(this, typeof(SerialRun));
        }


        private void OnUiConnect(object sender, EventArgs e) {
            if (this.selectedSerial == null) {
                this.DoDiscovery();
            }

            if (this.selectedSerial != null) {
                this.ui.IsBusy = true;
                DI.Wrapper.SerialUsbConnect(this.selectedSerial, this.OnConnectError);
            }
        }


        private void OnUiDisconnect(object sender, EventArgs e) {
            DI.Wrapper.SerialUsbDisconnect();
            this.selectedSerial = null;
        }


        private void OnUiSend(object sender, string msg) {
            DI.Wrapper.SerialUsbSend(msg);
        }


        private void OnUiInfo(object sender, EventArgs e) {
            if (this.selectedSerial != null) {
                DeviceInfo_USB.ShowBox(this, this.selectedSerial);
            }
        }


        private void OnUiSettings(object sender, EventArgs e) {
            SerialSelectUSB.ShowBox(this, false);
        }

        #endregion

        #region Delegates

        private void OnConnectError(string err) {
            this.ui.IsBusy = false;
            App.ShowMsg(err);
            this.selectedSerial = null;
        }


        private void DoDiscovery() {
            this.Title = "USB";
            this.selectedSerial = SerialSelectUSB.ShowBox(this, true);
            if (this.selectedSerial != null) {
                this.Title = this.selectedSerial.Display;
            }
        }

        #endregion

    }
}
