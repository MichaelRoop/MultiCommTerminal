using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net.interfaces;
using CommunicationStack.Net.interfaces;
using DependencyInjectorFactory.Net.interfaces;
using Ethernet.Common.Net.DataModels;
using Ethernet.Common.Net.interfaces;
using IconFactory.Net.interfaces;
using LanguageFactory.Net.interfaces;
using LogUtils.Net;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.Helpers;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.interfaces;
using SerialCommon.Net.StorageIndexExtraInfo;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System.Threading.Tasks;
using WifiCommon.Net.interfaces;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {

        #region Data

        // Container created on demand
        private IStorageManagerFactory _storageFactory = null;
        private ILangFactory _languages = null;
        private IIconFactory _iconFactory = null;
        private IBTInterface _classicBluetooth = null;
        private IBLETInterface _bleBluetooth = null;
        private IWifiInterface _wifi = null;
        private ISerialInterface _serial = null;
        private IEthernetInterface _ethernet = null;
        private ICommStackLevel0 _btClassicStack = null;
        private ICommStackLevel0 _bleStack = null;
        private ICommStackLevel0 _wifiStack = null;
        private ICommStackLevel0 _serialStack = null;
        private ICommStackLevel0 _ethernetStack = null;
        private IObjContainer container = null;
        // Other members
        private IStorageManager<SettingItems> settings = null;
        private IIndexedStorageManager<TerminatorDataModel, DefaultFileExtraInfo> terminatorStorage = null;
        private IIndexedStorageManager<ScriptDataModel, DefaultFileExtraInfo> scriptStorage = null;
        private IIndexedStorageManager<WifiCredentialsDataModel, DefaultFileExtraInfo> wifiCredStorage = null;
        private IIndexedStorageManager<EthernetParams, DefaultFileExtraInfo> ethernetStorage = null;
        private IIndexedStorageManager<SerialDeviceInfo, SerialIndexExtraInfo> serialStorage = null;
        private ScratchSet scratch = new ScratchSet();
        private ClassLog log = new ClassLog("CommWrapper");

        #endregion

        #region Properties

        private IStorageManagerFactory storageFactory {
            get {
                if (this._storageFactory == null) {
                    this._storageFactory = this.container.GetObjSingleton<IStorageManagerFactory>();
                }
                return this._storageFactory;
            }
        }

        private ILangFactory languages { 
            get {
                if (this._languages == null) {
                    this._languages = this.container.GetObjSingleton<ILangFactory>();
                }
                return this._languages;
            } 
        }

        private IIconFactory iconFactory {
            get {
                if (this._iconFactory == null) {
                    this._iconFactory = this.container.GetObjSingleton<IIconFactory>();
                }
                return this._iconFactory;
            }
        }

        private IBTInterface classicBluetooth {
            get {
                if (this._classicBluetooth == null) {
                    this._classicBluetooth = this.container.GetObjSingleton<IBTInterface>();
                }
                return this._classicBluetooth;
            }
        }

        private IBLETInterface bleBluetooth {
            get {
                if (this._bleBluetooth == null) {
                    this._bleBluetooth = this.container.GetObjSingleton<IBLETInterface>();
                }
                return this._bleBluetooth;
            }
        }

        private IWifiInterface wifi {
            get {
                if (this._wifi == null) {
                    this._wifi = this.container.GetObjSingleton<IWifiInterface>();
                }
                return this._wifi;
            }
        }

        private ISerialInterface serial {
            get {
                if (this._serial == null) {
                    this._serial = this.container.GetObjSingleton<ISerialInterface>();
                }
                return this._serial;
            }
        }

        private IEthernetInterface ethernet {
            get {
                if (this._ethernet == null) {
                    this._ethernet = this.container.GetObjSingleton<IEthernetInterface>();
                }
                return this._ethernet;
            }
        }

        private ICommStackLevel0 btClassicStack {
            get {
                if (this._btClassicStack == null) {
                    this._btClassicStack = this.container.GetObjInstance<ICommStackLevel0>();
                }
                return this._btClassicStack;
            }
        }

        private ICommStackLevel0 bleStack {
            get {
                if (this._bleStack == null) {
                    this._bleStack = this.container.GetObjInstance<ICommStackLevel0>();
                }
                return this._bleStack;
            }
        }

        private ICommStackLevel0 wifiStack {
            get {
                if (this._wifiStack == null) {
                    this._wifiStack = this.container.GetObjInstance<ICommStackLevel0>();
                }
                return this._wifiStack;
            }
        }

        private ICommStackLevel0 serialStack {
            get {
                if (this._serialStack == null) {
                    this._serialStack = this.container.GetObjInstance<ICommStackLevel0>();
                }
                return this._serialStack;
            }
        }

        private ICommStackLevel0 ethernetStack {
            get {
                if (this._ethernetStack == null) {
                    this._ethernetStack = this.container.GetObjInstance<ICommStackLevel0>();
                }
                return this._ethernetStack;
            }
        }

        #endregion

        #region Constructors

        /// <summary>Code logic and error handling here rather than UI</summary>
        /// <remarks>This makes less work when adding extra OS UIs</remarks>
        /// <param name="container">The Dependency injection container</param>
        public CommWrapper(IObjContainer container) {
            this.container = container;
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
            this.InitSettings();
            Task.Run(() => {
                this.InitStorage();
                this.InitBluetoothClassic();
                this.BLE_Init();
                this.WifiInit();
                this.SerialInit();
                this.EthernetInit();
            });
        }


        public void DisconnectAll() {
            this.BLE_Disconnect();
            this.BTClassicDisconnect();
            this.WifiDisconect();
            this.SerialUsbDisconnect();
            this.EthernetDisconnect();
        }

        #endregion


        public ScratchSet GetScratch() {
            return this.scratch;
        }

    }
}
