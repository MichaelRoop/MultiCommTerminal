using BluetoothCommon.Net.interfaces;
using BluetoothLE.Net.interfaces;
using CommunicationStack.Net.interfaces;
using DependencyInjectorFactory.Net.interfaces;
using Ethernet.Common.Net.interfaces;
using IconFactory.Net.interfaces;
using LanguageFactory.Net.interfaces;
using LogUtils.Net;
using MultiCommWrapper.Net.Helpers;
using MultiCommWrapper.Net.interfaces;
using SerialCommon.Net.interfaces;
using System.Threading.Tasks;
using VariousUtils.Net;
using WifiCommon.Net.interfaces;

namespace MultiCommWrapper.Net.WrapCode {


    public partial class CommWrapper : ICommWrapper {

        #region Data

        // Container created on demand
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
        private ScratchSet scratch = new ScratchSet();
        private ClassLog log = new ClassLog("CommWrapper");

        #endregion

        #region Properties

        private ILangFactory languages { 
            get {
                if (this._languages == null) {
                    this._languages = this.container.GetObjSingleton<ILangFactory>();
                    this._languages.LanguageChanged += Event_LanguageChanged;
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

                    // Connect comm channel and its stack - uses Property to ensure statck creation
                    this.btClassicStack.SetCommChannel(this._classicBluetooth);
                    this.btClassicStack.InTerminators = "\n\r".ToAsciiByteArray();
                    this.btClassicStack.OutTerminators = "\n\r".ToAsciiByteArray();
                    this.btClassicStack.MsgReceived += this.BTClassic_BytesReceivedHandler;

                    this._classicBluetooth.DiscoveredBTDevice += this.BTClassic_DiscoveredDeviceHandler;
                    this._classicBluetooth.DiscoveryComplete += this.BTClassic_DiscoveryCompleteHandler;
                    this._classicBluetooth.BT_DeviceInfoGathered += this.BTClassic_DeviceInfoGathered;
                    this._classicBluetooth.ConnectionCompleted += this.BTClassic_ConnectionCompletedHander;
                    this._classicBluetooth.BT_PairInfoRequested += BTClassic_PairInfoRequested;
                    this._classicBluetooth.BT_PairStatus += this.BTClassic_PairStatus;
                    this._classicBluetooth.BT_UnPairStatus += this.BTClassic_UnPairStatus;
                }
                return this._classicBluetooth;
            }
        }

        private IBLETInterface bleBluetooth {
            get {
                if (this._bleBluetooth == null) {
                    this._bleBluetooth = this.container.GetObjSingleton<IBLETInterface>();

                    // Connect comm channel and its stack - uses Property to ensure statck creation
                    this.bleStack.SetCommChannel(this._bleBluetooth);
                    this.bleStack.InTerminators = "\n\r".ToAsciiByteArray();
                    this.bleStack.OutTerminators = "\n\r".ToAsciiByteArray();
                    this.bleStack.MsgReceived += this.BleStack_MsgReceived;

                    this._bleBluetooth.DeviceDiscovered += this.BLE_DeviceDiscoveredHandler;
                    this._bleBluetooth.DeviceRemoved += this.BLE_DeviceRemovedHandler;
                    this._bleBluetooth.DeviceUpdated += BLE_DeviceUpdatedHandler;
                    this._bleBluetooth.DeviceDiscoveryCompleted += this.BLE_DeviceDiscoveryCompleted;
                    this._bleBluetooth.DeviceInfoAssembled += this.BleBluetooth_DeviceInfoAssembled;
                }
                return this._bleBluetooth;
            }
        }

        private IWifiInterface wifi {
            get {
                if (this._wifi == null) {
                    this._wifi = this.container.GetObjSingleton<IWifiInterface>();

                    // Connect comm channel and its stack - uses Property to ensure statck creation
                    this.wifiStack.SetCommChannel(this._wifi);
                    this.wifiStack.InTerminators = "\n\r".ToAsciiByteArray();
                    this.wifiStack.OutTerminators = "\n\r".ToAsciiByteArray();
                    this.wifiStack.MsgReceived += this.WifiStack_BytesReceivedHander;

                    this._wifi.DiscoveredNetworks += this.Wifi_DiscoveredNetworksHandler;
                    this._wifi.OnError += this.Wifi_OnErrorHandler;
                    this._wifi.OnWifiConnectionAttemptCompleted += this.Wifi_OnWifiConnectionAttemptCompletedHandler;

                }
                return this._wifi;
            }
        }

        private ISerialInterface serial {
            get {
                if (this._serial == null) {
                    this._serial = this.container.GetObjSingleton<ISerialInterface>();

                    // Connect comm channel and its stack - uses Property to ensure statck creation
                    this.serialStack.SetCommChannel(this._serial);
                    this.serialStack.InTerminators = "\r\n".ToAsciiByteArray();
                    this.serialStack.OutTerminators = "\r\n".ToAsciiByteArray();
                    this.serialStack.MsgReceived += this.SerialStack_MsgReceivedHandler;
                    this._serial.OnSerialConnectionAttemptCompleted += this.SerialConnectCompleteHandler;
                    this._serial.DiscoveredDevices += this.Serial_DiscoveredDevicesHandler;
                    this._serial.OnError += this.Serial_OnErrorHandler;
                }
                return this._serial;
            }
        }


        private IEthernetInterface ethernet {
            get {
                if (this._ethernet == null) {
                    this._ethernet = this.container.GetObjSingleton<IEthernetInterface>();

                    // Connect comm channel and its stack - uses Property to ensure statck creation
                    this.ethernetStack.SetCommChannel(this._ethernet);
                    this.ethernetStack.InTerminators = "\r\n".ToAsciiByteArray();
                    this.ethernetStack.OutTerminators = "\r\n".ToAsciiByteArray();
                    this.ethernetStack.MsgReceived += this.EthernetStack_MsgReceivedHandler;

                    this._ethernet.ParamsRequestedEvent += Ethernet_ParamsRequestedEventHandler;
                    this._ethernet.OnError += this.Ethernet_OnErrorHandler;
                    this._ethernet.OnEthernetConnectionAttemptCompleted += this.Ethernet_OnEthernetConnectionAttemptCompletedHandler;
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
