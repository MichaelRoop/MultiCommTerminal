using MultiCommTerminal.XamarinForms.Services;
using MultiCommTerminal.XamarinForms.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MultiCommWrapper.Net.interfaces;
using System.Threading.Tasks;
using LanguageFactory.Net.data;

namespace MultiCommTerminal.XamarinForms {
    public partial class App : Application {


        public static ICommWrapper Wrapper = null;


        public App(ICommWrapper wrapper) {
            InitializeComponent();

            Wrapper = wrapper;

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        public static void ShowError(Page page, string msg) {
            Task.Run(async () => {
                await page.DisplayAlert(
                    App.GetText(MsgCode.Error), msg, App.GetText(MsgCode.Ok));
            });
        }

        public static string GetText(MsgCode code) {
            return App.Wrapper.GetText(code);
        }


        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
