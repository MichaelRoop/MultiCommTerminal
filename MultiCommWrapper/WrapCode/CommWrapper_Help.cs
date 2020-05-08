using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.data;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace MultiCommWrapper.Net.WrapCode {
    
    
    public partial class CommWrapper : ICommWrapper {


        public void CommMediumHelpList(Action<List<CommMediumHelp>> onSuccess) {
            List<CommMediumHelp> helps = new List<CommMediumHelp>();
            this.CommMediumList((list) => {
                foreach (CommMedialDisplay item in list) {
                    helps.Add(new CommMediumHelp() {
                        Id = item.MediumType,
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
                    case CommMediumType.Bluetooth:
                        i.Text = "Communicate with device using traditional Bluetooth like Arduino with HC50 chip";
                        break;
                    case CommMediumType.BluetoothLE:
                        i.Text = "Communicate with devices on BLE with 2 characteristics defined for serial input and output";
                        break;
                    case CommMediumType.Wifi:
                        i.Text = "Communicate with devices using WIFI to send commands and receive responses";
                        break;
                    case CommMediumType.Ethernet:
                        i.Text = "Communicate with devices using a hard wired ethernet connection to send commands and receive responses";
                        break;
                    case CommMediumType.Usb:
                        i.Text = "Communicate with devices using a hard wired USB serial connection to send commands and receive responses";
                        break;

                        // TODO - others
                }
            }

            onSuccess(helps);

        }
    

        public void HasCodeSample(CommMediumType medium, Action<CommMediumType> onSuccess, OnErrTitle onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999,
                () => string.Format(""),
                () => {
                    string tmp = this.GetFilename(medium);
                    if (tmp.Length > 0) {
                        onSuccess(medium);
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


        public void GetCodeSample(CommMediumType medium, Action<string> onSuccess, OnErrTitle onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999,
                () => string.Format(""),
                () => {
                    string filename = this.GetFilename(medium);
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
    
    
        private string GetFilename(CommMediumType medium) {
            switch (medium) {
                case CommMediumType.Bluetooth:
                    return "./Samples/BTSample.txt";
                case CommMediumType.BluetoothLE:
                    return "./Samples/BLESample.txt";
                default: return "";
            }
        }
    }

}
