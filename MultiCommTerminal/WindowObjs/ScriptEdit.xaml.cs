using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
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

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for ScriptEdit.xaml</summary>
    public partial class ScriptEdit : Window {

        #region Data

        Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private ScriptIndexDataModel original = null;
        private ScriptIndexDataModel copy = null;

        IIndexItem<DefaultFileExtraInfo> index = null;

        private bool edit = false;

        #endregion

        private void RetrieveScript() {
            this.wrapper.RetrieveScriptData(
                this.index, 
                this.Delegate_OnRetrieveSuccess, 
                this.Delegate_OnRetrieveFailure);
        }

        private void Delegate_OnRetrieveSuccess(ScriptIndexDataModel dataModel) {
            this.original = dataModel;
            // Make a copy of the original to avoid changing it unless OK
            this.copy = new ScriptIndexDataModel() {
                Display = this.original.Display,
                UId = this.original.UId,
            };
            foreach (var item in original.Items) {
                this.copy.Items.Add(new ScriptItem() {
                    Display = item.Display,
                    Command = item.Command,
                });
            }
            this.txtName.Text = this.copy.Display;
            this.lbxCmds.ItemsSource = copy.Items;

            // TODO Hide or show
            if (this.edit) {
                // Call before rendering which will trigger initial resize events
                this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
                this.widthManager.PrepForChange();
            }
            else {
                this.btnCancel.Visibility = Visibility.Collapsed;
                this.txtName.IsEnabled = false;
                this.lbxCmds.IsEnabled = false;
                this.stPanelSideButtons.Visibility = Visibility.Collapsed;
            }
        }

        private void Delegate_OnRetrieveFailure(string msg) {
            App.ShowMsg(msg);
            this.Close();
        }



        public ScriptEdit(Window parent, IIndexItem<DefaultFileExtraInfo> index, bool edit = true) {
            this.parent = parent;
            this.index = index;
            this.edit = edit;
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.RetrieveScript();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (this.edit) {
                this.widthManager.Teardown();
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            App.ShowMsg("Add Not Implemented");
        }

        private void btnView_Click(object sender, RoutedEventArgs e) {
            App.ShowMsg("View Not Implemented");
        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            App.ShowMsg("Edit Not Implemented");
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            ScriptItem item = this.lbxCmds.SelectedItem as ScriptItem;
            if (item != null) {
                this.lbxCmds.ItemsSource = null;
                this.copy.Items.Remove(item);
                this.lbxCmds.ItemsSource = this.copy.Items;
            }
        }

        #region Main buttons

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            // TODO determine if true
            if (this.edit) {
                // Update index item
                this.index.Display = this.txtName.Text;
                // Update original from the copy
                this.original.Display = this.txtName.Text;
                this.original.UId = this.copy.UId;
                this.original.Items.Clear();
                foreach (var objItem in this.lbxCmds.Items) {
                    ScriptItem item = objItem as ScriptItem;
                    this.original.Items.Add(new ScriptItem() {
                        Display = item.Display,
                        Command = item.Command,
                    });
                }
                this.wrapper.SaveScript(this.index, this.original, () => { }, App.ShowMsg);
            }
            this.Close();
        }

        #endregion
    }
}
