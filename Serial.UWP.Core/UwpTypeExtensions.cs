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


        public static SerialStopBitCount Convert(this SerialStopBits sb) {
            switch (sb) {
                case SerialStopBits.One: return SerialStopBitCount.One;
                case SerialStopBits.OnePointFive: return SerialStopBitCount.OnePointFive;
                case SerialStopBits.Two: return SerialStopBitCount.Two;
                default: return SerialStopBitCount.One;
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


        public static SerialParity Convert(this SerialParityType sp) {
            switch (sp) {
                case SerialParityType.None: return SerialParity.None;
                case SerialParityType.Odd: return SerialParity.Odd;
                case SerialParityType.Even: return SerialParity.Even;
                case SerialParityType.Mark: return SerialParity.Mark;
                case SerialParityType.Space: return SerialParity.Space;
                default: return SerialParity.None;
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


        public static SerialHandshake Convert(this SerialFlowControlHandshake h) {
            switch (h) {
                case SerialFlowControlHandshake.None: return SerialHandshake.None;
                case SerialFlowControlHandshake.RequestToSend: return SerialHandshake.RequestToSend;
                case SerialFlowControlHandshake.XonXoff: return SerialHandshake.XOnXOff;
                case SerialFlowControlHandshake.RequestToSendXonXoff: return SerialHandshake.RequestToSendXOnXOff;
                default: return SerialHandshake.None;   
            }
        }

    }
}
