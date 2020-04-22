using MultiCommTerminal.WPF_Helpers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for MsgBoxSimple.xaml</summary>
    public partial class MsgBoxSimple : Window {

        Window parent = null;

        public static void ShowBox(Window win, string msg) {
            MsgBoxSimple box = new MsgBoxSimple(win, msg);
            box.ShowDialog();
        }


        public static void ShowBox(Window win, string title, string msg) {
            MsgBoxSimple box = new MsgBoxSimple(win, title, msg);
            box.ShowDialog();
        }


        public static void ShowBox(string msg) {
            MsgBoxSimple box = new MsgBoxSimple(null, msg);
            box.ShowDialog();
        }

        public static void ShowBox(string title, string msg) {
            MsgBoxSimple box = new MsgBoxSimple(null, title, msg);
            box.ShowDialog();
        }


        private MsgBoxSimple(Window parent) {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public MsgBoxSimple(Window parent, string msg) : this(parent) {
            this.parent = parent;
            this.txtBlock.Text = msg;
        }


        public MsgBoxSimple(Window parent, string title, string msg) : this(parent, msg) {
            this.Title = title;
        }


        private void Window_ContentRendered(object sender, System.EventArgs e) {
            if (parent != null) {
                WPF_ControlHelpers.CenterChild(parent, this);
            }
        }



        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }


    public static class MsgBoxSimpleExtensions {

        public static void ShowBox(this Window win, string msg) {
            MsgBoxSimple box = new MsgBoxSimple(win, msg);
            box.ShowDialog();
        }


        public static void ShowBox(this Window win, string title, string msg) {
            MsgBoxSimple box = new MsgBoxSimple(win, title, msg);
            box.ShowDialog();
        }


    }


}
