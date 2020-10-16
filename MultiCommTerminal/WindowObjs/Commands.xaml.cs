using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.NetCore.WindowObjs;
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

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for Commands.xaml</summary>
    public partial class Commands : Window {

        private Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;


        public Commands(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
            this.wrapper = DI.Wrapper;
            this.wrapper.CurrentScriptChanged += Wrapper_CurrentScriptChanged;
            this.spEditButtons.Visibility = Visibility.Collapsed;

            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSelect);
            this.widthManager.PrepForChange();
        }

        private void Wrapper_CurrentScriptChanged(object sender, ScriptDataModel e) {
            this.ReloadList(true);
        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbxCmds.SelectionChanged -= this.lbxCmds_SelectionChanged;
            this.wrapper.CurrentScriptChanged -= this.Wrapper_CurrentScriptChanged;
            this.widthManager.Teardown();
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            this.ReloadList(true);
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnSelect_Click(object sender, RoutedEventArgs e) {
            var item = this.lbxCmds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                this.wrapper.SetCurrentScript(item, this.Close, App.ShowMsg);
            }
        }


        private void btnView_Click(object sender, RoutedEventArgs e) {
            this.OpenViewer(ScriptEdit.UseType.View);
        }


        private void btnEdit_Click(object sender, RoutedEventArgs e) {
            this.OpenViewer(ScriptEdit.UseType.Edit);
        }


        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            ScriptEdit win = new ScriptEdit(this, null, ScriptEdit.UseType.New);
            win.ShowDialog();
        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            var item = this.lbxCmds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                this.wrapper.DeleteScriptData(item, this.ReloadList, App.ShowMsg);
            }
        }


        private void lbxCmds_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.spEditButtons.Visibility = Visibility.Visible;
        }


        private void ReloadList(bool isChanged) {
            if (isChanged) {
                this.lbxCmds.SelectionChanged -= this.lbxCmds_SelectionChanged;
                this.wrapper.GetScriptList(
                    (items) => {
                        this.lbxCmds.ItemsSource = null;
                        this.lbxCmds.ItemsSource = items;
                    }, App.ShowMsg);

                this.lbxCmds.SelectionChanged += this.lbxCmds_SelectionChanged;
            }
        }


        private void OpenViewer(ScriptEdit.UseType useType) {
            var item = this.lbxCmds.SelectedItem as IIndexItem<DefaultFileExtraInfo>;
            if (item != null) {
                ScriptEdit win = new ScriptEdit(this, item, useType);
                win.ShowDialog();
            }
        }

    }
}
