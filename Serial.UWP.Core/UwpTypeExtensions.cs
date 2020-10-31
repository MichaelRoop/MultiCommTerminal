using SerialCommon.Net.Enumerations;
using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.SerialCommunication;

namespace Serial.UWP.Core {

    public static class UwpTypeExtensions {


        public static SerialStopBits Convert(this SerialStopBitCount sb){
            switch (sb) {
                case SerialStopBitCount.One: return SerialStopBits.One;
                case SerialStopBitCount.OnePointFive: return SerialStopBits.OnePointFive;
                case SerialStopBitCount.Two: return SerialStopBits.Two;
                default: return SerialStopBits.One;
            }
        }


        public static SerialParityType Convert(this SerialParity sp) {
            switch (sp) {
                case SerialParity.None: return SerialParityType.None;
                case SerialParity.Odd: return SerialParityType.Odd;
                case SerialParity.Even: return SerialParityType.Even;
                case SerialParity.Mark: return SerialParityType.Mark;
                case SerialParity.Space: return SerialParityType.Space;
                default: return SerialParityType.None;
            }
        }


        public static SerialFlowControlHandshake Convert(this SerialHandshake handshake) {
            switch (handshake) {
                case SerialHandshake.None: return SerialFlowControlHandshake.None;
                case SerialHandshake.RequestToSend: return SerialFlowControlHandshake.RequestToSend;
                case SerialHandshake.XOnXOff: return SerialFlowControlHandshake.XonXoff;
                case SerialHandshake.RequestToSendXOnXOff: return SerialFlowControlHandshake.RequestToSendXonXoff;
                default: return SerialFlowControlHandshake.None;
            }
        }


    }
}
