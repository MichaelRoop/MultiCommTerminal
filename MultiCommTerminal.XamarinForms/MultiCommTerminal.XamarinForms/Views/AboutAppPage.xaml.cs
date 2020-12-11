using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommTerminal.XamarinForms.UIHelpers;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Input;
using Xamarin.Essentials;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views {
    [XamlCompilation(XamlCompilationOptions.Compile)]

    public partial class AboutAppPage : ContentPage {

        public ICommand TapLinkCmd => new Command<string>(this.LinkOpen);
            

        #region Constructor and page envents

        public AboutAppPage() {
            InitializeComponent();
            BindingContext = this;
        }

        protected override void OnAppearing() {
            App.Wrapper.LanguageChanged += this.OnLanguageChanged;
            App.Wrapper.CurrentSupportedLanguage(this.UpdateLanguage);
            base.OnAppearing();
        }


        protected override void OnDisappearing() {
            App.Wrapper.LanguageChanged -= this.OnLanguageChanged;
            base.OnDisappearing();
        }

        #endregion


        #region Wrapper event handlers

        private void btnUserManual_Clicked(object sender, EventArgs e) {
            Task.Run(()=> SampleLoader.LoadUserManual(this.OnErr));
        }


        private void OnLanguageChanged(object sender, SupportedLanguage e) {
            this.UpdateLanguage(e);
        }


        private void UpdateLanguage(SupportedLanguage l) {

            this.lbTitle.Text = l.GetText(MsgCode.About);
            this.btnUserManual.Text = l.GetText(MsgCode.UserManual);
            this.lbAuthor.Text = l.GetText(MsgCode.Author);
            this.lbIcons.Text = l.GetText(MsgCode.Icons);
            this.txtSupport.Text = l.GetText(MsgCode.Support);
        }


        private void LinkOpen(string url) {
            Task.Run(() => this.LaunchUrlTask(url));


            //Task t = new Task(`)


//            Task.Factory.StartNew(()=>Launcher.OpenAsync(url), null, TaskCreationOptions.LongRunning,TaskScheduler. );
        }


        private void LaunchUrlTask(string url) {
            try {
                Thread.CurrentThread.Priority = ThreadPriority.Lowest;
                Launcher.OpenAsync(url);
            }
            finally {
                Thread.CurrentThread.Priority = ThreadPriority.Normal;
            }
        }



        #endregion
    }
}