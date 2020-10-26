using LanguageFactory.Net.data;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for MsgBoxYesNo.xaml</summary>
    public partial class MsgBoxYesNo : Window {

        #region Data types

        /// <summary>Return values from this message box</summary>
        public enum MsgBoxResult {
            Yes,
            No,
        }

        #endregion

        #region Data

        private Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #endregion

        #region Properties

        /// <summary>Results from clicking either the yes or no buttons</summary>
        public MsgBoxResult Result { get; set; } = MsgBoxResult.No;

        #endregion

        #region Static methods

        public static MsgBoxResult ShowBoxDelete(Window win, string msg) {
            MsgBoxYesNo box = new MsgBoxYesNo(win, DI.Wrapper.GetText(MsgCode.Delete), msg, false);
            box.ShowDialog();
            return box.Result;
        }



        public static MsgBoxResult ShowBox(Window win, string msg, bool suppressContinue = false) {
            MsgBoxYesNo box = new MsgBoxYesNo(win, msg, suppressContinue);
            box.ShowDialog();
            return box.Result;
        }


        public static MsgBoxResult ShowBox(Window win, string title, string msg, bool suppressContinue = false) {
            MsgBoxYesNo box = new MsgBoxYesNo(win, title, msg, suppressContinue);
            box.ShowDialog();
            return box.Result;
        }


        public static MsgBoxResult ShowBox(string msg, bool suppressContinue = false) {
            MsgBoxYesNo box = new MsgBoxYesNo(msg, suppressContinue);
            box.ShowDialog();
            return box.Result;
        }


        public static MsgBoxResult ShowBox(string title, string msg, bool suppressContinue = false) {
            MsgBoxYesNo box = new MsgBoxYesNo(title, msg, suppressContinue);
            box.ShowDialog();
            return box.Result;
        }

        #endregion

        #region Constructors

        public MsgBoxYesNo(bool suppressContinue = false) {
            InitializeComponent();
            this.InitFromConstructors(suppressContinue);
        }


        public MsgBoxYesNo(string msg, bool suppressContinue = false) : this(suppressContinue) {
            this.txtBlock.Text = msg;
        }


        public MsgBoxYesNo(string title, string msg, bool suppressContinue = false) : this(msg, suppressContinue) {
            this.Title = title;
        }


        private MsgBoxYesNo(Window parent, bool suppressContinue = false) {
            InitializeComponent();
            this.InitFromConstructors(suppressContinue);
        }


        public MsgBoxYesNo(Window parent, string msg, bool suppressContinue = false) : this(parent, suppressContinue) {
            this.parent = parent;
            this.txtBlock.Text = msg;
        }


        public MsgBoxYesNo(Window parent, string title, string msg, bool suppressContinue = false) : this(parent, msg, suppressContinue) {
            this.Title = title;
        }

        #endregion

        #region Windows init and events

        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            this.HideTitleBarIcon();
            base.OnApplyTemplate();
        }


        private void InitFromConstructors(bool suppressContinue) {
            this.wrapper = DI.Wrapper;
            this.txtContinue.Visibility = suppressContinue ? Visibility.Collapsed : Visibility.Visible;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnYes, this.btnNo);
            this.widthManager.PrepForChange();
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        private void Window_ContentRendered(object sender, System.EventArgs e) {
            if (parent != null) {
                WPF_ControlHelpers.CenterChild(parent, this);
            }
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }

        #endregion

        #region Button events

        private void btnYes_Click(object sender, RoutedEventArgs e) {
            this.Result = MsgBoxResult.Yes;
            this.Close();
        }


        private void btnNo_Click(object sender, RoutedEventArgs e) {
            this.Result = MsgBoxResult.No;
            this.Close();
        }

        #endregion

    }
}
