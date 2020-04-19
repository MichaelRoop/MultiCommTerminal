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

        List<StackPanel> buttonPanels = new List<StackPanel>();
        List<Label> hex = new List<Label>();
        List<Label> names = new List<Label>();
        private int currentIndex = -1;
        private const int MAX_TERMINATORS = 5;


        public UC_TerminatorEdit() {
            InitializeComponent();
            this.buttonPanels.Add(this.sp1);
            this.buttonPanels.Add(this.sp2);
            this.buttonPanels.Add(this.sp3);
            this.buttonPanels.Add(this.sp4);
            this.buttonPanels.Add(this.sp5);
            this.CollapseButtonPanels();
            this.currentIndex = -1;

            this.hex.Add(this.hex1);
            this.hex.Add(this.hex2);
            this.hex.Add(this.hex3);
            this.hex.Add(this.hex4);
            this.hex.Add(this.hex5);

            this.names.Add(this.name1);
            this.names.Add(this.name2);
            this.names.Add(this.name3);
            this.names.Add(this.name4);
            this.names.Add(this.name5);


            //this.btnDelete.Visibility = Visibility.Collapsed;

            // Need an init once the current number loaded into the editor
            this.CollapseButtonPanels();
            this.Init(0);
        }



        private void Init(int numberSet) {
            if (numberSet >= 0 && numberSet <= MAX_TERMINATORS) {
                this.currentIndex = numberSet - 1;

                switch (numberSet) {
                    case 0:
                        this.CollapseButtonPanels();
                        this.SetVisible(this.btnAdd);
                        this.SetCollapsed(this.btnDelete);
                        break;
                    case MAX_TERMINATORS:
                        this.SetVisible(this.btnDelete);
                        this.SetCollapsed(this.btnAdd);
                        break;
                    default:
                        this.SetVisible(this.btnAdd);
                        this.SetVisible(this.btnDelete);
                        break;

                }
            }

        }




        private void CollapseButtonPanels() {
            foreach(var p in this.buttonPanels) {
                this.SetCollapsed(p);
            }
        }

        private void btnAdd_Click(object sender, RoutedEventArgs e) {
            //if (this.currentNumber == 0) {
            //    this.SetVisible(this.btnDelete);
            //}

            if (this.currentIndex < (MAX_TERMINATORS-1)) {
                this.currentIndex++;
                this.SetVisible(this.buttonPanels[this.currentIndex]);
                this.Init(this.currentIndex + 1);
            }
        }

        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            if (this.currentIndex > -1) {
                this.SetCollapsed(this.buttonPanels[this.currentIndex]);
                this.currentIndex--;
                this.Init(this.currentIndex + 1);
            }
        }




        private void SetVisible(StackPanel panel) {
            panel.Visibility = Visibility.Visible;
        }

        private void SetCollapsed(StackPanel panel) {
            panel.Visibility = Visibility.Hidden;
        }


        private void SetVisible(Control ctrl) {
            ctrl.Visibility = Visibility.Visible;
        }

        private void SetCollapsed(Control ctrl) {
            ctrl.Visibility = Visibility.Hidden;
        }



    }
}
