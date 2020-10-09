#define USE_BT_WRAPPER

using BluetoothCommon.Net;
using BluetoothCommon.Net.interfaces;
using LogUtils.Net;
using System;
using System.Collections.Generic;
using System.Threading;
using Windows.Networking.Sockets;
using Windows.Storage.Streams;

namespace BluetoothRfComm.UWP.Core {


    // STEPS TO USE UWP libs in Win32
    // Windows.Foundation.UniversalApiContract
    // C:\Program Files(x86)\Windows Kits\10\References\10.0.18362.0\Windows.Foundation.UniversalApiContract\8.0.0.0\Windows.Foundation.UniversalApiContract.winmd

    //Windows.Foundation.FoundationContract
    //C:\Program Files (x86)\Windows Kits\10\References\10.0.18362.0\Windows.Foundation.FoundationContract\3.0.0.0\Windows.Foundation.FoundationContract.winmd

    //Windows
    //C:\Program Files (x86)\Windows Kits\10\UnionMetadata\10.0.18362.0\Facade\windows.winmd

    //System.Runtime.WindowsRuntime
    //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETCore\v4.5\System.Runtime.WindowsRuntime.dll

    //System.Runtime.InteropServices.WindowsRuntime
    //C:\Program Files (x86)\Reference Assemblies\Microsoft\Framework\.NETFramework\v4.7.2\Facades\System.Runtime.InteropServices.WindowsRuntime.dll

    // Add to manifest
    //https://stackoverflow.com/questions/38845320/uwp-serialdevice-fromidasync-throws-element-not-found-exception-from-hresult



    // UPDATE: It looks like you only have to add the package:
    // Microsoft.Windows.SDK.Contracts(nn.n.nnnn.n)
    // You do this through the NuGet package manager. Not sure if you need to add to manifest. If so, would only be in main App


    public partial class BluetoothRfCommUwpCore : IBTInterface {

        #region Data

        private ClassLog log = new ClassLog("BluetoothRfCommImpl");
        private readonly string KEY_CAN_PAIR = "System.Devices.Aep.CanPair";
        private readonly string KEY_IS_PAIRED = "System.Devices.Aep.IsPaired";
        private readonly string KEY_CONTAINER_ID = "System.Devices.Aep.ContainerId";

        private StreamSocket socket = null;
        private DataWriter writer = null;
        private DataReader reader = null;
        private CancellationTokenSource readCancelationToken = null;
        private bool continueReading = false;
        private static uint READ_BUFF_MAX_SIZE = 256;
        private ManualResetEvent readFinishedEvent = new ManualResetEvent(false);

        #endregion

        #region Properties

        public bool Connected { get; private set; } = false;

        #endregion

        #region Events

        public event EventHandler<BTDeviceInfo> DiscoveredBTDevice;
        public event EventHandler<BTDeviceInfo> BT_DeviceInfoGathered;
        public event EventHandler<bool> DiscoveryComplete;
        public event EventHandler<bool> ConnectionCompleted;
        public event EventHandler<byte[]> MsgReceivedEvent;
        public event EventHandler<BT_PairInfoRequest> BT_PairInfoRequested;
        public event EventHandler<BTPairOperationStatus> BT_PairStatus;
        public event EventHandler<BTUnPairOperationStatus> BT_UnPairStatus;

        #endregion

        #region Constructors

        public BluetoothRfCommUwpCore() {
        }

        #endregion

        #region Private tools

        /// <summary>Get the boolean value from a Device Information property</summary>
        /// <param name="property">The device information property</param>
        /// <param name="key">The property key to lookup</param>
        /// <param name="defaultValue">Default value on error</param>
        /// <returns></returns>
        private bool GetBoolProperty(IReadOnlyDictionary<string, object> property, string key, bool defaultValue) {
            if (property.ContainsKey(key)) {
                if (property[key] is Boolean) {
                    return (bool)property[key];
                }
                this.log.Error(9999, () => string.Format(
                    "{0} Property is {1} rather than Boolean", key, property[key].GetType().Name));
            }
            return defaultValue;
        }

        #endregion

    }
}
