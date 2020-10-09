//using BluetoothCommon.Net;
//using BluetoothCommon.Net.Enumerations;
//using System;
//using System.Threading.Tasks;
//using Windows.Devices.Bluetooth;
//using Windows.Devices.Enumeration;

//namespace BluetoothRfComm.UWP.Core {

//    /// <summary>Contains code relative to pairing or un pairing devices</summary>
//    public partial class BluetoothRfCommWrapperUwpCore : IDisposable {


//        public async Task DoPairing(BTDeviceInfo info) {

//            // Code in example does not work but has the calls. This one forces a prompt
//            // https://stackoverflow.com/questions/53010320/uwp-bluetooth-pairing-without-prompt


//            this.log.Info("DoPairing", () => string.Format("'{0}'", info.Name));


//            using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(info.Address)) {

//                DeviceInformationPairing pairing = device.DeviceInformation.Pairing;

//                BTPairOperationStatus data = new BTPairOperationStatus() {
//                    Name = info.Name,
//                };

//                // TODO find if encryption required etc
//                //DevicePairingProtectionLevel pl = p.ProtectionLevel;
//                //pl.

//                if (pairing == null) {
//                    data.PairStatus = BT_PairingStatus.NoParingObject;
//                    this.BT_PairStatus?.Invoke(this, data);
//                }
//                else if (!pairing.CanPair) {
//                    data.PairStatus = BT_PairingStatus.NotSupported;
//                    this.BT_PairStatus?.Invoke(this, data);
//                }
//                else {
//                    if (pairing.IsPaired) {
//                        data.IsSuccessful = true;
//                        data.PairStatus = BT_PairingStatus.AlreadyPaired;
//                        this.BT_PairStatus?.Invoke(this, data);
//                    }
//                    else {
//                        DeviceInformationCustomPairing cpi = pairing.Custom;
//                        cpi.PairingRequested += this.OnPairRequestedAsyncCallback;
//                        // TODO - need to figure out if requesting PIN or just confirm
//                        DevicePairingResult result = await cpi.PairAsync(DevicePairingKinds.ProvidePin);
//                        cpi.PairingRequested -= this.OnPairRequestedAsyncCallback;
//                        this.log.Info("DoPairing", () =>
//                            string.Format("'{0}' Pair status {1}", info.Name, result.Status.ToString()));

//                        data.IsSuccessful = result.Status.IsSuccessful();
//                        data.PairStatus = result.Status.ConvertStatus();
//                        this.BT_PairStatus?.Invoke(this, data);
//                    }
//                }
//            }
//        }


//        private void OnPairRequestedAsyncCallback(
//            DeviceInformationCustomPairing sender,
//            DevicePairingRequestedEventArgs args) {

//            this.log.Info("OnPairRequested", () => string.Format("Paring kind {0}", args.PairingKind.ToString()));

//            BT_PairInfoRequest pairInfo = new BT_PairInfoRequest();
//            pairInfo.DeviceName = args.DeviceInformation.Name;

//            switch (args.PairingKind) {
//                case DevicePairingKinds.ConfirmOnly:
//                    // Windows itself will pop the confirmation dialog as part of "consent" if this is running on Desktop or Mobile
//                    // If this is an App for 'Windows IoT Core' or a Desktop and Console application
//                    // where there is no Windows Consent UX, you may want to provide your own confirmation.
//                    args.Accept();
//                    break;

//                case DevicePairingKinds.ProvidePin:
//                    // A PIN may be shown on the target device and the user needs to enter the matching PIN on 
//                    // this Windows device. Get a deferral so we can perform the async request to the user.
//                    using (Windows.Foundation.Deferral collectPinDeferral = args.GetDeferral()) {
//                        pairInfo.PinRequested = true;
//                        if (this.BT_PairInfoRequested != null) {
//                            this.BT_PairInfoRequested(this, pairInfo);
//                        }
//                        else {
//                            this.log.Error(9999, "No subscriber to pin request");
//                        }

//                        this.log.Info("OnPairRequested",
//                            () => string.Format("Pin '{0}' Response:{1}",
//                            pairInfo.Pin, pairInfo.Response));

//                        if (pairInfo.Response) {
//                            // TODO Check for the result to see if you go ahead
//                        }

//                        if (!string.IsNullOrEmpty(pairInfo.Pin)) {
//                            args.Accept(pairInfo.Pin);
//                        }
//                        // TODO - needs to be disposed
//                        collectPinDeferral.Complete();
//                    };
//                    break;
//            }
//        }


//        //https://stackoverflow.com/questions/45191412/deviceinformation-pairasync-not-working-in-wpf
//        public async Task DoUnPairing(BTDeviceInfo info) {
//            using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(info.Address)) {
//                this.log.Info("DoUnPairing", () => string.Format("'{0}'", info.Name));
//                DeviceUnpairingResult result = await device.DeviceInformation.Pairing.UnpairAsync();
//                this.log.Info("DoUnPairing", () => 
//                    string.Format("'{0}' Unpair status {1}",  info.Name, result.Status.ToString()));

//                this.BT_UnPairStatus?.Invoke(this, new BTUnPairOperationStatus() {
//                    Name = info.Name,
//                    UnpairStatus = result.Status.ConvertStatus(),
//                    IsSuccessful = result.Status.IsSuccessful(),
//                });
//            }
//        }
//    }


//    public static class PairStatusExtensions {

//        public static BT_PairingStatus ConvertStatus(this DevicePairingResultStatus result) {
//            switch (result) {
//                case DevicePairingResultStatus.Paired:
//                    return BT_PairingStatus.Paired;
//                case DevicePairingResultStatus.NotReadyToPair:
//                    return BT_PairingStatus.NotReadyToPair;
//                case DevicePairingResultStatus.NotPaired:
//                    return BT_PairingStatus.NotPaired;
//                case DevicePairingResultStatus.AlreadyPaired:
//                    return BT_PairingStatus.AlreadyPaired;
//                case DevicePairingResultStatus.ConnectionRejected:
//                    return BT_PairingStatus.Rejected;
//                case DevicePairingResultStatus.TooManyConnections:
//                    return BT_PairingStatus.TooManyConnections;
//                case DevicePairingResultStatus.HardwareFailure:
//                    return BT_PairingStatus.HardwareFailure;
//                case DevicePairingResultStatus.AuthenticationTimeout:
//                    return BT_PairingStatus.AuthenticationTimeout;
//                case DevicePairingResultStatus.AuthenticationNotAllowed:
//                    return BT_PairingStatus.AuthenticationNotAllowed;
//                case DevicePairingResultStatus.AuthenticationFailure:
//                    return BT_PairingStatus.AuthenticationFailure;
//                case DevicePairingResultStatus.NoSupportedProfiles:
//                    return BT_PairingStatus.NoSupportedProfiles;
//                case DevicePairingResultStatus.ProtectionLevelCouldNotBeMet:
//                    return BT_PairingStatus.ProtectionLevelCouldNotBeMet;
//                case DevicePairingResultStatus.AccessDenied:
//                    return BT_PairingStatus.AccessDenied;
//                case DevicePairingResultStatus.InvalidCeremonyData:
//                    return BT_PairingStatus.InvalidCeremonyData;
//                case DevicePairingResultStatus.PairingCanceled:
//                    return BT_PairingStatus.PairingCanceled;
//                case DevicePairingResultStatus.OperationAlreadyInProgress:
//                    return BT_PairingStatus.OperationAlreadyInProgress;
//                case DevicePairingResultStatus.RequiredHandlerNotRegistered:
//                    return BT_PairingStatus.RequiredHandlerNotRegistered;
//                case DevicePairingResultStatus.RejectedByHandler:
//                    return BT_PairingStatus.RejectedByHandler;
//                case DevicePairingResultStatus.RemoteDeviceHasAssociation:
//                    return BT_PairingStatus.RemoteDeviceHasAssociation;
//                case DevicePairingResultStatus.Failed:
//                    return BT_PairingStatus.Failed;
//                default:
//                    LogUtils.Net.Log.Error(9999, () => string.Format("Unknown Pair failure:{0}", result));
//                    return BT_PairingStatus.Failed;
//            }
//        }


//        public static bool IsSuccessful(this DevicePairingResultStatus result) {
//            switch (result) {
//                case DevicePairingResultStatus.Paired:
//                case DevicePairingResultStatus.AlreadyPaired:
//                    return true;
//                default:
//                    return false;
//            }
//        }


//        public static BT_UnpairingStatus ConvertStatus(this DeviceUnpairingResultStatus result) {
//            switch (result) {
//                case DeviceUnpairingResultStatus.Unpaired:
//                    return BT_UnpairingStatus.Success;
//                case DeviceUnpairingResultStatus.AlreadyUnpaired:
//                    return BT_UnpairingStatus.AlreadyUnPaired;
//                case DeviceUnpairingResultStatus.OperationAlreadyInProgress:
//                    return BT_UnpairingStatus.AlreadyInProgress;
//                case DeviceUnpairingResultStatus.AccessDenied:
//                    return BT_UnpairingStatus.AccessDenied;
//                case DeviceUnpairingResultStatus.Failed:
//                    return BT_UnpairingStatus.Failed;
//                default:
//                    LogUtils.Net.Log.Error(9999, () => string.Format("Unknown unpair failure:{0}", result));
//                    return BT_UnpairingStatus.Failed;
//            }
//        }


//        public static bool IsSuccessful(this DeviceUnpairingResultStatus result) {
//            switch (result) {
//                case DeviceUnpairingResultStatus.Unpaired:
//                case DeviceUnpairingResultStatus.AlreadyUnpaired:
//                    return true;
//                default:
//                    return false;
//            }
//        }

//    }
//}
