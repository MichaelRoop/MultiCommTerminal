using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>
    /// Interaction logic for MsgBoxEnterText.xaml
    /// </summary>
    public partial class MsgBoxEnterText : Window {

        #region Data

        private Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #endregion

        #region Data types

        /// <summary>Return values from this message box</summary>
        public enum MsgBoxTextInputResult {
            OK,
            Cancel,
        }

        public class MsgBoxTextInputData {
            public MsgBoxTextInputResult Result { get; set; } = MsgBoxTextInputResult.Cancel;
            public string Text { get; set; } = "";
        }

        #endregion

        #region Properties

        public MsgBoxTextInputData Result { get; set; } = new MsgBoxTextInputData();

        #endregion

        #region Static methods

        public static MsgBoxTextInputData ShowBox(Window win, string title, string msg, string defaultTxt = "") {
            MsgBoxEnterText box = new MsgBoxEnterText(win, title, msg, defaultTxt);
            box.ShowDialog();
            return box.Result;
        }

        #endregion

        #region Constructors

        public MsgBoxEnterText(Window parent, string title, string msg, string defaultTxt = "") {
            InitializeComponent();
            this.Title = title;
            this.txtBlock.Text = msg;
            this.txtInput.Text = defaultTxt;
            this.InitFromConstructors();
        }

        #endregion

        #region Windows init and events

        private void Window_ContentRendered(object sender, EventArgs e) {
            if (parent != null) {
                WPF_ControlHelpers.CenterChild(parent, this);
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            this.HideTitleBarIcon();
            base.OnApplyTemplate();
        }


        private void InitFromConstructors() {
            this.wrapper = DI.Wrapper;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnOk, this.btnCancel);
            this.widthManager.PrepForChange();
        }


        #endregion

        #region Button events

        private void btnOk_Click(object sender, RoutedEventArgs e) {
            this.Result.Result = MsgBoxTextInputResult.OK;
            this.Result.Text = this.txtInput.Text;
            this.Close();
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        #endregion

    }
}
