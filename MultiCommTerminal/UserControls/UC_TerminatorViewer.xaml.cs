using CommunicationStack.Net.Stacks;
using MultiCommData.Net.StorageDataModels;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.UserControls {

    /// <summary>
    /// Interaction logic for UC_TerminatorViewer.xaml
    /// </summary>
    public partial class UC_TerminatorViewer : UserControl {

        #region Data

        List<Label> hex = new List<Label>();
        List<Label> names = new List<Label>();
        private const int MAX_TERMINATORS = 5;

        #endregion


        public UC_TerminatorViewer() {
            InitializeComponent();

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
        }


        public void Initialise(TerminatorDataModel data) {
            int index = 0;
            foreach (TerminatorInfo info in data.TerminatorInfos) {
                if (index < MAX_TERMINATORS) {
                    this.AddTerminator(info, index);
                    index++;
                }
            }
        }


        private void AddTerminator(TerminatorInfo info, int index) {
            this.names[index].Content = info.Display;
            this.hex[index].Content = info.HexDisplay;
        }


    }
}
