using Ethernet.Common.Net.DataModels;
using LanguageFactory.Net.data;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs.EthernetWins {

    /// <summary>Interaction logic for EthernetSelect.xaml</summary>
    public partial class EthernetSelect : Window {

        private Window parent = null;
        private IIndexItem<DefaultFileExtraInfo> index = null;

        public EthernetSelectResult SelectedEthernet { get; private set; } = null;


        public static EthernetSelectResult ShowBox(Window parent) {
            EthernetSelect win = new EthernetSelect(parent);
            win.ShowDialog();
            return win.SelectedEthernet;
        }


        public EthernetSelect(Window parent) {
            this.parent = parent;
            InitializeComponent();
            WPF_ControlHelpers.CenterChild(parent, this);
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
            DI.Wrapper.GetEthernetDataList(this.OnLoadSuccess, this.OnErr);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbEthernet.SelectionChanged -= this.selectionChanged;
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void selectionChanged(object sender, SelectionChangedEventArgs e) {
            IIndexItem<DefaultFileExtraInfo> ndx = this.lbEthernet.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (ndx != null) {
                DI.Wrapper.RetrieveEthernetData(
                    ndx,
                    (data) => {
                        this.OnSelectSuccess(ndx, data);
                    },
                    this.OnErr);
            }
            else {
                this.OnErr(DI.Wrapper.GetText(MsgCode.NothingSelected));
            }
        }


        private void OnLoadSuccess(List<IIndexItem<DefaultFileExtraInfo>> data) {
            if (data.Count > 0) {
                this.lbEthernet.ItemsSource = data;
                this.lbEthernet.SelectionChanged += this.selectionChanged;
            }
            else {
                this.OnErr(DI.Wrapper.GetText(MsgCode.NotFound));
            }
        }


        private void OnErr(string err) {
            App.ShowMsg(err);
            this.Close();
        }


        private void OnSelectSuccess(IIndexItem<DefaultFileExtraInfo> index, EthernetParams data) {
            this.SelectedEthernet = new EthernetSelectResult() {
                Index = index,
                DataModel = data,
            };
            this.Close();
        }


    }
}
