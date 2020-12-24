using LanguageFactory.Net.data;
using LogUtils.Net;
using MultiCommTerminal.XamarinForms.Views.MessageBoxes;
using Rg.Plugins.Popup.Services;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    //https://stackoverflow.com/questions/57662491/detect-back-arrow-press-of-the-navigationpage-in-xamarin-forms

    public class NavigateBackInterceptor {

        private ContentPage page;
        private ClassLog log = new ClassLog("NavigateBackInterceptor");

        public bool Changed { get; set; } = false;

        public NavigateBackInterceptor(ContentPage page) {
            this.page = page;
            Shell.SetBackButtonBehavior(page, new BackButtonBehavior {
                Command = new Command(this.OnNavBarBack),
            });
        }


        public bool HardwareOnBackButtonQuestion(Func<bool> onExit) {
            this.log.Info("HardwareOnBackButtonQuestion", "Entry");
            // Disable this functionality since it double bounces and is impossible to manage
            return true;
        }


        public void MethodExitQuestion() {
            this.log.InfoEntry("MethodExitQuestion");
            Device.BeginInvokeOnMainThread(async () => {
                await this.AskExitQuestion(async () => {
                    await Shell.Current.Navigation.PopAsync();
                });
            });
        }


        public void MethodExitQuestion(Action onExit) {
            this.log.InfoEntry("MethodExitQuestion(onExit)");
            Device.BeginInvokeOnMainThread(async () => {
                await this.AskExitQuestion(() => {
                    onExit?.Invoke();
                });
            });
        }


        private async void OnNavBarBack() {
            await this.AskExitQuestion(async () => {
                await Shell.Current.Navigation.PopAsync();
            });
        }


        private async Task<bool> AskExitQuestion(Action onYes) {
            this.log.Info("AskExitQuestion", ()=>string.Format("Changed:{0}", this.Changed));
            return await Task<bool>.Run(() => {
                if (this.Changed) {
                    Device.BeginInvokeOnMainThread(async () => {
                        await PopupNavigation.Instance.PushAsync(
                            new YesNoPopup(
                                App.GetText(MsgCode.Warning),
                                App.GetText(MsgCode.AbandonChanges),
                                () => Device.BeginInvokeOnMainThread(()=> onYes?.Invoke()),
                                ()=> { }));
                    });
                }
                else {
                    Device.BeginInvokeOnMainThread(() => onYes?.Invoke());
                }
                return true;
            });
        }

    }

}
