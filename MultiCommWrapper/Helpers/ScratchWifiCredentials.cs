using MultiCommData.Net.StorageDataModels;

namespace MultiCommWrapper.Net.Helpers {

    /// <summary>
    /// Holds a WifiCredentialsDataModel to be stored in memory between 
    /// mobile screens which can only route parameters as strings where
    /// we lose instance on change
    /// </summary>
    public class ScratchWifiCredentials {
        public ScratchMode Mode { get; set; } = ScratchMode.Edit;
        public WifiCredentialsDataModel WifiCredentials { get; set; } = new WifiCredentialsDataModel();
    }
}
