using LanguageFactory.Net.data;
using System;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    //https://stackoverflow.com/questions/57662491/detect-back-arrow-press-of-the-navigationpage-in-xamarin-forms

    public class NavigateBackInterceptor {

        private ContentPage page;

        public bool Changed { get; set; } = false;

        public NavigateBackInterceptor(ContentPage page) {
            this.page = page;
            Shell.SetBackButtonBehavior(page, new BackButtonBehavior {
                Command = new Command(this.OnNavBarBack),
            });
        }


        public bool HardwareOnBackButtonQuestion(Func<bool> onExit) {
            Device.BeginInvokeOnMainThread(async () => {
                if (await AskExitQuestion()) {
                    await Shell.Current.Navigation.PopAsync();
                    onExit.Invoke();
                }
            });
            // To satisfy the OnBackButtonPressed override
            return true;
        }


        public void MethodExitQuestion() {
            Device.BeginInvokeOnMainThread(async () => {
                if (await AskExitQuestion()) {
                    await Shell.Current.Navigation.PopAsync();
                }
            });
        }


        public void MethodExitQuestion(Action onExit) {
            Device.BeginInvokeOnMainThread(async () => {
                if (await AskExitQuestion()) {
                    onExit.Invoke();
                }
            });
        }


        private async void OnNavBarBack() {
            if (await AskExitQuestion()) {
                await Shell.Current.Navigation.PopAsync();
            }
        }


        private async Task<bool> AskExitQuestion() {
            // TODO language
            if (this.Changed) {
                if (await this.page.DisplayAlert(
                    App.GetText(MsgCode.Warning),
                    App.GetText(MsgCode.AbandonChanges),
                    App.GetText(MsgCode.yes),
                    App.GetText(MsgCode.no))) {
                    // Question yes answer
                    return true;
                }
                else {
                    // answer no
                    return false;
                }
            }
            else {
                // No changes, no question
                return true;
            }
        }

    }
}
