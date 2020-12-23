using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using LogUtils.Net;
using Rg.Plugins.Popup.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace MultiCommTerminal.XamarinForms.Views.MessageBoxes {
    [XamlCompilation(XamlCompilationOptions.Compile)]
    public partial class YesNoPopup : Rg.Plugins.Popup.Pages.PopupPage {

        private Action yesAction = null;
        private Action noAction = null;
        private ClassLog log = new ClassLog("YesNoPopup");

        public YesNoPopup(string title, string msg, Action yes, Action no) {
            this.yesAction = yes;
            this.noAction = no;
            InitializeComponent();
            this.CloseWhenBackgroundIsClicked = false;
            this.lbTitle.Text = title;
            this.lbMsg.Text = msg;
        }


        public YesNoPopup(string title, string msg, bool supressContinue, Action yes) 
            : this(title, msg, yes, () => { }) {
        }


        protected override void OnAppearing() {
            App.Wrapper.CurrentSupportedLanguage(this.updateLanguage);
            
            base.OnAppearing();
        }


        protected override bool OnBackButtonPressed() {
            return true;
        }


        private void btnYes_Clicked(object sender, EventArgs args) {
            try {
                this.yesAction?.Invoke();
            }
            catch(Exception e) {
                this.log.Exception(9999, "btnYes_Clicked", "", e);
            }
            PopupNavigation.Instance.PopAsync(true);
        }

        private void btnNo_Clicked(object sender, EventArgs args) {
            try {
                this.noAction?.Invoke();
            }
            catch (Exception e) {
                this.log.Exception(9999, "btnNo_Clicked", "", e);
            }
            PopupNavigation.Instance.PopAsync(true);
        }


        private void updateLanguage(SupportedLanguage l) {
            this.btnYes.Text = l.GetText(MsgCode.yes);
            this.btnNo.Text = l.GetText(MsgCode.no);
        }

    }
}