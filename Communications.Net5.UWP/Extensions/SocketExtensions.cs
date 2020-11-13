using Common.Net.Network.Enumerations;
using System;
using Windows.Networking.Sockets;

namespace Communications.UWP.Core.Extensions {

    public static class SocketExtensions {

        public static SocketErrCode Convert(this SocketErrorStatus code) {
            // We know that mapping is 100%
            return (SocketErrCode)((int)(code));
        }


        public static SocketErrCode GetSocketCode(this Exception e) {
            var baseEx = e.GetBaseException();
            if (baseEx != null) {
                return SocketError.GetStatus(baseEx.HResult).Convert();
            }
            return SocketErrCode.Unknown;
        }


    }
}
