using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for EthernetEditRequest.xaml</summary>
    public partial class EthernetEditRequest : Window {

        #region Data

        private Window parent = null;
        private EthernetSelectResult data = null;

        #endregion


        public static void ShowBox(Window parent, EthernetSelectResult data) {
            EthernetEditRequest win = new EthernetEditRequest(parent, data);
            win.ShowDialog();
        }


        public EthernetEditRequest(Window parent, EthernetSelectResult data) {
            this.parent = parent;
            this.data = data;
            InitializeComponent();
            this.Init();
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


        private void btnSave_Click(object sender, RoutedEventArgs e) {
            this.data.DataModel.Display = this.txtName.Text;
            this.data.DataModel.EthernetAddress = this.txtHostName.Text;
            this.data.DataModel.EthernetServiceName = this.txtServiceName.Text;
            DI.Wrapper.SaveEthernetData(data.Index, data.DataModel, this.Close, this.OnFail);
        }


        private void Init() {
            this.txtName.Text = this.data.DataModel.Display;
            this.txtHostName.Text = this.data.DataModel.EthernetAddress;
            this.txtServiceName.Text = this.data.DataModel.EthernetServiceName;
        }


        private void OnFail(string err) {
            App.ShowMsg(err);
            this.Close();
        }

    }
}
