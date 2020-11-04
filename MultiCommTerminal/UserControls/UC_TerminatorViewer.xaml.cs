using CommunicationStack.Net.Stacks;
using LanguageFactory.Net.data;
using MultiCommData.Net.StorageDataModels;
using MultiCommTerminal.DependencyInjection;
using MultiCommTerminal.WPF_Helpers;
using MultiCommWrapper.Net.interfaces;
using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;

namespace MultiCommTerminal.UserControls {

    /// <summary>
    /// Interaction logic for UC_TerminatorViewer.xaml
    /// </summary>
    public partial class UC_TerminatorViewer : UserControl {

        #region Data

        ICommWrapper wrapper = null;
        List<Label> hex = new List<Label>();
        List<Label> names = new List<Label>();
        private const int MAX_TERMINATORS = 5;

        #endregion


        public UC_TerminatorViewer() {
            this.wrapper = DI.Wrapper;
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
            this.wrapper.LanguageChanged += this.Wrapper_LanguageChanged;
        }


        public void Initialise(TerminatorDataModel data) {
            this.lblTerminators.Content = (data.Name.Length > 0) ? data.Name : this.wrapper.GetText(MsgCode.Terminators);
            // Blank out any existing info
            for (int i = 0; i < MAX_TERMINATORS; i++) {
                this.names[i].Content = "";
                this.hex[i].Content = "";
            }

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


        private void Wrapper_LanguageChanged(object sender, LanguageFactory.Net.Messaging.SupportedLanguage e) {
            this.lblTerminators.Content = TxtBinder.Terminators;
        }

    }
}
