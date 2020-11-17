using System;
using System.Collections.Generic;
using System.Text;
using Windows.Networking.Connectivity;

namespace Ethernet.UWP.Core {

    public partial class EthernetImplUwp {

        public void DiscoverEthernetDevicesAsync() {
            //Task.Run(async () => {
            //    DeviceInformationCollection infos = await DeviceInformation.FindAllAsync(DeviceClass.All);
            //    foreach(DeviceInformation info in infos) {
            //        this.log.Info("Discover", () => string.Format("Name:{0}", info.Name));
            //        this.log.Info("Discover", () => string.Format("   Kind:{0}", info.Kind));
            //        this.log.Info("Discover", () => string.Format("     Id:{0}", info.Id));
            //    }
            //});

            IReadOnlyList<ConnectionProfile> connections = NetworkInformation.GetConnectionProfiles();
            foreach (var c in connections) {
                if (c == null) {
                    continue;
                }

                if (c.IsWlanConnectionProfile) {
                    //this.log.Info("Discover", () => string.Format("Name:{0}", info.Name));
                    //c.ProfileName,
                    //    c.NetworkAdapter.NetworkItem.NetworkId
                    this.log.Info("Discover", () => string.Format("Wwan Name:{0}", c.ProfileName));
                }
                else if (c.IsWlanConnectionProfile) {
                    this.log.Info("Discover", () => string.Format("WLAN Name:{0}", c.ProfileName));
                }


            }



        }


    }
}
