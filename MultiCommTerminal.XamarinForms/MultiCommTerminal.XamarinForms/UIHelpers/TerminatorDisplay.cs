using CommunicationStack.Net.Stacks;
using Xamarin.Forms;

namespace MultiCommTerminal.XamarinForms.UIHelpers {

    /// <summary>Manage the display of one terminator info object</summary>
    public class TerminatorDisplay {

        public Label Hex { get; set; }
        public Label Name { get; set; }

        public void Clear() {
            this.Hex.Text = "";
            this.Name.Text = "";
        }


        public TerminatorDisplay(Label name, Label hex) {
            this.Name = name;
            this.Hex = hex;
        }


        public void SetValues(TerminatorInfo info) {
            this.Hex.Text = info.HexDisplay;
            this.Name.Text = info.Display;
        }

    }
}
