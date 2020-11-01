using LogUtils.Net;
using SerialCommon.Net.DataModels;
using SerialCommon.Net.interfaces;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Serial.UWP.Core {

    public partial class SerialImplUwp : ISerialInterface {

        public event EventHandler<List<SerialDeviceInfo>> DiscoveredDevices;

        public event EventHandler<SerialUsbError> OnError;


        #region ICommStack Implementations

        /// <summary>
        /// 
        /// </summary>
        public event EventHandler<byte[]> MsgReceivedEvent;


        public bool SendOutMsg(byte[] msg) {
            throw new NotImplementedException();
        }

        #endregion



        ClassLog log = new ClassLog("SerialImplUwp");


        public bool Connected => throw new NotImplementedException();


        public void DiscoverSerialDevicesAsync() {
            try {
                Task.Run(this.DoDiscovery);
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
            }
        }


        public void ConnectAsync(SerialDeviceInfo dataModel) {
            //throw new NotImplementedException();
        }

        public void Disconnect() {
            //throw new NotImplementedException();
        }

    }
}
