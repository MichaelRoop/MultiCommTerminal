using BluetoothCommon.Net;
using LanguageFactory.Messaging;
using System;
using System.Collections.Generic;
using System.Text;

namespace MultiCommWrapper.Net.interfaces {
    
    public interface ICommWrapper {

        ///// <summary>Event raised when a device is dropped from OS</summary>
        //event EventHandler<string> BLE_DeviceRemoved;


        ///// <summary>Event raised when a device is discovered</summary>
        //event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceDiscovered;


        //event EventHandler<BTDeviceInfo> BT_DeviceDiscovered;
        //event EventHandler<bool> BT_DiscoveryComplete;
        //event EventHandler<bool> BT_ConnectionCompleted;
        ////event EventHandler<byte[]> BytesReceived;
        //// Intercept and assemble a full message from BT before raising this level event


        #region Language Methods and events

        /// <summary>Event raised when the language is changed</summary>
        event EventHandler<SupportedLanguage> LanguageChanged;


        #endregion

        void Teardown();



    }
}
