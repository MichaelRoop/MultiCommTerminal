using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using IconFactory.Net.data;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
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
            if (this.LanguageChanged != null) {
                this.LanguageChanged(sender, newLanguage);
            }
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
            this.SaveLanguage(code, () => { }, onError);
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
            return this.languages.GetMsgDisplay(code);
        }


        public string GetText(CommHelpType medium) {
            switch (medium) {
                case CommHelpType.Bluetooth: return "Classic";
                case CommHelpType.BluetoothLE: return String.Format("BLE  {0} {1}", '\u2b84', '\u2b86');
                case CommHelpType.Wifi: return "Wifi";
                case CommHelpType.Ethernet: return "Ethernet";
                case CommHelpType.Usb: return "USB";
                case CommHelpType.Application: return "Application"; // TODO Language support
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

        private void InitLanguages() {
            this.languages.LanguageChanged += Event_LanguageChanged;
        }

        private void TeardownLanguages() {
            this.languages.LanguageChanged -= this.Event_LanguageChanged;
        }

        #endregion


    }
}
