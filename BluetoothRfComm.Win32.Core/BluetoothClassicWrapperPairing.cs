using BluetoothCommon.Net;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Windows.Devices.Bluetooth;
using Windows.Devices.Enumeration;

namespace BluetoothRfComm.UWP.Core {

    /// <summary>Contains code relative to pairing or un pairing devices</summary>
    public partial class BluetoothRfCommWrapperUwpCore : IDisposable {


        private async Task DoPairing(BTDeviceInfo info) {

            // Code in example does not work but has the calls. This one forces a prompt
            // https://stackoverflow.com/questions/53010320/uwp-bluetooth-pairing-without-prompt


            this.log.Info("DoPairing", () => string.Format("'{0}'", info.Name));


            using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(info.Address)) {

                DeviceInformationPairing pairing = device.DeviceInformation.Pairing;
                // TODO find if encryption required etc
                //DevicePairingProtectionLevel pl = p.ProtectionLevel;
                //pl.

                if (pairing == null) {
                    // TODO raise a failed event. Done. not supported
                }
                else if (!pairing.CanPair) {
                    // TODO raise a failed event. Done. not supported
                }
                else {
                    if (pairing.IsPaired) {
                        // TODO Done. Already paired. Raise event
                    }
                    else {
                        DeviceInformationCustomPairing cpi = pairing.Custom;
                        cpi.PairingRequested += this.OnPairRequested;
                        DevicePairingResult result = await cpi.PairAsync(DevicePairingKinds.ProvidePin);
                        cpi.PairingRequested -= this.OnPairRequested;

                        this.log.Info("DoPairing", () =>
                            string.Format("'{0}' Pair status {1}", info.Name, result.Status.ToString()));
                    }
                }


                /*
                // pair without any info. This is supposed to bring up the UWP dialogs but does not. So use custom
                DevicePairingResult result = await device.DeviceInformation.Pairing.PairAsync();
                //result.Status == DevicePairingResultStatus.Paired
                this.log.Info("DoPairing", () =>
                    string.Format("'{0}' Pair status {1}", info.Name, result.Status.ToString()));
                */
            }

        }


        private void OnPairRequested(
            DeviceInformationCustomPairing sender,
            DevicePairingRequestedEventArgs args) {

            this.log.Info("OnPairRequested", () => string.Format("Paring kind {0}", args.PairingKind.ToString()));

            BT_PairInfoRequest pairInfo = new BT_PairInfoRequest();
            pairInfo.DeviceName = args.DeviceInformation.Name;

            switch (args.PairingKind) {
                case DevicePairingKinds.ConfirmOnly:
                    // Windows itself will pop the confirmation dialog as part of "consent" if this is running on Desktop or Mobile
                    // If this is an App for 'Windows IoT Core' or a Desktop and Console application
                    // where there is no Windows Consent UX, you may want to provide your own confirmation.
                    args.Accept();
                    break;

                case DevicePairingKinds.ProvidePin:
                    // A PIN may be shown on the target device and the user needs to enter the matching PIN on 
                    // this Windows device. Get a deferral so we can perform the async request to the user.
                    Windows.Foundation.Deferral collectPinDeferral = args.GetDeferral();

                    //string pinFromUser = "1234";
                    pairInfo.PinRequested = true;
                    if (this.BT_PairInfoRequested != null) {
                        this.BT_PairInfoRequested(this, pairInfo);
                    }
                    else {
                        this.log.Error(9999, "No subscriber to pin request");
                    }

                    this.log.Info("OnPairRequested", 
                        () => string.Format("Pin '{0}' Response:{1}",
                        pairInfo.Pin, pairInfo.Response));

                    //pinFromUser = pairInfo.Pin;
                    if (pairInfo.Response) {
                        // TODO Check for the result to see if you go ahead
                    }

                    if (!string.IsNullOrEmpty(pairInfo.Pin)) {
                        args.Accept(pairInfo.Pin);
                    }
                    // TODO - needs to be disposed
                    collectPinDeferral.Complete();
                    break;
            }
        }


        //https://stackoverflow.com/questions/45191412/deviceinformation-pairasync-not-working-in-wpf
        private async Task DoUnPairing(BTDeviceInfo info) {

            // call with following. Works perfectly
            //Task.Run(async () => {
            //    await this.DoUnPairing(deviceDataModel);
            //    this.ConnectionCompleted?.Invoke(this, true); // Raise whatever event
            //});
            //return;



            using (BluetoothDevice device = await BluetoothDevice.FromIdAsync(info.Address)) {
                this.log.Info("DoUnPairing", () => string.Format("'{0}'", info.Name));
                DeviceUnpairingResult result = await device.DeviceInformation.Pairing.UnpairAsync();
                this.log.Info("DoUnPairing", () => 
                    string.Format("'{0}' Unpair status {1}",  info.Name, result.Status.ToString()));

                // TODO You would need to raise an event to the user

            }


        }



    }
}
