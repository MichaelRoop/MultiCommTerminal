using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    //https://stackoverflow.com/questions/57662491/detect-back-arrow-press-of-the-navigationpage-in-xamarin-forms

    public class NavigateBackInterceptor {

        public class ChangedIndicator {
            public bool IsChanged { get; set; } = false;
        }

        private ChangedIndicator indicator = new ChangedIndicator();
        private ContentPage page;


        BackButtonBehavior bbBehavior;

        //public 

        public NavigateBackInterceptor(ContentPage page, ChangedIndicator indicator) {
            this.page = page;
            this.indicator = indicator;

            Shell.SetBackButtonBehavior(page, new BackButtonBehavior {
                Command = new Command(this.OnNavBarBack),
                //Command = new Command(async () => {
                //    if (!this.indicator.IsOk) {
                //        await Shell.Current.Navigation.PopAsync();
                //    }
                //})
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

        private async void OnNavBarBack() {
            if (await AskExitQuestion()) {
                await Shell.Current.Navigation.PopAsync();
            }


            //if (!this.indicator.IsOk) {
            //    if (await this.page.DisplayAlert(
            //        "title", 
            //        "Abandon changes?", 
            //        App.GetText(LanguageFactory.Net.data.MsgCode.yes), 
            //        App.GetText(LanguageFactory.Net.data.MsgCode.no))) {

            //        // Abandon
            //        await Shell.Current.Navigation.PopAsync();
            //    }
            //}
        }



        private async Task<bool> AskExitQuestion() {
            // TODO language
            if (this.indicator.IsChanged) {
                if (await this.page.DisplayAlert(
                    "title",
                    "Abandon changes?",
                    App.GetText(LanguageFactory.Net.data.MsgCode.yes),
                    App.GetText(LanguageFactory.Net.data.MsgCode.no))) {
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
