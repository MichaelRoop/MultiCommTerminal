﻿using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;

namespace MultiCommWrapper.Net.interfaces {

    /// <summary>Provides a way to inject OS specific storage managers into the factory</summary>
    public interface IStorageManagerSet {

        /// <summary>Storage for current settings</summary>
        IStorageManager<SettingItems> Settings { get; }


        /// <summary>Terminator indexed storage</summary>
        IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> Terminators { get; }


        /// <summary>Scipts indexed storage</summary>
        IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> Scripts { get; }


        /// <summary>BLE commands</summary>
        IIndexedStorageManager<BLECommandSetDataModel, BLECmdIndexExtraInfo> BLECommands { get; }

        /// <summary>WIFI credentials indexed storage</summary>
        IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> WifiCred { get; }


        /// <summary>Serial parameters indexed storage</summary>
        IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> Serial { get; }


        /// <summary>Ethernet params indexed storage</summary>
        IIndexedStorageManager<EthernetParams, EthernetExtraInfo> Ethernet { get; }

    }

}


