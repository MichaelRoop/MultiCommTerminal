﻿using BluetoothLE.Net.interfaces;

namespace Bluetooth.UWP.Core {

    public class BLE_WinPropertyKeys : IPropertyKeys {
        public string IsConnected => "System.Devices.Aep.IsConnected";

        public string IsConnectable => "System.Devices.Aep.IsConnectable";

        public string CanPair => "System.Devices.Aep.CanPair";

        public string IsPaired => "System.Devices.Aep.IsPaired";

        public string ContainerId => "System.Devices.Aep.ContainerId";

        public string IconPath => "System.Devices.Icon";

        public string GlyphIconPath => "System.Devices.GlyphIcon";

        public string ItemNameDisplay => "System.ItemNameDisplay";


    }
}
