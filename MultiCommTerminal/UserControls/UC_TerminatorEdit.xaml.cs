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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace MultiCommTerminal.UserControls {
    /// <summary>
    /// Interaction logic for UC_TerminatorView.xaml
    /// </summary>
    public partial class UC_TerminatorEdit : UserControl {

        List<UC_RoundIconButton> addButtons = new List<UC_RoundIconButton>();
        List<UC_RoundIconButton> deleteButtons = new List<UC_RoundIconButton>();
        List<StackPanel> buttonPanels = new List<StackPanel>();

        public UC_TerminatorEdit() {
            InitializeComponent();
            this.addButtons.Add(this.btnAdd1);
            this.addButtons.Add(this.btnAdd2);
            this.addButtons.Add(this.btnAdd3);
            this.addButtons.Add(this.btnAdd4);
            this.addButtons.Add(this.btnAdd5);

            this.deleteButtons.Add(this.btnDelete1);
            this.deleteButtons.Add(this.btnDelete2);
            this.deleteButtons.Add(this.btnDelete3);
            this.deleteButtons.Add(this.btnDelete4);
            this.deleteButtons.Add(this.btnDelete5);

            //foreach (var button in this.addButtons) {
            //    button.Visibility = Visibility.Collapsed;
            //}
            //foreach (var button in this.deleteButtons) {
            //    button.Visibility = Visibility.Collapsed;
            //}

            ////this.sp1.Visibility = Visibility.Collapsed;
            //this.sp2.Visibility = Visibility.Collapsed;
            //this.sp3.Visibility = Visibility.Collapsed;
            //this.sp4.Visibility = Visibility.Collapsed;
            //this.sp5.Visibility = Visibility.Collapsed;

            this.buttonPanels.Add(this.sp1);
            this.buttonPanels.Add(this.sp2);
            this.buttonPanels.Add(this.sp3);
            this.buttonPanels.Add(this.sp4);
            this.buttonPanels.Add(this.sp5);

            this.CollapseAllButtons();
            this.CollapseButtonPanels();

            this.sp1.Visibility = Visibility.Visible;
            this.btnAdd1.Visibility = Visibility.Visible;

        }



        private void SetButtonToNone(UC_RoundIconButton add, UC_RoundIconButton delete) {
            add.Visibility = Visibility.Collapsed;
            delete.Visibility = Visibility.Collapsed;
        }

        private void SetButtonToDelete(UC_RoundIconButton add, UC_RoundIconButton delete) {
            add.Visibility = Visibility.Collapsed;
            delete.Visibility = Visibility.Visible;
        }

        private void SetButtonToAdd(UC_RoundIconButton add, UC_RoundIconButton delete) {
            add.Visibility = Visibility.Visible;
            delete.Visibility = Visibility.Collapsed;
        }

        private void SetPanelToVisible(StackPanel sp) {

        }


        private void btnAdd1_Click(object sender, RoutedEventArgs e) {
            SetButtonToDelete(this.btnAdd1, this.btnDelete1);

            SetButtonToAdd(this.btnAdd2, this.btnDelete2);
        }

        private void btnDelete1_Click(object sender, RoutedEventArgs e) {
            // empty it

        }

        private void btnAdd2_Click(object sender, RoutedEventArgs e) {
            SetButtonToNone(this.btnAdd1, this.btnDelete1);
            SetButtonToDelete(this.btnAdd2, this.btnDelete2);
            SetButtonToAdd(this.btnAdd3, this.btnDelete3);

        }

        private void btnDelete2_Click(object sender, RoutedEventArgs e) {

        }

        private void btnAdd3_Click(object sender, RoutedEventArgs e) {

        }

        private void btnDelete3_Click(object sender, RoutedEventArgs e) {

        }

        private void btnAdd4_Click(object sender, RoutedEventArgs e) {

        }

        private void btnDelete4_Click(object sender, RoutedEventArgs e) {

        }

        private void btnAdd5_Click(object sender, RoutedEventArgs e) {

        }

        private void btnDelete5_Click(object sender, RoutedEventArgs e) {

        }




        private void btnCancel_Click(object sender, RoutedEventArgs e) {

        }

        private void btnSave_Click(object sender, RoutedEventArgs e) {

        }


        private void CollapseAllButtons() {
            foreach (var button in this.addButtons) {
                button.Visibility = Visibility.Collapsed;
            }
            foreach (var button in this.deleteButtons) {
                button.Visibility = Visibility.Collapsed;
            }
        }

        private void CollapseButtonPanels() {
            foreach(var p in this.buttonPanels) {
                p.Visibility = Visibility.Collapsed;
            }
        }



    }
}
