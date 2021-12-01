using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for EthernetInfo.xaml</summary>
    public partial class EthernetInfo : Window {

        Window parent;

        public static void ShowBox(Window parent, EthernetParams info) {
            EthernetInfo win = new EthernetInfo(parent, info);
            win.ShowDialog();
        }


        public EthernetInfo(Window parent, EthernetParams info) {
            this.parent = parent;
            InitializeComponent();
            this.Title = info.Display;
            this.txtHost.Content = info.EthernetAddress;
            this.txtService.Content = info.EthernetServiceName;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

    }
}
