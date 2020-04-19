using CommunicationStack.Net.Stacks;
using MultiCommTerminal.WindowObjs;
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

        #region Data

        Window parent = null;
        List<StackPanel> buttonPanels = new List<StackPanel>();
        List<Label> hex = new List<Label>();
        List<Label> names = new List<Label>();
        private int currentIndex = -1;
        private const int MAX_TERMINATORS = 5;
        private List<TerminatorInfo> selectedTerminators = new List<TerminatorInfo>();


        #endregion

        #region Events

        public event EventHandler<TerminatorData> OnSave;

        #endregion


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


            for (int i = 0; i < MAX_TERMINATORS; i++) {
                this.hex[i].Content = "";
                this.names[i].Content = "";
            }

            // Need an init once the current number loaded into the editor
            this.CollapseButtonPanels();
            this.Init(0);
        }


        public void InitialiseEditor(Window parent, TerminatorData data) {
            this.parent = parent;
            foreach (TerminatorInfo info in data.TerminatorInfos) {
                this.AddTerminator(info);
            }
        }


        private void Init(int numberSet) {
            if (numberSet >= 0 && numberSet <= MAX_TERMINATORS) {
                this.currentIndex = numberSet - 1;

                switch (numberSet) {
                    case 0:
                        this.CollapseButtonPanels();
                        this.SetVisible(this.btnAdd);
                        this.SetCollapsed(this.btnDelete);
                        this.SetCollapsed(this.btnSave);
                        break;
                    case MAX_TERMINATORS:
                        this.SetCollapsed(this.btnAdd);
                        this.SetVisible(this.btnDelete);
                        this.SetVisible(this.btnSave);
                        break;
                    default:
                        this.SetVisible(this.btnAdd);
                        this.SetVisible(this.btnDelete);
                        this.SetVisible(this.btnSave);
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
            if (this.currentIndex < (MAX_TERMINATORS-1)) {
                TerminatorSelector win = new TerminatorSelector(this.parent);
                win.ShowDialog();
                TerminatorInfo info = win.SelectedTerminator;
                if (info != null) {
                    this.AddTerminator(info);
                }
            }
        }


        private void AddTerminator(TerminatorInfo info) {
            this.selectedTerminators.Add(info);
            this.currentIndex++;
            // Set the hex and name before display
            // TODO - also save in a byte block
            this.names[this.currentIndex].Content = info.Display;
            this.hex[this.currentIndex].Content = info.HexDisplay;
            this.SetVisible(this.buttonPanels[this.currentIndex]);
            this.Init(this.currentIndex + 1);
        }



        private void btnDelete_Click(object sender, RoutedEventArgs e) {
            if (this.currentIndex > -1) {
                this.SetCollapsed(this.buttonPanels[this.currentIndex]);
                this.hex[this.currentIndex].Content = "";
                this.names[this.currentIndex].Content = "";
                this.currentIndex--;
                if (this.selectedTerminators.Count > 0) {
                    this.selectedTerminators.RemoveAt(this.selectedTerminators.Count - 1);
                }
                this.Init(this.currentIndex + 1);
            }
        }


        private void btnSave_Click(object sender, RoutedEventArgs e) {
            // Raise an event so the subscriber can save the data to persistent storage
            if (this.OnSave != null) {
                this.OnSave.Invoke(this, new TerminatorData(this.selectedTerminators));
            }
        }


        #region Private

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

        #endregion

    }
}
