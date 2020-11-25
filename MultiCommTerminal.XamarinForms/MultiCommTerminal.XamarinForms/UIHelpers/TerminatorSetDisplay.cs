using CommunicationStack.Net.Stacks;
using System.Collections.Generic;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    /// <summary>Encapsulates the add and removal or terminators from set</summary>
    public class TerminatorSetDisplay {

        #region Data

        private List<TerminatorInfo> infos = new List<TerminatorInfo>();
        private List<TerminatorDisplay> displays = new List<TerminatorDisplay>();
        private int currentCount = 0;
        private const int MAX_ENTRIES = 5;

        #endregion

        #region Properties

        public bool IsChanged { get; set; } = false;


        public List<TerminatorInfo> InfoList { get { return this.infos; } }

        #endregion

        #region Methods

        public bool CreateLabelSet(Label name, Label hex) {
            if (this.displays.Count < MAX_ENTRIES) {
                this.displays.Add(new TerminatorDisplay(name, hex));
                return true;
            }
            return false;
        }


        public void PopulateLabels(List<TerminatorInfo> terminatorList) {
            // Do an assert that there are 5 label sets
            this.Reset();
            foreach (var info in terminatorList) {
                if (this.currentCount <= MAX_ENTRIES) {
                    this.infos.Add(new TerminatorInfo(info.Code));
                    this.displays[this.currentCount-1].SetValues(info);
                    this.currentCount++;
                }
            }
        }


        public void AddEntry(TerminatorInfo info) {
            // Current count acts as next position on add
            if (this.currentCount < MAX_ENTRIES) {
                this.IsChanged = true;
                this.infos.Add(new TerminatorInfo(info.Code));
                this.displays[this.currentCount].SetValues(info);
                this.currentCount++;
            }
        }


        public void RemoveEntry() {
            // the overrun count set to max
            if (this.currentCount > MAX_ENTRIES) {
                this.currentCount = MAX_ENTRIES;
            }

            // underrun protection
            if (this.currentCount > 0) {
                // Decrement before delete. index becomes next available pos
                this.currentCount--;
                this.IsChanged = true;
                this.displays[this.currentCount].Clear();
                this.infos.RemoveAt(this.currentCount);
            }
        }


        /// <summary>Reset the internal display objects</summary>
        public void Reset() {
            this.displays.ForEach((item) => item.Clear());
            this.infos.Clear();
            this.currentCount = 0;
            this.IsChanged = false;
        }

        #endregion

    }
}
