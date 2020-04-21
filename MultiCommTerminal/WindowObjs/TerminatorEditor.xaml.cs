using MultiCommData.Net.StorageDataModels;
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
    
    
    /// <summary>
    /// Interaction logic for TerminatorEditor.xaml
    /// </summary>
    public partial class TerminatorEditor : Window {

        #region Data
        Window parent = null;
        private ICommWrapper wrapper = null;
        private ButtonGroupSizeSyncManager widthManager = null;

        TerminatorDataModel data = null;
        IIndexItem<DefaultFileExtraInfo> index = null;

        #endregion


        public TerminatorEditor(
            Window parent,
            IIndexItem<DefaultFileExtraInfo> index,
            TerminatorDataModel data) {
            this.parent = parent;
            this.index = index;
            this.data = data;
            InitializeComponent();
            this.btnSave.Visibility = Visibility.Collapsed;

            this.txtBoxDisplay.Text = index.Display;
            this.tEditor.InitialiseEditor(parent, this.data);
            this.tEditor.onOtherButton += TEditor_onOtherButton;
            this.tEditor.OnSave += TEditor_OnSave;


            this.SizeToContent = SizeToContent.WidthAndHeight;


            // Call before rendering which will trigger initial resize events
            this.widthManager = new ButtonGroupSizeSyncManager(this.btnCancel, this.btnSave);
            this.widthManager.PrepForChange();
        }

        private void TEditor_OnSave(object sender, TerminatorDataModel e) {
            //throw new NotImplementedException();
            this.btnSave.Visibility = Visibility.Visible;
        }

        private void TEditor_onOtherButton() {
            //throw new NotImplementedException();
            this.btnSave.Visibility = Visibility.Collapsed;
        }

        /// <summary>Connect the style mouse grab on title bar</summary>
        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void btnCancel_Click(object sender, RoutedEventArgs e) {

            this.Close();
        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {

            this.Close();
        }

        private void Window_ContentRendered(object sender, EventArgs e) {

            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

        }
    }
}
