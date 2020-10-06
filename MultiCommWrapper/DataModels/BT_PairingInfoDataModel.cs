using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.DataModels {

    public class BT_PairingInfoDataModel {

        public string RequestTitle { get; set; } = "";
        public string RequestMsg { get; set; } = "";
        public bool HasUserConfirmed { get; set; } = false;
        public bool IsPinRequested { get; set; } = false;
        public string PIN { get; set; } = "";
    }
}
