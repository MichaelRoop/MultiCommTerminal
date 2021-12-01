using LogUtils.Net;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.UserDisplayData;
using MultiCommData.UserDisplayData.Net;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for Help_CommunicationMediums.xaml</summary>
    public partial class Help_CommunicationMediums : Window {

        Window parent = null;
        private List<CommMediumHelp> testData = new List<CommMediumHelp>();
        List<CommMediumHelp> mediumHelps = new List<CommMediumHelp>();


        public Help_CommunicationMediums(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            DI.Wrapper.CommMediumHelpList(this.OnBuildMediumHelp);
        }


        /// <summary>Bind Mouse drag to Template style</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            this.CenterToParent(this.parent);
            this.listBoxMediums.SelectionChanged += this.listBoxMediums_SelectionChanged;
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.listBoxMediums.SelectionChanged -= this.listBoxMediums_SelectionChanged;
        }


        private void listBoxMediums_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            if (this.listBoxMediums.SelectedItem != null) {
                this.listBoxMediums.SelectionChanged -= this.listBoxMediums_SelectionChanged;
                CommMediumHelp help = this.listBoxMediums.SelectedItem as CommMediumHelp;
                DI.Wrapper.HasCodeSample(help.Id, this.OnHasCodeSampleView, App.ShowMsgTitle);
                this.listBoxMediums.SelectedItem = null;
                this.listBoxMediums.SelectionChanged += this.listBoxMediums_SelectionChanged;
            }
        }


        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void OnBuildMediumHelp(List<CommMediumHelp> helps) {
            this.mediumHelps = helps;
            this.listBoxMediums.ItemsSource = this.mediumHelps;
        }


        private void OnHasCodeSampleView(CommMedium helpType) {
            // This to avoid opening the code highlighted box because
            // its control malfunctions with no content
            MsgBoxCode win = new MsgBoxCode(this, helpType);
            win.ShowDialog();
        }

    }
}
