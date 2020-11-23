namespace MultiCommWrapper.Net.Helpers {
    
    /// <summary>
    /// Hold scratch object instances to pass between mobile screens
    /// that can only pass their parameters via string routing that
    /// lose the instance
    /// </summary>
    public class ScratchSet {

        public ScratchScriptDataModel ScriptCommandSet { get; set; } = new ScratchScriptDataModel();
        public ScratchScriptCommand ScriptCommand { get; set; } = new ScratchScriptCommand();
        public ScratchWifiCredentials WifiCred { get; set; } = new ScratchWifiCredentials();
    }
}
