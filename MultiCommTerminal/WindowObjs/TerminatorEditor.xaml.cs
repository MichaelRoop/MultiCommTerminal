using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.NetCore.DependencyInjection;
using MultiCommTerminal.NetCore.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Windows;
using WpfCustomControlLib.Core.Helpers;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.NetCore.WindowObjs {


    /// <summary>Interaction logic for TerminatorEditor.xaml</summary>
    public partial class TerminatorEditor : Window {

        #region Data

        Window parent = null;
        private ICommWrapper wrapper = null;
        IIndexItem<DefaultFileExtraInfo> index = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        #endregion

        #region Properties

        public bool IsChanged { get; set; } = false;

        #endregion

        #region Constructors and windows events

        public TerminatorEditor(Window parent, IIndexItem<DefaultFileExtraInfo> index) {
            this.wrapper = DI.Wrapper;
            this.parent = parent;
            this.index = index; 
            InitializeComponent();
            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnSave, this.btnCancel);
            this.widthManager.PrepForChange();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Loaded(object sender, RoutedEventArgs e) {
            if (this.index == null) {
                // New entry
                this.tEditor.InitialiseEditor(parent, new TerminatorDataModel());
            }
            else {
                this.wrapper.RetrieveTerminatorData(this.index, this.OnInitOk, this.OnInitFail);
            }

            this.tEditor.OnSave += TEditor_OnSave;
            DI.Wrapper.GetTerminatorEntitiesList(this.OnTerminatorLoadOk, App.ShowMsg);
            this.listBoxTerminators.SelectionChanged += selectionChangedHandler;


            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.listBoxTerminators.SelectionChanged -= selectionChangedHandler;
            this.widthManager.Teardown();
        }


        private void selectionChangedHandler(object sender, System.Windows.Controls.SelectionChangedEventArgs e) {
            this.listBoxTerminators.SelectionChanged -= selectionChangedHandler;
            TerminatorInfo info = this.listBoxTerminators.SelectedItem as TerminatorInfo;
            if (info != null) {
                this.tEditor.AddNewTerminator(info);
            }
            this.listBoxTerminators.SelectedItem = null;
            this.listBoxTerminators.SelectionChanged += selectionChangedHandler;
        }

        #endregion

        #region Button events

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            this.tEditor.RemoveLastTerminator();
        }


        private void btnSave_Click(object sender, RoutedEventArgs e) {
            this.tEditor.Save();
        }


        private void TEditor_OnSave(object sender, TerminatorDataModel data) {
            if (this.index == null) {
                data.Display = this.txtBoxDisplay.Text;
                this.wrapper.CreateNewTerminator(
                    this.txtBoxDisplay.Text, data, this.OnSaveOk, this.OnSaveFailed);
            }
            else {
                this.index.Display = this.txtBoxDisplay.Text;
                data.Display = this.index.Display;
                this.wrapper.SaveTerminator(this.index, data, this.OnSaveOk, this.OnSaveFailed);
            }
        }

        private void btnCancel_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        #endregion

        #region Delegates

        private void OnSaveOk() {
            this.IsChanged = true;
            this.Close();
        }


        private void OnSaveFailed(string err) {
            this.ShowMsgBox(err);
        }


        private void OnInitFail(string err) {
            this.ShowMsgBox(err);
            this.Close();
        }


        private void OnInitOk(TerminatorDataModel dataModel) {
            this.txtBoxDisplay.Text = index.Display;
            dataModel.Display = index.Display;
            this.tEditor.InitialiseEditor(parent, dataModel);
        }


        private void OnTerminatorLoadOk(List<TerminatorInfo> terminatorEntities) {
            this.listBoxTerminators.ItemsSource = terminatorEntities;
        }

        #endregion

    }
}
