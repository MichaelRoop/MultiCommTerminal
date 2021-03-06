using BluetoothCommon.Net;
using BluetoothLE.Net.DataModels;
using BluetoothLE.Net.Enumerations;
using ChkUtils.Net.ErrObjects;
using Common.Net.Network;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Stacks;
using Ethernet.Common.Net.DataModels;
using IconFactory.Net.data;
using LanguageFactory.Net.data;
using LanguageFactory.Net.Messaging;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommData.Net.UserDisplayData;
using MultiCommData.UserDisplayData.Net;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.Helpers;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
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

        event EventHandler<TerminatorDataModel> CurrentTerminatorChangedBT;
        event EventHandler<TerminatorDataModel> CurrentTerminatorChangedBLE;
        event EventHandler<TerminatorDataModel> CurrentTerminatorChangedUSB;
        event EventHandler<TerminatorDataModel> CurrentTerminatorChangedWIFI;
        event EventHandler<TerminatorDataModel> CurrentTerminatorChangedEthernet;


        /// <summary>Raised when the current script has changed</summary>
        event EventHandler<ScriptDataModel> CurrentScriptChanged;

        event EventHandler<ScriptDataModel> CurrentScriptChangedBT;
        event EventHandler<ScriptDataModel> CurrentScriptChangedBLE;
        event EventHandler<ScriptDataModel> CurrentScriptChangedUSB;
        event EventHandler<ScriptDataModel> CurrentScriptChangedWIFI;
        event EventHandler<ScriptDataModel> CurrentScriptChangedEthernet;

        /// <summary>Use in all public facing functions to signal unexpected condition</summary>
        event EventHandler<ErrReport> UnexpectedExceptionEvent;


        #endregion

        #region Properties

        /// <summary>Full path and file name of user manual PDF</summary>
        public string UserManualFullFileName { get; }
        
        #endregion

            #region Languages

        void CurrentStoredLanguage();

        void CurrentLanguage(Action<LangCode> onDone);
        void CurrentSupportedLanguage(Action<SupportedLanguage> onDone);
        void SetLanguage(LangCode code);
        void LanguageList(Action<List<LanguageDataModel>> onDone);

        void SaveLanguage(LangCode code, Action<string> onError);
        void SaveLanguage(LangCode code, Action onSuccess, Action<string> onError);

        string GetText(MsgCode code);

        string GetText(CommMedium medium);

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
        void GetCodeSample(CommMedium medium, Action<string> onSuccess, OnErrTitle onError);


        void HasCodeSample(CommMedium medium, Action<CommMedium> onSuccess, OnErrTitle onError);

        #endregion

        #region Terminators

        /// <summary>Get a list of individual Terminator chars</summary>
        /// <param name="onSuccess">Recovered list</param>
        void GetTerminatorEntitiesList(Action<List<TerminatorInfo>> onSuccess, OnErr onError);

        void GetCurrentTerminator(Action<TerminatorDataModel> onSuccess, OnErr onError);

        /// <summary>Get the current saved terminator set for a communication medium type</summary>
        /// <param name="medium">The communication medium type</param>
        /// <param name="onSuccess">Raise on success with the terminator set</param>
        /// <param name="onError">Raised on error</param>
        void GetCurrentTerminator(CommMedium medium, Action<TerminatorDataModel> onSuccess, OnErr onError);

        void SetCurrentTerminators(TerminatorDataModel data, OnErr onError);

        /// <summary>Set the default for a specific medium type</summary>
        /// <param name="data">The new terminator data</param>
        /// <param name="medium">The communication medium type</param>
        /// <param name="onError">Raised on storage error</param>
        void SetCurrentTerminators(TerminatorDataModel data, CommMedium medium, OnErr onError);


        /// <summary>Set the default for a specific medium type</summary>
        /// <param name="index">The index of the selected terminator set</param>
        /// <param name="medium">The communication medium type</param>
        /// <param name="onSuccess">Raised on success</param>
        /// <param name="onError">Raised on storage error</param>
        void SetCurrentTerminators(IIndexItem<DefaultFileExtraInfo> index, CommMedium medium, Action onSuccess, OnErr onError);


        void SetCurrentTerminators(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError);

        void GetTerminatorList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError);

        void CreateNewTerminator(string display, TerminatorDataModel data, Action onSuccess, OnErr onError);

        void CreateNewTerminator(string display, TerminatorDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError);

        void RetrieveTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<TerminatorDataModel> onSuccess, OnErr onError);

        void SaveTerminator(IIndexItem<DefaultFileExtraInfo> idx, TerminatorDataModel data, Action onSuccess, OnErr onError);

        void DeleteTerminatorData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError);

        void CreateArduinoTerminators(Action onSuccess, OnErr onError);
        void CreateDefaultTerminators(Action onSuccess, OnErr onError);

        #endregion

        #region Scripts

        /// <summary>Retrieve the currently selected script</summary>
        /// <param name="onSuccess">Raised on successful retrieval</param>
        /// <param name="onError">Raised on Error</param>
        void GetCurrentScript(Action<ScriptDataModel> onSuccess, OnErr onError);


        /// <summary>Get the current default script for comm medium type</summary>
        /// <param name="medium">The communication medium</param>
        /// <param name="onSuccess">Raised on success</param>
        /// <param name="onError">Raised on error</param>
        void GetCurrentScript(CommMedium medium, Action<ScriptDataModel> onSuccess, OnErr onError);


        void SetCurrentScript(ScriptDataModel data, OnErr onError);


        /// <summary>Set the default for a specific medium type</summary>
        /// <param name="data">The new terminator data</param>
        /// <param name="medium">The communication medium type</param>
        /// <param name="onError">Raised on storage error</param>
        void SetCurrentScript(ScriptDataModel data, CommMedium medium, OnErr onError);


        /// <summary>Set the default for a specific medium type</summary>
        /// <param name="index">The index of the selected terminator set</param>
        /// <param name="medium">The communication medium type</param>
        /// <param name="onSuccess">Raised on success</param>
        /// <param name="onError">Raised on storage error</param>
        void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, CommMedium medium, Action onSuccess, OnErr onError);


        void SetCurrentScript(IIndexItem<DefaultFileExtraInfo> index, Action onSuccess, OnErr onError);

        void GetScriptList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError);

        void CreateNewScript(string display, ScriptDataModel data, Action onSuccess, OnErr onError);

        void CreateNewScript(string display, ScriptDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError);
        void RetrieveScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<ScriptDataModel> onSuccess, OnErr onError);

        void SaveScript(IIndexItem<DefaultFileExtraInfo> idx, ScriptDataModel data, Action onSuccess, OnErr onError);


        void DeleteScriptData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError);

        void ValidateScriptItem(ScriptItem item, Action onSuccess, OnErr onError);

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

        void CreateHC05AtCmds(Action onSuccess, OnErr onError);

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
        event EventHandler<BLEGetInfoStatus> BLE_DeviceInfoGathered;

        /// <summary>Raised on completion of a connection attempt</summary>
        event EventHandler<BLEGetInfoStatus> BLE_DeviceConnectResult;

        /// <summary>Raised when BLE info on a device is finished gathering</summary>
        event EventHandler<BLE_CharacteristicReadResult> BLE_CharacteristicReadValueChanged;

        /// <summary>Used to track device provoked disconnection after connection</summary>
        event EventHandler<BLE_ConnectStatusChangeInfo> BLE_ConnectionStatusChanged;

        void BLE_DiscoverAsync();

        /// <summary>Cancel any discovery in progress</summary>
        void BLE_CancelDiscover();

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

        void BLE_Send(string value, BLE_CharacteristicDataModel dataModel, Action onSuccess, OnErr onError);

        void BLE_GetRangeDisplay(BLE_CharacteristicDataModel dataModel, Action<string, string> onSuccess, OnErr onError);

        void Translate(BluetoothLEDeviceInfo device);

        void Translate(BLE_CharacteristicDataModel device);

        #endregion

        #region BluetoothLECmds

        void GetBLECmdList(Action<List<IIndexItem<BLECmdIndexExtraInfo>>> onSuccess, OnErr onError);

        void CreateBLECmdSet(string display, BLECommandSetDataModel data, BLECmdIndexExtraInfo extra, Action<IIndexItem<BLECmdIndexExtraInfo>> onSuccess, OnErr onError);
        
        void RetrieveBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> index, Action<BLECommandSetDataModel> onSuccess, OnErr onError);

        void SaveBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> idx, BLECommandSetDataModel data, Action onSuccess, OnErr onError);

        void DeleteBLECmdSet(IIndexItem<BLECmdIndexExtraInfo> index, Action<bool> onComplete, OnErr onError);

        void DeleteAllBLECmds(Action onSuccess, OnErr onError);


        void ValidateBLECmdItem(BLE_DataType dataType, ScriptItem item, Action onSuccess, OnErr onError);

        #endregion



        #region WIFI

        event EventHandler<List<WifiNetworkInfo>> DiscoveredWifiNetworks;
        event EventHandler<WifiError> OnWifiError;
        event EventHandler<MsgPumpResults> OnWifiConnectionAttemptCompleted;
        event EventHandler<string> Wifi_BytesReceived;
        /// <summary>Raised if there is no password, host name or service name in the connection data model</summary>
        event EventHandler<WifiCredentials> CredentialsRequestedEvent;

        void WifiDiscoverAsync();

        WifiCredAndIndex ValidateCredentials(WifiNetworkInfo discoverData, OnErr onError);

        void WifiConnectAsync(WifiNetworkInfo dataModel);
        
        void WifiConnectPreValidatedAsync(WifiNetworkInfo dataModel);

        void WifiDisconect();

        void WifiSend(string msg);

        List<KeyValuePropertyDisplay> Wifi_GetDeviceInfoForDisplay(WifiNetworkInfo info);

        #endregion

        #region Serial USB

        event EventHandler<List<SerialDeviceInfo>> SerialDiscoveredDevices;
        event EventHandler<SerialUsbError> SerialOnError;
        event EventHandler<string> Serial_BytesReceived;
        event EventHandler<MsgPumpResults> OnSerialConnectionAttemptCompleted;
        /// <summary>Event invoked on connection to retrieve the configurable fields into the info object</summary>
        event EventHandler<SerialDeviceInfo> OnSerialConfigRequest;

        void SerialUsbDiscoverAsync();
        void SerialUsbConnect(SerialDeviceInfo dataModel, OnErr onError);
        void SerialUsbDisconnect();
        void SerialUsbSend(string msg);

        /// <summary>Fix for old entries that do not have the name initialized</summary>
        void BackCompatibilityInitializeExistingTerminatorNames();

        List<KeyValuePropertyDisplay> Serial_GetDeviceInfoForDisplay(SerialDeviceInfo info);

        void Serial_GetStopBitsForDisplay(SerialDeviceInfo info, Action<List<StopBitDisplayClass>, int> onSuccess);
        void Serial_GetParitysForDisplay(SerialDeviceInfo info, Action<List<SerialParityDisplayClass>, int> onSuccess);
        void Serial_GetBaudsForDisplay(SerialDeviceInfo info, Action<List<uint>, int> onSuccess);
        void Serial_GetDataBitsForDisplay(SerialDeviceInfo info, Action<List<ushort>,int> onSuccess);
        void Serial_FlowControlForDisplay(SerialDeviceInfo info, Action<List<FlowControlDisplay>, int> onSuccess);

        #region Serial USB storage

        void GetSerialCfgList(Action<List<IIndexItem<SerialIndexExtraInfo>>> onSuccess, OnErr onError);

        void CreateNewSerialCfg(string display, SerialDeviceInfo data, Action onSuccess, OnErr onError);

        /// <summary>Create or save updated configuration data for a serial connection</summary>
        /// <param name="display">Item to display in index</param>
        /// <param name="data">The object to store</param>
        /// <param name="onSuccess">Invoked on success</param>
        /// <param name="onError">Invoked on error</param>
        void CreateOrSaveSerialCfg(string display, SerialDeviceInfo data, Action onSuccess, OnErr onError);

        void RetrieveSerialCfg(IIndexItem<SerialIndexExtraInfo> index, Action<SerialDeviceInfo> onSuccess, OnErr onError);

        void SaveSerialCfg(IIndexItem<SerialIndexExtraInfo> idx, SerialDeviceInfo data, Action onSuccess, OnErr onError);


        void DeleteSerialCfg(IIndexItem<SerialIndexExtraInfo> index, Action<bool> onComplete, OnErr onError);

        void DeleteSerialCfg(SerialDeviceInfo device, Func<string, bool> areYouSure, Action<bool> onComplete, OnErr onError);

        /// <summary>Initialised device info object with configurable fields from storage or user</summary>
        /// <param name="info">The info object to initialise</param>
        /// <param name="onSuccess">invoked on successful initialisation</param>
        /// <param name="onError">Invoked on error condition</param>
        void InitSerialDeviceInfoConfigFields(SerialDeviceInfo info, Action onSuccess, OnErr onError);
        
        #endregion

        #endregion

        #region Wifi credentials storage

        void GetWifiCredList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError);

        void CreateNewWifiCred(string display, WifiCredentialsDataModel data, Action onSuccess, OnErr onError);

        void CreateNewWifiCred(string display, WifiCredentialsDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError);

        void RetrieveWifiCredData(IIndexItem<DefaultFileExtraInfo> index, Action<WifiCredentialsDataModel> onSuccess, OnErr onError);

        void SaveWifiCred(IIndexItem<DefaultFileExtraInfo> idx, WifiCredentialsDataModel data, Action onSuccess, OnErr onError);

        void DeleteWifiCredData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError);

        #endregion

        #region Ethernet

        event EventHandler<string> Ethernet_BytesReceived;

        /// <summary>Raised so that host name and service name can be requested</summary>
        event EventHandler<EthernetParams> EthernetParamsRequestedEvent;

        /// <summary>Async Connection completed</summary>
        event EventHandler<MsgPumpResults> OnEthernetConnectionAttemptCompleted;

        /// <summary>For various errors encountered in asynchronous operations</summary>
        event EventHandler<MsgPumpResults> OnEthernetError;

        /// <summary>Raised when items added, removed or changed</summary>
        event EventHandler<List<IIndexItem<DefaultFileExtraInfo>>> OnEthernetListChange;

        void EthernetConnectAsync(EthernetParams dataModel);
        void EthernetDisconnect();
        void EthernetSend(string msg);

        void GetEthernetDataList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError);

        void GetEthernetDataList(Action<List<EthernetDisplayDataModel>> onSuccess, OnErr onError);

        void CreateNewEthernetData(string display, EthernetParams data, Action onSuccess, OnErr onError);

        void CreateNewEthernetData(EthernetParams data, Action onSuccess, OnErr onError);

        void RetrieveEthernetData(IIndexItem<DefaultFileExtraInfo> index, Action<EthernetParams> onSuccess, OnErr onError);

        void SaveEthernetData(IIndexItem<DefaultFileExtraInfo> idx, EthernetParams data, Action onSuccess, OnErr onError);

        void DeleteEthernetData(IIndexItem<DefaultFileExtraInfo> index, string name, Func<string, bool> areYouSure, Action<bool> onComplete, OnErr onError);

        void DeleteEthernetData(object index, Action<bool> onComplete, OnErr onError);

        #endregion

        ScratchSet GetScratch();

        void Teardown();

        void DisconnectAll();

        string GetDataFilesPath();

        void RebuildAllData();
    }
}
