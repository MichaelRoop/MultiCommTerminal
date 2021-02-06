using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using IconFactory.Net.data;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.UserDisplayData;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        #region Events

        public event EventHandler<SupportedLanguage> LanguageChanged;


        private void Event_LanguageChanged(object sender, SupportedLanguage newLanguage) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000201, "Failure on Event_LanguageChanged", () => {
                this.LanguageChanged?.Invoke(sender, newLanguage);
            });
            this.RaiseIfException(report);
        }

        #endregion

        #region Public

        public void CurrentStoredLanguage() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 9999, () => {
                this.GetSettings((settings) => {
                    this.languages.SetCurrentLanguage(settings.Language);
                }, (str) => {
                    // Don't do anything
                });
            });
        }


        public void CurrentLanguage(Action<LangCode> onDone) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => { onDone(this.languages.CurrentLanguageCode); });
                if (report.Code != 0) {
                    WrapErr.SafeAction(() => { onDone(this.languages.CurrentLanguageCode); });
                }
            });
        }


        public void CurrentSupportedLanguage(Action<SupportedLanguage> onDone) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onDone(this.languages.CurrentLanguage);
                });
                if (report.Code != 0) {
                    WrapErr.SafeAction(() => {
                        onDone(this.languages.CurrentLanguage);
                    });
                }
            });
        }


        public void LanguageList(Action<List<LanguageDataModel>> onDone) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => { onDone(this.languages.AvailableLanguages); });
                if (report.Code != 0) {
                    WrapErr.SafeAction(() => { onDone(new List<LanguageDataModel>()); });
                }
            });
        }


        public void SetLanguage(LangCode code) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    this.languages.SetCurrentLanguage(code);
                });
            });
        }


        public void SaveLanguage(LangCode code, Action<string> onError) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000202, "Failure on SaveLanguage", () => {
                this.SaveLanguage(code, () => { }, onError);
            });
        }


        public void SaveLanguage(LangCode code, Action onSuccess, Action<string> onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    this.languages.SetCurrentLanguage(code);
                    SettingItems items = this.settings.ReadObjectFromDefaultFile();
                    items.Language = code;
                    items.LanguageName = this.languages.CurrentLanguage.Language.Display;
                    if (this.settings.WriteObjectToDefaultFile(items)) {
                        onSuccess.Invoke();
                    }
                    else {
                        // TODO Language
                        onError.Invoke("Failed");
                    }
                });
                if (report.Code != 0) {
                    // TODO - language
                    WrapErr.SafeAction(() => { onError.Invoke("Unhandled Error on saving language"); });
                }
            });
        }


        public string GetText(MsgCode code) {
            ErrReport report;
            string msg = "ERR";
            WrapErr.ToErrReport(out report, 2000203, "Failure on GetText", () => {
                msg = this.languages.GetMsgDisplay(code);
            });
            return msg;
        }


        public string GetText(CommMedium medium) {
            switch (medium) {
                case CommMedium.Bluetooth: return "Bluetooth";
                case CommMedium.BluetoothLE: return String.Format("BLE  {0} {1}", '\u2b84', '\u2b86');
                case CommMedium.Wifi: return "Wifi";
                case CommMedium.Ethernet: return "Ethernet";
                case CommMedium.Usb: return "USB";
                default: return "N/A";
                    // TODO - others
            }
        }


        public void GetMenuItemDataModel(
            MenuCode menuCode,
            MsgCode msgCode, 
            UIIcon iconCode, 
            string padding,
            Action<MenuItemDataModel> onSuccess,
            Action<MenuItemDataModel> onError) {

            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess(new MenuItemDataModel() {
                        Code = menuCode,
                        Display = this.GetText(msgCode),
                        IconSource =  this.IconSource(iconCode),
                        Padding = padding,
                    });
                });
                if (report.Code != 0) {
                    WrapErr.SafeAction(() => { 
                        onError.Invoke(new MenuItemDataModel() {
                            Code = menuCode,
                            Display = "**NA**",
                            IconSource = "",
                            Padding = padding,
                        });
                    });
                }
            });



        }

        #endregion

        #region Private

        private void TeardownLanguages() {
            this.languages.LanguageChanged -= this.Event_LanguageChanged;
        }

        #endregion


    }
}
