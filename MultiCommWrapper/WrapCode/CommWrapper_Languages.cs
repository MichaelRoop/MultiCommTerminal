using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using LanguageFactory.data;
using LanguageFactory.Messaging;
using MultiCommData.Net.StorageDataModels;
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
