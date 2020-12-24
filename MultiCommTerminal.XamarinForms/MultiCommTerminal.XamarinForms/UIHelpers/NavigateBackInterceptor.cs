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
            this.log.InfoEntry("***** Constructor *****");
            Shell.SetBackButtonBehavior(page, new BackButtonBehavior {
                Command = new Command(this.OnNavBarBack),
            });

            this.log.Info("Constructor", () => string.Format(
                "Nav stack:{0} Modal Stack:{1}",
                Shell.Current.Navigation.NavigationStack.Count, Shell.Current.Navigation.ModalStack.Count));
        }


        private bool hardwarePressed = false;

        public bool HardwareOnBackButtonQuestion(Func<bool> onExit) {
            this.log.Info("HardwareOnBackButtonQuestion", "Entry");

            return true;


            lock (this) {
                if (this.hardwarePressed) {
                    this.log.Info("HardwareOnBackButtonQuestion", "Caught bounce button already pressed");
                    return true;
                }

                this.log.Info("HardwareOnBackButtonQuestion", "First press of button");
                if (!this.Changed) {
                    this.log.Info("HardwareOnBackButtonQuestion", "No change. return. Invoke onExit to pop");
                    //return onExit.Invoke();
                    //Device.BeginInvokeOnMainThread(async() => await Shell.Current.Navigation.PopAsync());
                }

                this.hardwarePressed = true;
            }


            PopupNavigation.Instance.PushAsync(
                new YesNoPopup(
                    App.GetText(MsgCode.Warning),
                    App.GetText(MsgCode.AbandonChanges),
                    () => {
                        lock (this) {
                            this.log.Info("HardwareOnBackButtonQuestion", "Selected Yes to abandon. Popping page");
                            // This will pop the page. I think?
                            //onExit.Invoke();
                            Device.BeginInvokeOnMainThread(async () => await Shell.Current.Navigation.PopAsync());
                            this.hardwarePressed = false;
                        }
                    },
                    () => {
                        lock (this) {
                            this.log.Info("HardwareOnBackButtonQuestion", "Selected No. Stay here");
                            this.hardwarePressed = false;
                        }
                    }));

            return true;


            /*
            return await Task<bool>.Run(() => {
                if (this.Changed) {
                    Device.BeginInvokeOnMainThread(async () => {
                        await PopupNavigation.Instance.PushAsync(
                            new YesNoPopup(
                                App.GetText(MsgCode.Warning),
                                App.GetText(MsgCode.AbandonChanges),
                                () => {
                                    Device.BeginInvokeOnMainThread(()=> onYes?.Invoke());
                                },
                                () => { }));
                    });
                }
                else {
                    Device.BeginInvokeOnMainThread(() => onYes?.Invoke());
                }
                return true;
            });
             
             
             */






            //this.log.InfoEntry("HardwareOnBackButtonQuestion");
            //if (Shell.Current.Navigation.NavigationStack.Count < 2) {
            //    this.log.Error(9999, "Debounce second hit");
            //    return onExit.Invoke();
            //}


            //Device.BeginInvokeOnMainThread(async () => {
            //    await this.AskExitQuestion(async () => {
            //        Device.BeginInvokeOnMainThread(async () => {
            //            this.log.Info("HardwareOnBackButtonQuestion", "Poping the Shell current");

            //            this.log.Info("HardwareOnBackButtonQuestion", () => string.Format(
            //                "Nav stack:{0} Modal Stack:{1}",
            //                Shell.Current.Navigation.NavigationStack.Count, Shell.Current.Navigation.ModalStack.Count));

            //            // TODO likely will be a diffent number depending on 
            //            // Seems like a bug where we get a bounce on the hardware/software back button which 
            //            // will cause the a second pop which crashes on last in stack
            //            if (Shell.Current.Navigation.NavigationStack.Count > 1) {
            //                await Shell.Current.Navigation.PopAsync();
            //            }
            //            this.log.Info("HardwareOnBackButtonQuestion", "Invoke the onExit (Base called)");
            //            onExit.Invoke();
            //        });
            //    });
            //    //if (await AskExitQuestion()) {
            //    //    await Shell.Current.Navigation.PopAsync();
            //    //    onExit.Invoke();
            //    //}
            //});
            //// To satisfy the OnBackButtonPressed override



            //return true;
        }


        public void MethodExitQuestion() {
            this.log.InfoEntry("MethodExitQuestion");
            Device.BeginInvokeOnMainThread(async () => {
                await this.AskExitQuestion(async () => {
                    await Shell.Current.Navigation.PopAsync();
                });
                //if (await AskExitQuestion()) {
                //    await Shell.Current.Navigation.PopAsync();
                //}
            });
        }


        public void MethodExitQuestion(Action onExit) {
            this.log.InfoEntry("MethodExitQuestion(onExit)");
            Device.BeginInvokeOnMainThread(async () => {
                await this.AskExitQuestion(() => {
                    onExit?.Invoke();
                });
                //if (await AskExitQuestion()) {
                //    onExit.Invoke();
                //}
            });
        }


        private async void OnNavBarBack() {
            //if (await AskExitQuestion()) {
            //    await Shell.Current.Navigation.PopAsync();
            //}
            this.log.InfoEntry("OnNavBarBack");
            Device.BeginInvokeOnMainThread(async () => {
                await this.AskExitQuestion(async () => {
                    await Shell.Current.Navigation.PopAsync();
                });
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
                                () => {
                                    Device.BeginInvokeOnMainThread(()=> onYes?.Invoke());
                                },
                                () => { }));
                    });
                }
                else {
                    Device.BeginInvokeOnMainThread(() => onYes?.Invoke());
                }
                return true;
            });


            //if (this.Changed) {
            //    if (await this.page.DisplayAlert(
            //        App.GetText(MsgCode.Warning),
            //        App.GetText(MsgCode.AbandonChanges),
            //        App.GetText(MsgCode.yes),
            //        App.GetText(MsgCode.no))) {
            //        // Question yes answer
            //        return true;
            //    }
            //    else {
            //        // answer no
            //        return false;
            //    }
            //}
            //else {
            //    // No changes, no question
            //    return true;
            //}
        }





    }
}
