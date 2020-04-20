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

namespace MultiCommTerminal.WindowObjs {

    /// <summary>List, add, modify and delete terminator data items</summary>
    public partial class TerminatorDataSelector : Window {

        private Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #region Constructors and window events

        public TerminatorDataSelector(Window parent) {
            this.wrapper = DI.Wrapper;
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            // TODO load the list from storage

            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }

        #endregion

        #region Sidebar buttons

        private void btnAdd_Click(object sender, RoutedEventArgs e) {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {

        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

        #region Window buttons

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e) {

        }

        #endregion

    }
}
