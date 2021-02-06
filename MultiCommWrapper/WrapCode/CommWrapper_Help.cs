using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.UserDisplayData;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.IO;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {

        public void CommMediumHelpList(Action<List<CommMediumHelp>> onSuccess) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 200150, "Failure on CommMediumHelpList", () => {
                List<CommMediumHelp> helps = new List<CommMediumHelp>();
                this.CommHelpList((list) => {
                    foreach (CommHelpDisplay item in list) {
                        helps.Add(new CommMediumHelp() {
                            Id = item.HelpType,
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
                        case CommMedium.Bluetooth:
                            i.Text = "Communicate with device using traditional Bluetooth like Arduino with HC50 chip";
                            break;
                        case CommMedium.BluetoothLE:
                            i.Text = "View service structures and data from BLE devices";
                            break;
                        case CommMedium.Wifi:
                            i.Text = "Communicate with devices using WIFI to send commands and receive responses";
                            break;
                        case CommMedium.Ethernet:
                            i.Text = "Communicate with devices using a hard wired ethernet connection to send commands and receive responses";
                            break;
                        case CommMedium.Usb:
                            i.Text = "Communicate with devices using a hard wired USB serial connection to send commands and receive responses";
                            break;

                            // TODO - others
                    }
                }

                onSuccess(helps);
            });
            this.RaiseIfException(report);
        }


        public void HasCodeSample(CommMedium helpType, Action<CommMedium> onSuccess, OnErrTitle onError) {
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


        public void GetCodeSample(CommMedium helpType, Action<string> onSuccess, OnErrTitle onError) {
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
    

        private string AssembleWithSamplePath(string filename) {
            return string.Format("{0}/{1}", AppDomain.CurrentDomain.BaseDirectory, filename);
        }
    

        private string GetFilename(CommMedium medium) {
            switch (medium) {
                case CommMedium.Bluetooth:
                    return this.AssembleWithSamplePath("Samples/BTSample.txt");
                case CommMedium.BluetoothLE:
                    return this.AssembleWithSamplePath("Samples/BLESample.txt");
                case CommMedium.Wifi:
                    return this.AssembleWithSamplePath("Samples/WifiSample.txt");
                case CommMedium.Usb:
                    return this.AssembleWithSamplePath("Samples/USBSample.txt");
                case CommMedium.Ethernet:
                    return this.AssembleWithSamplePath("Samples/EthernetSample.txt");
                default: return "";
            }
        }

    }

}
