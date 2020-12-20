using LogUtils.Net;
using MultiCommTerminal.NetCore.WindowObjs;
using MultiCommTerminal.NetCore.WindowObjs.BLE;
using MultiCommTerminal.NetCore.WindowObjs.BTWins;
using MultiCommTerminal.NetCore.WindowObjs.EthernetWins;
using MultiCommTerminal.NetCore.WindowObjs.SerialWins;
using System;
using System.Windows;

namespace MultiCommTerminal.NetCore.WPF_Helpers {

    /// <summary>Manages singleton instances of different Run Pages</summary>
    /// <remarks>
    /// The run pages are closed when a request is received from the page or 
    /// if all are closed at the end
    /// </remarks>
    public class RunPageManager {

        #region Data

        private Window parent = null;
        private BTRun btPage = null;
        private WifiRun wifiPage = null;
        private SerialRun serialPage = null;
        private EthernetRun ethernetPage = null;
        private BLERun blePage = null;

        #endregion

        #region Constructors

        private RunPageManager() { }

        public RunPageManager(Window parent) {
            this.parent = parent;
        }

        #endregion

        #region Public

        /// <summary>Creates a new Run page instance if none exists</summary>
        /// <param name="_type">The type of run page to open</param>
        public void Open(Type _type) {
            lock (this) {
                if (_type == typeof(BTRun)) {
                    // make sure only one Bluetooth/BLE opened at the same time
                    this.cleanUpPage(this.blePage, typeof(BLERun));
                    this.CreateBTPage();
                }
                else if (_type == typeof(WifiRun)) {
                    this.CreateWifiPage();
                }
                else if (_type == typeof(SerialRun)) {
                    this.CreateSerialPage();
                }
                else if (_type == typeof(EthernetRun)) {
                    this.CreateEthernetPage();
                }
                else if (_type == typeof(BLERun)) {
                    // make sure only one Bluetooth/BLE opened at the same time
                    this.cleanUpPage(this.btPage, typeof(BTRun));
                    this.CreateBLEPage();
                }
            }
        }


        /// <summary>Close any open run pages. Must be called from Window thread</summary>
        public void CloseAll() {
            lock (this) {
                try {
                    this.btCloseRequest(this.btPage, typeof(BTRun));
                    this.bleCloseRequest(this.blePage, typeof(BLERun));
                    this.serialCloseRequest(this.serialPage, typeof(SerialRun));
                    this.ethernetCloseRequest(this.ethernetPage, typeof(EthernetRun));
                    this.wifiCloseRequest(this.wifiPage, typeof(WifiRun));
                }
                catch(Exception e) {
                    Log.Exception(9999, "RunPageManager", "CloseAll", "", e);
                }
            }
        }

        #endregion

        #region Create methods

        private void CreateBTPage() {
            if (BTRun.Instances == 0) {
                this.cleanUpPage(this.btPage, typeof(BTRun));
                this.btPage = new BTRun(this.parent);
                this.btPage.CloseRequest += this.btCloseRequest;
                this.btPage.Show();
            }
        }


        private void CreateBLEPage() {
            if (BLERun.Instances == 0) {
                this.cleanUpPage(this.blePage, typeof(BLERun));
                this.blePage = new BLERun(this.parent);
                this.blePage.CloseRequest += this.bleCloseRequest;
                this.blePage.Show();
            }
        }


        private void CreateSerialPage() {
            if (SerialRun.Instances == 0) {
                this.cleanUpPage(this.serialPage, typeof(SerialRun));
                this.serialPage = new SerialRun(this.parent);
                this.serialPage.CloseRequest += this.serialCloseRequest;
                this.serialPage.Show();
            }
        }


        private void CreateWifiPage() {
            if (WifiRun.Instances == 0) {
                this.cleanUpPage(this.wifiPage, typeof(WifiRun));
                this.wifiPage = new WifiRun(this.parent);
                this.wifiPage.CloseRequest += this.wifiCloseRequest;
                this.wifiPage.Show();
            }
        }


        private void CreateEthernetPage() {
            if (EthernetRun.Instances == 0) {
                this.cleanUpPage(this.ethernetPage, typeof(EthernetRun));
                this.ethernetPage = new EthernetRun(this.parent);
                this.ethernetPage.CloseRequest += this.ethernetCloseRequest;
                this.ethernetPage.Show();
            }
        }


        #endregion

        #region Close methods

        private void btCloseRequest(object sender, Type _type) {
            this.cleanUpPage(sender, _type);
        }


        private void bleCloseRequest(object sender, Type _type) {
            this.cleanUpPage(sender, _type);
        }

        private void serialCloseRequest(object sender, Type _type) {
            this.cleanUpPage(sender, _type);
        }


        private void ethernetCloseRequest(object sender, Type _type) {
            this.cleanUpPage(sender, _type);
        }


        private void wifiCloseRequest(object sender, Type _type) {
            this.cleanUpPage(sender, _type);
        }


        private void cleanUpPage(object sender, Type _type) {
            lock (this) {
                if (sender != null) {
                    ((Window)sender).Close();

                    if (_type == typeof(BTRun)) {
                        this.btPage.CloseRequest -= this.btCloseRequest;
                        this.btPage = null;
                    }
                    else if (_type == typeof(WifiRun)) {
                        this.wifiPage.CloseRequest -= this.wifiCloseRequest;
                        this.wifiPage = null;
                    }
                    else if (_type == typeof(SerialRun)) {
                        this.serialPage.CloseRequest -= this.serialCloseRequest;
                        this.serialPage = null;
                    }
                    else if (_type == typeof(EthernetRun)) {
                        this.ethernetPage.CloseRequest -= this.ethernetCloseRequest;
                        this.ethernetPage = null;
                    }
                    else if (_type == typeof(BLERun)) {
                        this.blePage.CloseRequest -= this.bleCloseRequest;
                        this.blePage = null;
                    }
                }
            }
        }

        #endregion

    }
}
