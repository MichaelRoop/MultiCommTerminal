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

            if (this.index == null) {
                // New entry
                this.txtBoxDisplay.Text = "NA";
                this.tEditor.InitialiseEditor(parent, new TerminatorDataModel());
            }
            else {
                this.wrapper.RetrieveTerminatorData(
                    this.index,
                    (dataModel) => {
                        this.txtBoxDisplay.Text = index.Display;
                        this.tEditor.InitialiseEditor(parent, dataModel);
                    },
                    this.delegate_OnInitFail);
            }

            this.tEditor.OnSave += TEditor_OnSave;
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }


        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_ContentRendered(object sender, EventArgs e) {
            WPF_ControlHelpers.CenterChild(parent, this);
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
        }

        #endregion

        #region Button events

        private void TEditor_OnSave(object sender, TerminatorDataModel data) {
            if (this.index == null) {
                this.wrapper.CreateNewTerminator(
                    this.txtBoxDisplay.Text, data, this.OnSaveOk, this.OnSaveFailed);
            }
            else {
                this.index.Display = this.txtBoxDisplay.Text;
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
            App.ShowMsg(err);
        }


        private void delegate_OnInitFail(string err) {
            App.ShowMsg(err);
            this.Close();
        }

        #endregion

    }
}
