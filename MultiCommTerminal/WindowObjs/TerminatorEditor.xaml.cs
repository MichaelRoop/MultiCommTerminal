using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Windows;
using WpfHelperClasses.Core;

namespace MultiCommTerminal.WindowObjs {


    /// <summary>Interaction logic for TerminatorEditor.xaml</summary>
    public partial class TerminatorEditor : Window {

        #region Data

        Window parent = null;
        private ICommWrapper wrapper = null;
        IIndexItem<DefaultFileExtraInfo> index = null;

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
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        #endregion

        #region Button events

        private void TEditor_OnSave(object sender, TerminatorDataModel data) {
            if (this.index == null) {
                data.Name = this.txtBoxDisplay.Text;
                this.wrapper.CreateNewTerminator(
                    this.txtBoxDisplay.Text, data, this.OnSaveOk, this.OnSaveFailed);
            }
            else {
                this.index.Display = this.txtBoxDisplay.Text;
                data.Name = this.index.Display;
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
            dataModel.Name = index.Display;
            this.tEditor.InitialiseEditor(parent, dataModel);
        }

        #endregion

    }
}
