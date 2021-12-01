using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfCustomControlLib.Net6.Helpers;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.WindowObjs {

    /// <summary>Interaction logic for ScriptEdit.xaml</summary>
    public partial class ScriptEdit : Window {

        #region Data types

        /// <summary>Functionality differs by use type</summary>
        public enum UseType {
            View,
            Edit,
            New,
        }

        #endregion

        #region Data

        Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;
        private ScriptDataModel original = null;
        private ScriptDataModel copy = null;
        private IIndexItem<DefaultFileExtraInfo> index = null;
        private UseType useType = UseType.View;

        #endregion

        #region Constructors and window events

        public ScriptEdit(Window parent, IIndexItem<DefaultFileExtraInfo> index, UseType useType) {
            this.parent = parent;
            this.index = index;
            this.useType = useType;
            this.wrapper = DI.Wrapper;
            InitializeComponent();
            this.PopulateDataFields();
            this.ShowControls();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            if (this.widthManager != null) {
                this.widthManager.Teardown();
            }
        }

        #endregion

        #region Side button event handlers

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            ScriptItem item = new ScriptItem() {
                Display = "Display Name",
                Command = "Command text",
            };
            ScriptCmdEdit win = new ScriptCmdEdit(this, item);
            win.ShowDialog();
            if (win.IsChanged) {
                this.lbxCmds.ItemsSource = null;
                this.copy.Items.Add(item);
                this.lbxCmds.ItemsSource = this.copy.Items;
            }
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            ScriptItem item = this.lbxCmds.SelectedItem as ScriptItem;
            if (item != null) {
                ScriptCmdEdit win = new ScriptCmdEdit(this, item);
                win.ShowDialog();
                if (win.IsChanged) {
                    this.lbxCmds.ItemsSource = null;
                    this.copy.Items.Remove(item);
                    this.copy.Items.Add(item);
                    this.lbxCmds.ItemsSource = this.copy.Items;
                }
            }
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            ScriptItem item = this.lbxCmds.SelectedItem as ScriptItem;
            if (item != null) {
                this.lbxCmds.ItemsSource = null;
                this.copy.Items.Remove(item);
                this.lbxCmds.ItemsSource = this.copy.Items;
            }
        }

        #endregion

        #region Main buttons

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }


        private void btnOk_Click(object sender, RoutedEventArgs e) {
            switch (this.useType) {
                case UseType.View:
                    // Nothing to do
                    break;
                case UseType.Edit:
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
                    break;
                case UseType.New:
                    this.copy.Display = this.txtName.Text;
                    this.wrapper.CreateNewScript(this.copy.Display, this.copy, () => { }, App.ShowMsg);
                    break;
            }
            this.Close();
        }

        #endregion

        #region Private

        /// <summary>Retrieve the script from storage</summary>
        private void RetrieveScript() {
            this.wrapper.RetrieveScriptData(
                this.index, this.Delegate_OnRetrieveSuccess, this.Delegate_OnRetrieveFailure);
        }


        /// <summary>Create a new dummy script template</summary>
        private void InitializeNewScript() {
            List<ScriptItem> items = new List<ScriptItem>();
            items.Add(new ScriptItem("Command Name 1", "Sample Command Text 1"));
            items.Add(new ScriptItem("Command Name 2", "Sample Command Text 2"));
            this.copy = new ScriptDataModel(items) {
                Display = "Sample Script Name",
            };
        }


        /// <summary>Populate controls in the dialog depending on use type</summary>
        private void PopulateDataFields() {
            switch (this.useType) {
                case UseType.View:
                case UseType.Edit:
                    this.RetrieveScript();
                    break;
                case UseType.New:
                    this.InitializeNewScript();
                    break;
            }
            this.txtName.Text = this.copy.Display;
            this.lbxCmds.ItemsSource = copy.Items;
        }


        /// <summary>Show or hide controls based on use type</summary>
        private void ShowControls() {
            switch (this.useType) {
                case UseType.View:
                    this.btnCancel.Visibility = Visibility.Collapsed;
                    this.txtName.IsEnabled = false;
                    this.lbxCmds.IsEnabled = false;
                    this.stPanelSideButtons.Visibility = Visibility.Collapsed;
                    break;
                case UseType.Edit:
                case UseType.New:
                    // Call before rendering which will trigger initial resize events
                    this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnOk);
                    this.widthManager.PrepForChange();
                    break;
            }
        }

        #endregion

        #region Private Delegates

        /// <summary>On successful retrieval make a copy of the script</summary>
        /// <param name="dataModel">The script data to dopy</param>
        private void Delegate_OnRetrieveSuccess(ScriptDataModel dataModel) {
            this.original = dataModel;
            // Make a copy of the original to avoid changing it unless OK
            this.copy = new ScriptDataModel() {
                Display = this.original.Display,
                UId = this.original.UId,
            };
            foreach (var item in original.Items) {
                this.copy.Items.Add(new ScriptItem() {
                    Display = item.Display,
                    Command = item.Command,
                });
            }
        }


        /// <summary>On failure of retrieval post message and close</summary>
        /// <param name="msg">The message to display</param>
        private void Delegate_OnRetrieveFailure(string msg) {
            App.ShowMsg(msg);
            this.Close();
        }

        #endregion

    }
}
