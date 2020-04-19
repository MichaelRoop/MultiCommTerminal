using CommunicationStack.Net.Stacks;
using MultiCommTerminal.WPF_Helpers;
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

namespace MultiCommTerminal.WindowObjs {
    /// <summary>
    /// Interaction logic for TerminatorSelector.xaml
    /// </summary>
    public partial class TerminatorSelector : Window {

        TerminatorFactory factory = new TerminatorFactory();
        private ButtonGroupSizeSyncManager widthManager = null;
        private Window parent = null;


        public TerminatorInfo SelectedTerminator { get; set; } = null;


        public TerminatorSelector(Window parent) {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            this.parent = parent;

            // TODO move to wrapper
            this.listBoxTerminators.ItemsSource = this.factory.Items;

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
        }


        /// <summary>Bind Mouse drag to Template style</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }


        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            TerminatorInfo info = this.listBoxTerminators.SelectedItem as TerminatorInfo;
            if (info != null) {
                this.SelectedTerminator = info;
                this.Close();
            }
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.SelectedTerminator = null;
            this.Close();
        }
    }
}
