using MultiCommData.Net.StorageDataModels;

namespace MultiCommData.Net.StorageIndexInfoModels {

    public class EthernetExtraInfo {

        /// <summary>Host name or IP</summary>
        public string Address { get; set; } = string.Empty;

        /// <summary>Socket port such as 80 for HTTP</summary>
        public string Port { get; set; } = string.Empty;

        public EthernetExtraInfo() { }


        public EthernetExtraInfo(EthernetParams info) {
            this.Address = info.EthernetAddress;
            this.Port = info.EthernetServiceName;
        }


    }
}
