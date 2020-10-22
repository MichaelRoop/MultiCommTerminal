using CommunicationStack.Net.Stacks;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Presents a list of potential terminators for selection</summary>
    public partial class TerminatorSelector : Window {

        #region Data

        private ButtonGroupSizeSyncManager widthManager = null;
        private Window parent = null;

        #endregion

        #region Properties

        /// <summary>Holds the selected terminator or null if none selected</summary>
        public TerminatorInfo SelectedTerminator { get; set; } = null;

        #endregion

        #region Constructor and window events

        public TerminatorSelector(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            DI.Wrapper.GetTerminatorEntitiesList(this.OnTerminatorLoadOk, App.ShowMsg);

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
        }


        /// <summary>Bind Mouse drag to Template style</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.widthManager.Teardown();
        }

        #endregion

        #region Private Button event handlers

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

        #endregion

        #region Delegates

        private void OnTerminatorLoadOk(List<TerminatorInfo> terminatorEntities) {
            this.listBoxTerminators.ItemsSource = terminatorEntities;
        }

        #endregion

    }
}
