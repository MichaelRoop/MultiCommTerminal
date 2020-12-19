using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using IconFactory.Net.data;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.UserDisplayData;
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
            items.Add(this.CommItem("Bluetooth", UIIcon.BluetoothClassic, CommMedium.Bluetooth));
            // TODO - for now serial, need a full BLE
            items.Add(this.CommItem(String.Format("BLE  {0} {1}", '\u2b84', '\u2b86'), 
                UIIcon.BluetoothLE, CommMedium.BluetoothLE));
            items.Add(this.CommItem("Wifi", UIIcon.Wifi, CommMedium.Wifi));
            items.Add(this.CommItem("Ethernet", UIIcon.Ethernet, CommMedium.Ethernet));
            items.Add(this.CommItem("USB", UIIcon.Usb, CommMedium.Usb));
            mediums.Invoke(items);
        }


        private CommMedialDisplay CommItem(string name, UIIcon icon, CommMedium mediumType) {
            return new CommMedialDisplay() {
                Display = name,
                IconHeight = 16,
                IconWidth = 16,
                IconSource = this.IconSource(icon),
                MediumType = mediumType
            };
        }


        private void CommHelpList(Action<List<CommHelpDisplay>> helps) {
            List<CommHelpDisplay> items = new List<CommHelpDisplay>();
            items.Add(this.HelpItem("Bluetooth", UIIcon.BluetoothClassic, CommMedium.Bluetooth));
            // TODO - for now serial, need a full BLE
            items.Add(this.HelpItem(String.Format("BLE  {0} {1}", '\u2b84', '\u2b86'),
                UIIcon.BluetoothLE, CommMedium.BluetoothLE));
            items.Add(this.HelpItem("Wifi", UIIcon.Wifi, CommMedium.Wifi));
            items.Add(this.HelpItem("USB", UIIcon.Usb, CommMedium.Usb));
            items.Add(this.HelpItem(this.GetText(MsgCode.Ethernet), UIIcon.Ethernet, CommMedium.Ethernet));
            helps.Invoke(items);
        }


        private CommHelpDisplay HelpItem(string name, UIIcon icon, CommMedium helpType) {
            return new CommHelpDisplay() {
                Display = name,
                HelpType = helpType,
                IconHeight = 16,
                IconWidth = 16,
                IconSource = this.IconSource(icon),
            };
        }

    }
}
