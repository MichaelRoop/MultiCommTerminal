using MultiCommTerminal.XamarinForms.Services;
using MultiCommTerminal.XamarinForms.Views;
using System;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;
using MultiCommWrapper.Net.interfaces;


namespace MultiCommTerminal.XamarinForms {
    public partial class App : Application {


        public static ICommWrapper Wrapper = null;


        public App(ICommWrapper wrapper) {
            InitializeComponent();

            Wrapper = wrapper;

            DependencyService.Register<MockDataStore>();
            MainPage = new AppShell();
        }

        protected override void OnStart() {
        }

        protected override void OnSleep() {
        }

        protected override void OnResume() {
        }
    }
}
