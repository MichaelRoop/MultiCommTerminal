using BluetoothCommon.Net;
using LogUtils.Net;
using MultiCommTestCases.Core.Wrapper.Utils;
using NUnit.Framework;
using System.Collections.Generic;
using System.Threading;
using TestCaseSupport.Core;

namespace MultiCommTestCases.Core.Wrapper.Bluetooth {

    [TestFixture]
    public class Test01_BluetoothClassicTests : WrapperTestBase {

        #region Data

        //private readonly ClassLog log = new ("BluetoothClassicTests");
        private readonly List<BTDeviceInfo> devices = new ();
        private readonly ManualResetEvent discoveryComplet = new (false);
        private readonly AutoResetEvent connectComplet = new (false);

        #endregion

        #region Setup

        [OneTimeSetUp]
        public void TestSetSetup() {
            this.OneTimeSetup();
            TDI.Wrapper.BT_DiscoveryComplete += this.Wrapper_BT_DiscoveryComplete;
            TDI.Wrapper.BT_DeviceDiscovered += this.Wrapper_BT_DeviceDiscovered;
            TDI.Wrapper.BTClassicDiscoverAsync(true);
            Assert.True(this.discoveryComplet.WaitOne(5000));
            Assert.True(this.devices.Count > 0, "Failed to find Paired BT devices");
        }


        [OneTimeTearDown]
        public void TestSetTeardown() {
            this.OneTimeTeardown();
            TDI.Wrapper.BT_DiscoveryComplete -= this.Wrapper_BT_DiscoveryComplete;
            TDI.Wrapper.BT_DeviceDiscovered -= this.Wrapper_BT_DeviceDiscovered;
        }

        [SetUp]
        public void SetupEachTest() {
        }

        #endregion


        [Test]
        public void Test01_ConnectDisconnect() {
            this.connectComplet.Reset();
            this.AssureDiscovered();
            TDI.Wrapper.BTClassicDisconnect();

            TDI.Wrapper.BT_ConnectionCompleted += this.Wrapper_BT_ConnectionCompleted;
            TDI.Wrapper.BTClassicConnectAsync(this.devices[0]);
            bool completed = this.connectComplet.WaitOne(10000);
            TDI.Wrapper.BT_ConnectionCompleted -= this.Wrapper_BT_ConnectionCompleted;
            Assert.True(completed, "Connect timed out");
            TDI.Wrapper.BTClassicDisconnect();
        }


        #region Event handlers

        private void Wrapper_BT_DeviceDiscovered(object sender, BTDeviceInfo e) {
            this.devices.Add(e);
        }

        private void Wrapper_BT_DiscoveryComplete(object sender, bool e) {
            this.discoveryComplet.Set();
        }

        private void Wrapper_BT_ConnectionCompleted(object sender, bool e) {
            this.connectComplet.Set();
        }

        private void AssureDiscovered() {
            Assert.True(this.discoveryComplet.WaitOne(5000), "Discovery timed out");
            Assert.True(this.devices.Count > 0, "No BT devices discovered");
        }

        #endregion

    }
}
