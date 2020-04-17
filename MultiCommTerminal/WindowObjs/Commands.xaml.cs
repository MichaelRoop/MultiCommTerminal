using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.WPF_Helpers;
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

        // temp
        private List<ScriptInfoDataModel> scripts = new List<ScriptInfoDataModel>();

        public Commands(Window parent) {
            this.parent = parent;
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;

            // Temp for dev
            this.scripts.Add(new ScriptInfoDataModel("CommandSet1.txt"));
            this.scripts.Add(new ScriptInfoDataModel("CommandSet2.txt"));
            this.scripts.Add(new ScriptInfoDataModel("CommandSet3.txt"));
            this.scripts.Add(new ScriptInfoDataModel("CommandSet4.txt"));
            this.scripts.Add(new ScriptInfoDataModel("CommandSet5.txt"));

            this.spEditButtons.Visibility = Visibility.Collapsed;
        }

        public override void OnApplyTemplate() {
            this.BindMouseDownToCustomTitleBar();
            base.OnApplyTemplate();
        }


        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {
            this.lbxCmds.SelectionChanged -= this.lbxCmds_SelectionChanged;
        }

        private void Window_ContentRendered(object sender, EventArgs e) {
            this.lbxCmds.ItemsSource = this.scripts;
            this.lbxCmds.SelectionChanged += this.lbxCmds_SelectionChanged;
            WPF_ControlHelpers.CenterChild(parent, this);
        }

        private void btnExit_Click(object sender, RoutedEventArgs e) {
            this.Close();
        }

        private void btnView_Click(object sender, RoutedEventArgs e) {

        }

        private void btnEdit_Click(object sender, RoutedEventArgs e) {

        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {

        }


        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            ScriptInfoDataModel dm = this.lbxCmds.SelectedItem as ScriptInfoDataModel;
            if (dm != null) {
                this.lbxCmds.SelectionChanged -= this.lbxCmds_SelectionChanged;
                this.lbxCmds.ItemsSource = null;
                this.scripts.Remove(dm);
                this.lbxCmds.ItemsSource = this.scripts;
                this.lbxCmds.UnselectAll();
                this.spEditButtons.Visibility = Visibility.Collapsed;
                this.lbxCmds.SelectionChanged += this.lbxCmds_SelectionChanged;
            }
        }


        private void lbxCmds_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            this.spEditButtons.Visibility = Visibility.Visible;
        }
    }
}
