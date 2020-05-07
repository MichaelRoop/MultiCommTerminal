using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using IconFactory.data;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        public void IconInfo(UIIcon code, Action<IconDataModel> onSuccess) {
            this.IconInfo(code, onSuccess, (msg) => { });
        }


        public void IconInfo(UIIcon code, Action<IconDataModel> onSuccess, Action<string> onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999,
                () => string.Format("No icon for {0}", code),
                () => {
                    onSuccess.Invoke(this.iconFactory.GetIcon(code));
                });
            if (report.Code != 0) {
                WrapErr.SafeAction(() => {
                    onError(string.Format("Unhandled error retrieving icon {0}", code));
                });
            }
        }

        public string IconSource(UIIcon code) {
            ErrReport report;
            string source = WrapErr.ToErrReport(out report, 9999,
                () => string.Format(""),
                () => {
                    string tmp = "";
                    this.IconInfo(code, (info) => {
                        tmp = info.IconSource as string;
                        WrapErr.ChkVar(tmp, 9999, () => string.Format("No source string for {0}", code));
                        WrapErr.ChkTrue(tmp.Length > 0, 9999, () => string.Format("0 length source string for {0}", code));
                    }, (msg) => {
                        tmp = "";
                    });
                    return tmp;
                });
            return report.Code == 0 ? source : "";
        }

        public void CommMediumList(Action<List<CommMedialDisplay>> mediums) {
            List<CommMedialDisplay> items = new List<CommMedialDisplay>();
            items.Add(this.CommItem("Classic", UIIcon.BluetoothClassic, CommMediumType.Bluetooth));
            // TODO - for now serial, need a full BLE
            items.Add(this.CommItem(String.Format("BLE  {0} {1}", '\u2b84', '\u2b86'), 
                UIIcon.BluetoothLE, CommMediumType.BluetoothLE));
            items.Add(this.CommItem("Ethernet", UIIcon.Ethernet, CommMediumType.Ethernet));
            items.Add(this.CommItem("Wifi", UIIcon.Wifi, CommMediumType.Wifi));
            mediums.Invoke(items);
        }


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
                        // TODO - others
                }
            }

            onSuccess(helps);

        }


        private CommMedialDisplay CommItem(string name, UIIcon icon, CommMediumType mediumType) {
            return new CommMedialDisplay() {
                Display = name,
                IconHeight = 16,
                IconWidth = 16,
                IconSource = this.IconSource(icon),
                MediumType = mediumType
            };
        }






    }
}
