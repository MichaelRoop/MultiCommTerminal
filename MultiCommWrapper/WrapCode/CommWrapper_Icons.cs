using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using IconFactory.Net.data;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        private void IconInfo(UIIcon code, Action<IconDataModel> onSuccess, Action<string> onError) {
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
            items.Add(this.CommItem("USB", UIIcon.Usb, CommMediumType.Usb));
            mediums.Invoke(items);
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
