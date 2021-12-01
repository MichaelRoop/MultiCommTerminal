using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using System;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using WpfHelperClasses.Net6;

namespace MultiCommTerminal.NetCore.UserControls {

    /// <summary>Interaction logic for UC_TerminatorView.xaml</summary>
    public partial class UC_TerminatorEdit : UserControl {

        #region Data

        Window parent = null;
        List<StackPanel> panels = new List<StackPanel>();
        List<Label> hex = new List<Label>();
        List<Label> names = new List<Label>();
        private int currentIndex = -1;
        private const int MAX_TERMINATORS = 5;
        private List<TerminatorInfo> selectedTerminators = new List<TerminatorInfo>();
        private string objUID = "";

        #endregion

        #region Events

        public event EventHandler<TerminatorDataModel> OnSave;

        #endregion

        #region Constructors

        public UC_TerminatorEdit() {
            InitializeComponent();
            this.SetupControl();
        }

        #endregion

        #region Public

        public void InitialiseEditor(Window parent, TerminatorDataModel dataModel) {
            this.parent = parent;
            this.objUID = dataModel.UId;

            this.ResetAllBoxes();
            // Make a copy to not mess up the original in case you do not save
            List<TerminatorInfo> infos = new List<TerminatorInfo>();
            foreach (var i in dataModel.TerminatorInfos) {
                infos.Add(new TerminatorInfo(i.Code));
            }

            foreach (TerminatorInfo info in infos) {
                this.AddTerminator(info);
            }
        }


        public bool AddNewTerminator(TerminatorInfo info) {
            if (this.currentIndex < (MAX_TERMINATORS - 1)) {
                if (info != null) {
                    this.AddTerminator(info);
                    return true;
                }
            }
            return false;
        }


        public void RemoveLastTerminator() {
            if (this.currentIndex > -1) {
                this.panels[this.currentIndex].Collapse();
                this.hex[this.currentIndex].Content = "";
                this.names[this.currentIndex].Content = "";
                this.currentIndex--;
                if (this.selectedTerminators.Count > 0) {
                    this.selectedTerminators.RemoveAt(this.selectedTerminators.Count - 1);
                }
                this.Init(this.currentIndex + 1);
            }
        }


        public void Save() {
            // Raise an event so the subscriber can save the data to persistent storage
            if (this.OnSave != null) {
                this.OnSave.Invoke(this, new TerminatorDataModel(this.selectedTerminators) {
                    // Return preserved UID
                    UId = this.objUID,
                });
            }
        }

        #endregion

        #region Private

        private void SetupControl() {
            this.panels.Add(this.sp1);
            this.panels.Add(this.sp2);
            this.panels.Add(this.sp3);
            this.panels.Add(this.sp4);
            this.panels.Add(this.sp5);
            this.CollapsePanels();
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
            this.ResetAllBoxes();

            // Need an init once the current number loaded into the editor
            this.CollapsePanels();
            this.Init(0);
        }


        private void Init(int numberSet) {
            if (numberSet >= 0 && numberSet <= MAX_TERMINATORS) {
                this.currentIndex = numberSet - 1;
            }
        }


        private void CollapsePanels() {
            foreach (var p in this.panels) {
                p.Collapse();
            }
        }


        private void AddTerminator(TerminatorInfo info) {
            this.selectedTerminators.Add(info);
            this.currentIndex++;
            // Set the hex and name before display
            // TODO - also save in a byte block
            this.names[this.currentIndex].Content = info.Display;
            this.hex[this.currentIndex].Content = info.HexDisplay;
            this.panels[this.currentIndex].Show();
            this.Init(this.currentIndex + 1);
        }


        private void ResetAllBoxes() {
            for (int i = 0; i < MAX_TERMINATORS; i++) {
                this.hex[i].Content = "";
                this.names[i].Content = "";
            }
        }

        #endregion

    }
}
