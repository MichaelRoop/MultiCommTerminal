using MultiCommTerminal.XamarinForms.Services;
using MultiCommTerminal.XamarinForms.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MultiCommWrapper.Net.interfaces;
using System.Threading.Tasks;
using LanguageFactory.Net.data;
using IconFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.StorageDataModels;

namespace MultiCommTerminal.XamarinForms {
    public partial class App : Application {


        public static ICommWrapper Wrapper = null;

        public static EventHandler<SupportedLanguage> LanguageUpdated;
        public static EventHandler<TerminatorDataModel> TerminatorsUpdated;

        public App(ICommWrapper wrapper) {
            InitializeComponent();

            Wrapper = wrapper;
            Wrapper.LanguageChanged += OnLanguageChanged;
            Wrapper.CurrentTerminatorChanged += this.OnCurrentTerminatorChanged;

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();


        }


        public static void ShowError(Page page, string msg) {
            Device.BeginInvokeOnMainThread(async () => {
                await page.DisplayAlert(
                    App.GetText(MsgCode.Error), msg, App.GetText(MsgCode.Ok));

            });

            //Task.Run(async () => {
            //    await page.DisplayAlert(
            //        App.GetText(MsgCode.Error), msg, App.GetText(MsgCode.Ok));
            //});
        }

        public static string GetText(MsgCode code) {
            return App.Wrapper.GetText(code);
        }

        public static ImageSource GetImageSource(UIIcon code) {
            Image i = new Image() {
                Source = App.Wrapper.IconSource(code) 
            };
            return i.Source;

            //ImageSource.FromResource(App.Wrapper.IconSource(code));

            

        }




        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }


        #region Wrapper event handlers

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

    }
}
