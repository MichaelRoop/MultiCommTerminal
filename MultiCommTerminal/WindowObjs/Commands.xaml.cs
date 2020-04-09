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

namespace MultiCommTerminal.WindowObjs {

    /// <summary>Interaction logic for Commands.xaml</summary>
    public partial class Commands : Window {
        public Commands() {
            InitializeComponent();
            this.SizeToContent = SizeToContent.WidthAndHeight;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e) {

        }

        private void Window_ContentRendered(object sender, EventArgs e) {

        }

        private void lbTitle_MouseDown(object sender, MouseButtonEventArgs e) {
            this.DragMove();
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
        }

    }
}
