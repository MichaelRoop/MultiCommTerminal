using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net.interfaces;
using CommunicationStack.Net.interfaces;
using Ethernet.Common.Net.interfaces;
using IconFactory.Net.interfaces;
using LanguageFactory.Net.interfaces;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.interfaces;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using WifiCommon.Net.interfaces;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {

        #region Data

        IStorageManagerFactory storageFactory = null;
        ILangFactory languages = null;
        IIconFactory iconFactory = null;
        IBTInterface classicBluetooth = null;
        IBLETInterface bleBluetooth = null;
        IWifiInterface wifi = null;
        ISerialInterface serial = null;
        IEthernetInterface ethernet = null;
        IStorageManager<SettingItems> settings = null;
        IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> terminatorStorage = null;
        IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> scriptStorage = null;
        IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> wifiCredStorage = null;
        IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> serialStorage = null;
        ICommStackLevel0 btClassicStack = null;
        ICommStackLevel0 bleStack = null;
        ICommStackLevel0 wifiStack = null;
        ICommStackLevel0 serialStack = null;
        ICommStackLevel0 ethernetStack = null;
        private ClassLog log = new ClassLog("CommWrapper");

        #endregion

        #region Constructors

        /// <summary>Code logic and error handling here rather than UI</summary>
        /// <remarks>This makes less work when adding extra OS UIs</remarks>
        /// <param name="storageFactory"></param>
        /// <param name="languages"></param>
        /// <param name="iconFactory"></param>
        /// <param name="classicBluetooth"></param>
        /// <param name="bleBluetooth"></param>
        public CommWrapper(
            IStorageManagerFactory storageFactory,
            ILangFactory languages,
            IIconFactory iconFactory,
            IBTInterface classicBluetooth,
            ICommStackLevel0 classicBluetoothStack,
            IBLETInterface bleBluetooth,
            ICommStackLevel0 bleStack,
            IWifiInterface wifi,
            ICommStackLevel0 wifiStack,
            ISerialInterface serial,
            ICommStackLevel0 serialStack,
            IEthernetInterface ethernet,
            ICommStackLevel0 ethernetStack) {

            this.storageFactory = storageFactory;
            this.languages = languages;
            this.iconFactory = iconFactory;
            this.classicBluetooth = classicBluetooth;
            this.btClassicStack = classicBluetoothStack;
            this.bleBluetooth = bleBluetooth;
            this.bleStack = bleStack;
            this.wifi = wifi;
            this.wifiStack = wifiStack;
            this.serial = serial;
            this.serialStack = serialStack;
            this.ethernet = ethernet;
            this.ethernetStack = ethernetStack;
            this.InitializeAll();
        }

        #endregion

        #region Init and teardown

        public void Teardown() {
            // TODO - shut down anything needed and dispose
            this.TeardownLanguages();
            this.TeardownBluetoothClassic();
            this.BLE_TearDown();
            this.WifiTeardown();
            this.SerialTeardown();
            this.EthernetTeardown();
        }


        /// <summary>Make sure there are default files for all storage managers</summary>
        private void InitializeAll() {
            this.InitLanguages();
            this.InitStorage();
            this.InitBluetoothClassic();
            this.BLE_Init();
            this.WifiInit();
            this.SerialInit();
            this.EthernetInit();
        }


        public void DisconnectAll() {
            this.BLE_Disconnect();
            this.BTClassicDisconnect();
            this.WifiDisconect();
            this.SerialUsbDisconnect();
            this.EthernetDisconnect();
        }

        #endregion

    }
}
