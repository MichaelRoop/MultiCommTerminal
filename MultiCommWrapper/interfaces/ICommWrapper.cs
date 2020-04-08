using LanguageFactory.data;
using LanguageFactory.Messaging;
using MultiCommData.Net.StorageDataModels;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.interfaces {

    public interface ICommWrapper {

        #region For future

        ///// <summary>Event raised when a device is dropped from OS</summary>
        //event EventHandler<string> BLE_DeviceRemoved;


        ///// <summary>Event raised when a device is discovered</summary>
        //event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceDiscovered;


        //event EventHandler<BTDeviceInfo> BT_DeviceDiscovered;
        //event EventHandler<bool> BT_DiscoveryComplete;
        //event EventHandler<bool> BT_ConnectionCompleted;
        ////event EventHandler<byte[]> BytesReceived;
        //// Intercept and assemble a full message from BT before raising this level event

        #endregion

        #region events

        /// <summary>Event raised when the language is changed</summary>
        event EventHandler<SupportedLanguage> LanguageChanged;

        #endregion

        #region Languages

        void CurrentStoredLanguage();

        void CurrentLanguage(Action<LangCode> onDone);
        void SetLanguage(LangCode code);
        void LanguageList(Action<List<LanguageDataModel>> onDone);

        void SaveLanguage(LangCode code, Action<string> onError);
        void SaveLanguage(LangCode code, Action onSuccess, Action<string> onError);

        string GetText(MsgCode code);

        #endregion

        #region Settings

        void GetSettings(Action<SettingItems> onSuccess, Action<string> onError);
        void SaveSettings(SettingItems settings, Action onSuccess, Action<string> onError);

        #endregion

        void Teardown();

    }
}
