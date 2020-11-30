using IconFactory.Net.data;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.XamarinForms.interfaces;
using MultiCommTerminal.XamarinForms.Services;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Threading.Tasks;
using Xamarin.Essentials;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms {
    public partial class App : Application {

        #region Properties

        public static ICommWrapper Wrapper = null;

        #endregion

        #region Events

        public static EventHandler<SupportedLanguage> LanguageUpdated;
        public static EventHandler<TerminatorDataModel> TerminatorsUpdated;

        #endregion

        #region Constructor and overrides

        public App(ICommWrapper wrapper) {
            InitializeComponent();
            Wrapper = wrapper;
            Wrapper.LanguageChanged += OnLanguageChanged;
            Wrapper.CurrentTerminatorChanged += this.OnCurrentTerminatorChanged;

            // TODO - remove this
            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }


        protected override void OnStart() {
            // This will abort the app at the start if the WIFI permissions are not given
            //this.DoPermissionsCheck();
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
            Device.BeginInvokeOnMainThread(async () => {
                await page.DisplayAlert(title, msg, App.GetText(MsgCode.Ok));
            });
        }


        public static void ShowError(Page page, MsgCode code) {
            ShowError(page, App.GetText(code));
        }


        private void OnCurrentTerminatorChanged(object sender, TerminatorDataModel e) {
            Device.BeginInvokeOnMainThread(() => {
                TerminatorsUpdated?.Invoke(this, e);
            });
        }

        private void OnLanguageChanged(object sender, SupportedLanguage language) {
            Device.BeginInvokeOnMainThread(() => {
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
            Device.BeginInvokeOnMainThread(async () => {
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
