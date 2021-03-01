using ChkUtils.Net;
using ChkUtils.Net.ErrObjects;
using CommunicationStack.Net.DataModels;
using CommunicationStack.Net.Enumerations;
using LanguageFactory.Net.data;
using MultiCommData.Net.Enumerations;
using MultiCommData.Net.StorageDataModels;
using MultiCommWrapper.Net.DataModels;
using MultiCommWrapper.Net.Helpers;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.StorageManagers;
using System;
using System.Collections.Generic;
using System.Text;
using VariousUtils.Net;
using WifiCommon.Net.DataModels;
using WifiCommon.Net.Enumerations;

namespace MultiCommWrapper.Net.WrapCode {

    public partial class CommWrapper : ICommWrapper {

        WifiNetworkInfo pendingSaveConnectNetInfo = null;


        #region ICommWrapper events

        public event EventHandler<List<WifiNetworkInfo>> DiscoveredWifiNetworks;
        public event EventHandler<WifiError> OnWifiError;
        public event EventHandler<MsgPumpResults> OnWifiConnectionAttemptCompleted;
        public event EventHandler<string> Wifi_BytesReceived;
        public event EventHandler<WifiCredentials> CredentialsRequestedEvent;

        #endregion

        #region ICommWrapper methods

        public void WifiDiscoverAsync() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000350, "Failure on WifiDiscoverAsync", () => {
                this.wifi.DiscoverWifiAdaptersAsync();
            });
            this.RaiseIfException(report);
        }


        public void WifiConnectAsync(WifiNetworkInfo dataModel) {
            this.log.InfoEntry("WifiConnectAsync");
            // TODO use a try catch and change signature to have an error delegate return?
            try {
                bool save = false;
                WifiErrorCode result = this.WifiGetConnectCredentials(dataModel, ref save);
                this.log.Info("********************************", "Comm Wrapper - WifiConnectAsync after get credentials");

                if (result != WifiErrorCode.Success) {
                    this.OnWifiError?.Invoke(this, new WifiError(result));
                }
                else {
                    if (save) {
                        this.pendingSaveConnectNetInfo = dataModel;
                    }
                    this.wifi.ConnectAsync(dataModel);
                }
            }
            catch (ErrReportException erE) {
                this.OnWifiError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = erE.Report.Msg });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                this.OnWifiError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = e.Message });
            }
        }


        public void WifiDisconect() {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000351, "Failure on WifiDisconnect", () => {
                this.wifi.Disconnect();
            });
            this.RaiseIfException(report);

        }


        public void WifiSend(string msg) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000352, "Failure on WifiSend", () => {
                this.GetCurrentTerminator(
                    CommMedium.Wifi,
                    (data) => {
                        this.wifiStack.InTerminators = data.TerminatorBlock;
                        this.wifiStack.OutTerminators = data.TerminatorBlock;

                    }, (err) => { });
                this.wifiStack.SendToComm(msg);
            });
            this.RaiseIfException(report);
        }


        public List<KeyValuePropertyDisplay> Wifi_GetDeviceInfoForDisplay(WifiNetworkInfo info) {
            List<KeyValuePropertyDisplay> list = new List<KeyValuePropertyDisplay>();

            // TODO - language

            list.Add(new KeyValuePropertyDisplay("SSID", info.SSID));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.AuthenticationType), info.AuthenticationType.ToString().UnderlineToSpaces()));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.EncryptionType), info.EncryptionType.ToString().UnderlineToSpaces()));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.UpTime), info.UpTime.ToString("g")));
            list.Add(new KeyValuePropertyDisplay(string.Format("{0} (Bars)",this.GetText(MsgCode.SignalStrength)), info.SignalStrengthInBars));
            list.Add(new KeyValuePropertyDisplay("RSSI (Decible Milliwatts)", info.RssiInDecibleMilliwatts));
            list.Add(new KeyValuePropertyDisplay("Physical Layer Kind", info.PhysicalLayerKind.ToString().UnderlineToSpaces()));
            list.Add(new KeyValuePropertyDisplay(string.Format("{0} (BSSID)", this.GetText(MsgCode.MacAddress)), info.MacAddress_BSSID));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Kind), info.Kind));
            list.Add(new KeyValuePropertyDisplay("WIFI Direct", info.IsWifiDirect));
            list.Add(new KeyValuePropertyDisplay("Channel Center Frequency (Khz)", info.ChanneCenterFrequencyKlhz));
            list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.BeaconInterval), info.BeaconInterval.ToString("g")));


            // The host name and service name are credentials that are added to connect to exposed service
            //list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.Port), info.RemoteServiceName));
            //list.Add(new KeyValuePropertyDisplay(this.GetText(MsgCode.HostName), info.RemoteHostName));
            // User name not used in connections
            //list.Add(new KeyValuePropertyDisplay("User Name", info.UserName));
            return list;
        }


        #endregion

        #region New mobile async

        public void WifiConnectPreValidatedAsync(WifiNetworkInfo dataModel) {
            this.log.InfoEntry("WifiConnectPreValidatedAsync");
            try {
                this.wifi.ConnectAsync(dataModel);
            }
            catch (ErrReportException erE) {
                this.OnWifiError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = erE.Report.Msg });
            }
            catch (Exception e) {
                this.log.Exception(9999, "", e);
                this.OnWifiError?.Invoke(this, new WifiError(WifiErrorCode.Unknown) { ExtraInfo = e.Message });
            }
        }


        /// <summary>Get discovered info to look up the stored WifiCredentials object to check for credentials
        /// 
        /// </summary>
        /// <param name="discoverData">The WifiNetworkInfo with info from discovered devices</param>
        /// <returns></returns>
        public WifiCredAndIndex ValidateCredentials(WifiNetworkInfo discoverData, OnErr onError) {
            WifiCredAndIndex result = null;
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000353, "Failure on ValidateCredentials", () => {
                this.log.Info("WifiGetConnectCredentials", () => string.Format(""));
                if (discoverData.SSID.Trim().Length == 0) {
                    this.log.Error(9999, "ValidateCredentialsAsync", () => string.Format("No SSID in data retrieved from Discover"));
                }
                else {
                    this.log.Info("ValidateCredentialsAsync", () => string.Format("SSID:{0}", discoverData.SSID));
                    result = this.WifiGetStoredCredentials(discoverData.SSID, onError);
                    if (result != null && !result.RequiresUserData) {
                        // initialize the fields in the data model sent to connect to WIFI
                        discoverData.RemoteHostName = result.Data.RemoteHostName;
                        discoverData.RemoteServiceName = result.Data.RemoteServiceName;
                        discoverData.Password = result.Data.WifiPassword;
                    }
                }
            });
            this.RaiseIfException(report);
            return result;
        }


        //private async Task<WifiCredAndIndex> WifiGetStoredCredentials(string ssid, OnErr onError) {
        private WifiCredAndIndex WifiGetStoredCredentials(string ssid, OnErr onError) {
            //return await Task.Run<WifiCredAndIndex>(() => {
                WifiCredAndIndex result = null;
            this.GetWifiCredList(
                (list) => {
                    foreach (var item in list) {
                        this.log.Info("WifiGetStoredCredentials", () => string.Format("SSID '{0}' List itm display '{1}", ssid, item.Display));
                        if (item.Display == ssid) {
                                this.RetrieveWifiCredData(
                                    item,
                                    (data) => {
                                        this.log.Info("WifiGetStoredCredentials", () => string.Format("FOUND SSID '{0}'", item.Display));
                                        result = new WifiCredAndIndex() {
                                            Index = item,
                                            Data = data,
                                            RequiresUserData =
                                                (data.RemoteHostName.Trim().Length == 0 ||
                                                 data.RemoteServiceName.Trim().Length == 0 ||
                                                 data.WifiPassword.Trim().Length == 0),
                                        };
                                    },onError);
                            break;
                        }
                    }
                    if (result == null) {
                            // Create new one
                            WifiCredentialsDataModel dm = new WifiCredentialsDataModel() {
                            SSID = ssid,
                        };
                        this.CreateNewWifiCred(
                            ssid,
                            dm,
                            (ndx) => {
                                result = new WifiCredAndIndex() {
                                    Data = dm,
                                    Index = ndx,
                                    RequiresUserData = true,
                                };
                            }, onError);
                    }
                }, onError);

                return result;
            //});
        }




        #endregion

        private void WifiTeardown() {
            this.wifi.DiscoveredNetworks -= this.Wifi_DiscoveredNetworksHandler;
            this.wifi.OnError -= this.Wifi_OnErrorHandler;
            this.wifi.OnWifiConnectionAttemptCompleted -= this.Wifi_OnWifiConnectionAttemptCompletedHandler;
            this.wifiStack.MsgReceived -= this.WifiStack_BytesReceivedHander;
        }


        #region Private Connection and credentials

        /// <summary>Retrieve credentials if not already stored with data model</summary>
        /// <param name="dataModel">The data on the connection</param>
        private WifiErrorCode WifiGetConnectCredentials(WifiNetworkInfo dataModel, ref bool save) {
            // By default we do not want to save unless
            this.log.Info("WifiGetConnectCredentials", () => string.Format(""));
            WifiErrorCode result = WifiErrorCode.Success;
            if (dataModel.RemoteHostName.Trim().Length == 0 ||
                dataModel.RemoteServiceName.Trim().Length == 0 ||
                dataModel.Password.Trim().Length == 0) {

                this.log.Info("********************************", "Comm Wrapper - GetConnectionCredentials");


                this.log.Info("WifiGetConnectCredentials", () => string.Format("No data in data model"));
                if (!this.WifiGetStoredCredentials(dataModel)) {
                    this.log.Info("WifiGetConnectCredentials", () => string.Format("No stored data - get from user"));
                    result = this.GetUserCredentials(dataModel, ref save);
                    this.log.Info("WifiGetConnectCredentials", () => string.Format("result {0}", result));
                    //if (result == WifiErrorCode.Success) {
                    //    if (save) {
                    //        this.WifiStoreCredentials(dataModel);
                    //    }
                    //}
                }
            }
            else {
                this.log.Info("WifiGetConnectCredentials", 
                    () => string.Format("Has data:{0}:{1}", dataModel.RemoteHostName, dataModel.RemoteServiceName));
            }
            return result;
        }


        /// <summary>Retrieve the stored credentials for this network</summary>
        /// <param name="dataModel">Data about the network</param>
        /// <returns>true on success, otherwise false</returns>
        private bool WifiGetStoredCredentials(WifiNetworkInfo dataModel) {
            bool found = false;

            this.GetWifiCredList(
                (list) => {
                    foreach (var item in list) {
                        this.log.Info("WifiGetStoredCredentials", () => string.Format("SSID '{0}' List itm display '{1}", dataModel.SSID, item.Display));
                        if (item.Display == dataModel.SSID) {
                            // We can do double check by loading the actual object and compare other data like GUID
                            this.RetrieveWifiCredData(
                                item,
                                (data) => {
                                    this.log.Info("WifiGetStoredCredentials", () => string.Format("FOUND SSID '{0}'", item.Display));

                                    dataModel.RemoteHostName = data.RemoteHostName;
                                    dataModel.RemoteServiceName = data.RemoteServiceName;
                                    dataModel.Password = data.WifiPassword;
                                    dataModel.UserName = data.WifiUserName; // Not using user name for now
                                    found = true;
                                },
                                (err) => {
                                    // TODO log error
                                    this.log.Error(9999, () => string.Format("Did not find '{0}'", dataModel.SSID));
                                    found = false;
                                });
                            break;
                        }
                    }
                },
                (err) => {
                    found = false;
                });

            // TODO - implement. Could wifi ssid or GUID to retrieve a stored WifiCredentials object
            return found;

            //dataModel.RemoteHostName = "192.168.4.1"; // IP of Arduino socket
            //dataModel.RemoteServiceName = "80"; // Arduino HTTP port 80
            //dataModel.Password = "1234567890";
            //dataModel.UserName = ""; // Not using User name for now
            //return true;
        }


        /// <summary>Store credentials entered by user</summary>
        /// <param name="dataModel">The network data model with the network credentials</param>
        private void WifiStoreNewCredentials(WifiNetworkInfo dataModel) {
            this.log.InfoEntry("WifiStoreNewCredentials");
            this.CreateNewWifiCred(
                dataModel.SSID,
                new WifiCredentialsDataModel() {
                    SSID = dataModel.SSID,
                    //Id = dataModel.id // TODO Put a GUID id into the network info
                    RemoteHostName = dataModel.RemoteHostName,
                    RemoteServiceName = dataModel.RemoteServiceName,
                    WifiPassword = dataModel.Password,
                    WifiUserName = dataModel.UserName,
                }, 
                () => {
                    // Success. Do nothing
                    this.log.Info("WifiStoreNewCredentials", () => string.Format("Cred for {0} stored successfuly", dataModel.SSID));
                }, 
                (err) => {
                    this.log.Error(9999, () => string.Format("Error creating new cred on wifi connect '{0}'", err));
                });
        }


        /// <summary>Raise an event to request credentials from user</summary>
        /// <param name="dataModel">The network info data model</param>
        /// <returns>WifiErrorCode.Success on success, otherwise and error</returns>
        private WifiErrorCode GetUserCredentials(WifiNetworkInfo dataModel, ref bool save) {
            save = false;

            this.log.Info("********************************", "Comm Wrapper - GetUserCredentials");

            // TODO - implement. Could also use the wifi GUID
            WifiCredentials cred = new WifiCredentials() {
                SSID = dataModel.SSID,
                RemoteHostName = dataModel.RemoteHostName,
                RemoteServiceName = dataModel.RemoteServiceName,
            };
            WrapErr.ChkVar(this.CredentialsRequestedEvent, 9999, "No subscribers to CredentialsRequestedEvent");
            this.CredentialsRequestedEvent?.Invoke(this, cred);

            if (cred.IsUserCanceled) {
                this.log.Info("********************************", "Comm Wrapper - GetUserCredentials Canceled");

                return WifiErrorCode.UserCanceled;
            }

            if (cred.SSID.Trim().Length == 0) {
                this.log.Info("********************************", "Comm Wrapper - GetUserCrendentials Empty SSID");
                return WifiErrorCode.EmptySsid;
            }
            else if (cred.RemoteHostName.Trim().Length == 0) {
                this.log.Info("********************************", "Comm Wrapper - GetUserCrendentials Empty host name");
                return WifiErrorCode.EmptyHostName;
            }
            else if (cred.RemoteServiceName.Trim().Length == 0) {
                this.log.Info("********************************", "Comm Wrapper - GetUserCrendentials Empty port name");
                return WifiErrorCode.EmptyServiceName;
            }
            else if (cred.WifiPassword.Trim().Length == 0) {
                this.log.Info("********************************", "Comm Wrapper - GetUserCrendentials Empty password");

                return WifiErrorCode.EmptyPassword;
            }

            this.log.Info("GetUserCredentials", () => string.Format("User request save:{0}", cred.IsUserSaveRequest));
            save = cred.IsUserSaveRequest;
            dataModel.RemoteHostName = cred.RemoteHostName;
            dataModel.RemoteServiceName = cred.RemoteServiceName;
            dataModel.Password = cred.WifiPassword;
            return WifiErrorCode.Success;
        }

        #endregion

        #region Wifi event handlers

        private void WifiStack_BytesReceivedHander(object sender, byte[] e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000354, "Failure on WifiStack_BytesReceivedHandler", () => {
                string msg = Encoding.ASCII.GetString(e, 0, e.Length);
                this.log.Info("", () => string.Format("Msg In: '{0}'", msg));
                this.Wifi_BytesReceived?.Invoke(sender, msg);
            });
            this.RaiseIfException(report);
        }


        private void Wifi_OnWifiConnectionAttemptCompletedHandler(object sender, MsgPumpResults result) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000355, "Failure on Wifi_OnWifiConnectionAttemptCompletedHandler", () => {
                this.log.Info("Wifi_OnWifiConnectionAttemptCompletedHandler", () => string.Format(
                    "Is OnWifiConnectionAttemptCompleted null={0}",
                    this.OnWifiConnectionAttemptCompleted == null));

                // Set when the current connection is using new parameters
                if (this.pendingSaveConnectNetInfo != null) {
                    if (result.Code == MsgPumpResultCode.Connected) {
                        this.WifiStoreNewCredentials(this.pendingSaveConnectNetInfo);
                    }
                    else {
                        // Wipping out password will cause dialog to come up on next connect
                        this.pendingSaveConnectNetInfo.Password = "";
                    }
                    this.pendingSaveConnectNetInfo = null;
                }
                this.OnWifiConnectionAttemptCompleted?.Invoke(sender, result);
            });
            this.RaiseIfException(report);
        }


        private void Wifi_OnErrorHandler(object sender, WifiError e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000356, "Failure on Wifi_OnErrorHandler", () => {
                this.log.Info("Wifi_OnErrorHandler", () => string.Format("Is OnWifiError null={0}", this.OnWifiError == null));
                // Possible to have other errors on connect so we will also wipe out connect params here
                if (this.pendingSaveConnectNetInfo != null) {
                    // Wipping out password will cause dialog to come up on next connect
                    this.pendingSaveConnectNetInfo.Password = "";
                    this.pendingSaveConnectNetInfo = null;
                }
                this.OnWifiError?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }


        private void Wifi_DiscoveredNetworksHandler(object sender, List<WifiNetworkInfo> e) {
            ErrReport report;
            WrapErr.ToErrReport(out report, 2000357, "Failure on Wifi_DiscoveredNetworksHandler", () => {
                this.log.Info("Wifi_DiscoveredNetworksHandler", () => string.Format("Is DiscoveredNetworks null={0}", this.DiscoveredWifiNetworks == null));
                this.DiscoveredWifiNetworks?.Invoke(sender, e);
            });
            this.RaiseIfException(report);
        }

        #endregion

        #region Wifi credentials storage

        // TODO - need a private function for internal use during connections

        public void GetWifiCredList(Action<List<IIndexItem<DefaultFileExtraInfo>>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    onSuccess.Invoke(this.wifiCredStorage.IndexedItems);
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void CreateNewWifiCred(string display, WifiCredentialsDataModel data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(data.UId) {
                            Display = display,
                        };
                        this.SaveWifiCred(idx, data, onSuccess, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });
        }


        public void CreateNewWifiCred(string display, WifiCredentialsDataModel data, Action<IIndexItem<DefaultFileExtraInfo>> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else {
                        IIndexItem<DefaultFileExtraInfo> idx = new IndexItem<DefaultFileExtraInfo>(data.UId) {
                            Display = display,
                        };
                        this.SaveWifiCred(idx, data, () => { onSuccess(idx); }, onError);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });

        }


        public void RetrieveWifiCredData(IIndexItem<DefaultFileExtraInfo> index, Action<WifiCredentialsDataModel> onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    // TODO - check if exists
                    onSuccess.Invoke(this.wifiCredStorage.Retrieve(index));
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }


        public void SaveWifiCred(IIndexItem<DefaultFileExtraInfo> idx, WifiCredentialsDataModel data, Action onSuccess, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (idx.Display.Length == 0) {
                        onError.Invoke(this.GetText(MsgCode.EmptyName));
                    }
                    else if (string.IsNullOrWhiteSpace(data.SSID)) {
                        onError.Invoke(this.GetText(MsgCode.EmptySSID));
                    }
                    else if (string.IsNullOrWhiteSpace(data.RemoteHostName)) {
                        onError.Invoke(this.GetText(MsgCode.EmptyHostName));
                    }
                    else if (string.IsNullOrWhiteSpace(data.RemoteServiceName)) {
                        onError.Invoke(this.GetText(MsgCode.EmptyPort));
                    }
                    else if (string.IsNullOrWhiteSpace(data.WifiPassword)) {
                        onError.Invoke(this.GetText(MsgCode.EmptyPwd));
                    }
                    else {
                        idx.Display = data.SSID;
                        this.wifiCredStorage.Store(data, idx);
                        onSuccess.Invoke();
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.SaveFailed));
                }
            });

        }


        public void DeleteWifiCredData(IIndexItem<DefaultFileExtraInfo> index, Action<bool> onComplete, OnErr onError) {
            WrapErr.ToErrReport(9999, () => {
                ErrReport report;
                WrapErr.ToErrReport(out report, 9999, () => {
                    if (index == null) {
                        onError.Invoke(this.GetText(MsgCode.NothingSelected));
                    }
                    else {
                        bool ok = this.wifiCredStorage.DeleteFile(index);
                        onComplete(ok);
                    }
                });
                if (report.Code != 0) {
                    onError.Invoke(this.GetText(MsgCode.LoadFailed));
                }
            });
        }

        #endregion



    }
}
