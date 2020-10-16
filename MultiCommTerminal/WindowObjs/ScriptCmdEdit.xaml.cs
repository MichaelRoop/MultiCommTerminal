using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.WPF_Helpers;
using System;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for ScriptCmdEdit.xaml</summary>
    public partial class ScriptCmdEdit : Window {

        private Window parent = null;
        private ScriptItem scriptItem = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        public bool IsChanged { get; set; } = false;

        public ScriptCmdEdit(Window parent, ScriptItem scriptItem, bool viewOnly = false) {
            this.parent = parent;
            this.scriptItem = scriptItem;
            InitializeComponent();

            this.txtName.Text = this.scriptItem.Display;
            this.txtCommand.Text = this.scriptItem.Command;

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
            this.widthManager.PrepForChange();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.txtCommand.IsEnabled = !viewOnly;
            this.txtName.IsEnabled = !viewOnly;
        }


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


        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            string name = this.txtName.Text.Trim();
            string cmd = this.txtCommand.Text.Trim();
            if (name.Length > 0 && cmd.Length > 0) {
                this.IsChanged = true;
                this.scriptItem.Display = name;
                this.scriptItem.Command = cmd;
                this.Close();
            }
        }
    }
}
