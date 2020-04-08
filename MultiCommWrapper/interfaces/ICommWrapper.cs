using BluetoothCommon.Net;
using IconFactory.data;
using LanguageFactory.data;
using LanguageFactory.Messaging;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.UserDisplayData.Net;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.interfaces {

    public interface ICommWrapper {

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

        void GetMenuItemDataModel(
            MenuCode menuCode,
            MsgCode msgCode, 
            UIIcon iconCode, 
            string Padding,
            Action<MenuItemDataModel> onSuccess,
            Action<MenuItemDataModel> onError); 

        #endregion

        #region Settings

        void GetSettings(Action<SettingItems> onSuccess, Action<string> onError);
        void SaveSettings(SettingItems settings, Action onSuccess, Action<string> onError);

        #endregion

        #region Icons

        void IconInfo(UIIcon code, Action<IconDataModel> onSuccess, Action<string> onError);
        void IconInfo(UIIcon code, Action<IconDataModel> onSuccess);

        string IconSource(UIIcon code);

        #endregion

        #region Bluetooth Classic

        event EventHandler<BTDeviceInfo> BluetoothClassicDeviceDiscovered;
        event EventHandler<bool> BluetoothClassicDiscoveryComplete;
        event EventHandler<bool> BluetoothClassicConnectionCompleted;
        event EventHandler<string> BluetoothClassicBytesReceived;

        void BluetoothClassicDiscoverAsync();
        void BluetoothClassicConnectAsync(BTDeviceInfo device);

        void BluetoothClassicDisconnect();

        void BluetoothClassicSend(string msg);

        #endregion

        #region BluetoothLE

        /// <summary>Event raised when a device is dropped from OS</summary>
        //event EventHandler<string> BluetoothLE_DeviceRemoved;


        /// <summary>Event raised when a device is discovered</summary>
        event EventHandler<BluetoothLEDeviceInfo> BluetoothLE_DeviceDiscovered;

        void BluetoothLEDiscoverAsync();

        void BluetoothLEConnectAsync(BluetoothLEDeviceInfo device);

        #endregion

        void Teardown();

    }
}
