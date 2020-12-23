using IconFactory.Net.data;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.interfaces;
using MultiCommTerminal.XamarinForms.Services;
using MultiCommTerminal.XamarinForms.Views.MessageBoxes;
using MultiCommWrapper.Net.interfaces;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;
//using Microsoft.AppCenter;
//using Microsoft.AppCenter.Analytics;
//using Microsoft.AppCenter.Crashes;

namespace MultiCommTerminal.XamarinForms {
    public partial class App : Application {

        #region Properties

        public static ICommWrapper Wrapper = null;
        public static bool IsRunning { get; private set; } = false;

        #endregion

        #region Events

        public static EventHandler<SupportedLanguage> LanguageUpdated;
        public static EventHandler<TerminatorDataModel> TerminatorsUpdated;

        #endregion

        #region Constructor and overrides

        public App(Func<ICommWrapper> onGet) {
            InitializeComponent();
            Task.Run(() => {
                Wrapper = onGet();
                Wrapper.LanguageChanged += OnLanguageChanged;
                Wrapper.CurrentTerminatorChanged += this.OnCurrentTerminatorChanged;
                Device.BeginInvokeOnMainThread(() => MainPage = new AppShell());
            });

        }


        protected override void OnStart() {
                App.IsRunning = true;
            // This will abort the app at the start if the WIFI permissions are not given
            //this.DoPermissionsCheck();

            //AppCenter.Start(
            //    "android=2bcd0252-b20d-4cb2-b47e-dd4d206579ce;",
            //    typeof(Analytics), typeof(Crashes));
        }


        protected override void OnSleep() {
        }


        protected override void OnResume() {
        }

        #endregion

        #region Wrapper calls and event handlers exposed to pages on main thread

        public static string GetText(MsgCode code) {
            return App.Wrapper.GetText(code);
        }


        public static ImageSource GetImageSource(UIIcon code) {
            Image i = new Image() {
                Source = App.Wrapper.IconSource(code)
            };
            return i.Source;
        }


        public static void ShowError(Page page, string msg) {
            ShowError(page, App.GetText(MsgCode.Error), msg);
        }


        public static void ShowError(Page page, string title, string msg) {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => {
                //await page.DisplayAlert(title, msg, App.GetText(MsgCode.Ok));

                await PopupNavigation.Instance.PushAsync(new AlertPopup(title, msg));
            });
        }


        public static void ShowError(Page page, MsgCode code) {
            ShowError(page, App.GetText(code));
        }


        public static void ShowYesNo(Page page, string title, string msg, Action onYes, Action onNo) {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => {
                bool ok = await page.DisplayAlert(title, msg, GetText(MsgCode.yes), GetText(MsgCode.no));
                if (ok) {
                    onYes?.Invoke();
                }
                else {
                    onNo?.Invoke();
                }
            });
        }

        public static void ShowYesNo(Page page, string title, string msg, Action onYes) {
            ShowYesNo(page, title, msg, onYes, () => { });
        }


        public static void ShowYesNo(Page page, string title, MsgCode code, Action onYes) {
            ShowYesNo(page, title, GetText(code), onYes, () => { });
        }


        public static void ShowYesNo(Page page, string msg, Action onYes) {
            ShowYesNo(page, "", msg, onYes, () => { });
        }


        private void OnCurrentTerminatorChanged(object sender, TerminatorDataModel e) {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                TerminatorsUpdated?.Invoke(this, e);
            });
        }

        private void OnLanguageChanged(object sender, SupportedLanguage language) {
            Xamarin.Forms.Device.BeginInvokeOnMainThread(() => {
                LanguageUpdated?.Invoke(this, language);
            });
        }

        #endregion

        #region Private

        /// <summary>
        /// Do check if permissions granted and request if not. Abort if User denies
        /// </summary>
        private void DoPermissionsCheck() {
            // This will abort the app at the start if the WIFI permissions are not given
            Xamarin.Forms.Device.BeginInvokeOnMainThread(async () => {
                if (!await this.CheckPermissions()) {
                    ICloseApplication closeApp = DependencyService.Get<ICloseApplication>();
                    await Application.Current.MainPage.DisplayAlert(
                        App.GetText(MsgCode.Error),
                        "Insufficient permissions",
                        App.GetText(MsgCode.Ok));
                    closeApp.CloseApp();
                }
            });
        }


        /// <summary>Abort the App if Wifi permissions not enabled</summary>
        /// <returns>true if permissions granted, otherwise false</returns>
        private async Task<bool> CheckPermissions() {
            ILocationWhileInUsePermission wifiPermissions =
                DependencyService.Get<ILocationWhileInUsePermission>();
            PermissionStatus status = await wifiPermissions.CheckStatusAsync();
            if (status != PermissionStatus.Granted) {
                status = await wifiPermissions.RequestAsync();
            }
            return status == PermissionStatus.Granted;
        }

        #endregion

    }
}
