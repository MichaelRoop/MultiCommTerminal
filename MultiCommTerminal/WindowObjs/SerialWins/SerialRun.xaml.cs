using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using SerialCommon.Net.DataModels;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.SerialWins {

    /// <summary>Interaction logic for SerialRun.xaml</summary>
    public partial class SerialRun : Window {


        private Window parent = null;
        private SerialDeviceInfo selectedSerial = null;


        public SerialRun(Window parent) {
            this.parent = parent;
            InitializeComponent();

            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;

            this.ui.ExitClicked += this.OnUiExit;
            this.ui.ConnectCicked += this.OnUiConnect;
            this.ui.DiscoverClicked += this.OnUiDiscover;
            this.ui.DisconnectClicked += this.OnUiDisconnect;
            this.ui.SendClicked += this.OnUiSend;
            DI.Wrapper.OnSerialConfigRequest += this.onSerialConfigRequest;
            DI.Wrapper.SerialOnError += this.onSerialError;
            DI.Wrapper.Serial_BytesReceived += this.bytesReceived;
            DI.Wrapper.OnSerialConnectionAttemptCompleted += this.onSerialConnectionAttemptCompleted;
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            this.ui.OnLoad(this.parent);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.ui.ExitClicked -= this.OnUiExit;
            this.ui.ConnectCicked -= this.OnUiConnect;
            this.ui.DiscoverClicked -= this.OnUiDiscover;
            this.ui.DisconnectClicked -= this.OnUiDisconnect;
            this.ui.SendClicked -= this.OnUiSend;
            DI.Wrapper.OnSerialConnectionAttemptCompleted -= this.onSerialConnectionAttemptCompleted;
            DI.Wrapper.OnSerialConfigRequest -= this.onSerialConfigRequest;
            DI.Wrapper.SerialOnError -= this.onSerialError;
            DI.Wrapper.Serial_BytesReceived -= this.bytesReceived;
            this.ui.OnClosing();
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
            this.Close();
        }

        private void OnUiDiscover(object sender, EventArgs e) {
            this.Title = "USB";
            this.selectedSerial = SerialSelectUSB.ShowBox(this);
            if (this.selectedSerial != null) {
                this.Title = this.selectedSerial.Name;
            }
        }


        private void OnUiConnect(object sender, EventArgs e) {
            if (this.selectedSerial == null) {
                this.OnUiDiscover(sender, e);
            }
            if (this.selectedSerial != null) {
                this.ui.IsBusy = true;
                DI.Wrapper.SerialUsbConnect(
                    this.selectedSerial,
                    (err) => {
                        this.ui.IsBusy = false;
                        App.ShowMsg(err);
                    });
            }
        }


        private void OnUiDisconnect(object sender, EventArgs e) {
            DI.Wrapper.SerialUsbDisconnect();
        }


        private void OnUiSend(object sender, string msg) {
            DI.Wrapper.SerialUsbSend(msg);
        }

        #endregion

    }
}
