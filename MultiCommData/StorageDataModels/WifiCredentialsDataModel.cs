using System;

namespace MultiCommData.Net.StorageDataModels {

    /// <summary>Storage and retrieval of data on Wifi Connection</summary>
    public class WifiCredentialsDataModel {

        /// <summary>The UID for storage identification</summary>
        public string UId { get; set; } = "";


        /// <summary>Part of MS to differentiate between networks with same SSID</summary>
        public Guid Id { get; set; } = Guid.Empty;

        public string SSID { get; set; } = string.Empty;

        /// <summary>The password for the wifi network</summary>
        public string WifiPassword { get; set; } = string.Empty;

        /// <summary>The user name for the wifi network, required for some protocols</summary>
        public string WifiUserName { get; set; } = string.Empty;

        /// <summary>Socket host name or IP</summary>
        public string RemoteHostName { get; set; } = string.Empty;

        /// <summary>Socket port such as 80 for HTTP</summary>
        public string RemoteServiceName { get; set; } = string.Empty;

        public WifiCredentialsDataModel() {
            this.UId = Guid.NewGuid().ToString();
        }

    }
}
