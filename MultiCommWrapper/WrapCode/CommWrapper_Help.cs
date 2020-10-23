using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using MultiCommData.Net.UserDisplayData;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {


        public void CommMediumHelpList(Action<List<CommMediumHelp>> onSuccess) {
            List<CommMediumHelp> helps = new List<CommMediumHelp>();
            this.CommHelpList((list) => {
                foreach (CommHelpDisplay item in list) {
                    helps.Add(new CommMediumHelp() {
                        Id =  item.HelpType,
                        Title = item.Display,
                        Icon = item.IconSource,
                        Text = "N/A",
                        Code = "{\n  N/A\n}",
                    });
                }
            });

            // TODO Add specific text and code here from storage
            foreach (var i in helps) {
                switch (i.Id) {
                    case CommHelpType.Bluetooth:
                        i.Text = "Communicate with device using traditional Bluetooth like Arduino with HC50 chip";
                        break;
                    case CommHelpType.BluetoothLE:
                        i.Text = "Communicate with devices on BLE with 2 characteristics defined for serial input and output";
                        break;
                    case CommHelpType.Wifi:
                        i.Text = "Communicate with devices using WIFI to send commands and receive responses";
                        break;
                    case CommHelpType.Ethernet:
                        i.Text = "Communicate with devices using a hard wired ethernet connection to send commands and receive responses";
                        break;
                    case CommHelpType.Usb:
                        i.Text = "Communicate with devices using a hard wired USB serial connection to send commands and receive responses";
                        break;
                    case CommHelpType.Application:
                        i.Text = "User Document";
                        break;

                        // TODO - others
                }
            }

            onSuccess(helps);

        }
    

        public void HasCodeSample(CommHelpType helpType, Action<CommHelpType> onSuccess, OnErrTitle onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999,
                () => string.Format(""),
                () => {
                    string tmp = this.GetFilename(helpType);
                    if (tmp.Length > 0) {
                        onSuccess(helpType);
                    }
                    else {
                        onError(this.GetText(MsgCode.Error), this.GetText(MsgCode.LoadFailed));
                    }
                });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => {
                    onError.Invoke(this.GetText(MsgCode.Error), report.Msg);
                });
            }
        }


        public void GetCodeSample(CommHelpType helpType, Action<string> onSuccess, OnErrTitle onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999,
                () => string.Format(""),
                () => {
                    string filename = this.GetFilename(helpType);
                    if (filename.Length > 0) {
                        // TODO - Move to cross platform access
                        if (File.Exists(filename)) {
                            onSuccess.Invoke(File.ReadAllText(filename));
                            return;
                        }
                    }
                    onError(this.GetText(MsgCode.Error), "* N/Ax *");
                });
            if (report.Code!= 0) {
                WrapErr.SafeAction(() => {
                    onError.Invoke(this.GetText(MsgCode.Error), report.Msg);
                });
            }
        }
    
    
        private string GetFilename(CommHelpType medium) {
            switch (medium) {
                case CommHelpType.Bluetooth:
                    return "./Samples/BTSample.txt";
                case CommHelpType.BluetoothLE:
                    return "./Samples/BLESample.txt";
                case CommHelpType.Wifi:
                    return "./Samples/WifiSample.txt";
                case CommHelpType.Usb:
                    return "./Samples/USBSample.txt";
                case CommHelpType.Ethernet:
                    return "./Samples/EthernetSample.txt";
                case CommHelpType.Application:
                    return "./Documents/MultiCommTerminal.txt";
                default: return "";
            }
        }
    }

}
