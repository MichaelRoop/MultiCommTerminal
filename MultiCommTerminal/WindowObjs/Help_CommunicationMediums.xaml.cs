using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for Help_CommunicationMediums.xaml</summary>
    public partial class Help_CommunicationMediums : Window {

        Window parent = null;
        private List<CommMediumHelp> testData = new List<CommMediumHelp>();
        List<CommMediumHelp> mediumHelps = new List<CommMediumHelp>();


        public Help_CommunicationMediums(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.spView.Visibility = Visibility.Collapsed;
            this.SizeToContent = SizeToContent.WidthAndHeight;
            DI.Wrapper.CommMediumHelpList(this.OnBuildMediumHelp);
        }


        /// <summary>Bind Mouse drag to Template style</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            this.CenterToParent(this.parent);
        }


        private void listBoxMediums_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (this.listBoxMediums.SelectedItem != null) {
                CommMediumHelp help = this.listBoxMediums.SelectedItem as CommMediumHelp;
                DI.Wrapper.HasCodeSample(help.Id, this.OnSelectedHasCodeSample, this.OnSelectedNoCodeSample);
            }
        }


        private void btnCode_Click(object sender, RoutedEventArgs e) {
            if (this.listBoxMediums.SelectedItem != null) {
                CommMediumHelp help = this.listBoxMediums.SelectedItem as CommMediumHelp;
                DI.Wrapper.HasCodeSample(help.Id, this.OnHasCodeSampleView, WindowHelpers.ShowMsgTitle);
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void OnBuildMediumHelp(List<CommMediumHelp> helps) {
            this.mediumHelps = helps;
            this.listBoxMediums.ItemsSource = this.mediumHelps;
        }


        private void OnHasCodeSampleView(CommMediumType medium) {
            // This to avoid opening the code highlighted box because
            // its control malfunctions with no content
            MsgBoxCode win = new MsgBoxCode(this, medium);
            win.ShowDialog();
        }


        private void OnSelectedHasCodeSample(CommMediumType medium) {
            this.spView.Visibility = Visibility.Visible;
        }


        private void OnSelectedNoCodeSample(string title, string msg) {
            this.spView.Visibility = Visibility.Collapsed;
        }

    }
}
