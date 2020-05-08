﻿using BluetoothCommon.Net;
using BluetoothLE.Net.DataModels;
using CommunicationStack.Net.Stacks;
using IconFactory.data;
using LanguageFactory.data;
using LanguageFactory.Messaging;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.UserDisplayData.Net;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;

namespace MultiCommWrapper.Net.interfaces {

    #region Delegates used throughout the wrapper

    public delegate void OnErr(string msg);
    public delegate void OnErrTitle(string title, string msg);


    #endregion

    public interface ICommWrapper {

        #region events

        /// <summary>Event raised when the language is changed</summary>
        event EventHandler<SupportedLanguage> LanguageChanged;

        /// <summary>When the current terminator is changed</summary>
        event EventHandler<TerminatorDataModel> CurrentTerminatorChanged;

        #endregion

        #region Languages

        void CurrentStoredLanguage();

        void CurrentLanguage(Action<LangCode> onDone);
        void SetLanguage(LangCode code);
        void LanguageList(Action<List<LanguageDataModel>> onDone);

        void SaveLanguage(LangCode code, Action<string> onError);
        void SaveLanguage(LangCode code, Action onSuccess, Action<string> onError);

        string GetText(MsgCode code);

        string GetText(CommMediumType medium);

        void GetMenuItemDataModel(
            MenuCode menuCode,
            MsgCode msgCode, 
            UIIcon iconCode, 
            string Padding,
            Action<MenuItemDataModel> onSuccess,
            Action<MenuItemDataModel> onError); 

        #endregion

        #region Settings

        void GetSettings(Action<SettingItems> onSuccess, OnErr onError);
        void SaveSettings(SettingItems settings, Action onSuccess, OnErr onError);

        #endregion

        #region Icons

        void IconInfo(UIIcon code, Action<IconDataModel> onSuccess, Action<string> onError);
        void IconInfo(UIIcon code, Action<IconDataModel> onSuccess);

        string IconSource(UIIcon code);

        void CommMediumList(Action<List<CommMedialDisplay>> mediums);


        #endregion

        #region Help            

        /// <summary>Help information on communication mediums</summary>
        /// <param name="onSuccess">List returned on success</param>
        void CommMediumHelpList(Action<List<CommMediumHelp>> onSuccess);

        /// <summary>Retrieve a sample code file for display</summary>
        /// <param name="medium">The type of communication medium to find sample</param>
        /// <param name="onSuccess">Raised when file is found and opened</param>
        /// <param name="onError">Raised on error</param>
        void GetCodeSample(CommMediumType medium, Action<string> onSuccess, OnErrTitle onError);


        void HasCodeSample(CommMediumType medium, Action<CommMediumType> onSuccess, OnErrTitle onError);

        #endregion

        #region Terminators

        /// <summary>Get a list of individual Terminator chars</summary>
        /// <param name="onSuccess">Recovered list</param>
        void GetTerminatorEntitiesList(Action<List<TerminatorInfo>> onSuccess, OnErr onError);

        void GetCurrentTerminator(Action<TerminatorDataModel> onSuccess, OnErr onError);

        void SetCurrentTerminators(TerminatorDataModel data, OnErr onError);

        void SetCurrentTerminators(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError);

        void GetTerminatorList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError);

        void CreateNewTerminator(string display, TerminatorDataModel data, Action onSuccess, OnErr onError);

        void RetrieveTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<TerminatorDataModel> onSuccess, OnErr onError);

        void SaveTerminator(IIndexItem<DefaultFileExtraInfo> idx, TerminatorDataModel data, Action onSuccess, OnErr onError);

        void DeleteTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError);

        #endregion

        #region Bluetooth Classic

        event EventHandler<BTDeviceInfo> BT_DeviceDiscovered;
        event EventHandler<bool> BT_DiscoveryComplete;
        event EventHandler<bool> BT_ConnectionCompleted;
        event EventHandler<string> BT_BytesReceived;

        void BTClassicDiscoverAsync();
        void BTClassicConnectAsync(BTDeviceInfo device);

        void BTClassicDisconnect();

        void BTClassicSend(string msg);

        #endregion

        #region BluetoothLE

        /// <summary>Event raised when a device is dropped from OS</summary>
        //event EventHandler<string> BluetoothLE_DeviceRemoved;


        /// <summary>Event raised when a device is discovered</summary>
        event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceDiscovered;

        /// <summary>Event raised when a device is removed</summary>
        event EventHandler<string> BLE_DeviceRemoved;

        /// <summary>Event raised when BLE device discovery complete</summary>
        event EventHandler<bool> BLE_DeviceDiscoveryComplete;

        /// <summary>Event raised when BLE device properties change</summary>
        event EventHandler<BLE_PropertiesUpdateDataModel> BLE_DeviceUpdated;

        void BLE_DiscoverAsync();

        void BLE_ConnectAsync(BluetoothLEDeviceInfo device);


        /// <summary>Debug method to get string of properties</summary>
        /// <param name="obj">The selected BLE info object</param>
        /// <param name="onComplete">Raisd with title and text for message box</param>
        void BLE_GetDbgInfoStringDump(object obj, Action<string, string> onComplete);

        void BLE_Disconnect();

        #endregion

        void Teardown();

    }
}
