using BluetoothCommon.Net;
using BluetoothLE.Net.DataModels;
using Common.Net.Network;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Stacks;
using IconFactory.Net.data;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.UserDisplayData;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.DataModels;
using SerialCommon.Net.DataModels;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using WifiCommon.Net.DataModels;

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

        /// <summary>Raised when the current script has changed</summary>
        event EventHandler<ScriptDataModel> CurrentScriptChanged;

        #endregion

        #region Properties

        /// <summary>Full path and file name of user manual PDF</summary>
        public string UserManualFullFileName { get; }
        
        #endregion

            #region Languages

        void CurrentStoredLanguage();

        void CurrentLanguage(Action<LangCode> onDone);
        void SetLanguage(LangCode code);
        void LanguageList(Action<List<LanguageDataModel>> onDone);

        void SaveLanguage(LangCode code, Action<string> onError);
        void SaveLanguage(LangCode code, Action onSuccess, Action<string> onError);

        string GetText(MsgCode code);

        string GetText(CommHelpType medium);

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

        string IconSource(UIIcon code);

        void CommMediumList(Action<List<CommMedialDisplay>> mediums);


        //void CommHelpList(Action<List>)

        #endregion

        #region Help            

        /// <summary>Help information on communication mediums</summary>
        /// <param name="onSuccess">List returned on success</param>
        void CommMediumHelpList(Action<List<CommMediumHelp>> onSuccess);

        /// <summary>Retrieve a sample code file for display</summary>
        /// <param name="medium">The type of communication medium to find sample</param>
        /// <param name="onSuccess">Raised when file is found and opened</param>
        /// <param name="onError">Raised on error</param>
        void GetCodeSample(CommHelpType medium, Action<string> onSuccess, OnErrTitle onError);


        void HasCodeSample(CommHelpType medium, Action<CommHelpType> onSuccess, OnErrTitle onError);

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

        #region Scripts

        /// <summary>Retrieve the currently selected script</summary>
        /// <param name="onSuccess">Raised on successful retrieval</param>
        /// <param name="onError">Raised on Error</param>
        void GetCurrentScript(Action<ScriptDataModel> onSuccess, OnErr onError);


        void SetCurrentScript(ScriptDataModel data, OnErr onError);


        void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError);

        void GetScriptList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError);

        void CreateNewScript(string display, ScriptDataModel data, Action onSuccess, OnErr onError);


        void RetrieveScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<ScriptDataModel> onSuccess, OnErr onError);

        void SaveScript(IIndexItem<DefaultFileExtraInfo> idx, ScriptDataModel data, Action onSuccess, OnErr onError);


        void DeleteScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError);

        #endregion

        #region Bluetooth Classic

        event EventHandler<BTDeviceInfo> BT_DeviceDiscovered;
        event EventHandler<BTDeviceInfo> BT_DeviceInfoGathered;
        event EventHandler<bool> BT_DiscoveryComplete;
        event EventHandler<bool> BT_ConnectionCompleted;
        event EventHandler<string> BT_BytesReceived;

        /// <summary>Raised when pairing with BT</summary>
        event EventHandler<BT_PairingInfoDataModel> BT_PairInfoRequested;

        /// <summary>Raised on completion of pair operation</summary>
        event EventHandler<BTPairOperationStatus> BT_PairStatus;

        /// <summary>Raised on completion of unpair operation</summary>
        event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

        void BTClassicDiscoverAsync(bool paired);
        void BTClassicConnectAsync(BTDeviceInfo device);

        void BTClassicGetExtraInfoAsync(BTDeviceInfo device);

        void BTClassicDisconnect();

        void BTClassicSend(string msg);

        void BTClassicPairAsync(BTDeviceInfo device);

        void BTClassicUnPairAsync(BTDeviceInfo device);


        /// <summary>Create a list of key value pairs for display of BT Device fields</summary>
        /// <param name="info">The info to parse</param>
        /// <returns>A display list of the fields</returns>
        List<KeyValuePropertyDisplay> BT_GetDeviceInfoForDisplay(BTDeviceInfo info);

        List<NetPropertyDataModelDisplay> BT_GetProperties(BTDeviceInfo info);

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
        event EventHandler<NetPropertiesUpdateDataModel> BLE_DeviceUpdated;

        /// <summary>Raised when BLE info on a device is finished gathering</summary>
        event EventHandler<BluetoothLEDeviceInfo> BLE_DeviceInfoGathered;

        void BLE_DiscoverAsync();

        void BLE_ConnectAsync(BluetoothLEDeviceInfo device);

        /// <summary>Get complete info populated in device</summary>
        /// <param name="device"></param>
        void BLE_GetInfo(BluetoothLEDeviceInfo device);
        
        void BLE_Disconnect();


        /// <summary>Create a list of key value pairs for display of LE Device fields</summary>
        /// <param name="info">The info to parse</param>
        /// <returns>A display list of the fields</returns>
        List<KeyValuePropertyDisplay> BLE_GetDeviceInfoForDisplay(BluetoothLEDeviceInfo info);


        /// <summary>Get a displayable list for the BLE device Service Properties</summary>
        /// <param name="info">The device info to parse</param>
        /// <returns>A display list of BLE device service properties</returns>
        List<NetPropertyDataModelDisplay> BLE_GetServiceProperties(BluetoothLEDeviceInfo info);


        /// <summary>Debug method to get string of properties</summary>
        /// <param name="obj">The selected BLE info object</param>
        /// <param name="onComplete">Raisd with title and text for message box</param>
        void BLE_GetDbgInfoStringDump(object obj, Action<string, string> onComplete);

        #endregion

        #region WIFI

        event EventHandler<List<WifiNetworkInfo>> DiscoveredWifiNetworks;
        event EventHandler<WifiError> OnWifiError;
        event EventHandler<MsgPumpConnectResults> OnWifiConnectionAttemptCompleted;
        event EventHandler<string> Wifi_BytesReceived;
        /// <summary>Raised if there is no password, host name or service name in the connection data model</summary>
        event EventHandler<WifiCredentials> CredentialsRequestedEvent;

        void WifiDiscoverAsync();

        void WifiConnectAsync(WifiNetworkInfo dataModel);
        void WifiDisconect();

        void WifiSend(string msg);

        #endregion

        #region Serial USB

        event EventHandler<List<SerialDeviceInfo>> SerialDiscoveredDevices;
        event EventHandler<SerialUsbError> SerialOnError;
        event EventHandler<string> Serial_BytesReceived;
        // TODO Connection complete event

        void SerialUsbDiscoverAsync();
        void SerialUsbConnect(SerialDeviceInfo dataModel);
        void SerialUsbDisconnect();
        void SerialUsbSend(string msg);

        List<KeyValuePropertyDisplay> Serial_GetDeviceInfoForDisplay(SerialDeviceInfo info);

        #endregion

        #region Wifi credentials storage

        void GetWifiCredList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError);

        void CreateNewWifiCred(string display, WifiCredentialsDataModel data, Action onSuccess, OnErr onError);

        void RetrieveWifiCredData(IIndexItem<DefaultFileExtraInfo> index, Action<WifiCredentialsDataModel> onSuccess, OnErr onError);

        void SaveWifiCred(IIndexItem<DefaultFileExtraInfo> idx, WifiCredentialsDataModel data, Action onSuccess, OnErr onError);


        void DeleteWifiCredData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError);

        #endregion

        void Teardown();

        void DisconnectAll();

    }
}
