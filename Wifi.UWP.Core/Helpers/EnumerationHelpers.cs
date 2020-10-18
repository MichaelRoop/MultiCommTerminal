using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;
using WifiCommon.Net.Enumerations;
using Windows.Devices.WiFi;
using Windows.Networking.Connectivity;

namespace Wifi.UWP.Core.Helpers {

    public static class EnumerationHelpers {

        public static NetworkKind Convert(this WiFiNetworkKind kind) {
            return EnumHelpers.ToEnum<NetworkKind>(
                EnumHelpers.ToInt(kind));
        }


        public static WifiPhyPhysicalLayerKind Convert(this WiFiPhyKind phyKind) {
            return EnumHelpers.ToEnum<WifiPhyPhysicalLayerKind>(
                EnumHelpers.ToInt(phyKind));
        }


        public static NetAuthenticationType Convert (this NetworkAuthenticationType authenticationType) {
            return EnumHelpers.ToEnum<NetAuthenticationType>(
                EnumHelpers.ToInt(authenticationType));
        }


        public static NetEncryptionType Convert(this NetworkEncryptionType encryptionType) {
            return EnumHelpers.ToEnum<NetEncryptionType>(
                EnumHelpers.ToInt(encryptionType));
        }


        public static WifiErrorCode Convert (this WiFiConnectionStatus connectionStatus) {
            switch (connectionStatus) {
                case WiFiConnectionStatus.UnspecifiedFailure: return WifiErrorCode.Unknown;
                case WiFiConnectionStatus.Success: return WifiErrorCode.Success;
                case WiFiConnectionStatus.AccessRevoked: return WifiErrorCode.AccessRevoked;
                case WiFiConnectionStatus.InvalidCredential: return WifiErrorCode.InvalidCredentials;
                case WiFiConnectionStatus.NetworkNotAvailable: return WifiErrorCode.NetworkNotAvailable;
                case WiFiConnectionStatus.Timeout: return WifiErrorCode.Timeout;
                case WiFiConnectionStatus.UnsupportedAuthenticationProtocol: return WifiErrorCode.UnsupportedAuthenticationProtocol;
                default: return WifiErrorCode.Unknown;
            }
        }



    }
}
