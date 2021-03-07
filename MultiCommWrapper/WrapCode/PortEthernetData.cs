using MultiCommData.Net.StorageDataModels;
using MultiCommData.Net.StorageIndexInfoModels;
using MultiCommWrapper.Net.interfaces;
using StorageFactory.Net.interfaces;
using StorageFactory.Net.Serializers;
using StorageFactory.Net.StorageManagers;
using System;
using System.IO;

namespace MultiCommWrapper.Net.WrapCode {

    /// <summary>This file is to port Ethernet data over to object with Extra Index info</summary>
    /// <remarks>
    /// TODO
    /// Created in March 6, 2021 so I should be able to get rid of it in a year. Only 300 Apps
    /// out there with likely no one using Ethernet
    /// </remarks>
    public partial class CommWrapper : ICommWrapper {

        private void DoEthernetPort() {
            string subDir = @"MultiCommSerialTerminal\EthernetData";
            string oldPath = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                subDir);

            this.log.Info("DoEthernetPort", () => string.Format("oldPath:{0}", oldPath));
            // Old storage exits
            if (Directory.Exists(oldPath)) {
                this.log.Info("DoEthernetPort", () => string.Format("Path exist"));

                IIndexedStorageManager<Ethernet.Common.Net.DataModels.EthernetParams, DefaultFileExtraInfo> ethSt =
                    new IndexedStorageManager<Ethernet.Common.Net.DataModels.EthernetParams, DefaultFileExtraInfo>(
                        new JsonReadWriteSerializerIndented<Ethernet.Common.Net.DataModels.EthernetParams>(),
                        new JsonReadWriteSerializerIndented<IIndexGroup<DefaultFileExtraInfo>>());
                ethSt.StorageSubDir = subDir;
                ethSt.IndexFileName = "EthernetDataIndex.txt";

                this.log.Info("DoEthernetPort", () => string.Format("Number Items:", ethSt.IndexedItems.Count));
                foreach (var ndx in ethSt.IndexedItems) {
                    Ethernet.Common.Net.DataModels.EthernetParams data = ethSt.Retrieve(ndx);
                    if (data != null) {
                        //this.log.Info("DoEthernetPort", () => string.Format(""));
                        this.log.Info("DoEthernetPort", () => string.Format("Data good:{0}", data.Display));

                        // the objects have either Display or Name. So we will get it from the old index first part

                        string display = data.Display;
                        if (display.Length == 0) {
                            string[] parts = ndx.Display.Split(':');
                            if (parts != null && parts.Length > 0) {
                                display = parts[0];
                            }
                        }

                        EthernetParams newData = new EthernetParams() {
                            UId = data.UId,
                            Display = display,
                            EthernetAddress = data.EthernetAddress,
                            EthernetServiceName = data.EthernetServiceName,
                        };

                        IIndexItem<EthernetExtraInfo> idx =
                            new IndexItem<EthernetExtraInfo>(newData.UId, new EthernetExtraInfo(newData)) {
                                Display = newData.Display,
                            };
                        bool ok = this.ethernetStorage.Store(newData, idx);
                        this.log.Info("DoEthernetPort", () => string.Format("Stored result:{0}", ok));
                    }
                }

                // Try it once
                ethSt.DeleteStorageDirectory();

            }






        }



}
}
