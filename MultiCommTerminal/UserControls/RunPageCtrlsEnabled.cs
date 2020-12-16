
namespace MultiCommTerminal.NetCore.UserControls {

    /// <summary>Specify which controls are enabled</summary>
    public class RunPageCtrlsEnabled {
        public bool Connect { get; set; } = true;
        public bool Disconnect { get; set; } = true;
        public bool Info { get; set; } = true;
        public bool Settings { get; set; } = true;
    }

}
